#include "Include/Shadow.h"
//#include "Include/ModelPixelShaders.h"


//			Main Properties
//*********************************************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;
float Alpha = 1;

float SpecularIntensity = 1;
float SpecularPower = 5;

float3 ViewVector = float3(1, 0, 0);

// The light direction is shared between the Lambert and Toon lighting techniques.
float3 LightDirection = normalize(float3(1, 1, 1));
float4 AmbientLight = float4(0.5, 0.5, 0.5, 1);
float4 EvissiveColour = float4(0, 0, 0, 0);

//Fog Variables
float FogNear;
float FogFar;
float4 FogColor = { 1, 1, 1, 1 };
bool DoFog;
float3 CameraPos;

//			Shadow Properties
//*********************************************************************************************
bool DoShadow = true;
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
	float FogFactor : TEXCOORD1;
	float3x3 tangentToWorld : TEXCOORD2;
	ShadowData Shadow : TEXCOORD5;
};

//Computes the Fog Factor
float ComputeFogFactor(float d)
{
	return clamp((d - FogNear) / (FogFar - FogNear), 0, 1);
}

MainVSOutput MainVSFunction(MainVSInput input, float4x4 worldTransform)
{
	MainVSOutput output;

	float4 worldPosition = mul(float4(input.Position.xyz, 1), worldTransform);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;

	// calculate tangent space to world space matrix using the world space tangent,
	// binormal, and normal as basis vectors
	output.tangentToWorld[0] = mul(input.Tangent, worldTransform);
	output.tangentToWorld[1] = mul(input.Binormal, worldTransform);
	output.tangentToWorld[2] = mul(input.Normal, worldTransform);

	
	//Compute Fog Factor
	if (DoFog)
	{
		output.FogFactor = ComputeFogFactor(length(CameraPos - worldPosition));
	}
	else
		output.FogFactor = 0;
	
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

	//Set Colour From the Diffuse Sampler Colour and the Shadow Factor
	
	float shadow = GetShadowFactor(input.Shadow, 1);
	float4 Color = diffusecolor * shadow;

	Color = lerp(Color, FogColor, input.FogFactor);

	if (ShadowDebug)
	{
		Color = GetSplitIndexColor(input.Shadow) * shadow;
	}

	return Color + float4(0, 0, 0, Alpha) + EvissiveColour;

	/*
	float specFactor = tex2D(specularSampler, input.TexCoord).x;

	//Now, get the Normal from both the Gemoetry and any supplied Normal Maps.
	//*********************************************************************************************
	float3 normalMap = 2.0 *(tex2D(normalSampler, input.TexCoord)) - 1.0;
	normalMap = normalize(mul(normalMap, input.tangentToWorld));
	float4 Normal = float4(normalMap, 1.0);

	//Calculate Light Amount for Diffuse Lighting
	float LightAmount = dot(Normal, LightDirection);
	
	

	float3 light = normalize(LightDirection);
	float3 normal = normalize(Normal);
	float3 r = normalize(2 * dot(light, normal) * normal - light);
	float3 v = normalize(mul(normalize(ViewVector), World));

	float dotProduct = dot(r, v);
	float4 specular = SpecularIntensity * max(pow(dotProduct, SpecularPower), 0) * length(Color);

	Color.rgb *= (saturate(LightAmount) + AmbientLight + specular * specFactor) ;// *Emissive;

	Color = lerp(Color, FogColor, input.FogFactor);

	/*
	if (ShadowDebug)
	{
	Color = GetSplitIndexColor(input.Shadow);
	Color.a = Alpha;
	}
	*/
}

technique Technique_Main
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MainVSFunctionNonInstVS();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}

technique Technique_Main_Instanced
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MainVSFunctionInstancedVS();
		PixelShader = compile ps_3_0 MainPSFunction();
	}
}