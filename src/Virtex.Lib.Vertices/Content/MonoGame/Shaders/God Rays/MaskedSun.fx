float4x4 World;
float4x4 View;
float4x4 Projection;

texture2D Texture;
sampler TextureSampler : register(s0) = sampler_state
{
	texture = <Texture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = wrap;
	AddressV = wrap;
};

texture2D DepthMap;
sampler DepthMapSampler : register(s1) = sampler_state
{
	texture = <DepthMap>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    return tex2D(DepthMapSampler, texCoord).r > 0 ? float4 (0, 0, 0, 1) : tex2D(TextureSampler, texCoord);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
