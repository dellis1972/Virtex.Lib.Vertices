#include "../Include/Shadow.h"

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