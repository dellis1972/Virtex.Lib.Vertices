#include "Include/Shadow.h"
#include "Include/ModelPixelShaders.h"


//			Main Properties
//*********************************************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;
float Alpha = 1;

float SpecularIntensity = 0.5;
float SpecularPower = 1;

bool DoShadow = true;

//			Shadow Properties
//*********************************************************************************************
float4x4 ViewProjection_Sdw;
float2 BlurStep_Sdw;
float2 DepthBias_Sdw;
bool ShadowDebug = false;

texture Texture;
sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture SpecularMap;
sampler specularSampler = sampler_state
{
    Texture = (SpecularMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};




//**************************************************
//					Perp Shader
//**************************************************

/*
	This Technique draws the rendertargets which are used
	in other techniques later on, such as Normal and Depth
	Calculations, Mask for God Rays. It performs all of 
	this in one pass rendering to multiple render targets 
	at once.
*/

// Vertex shader input structure.
struct PrepVSInput
{
    float4 Position : POSITION0;
    float3 Normal 	: NORMAL0;
    float2 TexCoord	: TEXCOORD0;
    float3 Binormal	: BINORMAL0;
    float3 Tangent	: TANGENT0;
};

struct PrepVSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;
	//ShadowData Shadow		: TEXCOORD5;
};

struct PrepPSOutput
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
};


PrepVSOutput PrepPassVSFunction(PrepVSInput input, float4x4 worldTransform)
{
    PrepVSOutput output;

    float4 worldPosition = mul(float4(input.Position.xyz,1), worldTransform);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, worldTransform);
    output.tangentToWorld[1] = mul(input.Binormal, worldTransform);
    output.tangentToWorld[2] = mul(input.Normal, worldTransform);
	
	//output.Shadow = GetShadowData( worldPosition, output.Position);

    return output;
}


PrepVSOutput PrepPassVSFunctionInstancedVS( PrepVSInput input, float4x4 instanceTransform : TEXCOORD2)
{
	float4x4 worldTransform = mul( transpose(instanceTransform), World  );
	return PrepPassVSFunction(input, worldTransform); 
}

PrepVSOutput PrepPassVSFunctionNonInstVS( PrepVSInput input )
{
	return PrepPassVSFunction(input, World);
}


PrepPSOutput PrepPassPSFunction(PrepVSOutput input)
{
    PrepPSOutput output = (PrepPSOutput)0;
	
	


	//First, Get the Diffuse Colour of from the Texture
	//*********************************************************************************************
	float4 diffusecolor = tex2D(diffuseSampler, input.TexCoord);
    
	output.Color = diffusecolor;


	//Next get the Specular Attribute from the Specular Map.
	//*********************************************************************************************
    float4 specularAttributes = tex2D(specularSampler, input.TexCoord);
    

	output.Color.a = specularAttributes.r * SpecularIntensity;
	

	//
	//Thirdly, get the Normal from both the Gemoetry and any supplied Normal Maps.
	//*********************************************************************************************
    // read the normal from the normal map
    float3 normalFromMap = tex2D(normalSampler, input.TexCoord);
    //tranform to [-1,1]
    normalFromMap = 2.0f * normalFromMap - 1.0f;
    //transform into world space
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
    //normalize the result
    normalFromMap = normalize(normalFromMap);
    //output the normal, in [0,1] space
    output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);
	    
    //specular Power
    output.Normal.a = specularAttributes.a * SpecularPower;
	


	//Next, Set the Depth Value
	//*********************************************************************************************
    output.Depth = input.Depth.x / input.Depth.y;
	
	//Now, get the Shadow factor from the Cascaded Shadow Map
	//*********************************************************************************************
	//Get Shadow Factor
	//float shadow =  GetShadowFactor( input.Shadow, 1);
	

	
	//Finally, Calculate the Color from the appropriate Pixel Shader.
	//*********************************************************************************************	
	ModelPixelShaderData modelPSData;

	modelPSData.PixelColor = diffusecolor;
	modelPSData.PixelNormal = output.Normal.rgb;

	//output.Color = ToonPixelShader(modelPSData);
	//output.Color = LambertPixelShader(modelPSData);

    return output;
}

technique Technique_PrepPass
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 PrepPassVSFunctionNonInstVS();
        PixelShader = compile ps_3_0 PrepPassPSFunction();
    }
}
technique Technique_PrepPass_Instanced
{
    pass Pass0
    {
		VertexShader = compile vs_3_0 PrepPassVSFunctionInstancedVS();
        PixelShader = compile ps_3_0 PrepPassPSFunction();
    }
}



struct VertexShaderInput_Sdw
{
    float4 Position			: POSITION0;
};

struct VertexShaderOutput_Sdw
{
    float4 Position			: POSITION0;
	float  Depth			: COLOR0;
};

VertexShaderOutput_Sdw ShadowVS(VertexShaderInput_Sdw input, float4x4 worldTransform)
{
    VertexShaderOutput_Sdw output = (VertexShaderOutput_Sdw)0;

    float4 worldPosition = mul(input.Position, worldTransform);
    output.Position = mul(worldPosition, ViewProjection_Sdw);

    output.Depth = output.Position.z / output.Position.w;

    return output;
}

VertexShaderOutput_Sdw ShadowInstancedVS( VertexShaderInput_Sdw input, float4x4 instanceTransform : TEXCOORD2)
{
	float4x4 worldTransform = mul( transpose(instanceTransform), World  );
	return ShadowVS(input, worldTransform); 
}

VertexShaderOutput_Sdw ShadowNonInstancedVS( VertexShaderInput_Sdw input )
{
	return ShadowVS(input, World);
}

float4 ShadowPS(VertexShaderOutput_Sdw input) : COLOR0
{
	float depthSlopeBias = max(
		abs(ddx(input.Depth)), 
		abs(ddy(input.Depth))
	);
	return float4( input.Depth + depthSlopeBias * DepthBias_Sdw.x + DepthBias_Sdw.y, input.Depth*input.Depth, 0, 0 );
}

technique Shadow
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 ShadowNonInstancedVS();
        PixelShader = compile ps_3_0 ShadowPS();
    }
}

technique ShadowInstanced
{
    pass Pass1
    {
		VertexShader = compile vs_3_0 ShadowInstancedVS();
        PixelShader = compile ps_3_0 ShadowPS();
    }
}

















//**************************************************
//					Main Shader
//**************************************************

/*
This Technique draws the rendertargets which are used
in other techniques later on, such as Normal and Depth
Calculations, Mask for God Rays. It performs all of
this in one pass rendering to multiple render targets
at once.
*/

// Vertex shader input structure.
struct MainVSInput
{
	float4 Position : POSITION0;
	float3 Normal 	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
	float3 Binormal	: BINORMAL0;
	float3 Tangent	: TANGENT0;
};

struct MainVSOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float2 Depth : TEXCOORD1;
	float3x3 tangentToWorld : TEXCOORD2;
	//float FogFactor : TEXCOORD5;
	ShadowData Shadow : TEXCOORD5;
};


MainVSOutput MainVSFunction(MainVSInput input, float4x4 worldTransform)
{
	MainVSOutput output;

	float4 worldPosition = mul(float4(input.Position.xyz, 1), worldTransform);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	// calculate tangent space to world space matrix using the world space tangent,
	// binormal, and normal as basis vectors
	output.tangentToWorld[0] = mul(input.Tangent, worldTransform);
	output.tangentToWorld[1] = mul(input.Binormal, worldTransform);
	output.tangentToWorld[2] = mul(input.Normal, worldTransform);

	//Compute Fog Factor
	if (DoFog)
	{
		//output.FogFactor = ComputeFogFactor(length(CameraPos - worldPosition));
	}

	if (DoShadow)
	{
		output.Shadow = GetShadowData(worldPosition, output.Position);
	}
	return output;
}


MainVSOutput MainVSFunctionInstancedVS(MainVSInput input, float4x4 instanceTransform : TEXCOORD2)
{
	float4x4 worldTransform = mul(transpose(instanceTransform), World);
	return MainVSFunction(input, worldTransform);
}

MainVSOutput MainVSFunctionNonInstVS(MainVSInput input)
{
	return MainVSFunction(input, World);
}


// Pixel shader applies a cartoon shading algorithm.
float4 MainPSFunction(MainVSOutput input) : COLOR0
{
	//First, Get the Diffuse Colour of from the Texture
	//*********************************************************************************************
	float4 diffusecolor = tex2D(diffuseSampler, input.TexCoord);

	float4 Normal = tex2D(normalSampler, input.TexCoord);
	/*
	//Thirdly, get the Normal from both the Gemoetry and any supplied Normal Maps.
	//*********************************************************************************************
	// read the normal from the normal map
	float3 normalFromMap = tex2D(normalSampler, input.TexCoord);
	//tranform to [-1,1]
	normalFromMap = 2.0f * normalFromMap - 1.0f;
	//transform into world space
	normalFromMap = mul(normalFromMap, input.tangentToWorld);
	//normalize the result
	normalFromMap = normalize(normalFromMap);
	//output the normal, in [0,1] space
	Normal.rgb = 0.5f * (normalFromMap + 1.0f);
	*/
	//Now, get the Shadow factor from the Cascaded Shadow Map
	//*********************************************************************************************
	//Get Shadow Factor
	float shadow = 1;
	
	if (DoShadow)
	{
		shadow = GetShadowFactor(input.Shadow, 1);
	}

	//Finally, Calculate the Color from the appropriate Pixel Shader.
	//*********************************************************************************************	
	ModelPixelShaderData modelPSData;

	modelPSData.PixelColor = diffusecolor * shadow;
	modelPSData.PixelNormal = Normal.rgb;

	//float4 Color = ToonPixelShader(modelPSData);
	float4 Color = diffusecolor * shadow;// LambertPixelShader(modelPSData);

	//Color *= shadow;

	Color.a = Alpha;

	if (DoFog)
	{
		//Color = lerp(Color, FogColor, 0.5);
	}


	if (ShadowDebug)
	{
		Color = GetSplitIndexColor(input.Shadow) * shadow;
		Color.a = Alpha;
	}

	return Color;
}

technique Technique_Main
{
	pass Pass0
	{
		//AlphaBlendEnable=true;
		//SrcBlend=srcalpha;
		//DestBlend=invsrcColor;
		VertexShader = compile vs_3_0 MainVSFunctionNonInstVS();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}

technique Technique_Main_Instanced
{
	pass Pass0
	{
		//AlphaBlendEnable = true;
		//SrcBlend = srcalpha;
		//DestBlend = invsrcColor;
		VertexShader = compile vs_3_0 MainVSFunctionInstancedVS();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}













// Pixel shader applies a cartoon shading algorithm.
float4 AlphaPSFunction(MainVSOutput input) : COLOR0
{
	//First, Get the Diffuse Colour of from the Texture
	//*********************************************************************************************
	float4 diffusecolor = tex2D(diffuseSampler, input.TexCoord);
	
	//output.Color = ToonPixelShader(modelPSData);
	float4 Color = diffusecolor;

	return Color;
}

technique Technique_Alpha
{
	pass Pass0
	{
		//AlphaBlendEnable=true;
		//SrcBlend=srcalpha;
		//DestBlend=invsrcColor;
		VertexShader = compile vs_3_0 MainVSFunctionNonInstVS();
		PixelShader = compile ps_3_0 AlphaPSFunction();
	}
}