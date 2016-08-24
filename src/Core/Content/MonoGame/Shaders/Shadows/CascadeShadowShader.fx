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
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define NumSplits 4
#define CSM

#ifdef CSM
float4x4 ShadowTransform[NumSplits];
#else
float4x4 ShadowTransform;
#endif

float ShadowMapSize = 512.0f;
float4 TileBounds[NumSplits];
float4 SplitColors[NumSplits+1];

float2 PoissonKernel[12];
float  PoissonKernelScale[NumSplits + 1] = { 1.1f, 1.10f, 1.2f, 1.3f, 0.0f };

float ShadowBrightness = 0.25f;

texture2D ShadowMap;

texture2D RandomTexture2D;
texture3D RandomTexture3D;

sampler ShadowMapSampler = sampler_state 
{	
	texture = <ShadowMap>;
	magfilter = POINT;	
	minfilter = POINT;	
	mipfilter = POINT;	
	AddressU  = clamp;	
	AddressV  = clamp;
};

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

sampler RandomSampler2D = sampler_state 
{	
	texture = <RandomTexture2D>; 
	magfilter = POINT;	
	minfilter = POINT;	
	mipfilter = POINT;	
	AddressU  = wrap;	
	AddressV  = wrap; 
};




//			Main Properties
//*********************************************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;

//			Shadow Properties
//*********************************************************************************************
float4x4 ViewProjection_Sdw;
float2 BlurStep_Sdw;
float2 DepthBias_Sdw;
bool ShadowDebug = false;



//**************************************************
//			Cascade Shadow Mapping Shader
//**************************************************

/*
	This Technique //TODO Description
*/

struct VertexShaderInput_Sdw
{
    float4 Position			: SV_POSITION;
};

struct VertexShaderOutput_Sdw
{
    float4 Position			: SV_POSITION;
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
		VertexShader = compile VS_SHADERMODEL ShadowNonInstancedVS();
        PixelShader = compile PS_SHADERMODEL ShadowPS();
    }
}