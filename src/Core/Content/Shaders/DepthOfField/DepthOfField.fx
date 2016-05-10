float FarClip;
float FocalDistance;
float FocalWidth;

texture SceneTexture;
sampler SceneSampler : register(s0) = sampler_state
{
	Texture = (SceneTexture);

	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};

texture DepthTexture;
sampler DepthSampler : register(s1) = sampler_state
{
	Texture = (DepthTexture);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

texture BlurTexture;
sampler BlurSampler : register(s2) = sampler_state
{
	Texture = (BlurTexture);

	MinFilter = Linear;
	MagFilter = Linear;

	AddressU = Clamp;
	AddressV = Clamp;
};

float GetBlurFactor(float DepthVS)
{
	//return smoothstep(0, FocalWidth, abs(FocalDistance - (DepthVS * FarClip)));
	//DepthVS = 1 - DepthVS;
	float fSceneZ = -FarClip / (DepthVS*FarClip - FarClip);
	return  saturate(abs(fSceneZ - FocalDistance) / FocalWidth);
}


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	// Look up the original color from the main scene.
	float4 scene = tex2D(SceneSampler, texCoord);
	float depthVal = tex2D(DepthSampler, texCoord).r;
	float4 blur = tex2D(BlurSampler, texCoord);

	float blurFactor = GetBlurFactor(depthVal);

    return lerp(scene, blur, blurFactor);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
