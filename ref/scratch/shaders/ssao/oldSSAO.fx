float sampleRadius;
float distanceScale;
float4x4 Projection;
float3 cornerFustrum;

struct VS_OUTPUT
{
	float4 pos              : POSITION;
	float2 TexCoord         : TEXCOORD0;
	float3 viewDirection    : TEXCOORD1;
};

VS_OUTPUT VertexShaderFunction(
	float4 Position : POSITION, float2 TexCoord : TEXCOORD0)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	Out.pos = Position;
	Position.xy = sign(Position.xy);
	Out.TexCoord = (float2(Position.x, -Position.y) + float2(1.0f, 1.0f)) * 0.5f;
	float3 corner = float3(-cornerFustrum.x * Position.x,
		cornerFustrum.y * Position.y, cornerFustrum.z);
	Out.viewDirection = corner;

	return Out;
}


//texture depthTexture;
texture randomTexture;

sampler2D depthSampler : register(s1);


sampler2D RandNormal = sampler_state
{
	Texture = <randomTexture>;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
};

float4 PixelShaderFunction(VS_OUTPUT IN) : COLOR0
{
	float4 samples[16] =
{
	float4(0.355512,    -0.709318,  -0.102371,  0.0),
	float4(0.534186,    0.71511,    -0.115167,  0.0),
	float4(-0.87866,    0.157139,   -0.115167,  0.0),
	float4(0.140679,    -0.475516,  -0.0639818, 0.0),
	float4(-0.0796121,  0.158842,   -0.677075,  0.0),
	float4(-0.0759516,  -0.101676,  -0.483625,  0.0),
	float4(0.12493,     -0.0223423, -0.483625,  0.0),
	float4(-0.0720074,  0.243395,   -0.967251,  0.0),
	float4(-0.207641,   0.414286,   0.187755,   0.0),
	float4(-0.277332,   -0.371262,  0.187755,   0.0),
	float4(0.63864,     -0.114214,  0.262857,   0.0),
	float4(-0.184051,   0.622119,   0.262857,   0.0),
	float4(0.110007,    -0.219486,  0.435574,   0.0),
	float4(0.235085,    0.314707,   0.696918,   0.0),
	float4(-0.290012,   0.0518654,  0.522688,   0.0),
	float4(0.0975089,   -0.329594,  0.609803,   0.0)
};

IN.TexCoord.x += 1.0 / 1600.0;
IN.TexCoord.y += 1.0 / 1200.0;

normalize(IN.viewDirection);
float depth = tex2D(depthSampler, IN.TexCoord).a;
float3 se = depth * IN.viewDirection;

float3 randNormal = tex2D(RandNormal, IN.TexCoord * 200.0).rgb;

float3 normal = tex2D(depthSampler, IN.TexCoord).rgb;
float finalColor = 0.0f;

for (int i = 0; i < 12; i++)
{
	float3 ray = reflect(samples[i].xyz,randNormal) * sampleRadius;

	//if (dot(ray, normal) < 0)
	//  ray += normal * sampleRadius;

	float4 sample = float4(se + ray, 1.0f);
	float4 ss = mul(sample, Projection);

	float2 sampleTexCoord = 0.5f * ss.xy / ss.w + float2(0.5f, 0.5f);

	sampleTexCoord.x += 1.0 / 1600.0;
	sampleTexCoord.y += 1.0 / 1200.0;
	float sampleDepth = tex2D(depthSampler, sampleTexCoord).a;

	if (sampleDepth == 1.0)
	{
		finalColor++;
	}
	else
	{
		float occlusion = distanceScale * max(sampleDepth - depth, 0.0f);
		finalColor += 1.0f / (1.0f + occlusion * occlusion * 0.1);
	}
}

return float4(depth, 0, 0, 1);// float4(finalColor / 16, finalColor / 16, finalColor / 16, 1.0f);
}


technique SSAO
{
	pass P0
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}