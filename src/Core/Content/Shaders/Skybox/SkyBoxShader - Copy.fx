float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CameraPosition;

float4 ClipPlane0;

Texture SkyBoxTexture;
samplerCUBE SkyBoxSampler : register(s0) = sampler_state
{
	texture = (SkyBoxTexture);
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 VertexPosition = mul(input.Position, World);
	output.TextureCoordinate = VertexPosition - CameraPosition;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate)) + float4(0,0,0,1);;
}

technique Skybox
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}



struct WaterVSInput
{
	float4 Position : POSITION0;
};

struct WaterVSOutput
{
	float4 Position : POSITION0;
	float3 TextureCoordinate : TEXCOORD0;
	float4 ClipDistances	: TEXCOORD1;
};

WaterVSOutput WaterVSFunction(WaterVSInput input)
{
	WaterVSOutput output;


	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 VertexPosition = mul(input.Position, World);
	output.TextureCoordinate = VertexPosition - CameraPosition;


	output.ClipDistances.x = dot(worldPosition, ClipPlane0);
	output.ClipDistances.y = 0;
	output.ClipDistances.z = 0;
	output.ClipDistances.w = 0;
	
	return output;
}

float4 WaterPSFunction(WaterVSOutput input) : COLOR0
{
	// TODO: add your pixel shader code here.
	clip(input.ClipDistances);
	return texCUBE(SkyBoxSampler, input.TextureCoordinate) / 2 + float4(0,0,0,1);
}

technique Technique_WtrRflcnPass
{
	pass Pass1
	{
		// TODO: set renderstates here.

		VertexShader = compile vs_3_0 WaterVSFunction();
		PixelShader = compile ps_3_0 WaterPSFunction();
	}
}