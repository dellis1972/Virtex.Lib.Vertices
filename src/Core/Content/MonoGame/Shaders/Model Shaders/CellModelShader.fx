//			Shadow Properties
//*********************************************************************************************
//-----------------------------------------------------------------------------
// Shadow.h
//
// Virtices Engine
// Collection of Common Cascade Shadow Mapping Code for use in 
// both the CascadeShadowShader.fx as well as in any other *.fx
// files which use it.
//
// It is adapted from theomader's source code here: http://dev.theomader.com/cascaded-shadow-mapping-2/
// It is released under the MIT License (https://opensource.org/licenses/mit-license.php)
//
// It has been modified for the Virtices Engine.
//-----------------------------------------------------------------------------
#define NumSplits 4
#define CSM

#ifdef CSM
float4x4 ShadowTransform[NumSplits];
#else
float4x4 ShadowTransform;
#endif

//float ShadowMapSize = 512.0f;
float4 TileBounds[NumSplits];
float4 SplitColors[NumSplits+1];


float ShadowBrightness = 0.25f;


//			Main Properties
//*********************************************************************************************

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Basic Variables
float4x4 World;
float4x4 View;
float4x4 Projection;
float Alpha = 1;

// The light direction is shared between the Lambert and Toon lighting techniques.
float3 LightDirection = normalize(float3(1, 1, 1));
float4 AmbientLight = float4(0.5, 0.5, 0.5, 1);
float4 EvissiveColour = float4(0, 0, 0, 0);

float ToonThresholds[2] = { 0.8, 0.4 };
float ToonBrightnessLevels[3] = {  0.9, 0.5, 0.2 };

//Fog Variables
float FogNear;
float FogFar;
float4 FogColor = { 1, 1, 1, 1 };
bool DoFog;
float3 CameraPos;

//bool DoShadow = true;
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



texture2D ShadowMap;
sampler ShadowMapSampler = sampler_state 
{	
	texture = <ShadowMap>;
	magfilter = POINT;	
	minfilter = POINT;	
	mipfilter = POINT;	
	AddressU  = clamp;	
	AddressV  = clamp;
};

texture3D RandomTexture3D;
sampler RandomSampler3D = sampler_state 
{	
	texture = <RandomTexture3D>; 
	magfilter = POINT;	
	minfilter = POINT;	
	mipfilter = POINT;	
	AddressU  = wrap;	
	AddressV  = wrap; 
	AddressW  = wrap;
};



struct ShadowData
{
#ifdef CSM
	float4 TexCoords_0_1;
	float4 TexCoords_2_3;
	float4 LightSpaceDepth;
#else
    float4 Position;
#endif
//	float4 ClipPosition;
	float3 WorldPosition;
};



ShadowData GetShadowData( float4 worldPosition, float4 clipPosition )
{
	ShadowData result;
#ifdef CSM
	float4 texCoords[NumSplits];
	float lightSpaceDepth[NumSplits];

	for( int i=0; i<NumSplits; ++i )
	{
		float4 lightSpacePosition = mul( worldPosition, ShadowTransform[i] );
		texCoords[i] = lightSpacePosition / lightSpacePosition.w;
		lightSpaceDepth[i] = texCoords[i].z;
	}

	result.TexCoords_0_1 = float4(texCoords[0].xy, texCoords[1].xy);
	result.TexCoords_2_3 = float4(texCoords[2].xy, texCoords[3].xy);
	result.LightSpaceDepth = float4(lightSpaceDepth[0], lightSpaceDepth[1], lightSpaceDepth[2], lightSpaceDepth[3]); 

#else
	result.Position = mul( worldPosition, ShadowTransform );
#endif

//	result.ClipPosition = clipPosition;
	result.WorldPosition = worldPosition;
	return result;
}



struct ShadowSplitInfo
{
	float2 TexCoords;
	float  LightSpaceDepth;
	int    SplitIndex;
};


ShadowSplitInfo GetSplitInfo( ShadowData shadowData )
{
	
	float2 shadowTexCoords[NumSplits] = 
	{
		shadowData.TexCoords_0_1.xy, 
		shadowData.TexCoords_0_1.zw,
		shadowData.TexCoords_2_3.xy,
		shadowData.TexCoords_2_3.zw
	};

	float lightSpaceDepth[NumSplits] = 
	{
		shadowData.LightSpaceDepth.x,
		shadowData.LightSpaceDepth.y,
		shadowData.LightSpaceDepth.z,
		shadowData.LightSpaceDepth.w,
	};
	
	for( int splitIndex=0; splitIndex < NumSplits; splitIndex++ )
	{
		if( shadowTexCoords[splitIndex].x >= TileBounds[splitIndex].x && shadowTexCoords[splitIndex].x <= TileBounds[splitIndex].y && 
			shadowTexCoords[splitIndex].y >= TileBounds[splitIndex].z && shadowTexCoords[splitIndex].y <= TileBounds[splitIndex].w )
		{
			ShadowSplitInfo result;
			result.TexCoords = shadowTexCoords[splitIndex];
			result.LightSpaceDepth = lightSpaceDepth[splitIndex];
			result.SplitIndex = splitIndex;

			return result;
		}
	}

	ShadowSplitInfo result = { float2(0,0), 0, NumSplits };
	return result;
}

float4 GetSplitIndexColor( ShadowData shadowData )
{
	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);
	return SplitColors[splitInfo.SplitIndex];
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//														THIS ONE!!!
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
float GetShadowFactor( ShadowData shadowData, float ndotl )
{
	float  PoissonKernelScale[NumSplits + 1] = { 1.1f, 1.10f, 1.2f, 1.3f, 0.0f };

	float2 PoissonKernel[12] = { float2(-0.326212f, -0.405810f),
		float2(-0.840144f, -0.073580f),
		float2(-0.695914f, 0.457137f),
		float2(-0.203345f, 0.620716f),
		float2(0.962340f, -0.194983f),
		float2(0.473434f, -0.480026f),
		float2(0.519456f, 0.767022f),
		float2(0.185461f, -0.893124f),
		float2(0.507431f, 0.064425f),
		float2(0.896420f, 0.412458f),
		float2(-0.321940f, -0.932615f),
		float2(-0.791559f, -0.597710f) };


	float4 randomTexCoord3D = float4(shadowData.WorldPosition.xyz*100, 0);
	float2 randomValues = tex3Dlod(RandomSampler3D, randomTexCoord3D).rg;
	float2 rotation = randomValues * 2 - 1;

	//float l = saturate(smoothstep(-0.2, 0.2, ndotl));
	float l = saturate(smoothstep(0, 0.2, ndotl));
	float t = smoothstep(randomValues.x * 0.5, 1.0f, l);

	const int numSamples = 2;
	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);
	
	//return lerp(0.25f, 1.0, (splitInfo.LightSpaceDepth < tex2Dlod(ShadowMapSampler, float4(splitInfo.TexCoords, 0, 0)).r));
	return lerp(0.25f, 1.0, splitInfo.LightSpaceDepth <  tex2D(ShadowMapSampler, splitInfo.TexCoords).r);
	
	float result = 0;
	
	for(int s=0; s<numSamples; ++s)
	{
		
		float2 poissonOffset = float2(
			rotation.x * PoissonKernel[s].x - rotation.y * PoissonKernel[s].y,
			rotation.y * PoissonKernel[s].x + rotation.x * PoissonKernel[s].y
		);
		
		//float2 poissonOffset = float2(
		//	rotation.x - rotation.y,
		//	rotation.y + rotation.x 
		//	);

		const float4 randomizedTexCoords = float4(splitInfo.TexCoords + poissonOffset * PoissonKernelScale[splitInfo.SplitIndex] * 0.75, 0, 0);
		//const float4 randomizedTexCoords = float4(splitInfo.TexCoords, 0, 0);
		result += splitInfo.LightSpaceDepth <  tex2Dlod( ShadowMapSampler, randomizedTexCoords).r;
	}

	float shadowFactor = result / numSamples * t;
	return lerp(0.25f, 1.0, shadowFactor); 
	
}

float GetShadowFactor( ShadowData shadowData )
{
	return GetShadowFactor( shadowData, 1 );
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
	float4 Position : SV_POSITION;
	float3 Normal 	: NORMAL0;
	float2 TexCoord	: TEXCOORD0;
	float3 Binormal	: BINORMAL0;
	float3 Tangent	: TANGENT0;
};

struct MainVSOutput
{
	float4 Position : SV_POSITION;
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
	if (DoFog==true)
	{
		output.FogFactor = ComputeFogFactor(length(CameraPos - worldPosition));
	}
	else
		output.FogFactor = 0;
	

	output.Shadow = GetShadowData(worldPosition, output.Position);
	
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

	float LightAmount = dot(input.tangentToWorld[0], LightDirection);

	float light;

	if (LightAmount > ToonThresholds[0])
		light = ToonBrightnessLevels[0];
	else if (LightAmount > ToonThresholds[1])
		light = ToonBrightnessLevels[1];
	else
		light = ToonBrightnessLevels[2];

	Color.rgb *= light + AmbientLight;


	if (ShadowDebug==true)
	{
		Color = GetSplitIndexColor(input.Shadow) * shadow;
	}


	return Color * shadow + float4(0, 0, 0, Alpha) + EvissiveColour;
}

technique Technique_Main
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVSFunctionNonInstVS();
		PixelShader = compile PS_SHADERMODEL MainPSFunction();
	}
};