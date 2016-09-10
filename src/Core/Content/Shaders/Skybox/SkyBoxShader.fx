
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CameraPosition;

Texture SkyBoxTexture;
samplerCUBE SkyBoxSampler = sampler_state
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
	float4 Position : SV_POSITION;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 VertexPosition = mul(input.Position, World);
	output.TextureCoordinate = VertexPosition - CameraPosition;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate)) + float4(0,0,0,1);;
}

technique Skybox
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};