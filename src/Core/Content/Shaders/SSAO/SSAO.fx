
float2 Radius;
float RandomTile;
float Bias;
float FarClip;

#define SAMPLE_COUNT 16
float3 RAND_SAMPLES[SAMPLE_COUNT] =
{
	float3(0.5381, 0.1856,-0.4319),
	float3(0.1379, 0.2486, 0.4430),
	float3(0.3371, 0.5679,-0.0057),
	float3(-0.6999,-0.0451,-0.0019),
	float3(0.0689,-0.1598,-0.8547),
	float3(0.0560, 0.0069,-0.1843),
	float3(-0.0146, 0.1402, 0.0762),
	float3(0.0100,-0.1924,-0.0344),
	float3(-0.3577,-0.5301,-0.4358),
	float3(-0.3169, 0.1063, 0.0158),
	float3(0.0103,-0.5869, 0.0046),
	float3(-0.0897,-0.4940, 0.3287),
	float3(0.7119,-0.0154,-0.0918),
	float3(-0.0533, 0.0596,-0.5411),
	float3(0.0352,-0.0631, 0.5460),
	float3(-0.4776, 0.2847,-0.0271)
};


// This texture contains the main scene image, which the edge detection
// and/or sketch filter are being applied over the top of.
texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
	Texture = (SceneTexture);

	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};


// This texture contains the Depth Value.
texture DepthBuffer;

sampler depthSampler : register(s1) = sampler_state
{
	Texture = (DepthBuffer);

	MinFilter = Point;
	MagFilter = Point;

	AddressU = Clamp;
	AddressV = Clamp;
};


texture NormalBuffer;
sampler2D normalSampler = sampler_state
{
	Texture = <NormalBuffer>;
	MipFilter = NONE;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
};


texture RandomMap;
sampler2D randomSampler = sampler_state
{
	Texture = <RandomMap>;
	MipFilter = NONE;
	MagFilter = POINT;
	MinFilter = POINT;
	AddressU = Wrap;
	AddressV = Wrap;
};

//-------------------------------
// Helper functions
//-------------------------------
half3 DecodeNormal(half4 enc)
{
	float kScale = 1.7777;
	float3 nn = enc.xyz*float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
	float g = 2.0 / dot(nn.xyz, nn.xyz);
	float3 n;
	n.xy = g*nn.xy;
	n.z = g - 1;
	return n;
}


float4x4 MatrixTransform;

void SpriteVertexShader(inout float4 vColor : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
	position = mul(position, MatrixTransform);
}

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // TODO: add your pixel shader code here.
	float depth = tex2D(depthSampler, texCoord).r;


// total occlusion
float totalOcclusion = 0;

depth *= FarClip;
//prevent near 0 divisions
float scale = min(Radius.y, Radius.x / max(1, depth));

half3 normal = tex2D(normalSampler, texCoord);
normal.y = -normal.y;

//this will be used to avoid self-shadowing		  
half3 normalScaled = normal * 0.25f;

//pick a random normal, to add some "noise" to the output
half3 randNormal = (tex2D(randomSampler, texCoord).rgb* 2.0 - 1.0);


for (int i = 0; i < 12; i++)
{
	// reflect the pre-computed direction on the random normal 
	half3 randomDirection = reflect(RAND_SAMPLES[i], randNormal);

	// Prevent it pointing inside the geometry
	randomDirection *= sign(dot(normal, randomDirection));

	// add that scaled normal
	randomDirection += normalScaled;

	//we use a modified depth in the tests
	half modifiedDepth = depth - (randomDirection.z * Radius.x);
	//according to the distance to the camera, we should scale the direction to account the perspective
	half2 offset = randomDirection.xy * scale;

	// Sample depth at offset location
	float newDepth = tex2D(depthSampler, texCoord + offset).r * FarClip;

	//we only care about samples in front of our original-modifies 
	float deltaDepth = saturate(modifiedDepth - newDepth);

	//ignore negative deltas
	totalOcclusion += (1 - deltaDepth)*(deltaDepth > Bias);

}
totalOcclusion /= 12;

return totalOcclusion;
    //return float4(depth, depth, depth,1);
}

technique Technique1
{
    pass Pass0
    {
		VertexShader = compile vs_3_0 SpriteVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
