// Pixel shader combines the bloom image with the original
// scene, using tweakable intensity levels and saturation.
// This is the final step in applying a bloom postprocess.

//sampler BaseSampler : register(s0);

texture2D Texture;
sampler TextureSampler = sampler_state
{
	texture = <Texture>;
	AddressU  = Clamp;
	AddressV  = Clamp;
};

#define NUM_SAMPLES 25

float2 lightScreenPosition;

float2 halfPixel;

float Density = .5f;
float Decay = .95f;
float Weight = 1.0f;
float Exposure = .15f;

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the bloom and original base image colors.
    float4 base = tex2D(TextureSampler, texCoord);
	float3 col = tex2D(TextureSampler,texCoord);

	float IlluminationDecay = 1.0;
	float3 Sample;

	texCoord = texCoord - halfPixel;
	
	float2 DeltaTexCoord = (texCoord - lightScreenPosition) * (1.0f / (NUM_SAMPLES) * Density);
	
	for( int i = 0; i < NUM_SAMPLES; ++i )
	{
		texCoord -= DeltaTexCoord;
		Sample = tex2D(TextureSampler, texCoord);
		Sample *= IlluminationDecay * Weight;
		col += Sample;
		IlluminationDecay *= Decay;			
	}
	
	return float4(col * Exposure,1);
}


technique LightRayFX
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
