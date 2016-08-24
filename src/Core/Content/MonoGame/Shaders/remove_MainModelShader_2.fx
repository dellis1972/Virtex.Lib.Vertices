//#include "Include/ModelPixelShaders.h"

// ***** Base Properties **** //
float4x4 World;
float4x4 View;
float4x4 Projection;
float Alpha = 1;

float SpecularIntensity = 0.5;
float SpecularPower = 1;

// ***** Texture Properties **** //
bool TextureEnabled = true;
bool IsSun = false;

texture2D Texture;
sampler TextureSampler = sampler_state
{
	texture = <Texture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = wrap;
	AddressV  = wrap;
};

texture2D NormalMap;
sampler NormalSampler = sampler_state
{
	texture = <NormalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = wrap;
	AddressV  = wrap;
};

texture2D SpecularMap;
sampler SpecularSampler = sampler_state
{
	texture = <SpecularMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = wrap;
	AddressV  = wrap;
};


// The light direction is shared between the Lambert and Toon lighting techniques.
float3 LightDirection = normalize(float3(1, 1, 1));


// Settings controlling the Lambert lighting technique.
float3 DiffuseLight = 0.5;
float3 AmbientLight = 0.5;

// Settings controlling the Toon lighting technique.
float ToonThresholds[2] = { 0.8, 0.4 };
float ToonBrightnessLevels[3] = { 1.3, 0.9, 0.5 };



//Struct too Pass Data between the Main Classes and these here
struct ModelPixelShaderData
{
	float4 PixelColor;
	float3 PixelNormal;
};




//A Basic Toon Pixel Shader
float4 ToonPixelShader(ModelPixelShaderData input)
{
	float LightAmount = dot(input.PixelNormal, LightDirection);
    
    float light;

    if (LightAmount > ToonThresholds[0])
        light = ToonBrightnessLevels[0];
    else if (LightAmount > ToonThresholds[1])
        light = ToonBrightnessLevels[1];
    else
        light = ToonBrightnessLevels[2];
                
    input.PixelColor.rgb *= light  + AmbientLight;
    
    return input.PixelColor;
}





// Pixel shader applies a simple Lambert shading algorithm.
float4 LambertPixelShader(ModelPixelShaderData input)
{   
	float LightAmount = dot(input.PixelNormal, LightDirection);

	float Emissive = 1.75f;

    input.PixelColor.rgb *= (saturate(LightAmount) * DiffuseLight + AmbientLight) * Emissive;
    
    return input.PixelColor;
}


//**************************************************
//			Prep Shader
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
};

struct PrepPSOutput
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
};


PrepVSOutput PrepPassVSFunction(PrepVSInput input)
{
    PrepVSOutput output;
    //Calculate Projected Position
    //float4 worldPosition = mul(input.Position, World);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, World);
    output.tangentToWorld[1] = mul(input.Binormal, World);
    output.tangentToWorld[2] = mul(input.Normal, World);

    return output;  
}

PrepPSOutput PrepPassPSFunction(PrepVSOutput input)
{
    PrepPSOutput output = (PrepPSOutput)0;

    output.Color = tex2D(TextureSampler, input.TexCoord);

	float4 specularAttributes = tex2D(SpecularSampler, input.TexCoord);

    //specular Intensity
    output.Color.a = specularAttributes.r * SpecularIntensity;
    
    // read the normal from the normal map
    float3 normalFromMap = tex2D(NormalSampler, input.TexCoord);
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

    //Depth
    output.Depth = input.Depth.x / input.Depth.y;

    return output;
}

technique Technique_PrepPass
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 PrepPassVSFunction();
        PixelShader = compile ps_3_0 PrepPassPSFunction();
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
};


MainVSOutput MainVSFunction(MainVSInput input, float4x4 worldTransform)
{
	MainVSOutput output;

	float4 worldPosition = mul(float4(input.Position.xyz, 1), worldTransform);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;
	//output.Depth.x = output.Position.z;
	//output.Depth.y = output.Position.w;

	//// calculate tangent space to world space matrix using the world space tangent,
	//// binormal, and normal as basis vectors
	//output.tangentToWorld[0] = mul(input.Tangent, worldTransform);
	//output.tangentToWorld[1] = mul(input.Binormal, worldTransform);
	//output.tangentToWorld[2] = mul(input.Normal, worldTransform);

	//output.Shadow = GetShadowData(worldPosition, output.Position);

	return output;
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
	float4 diffusecolor = tex2D(TextureSampler, input.TexCoord);

	float4 Normal = tex2D(NormalSampler, input.TexCoord);

	//Now, get the Shadow factor from the Cascaded Shadow Map
	//*********************************************************************************************
	//Get Shadow Factor
	//float shadow = GetShadowFactor(input.Shadow, 1);

	//Finally, Calculate the Color from the appropriate Pixel Shader.
	//*********************************************************************************************	
	ModelPixelShaderData modelPSData;

	modelPSData.PixelColor = diffusecolor;// * shadow;
	modelPSData.PixelNormal = Normal.rgb;

	//float4 Color = ToonPixelShader(modelPSData);
	float4 Color = LambertPixelShader(modelPSData);
	Color.a = Alpha;

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