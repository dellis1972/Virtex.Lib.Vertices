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

#ifdef VSM
float GetShadowFactor( ShadowData shadowData )
{
	if( any(saturate(abs(shadowData.Position.xy)-shadowData.Position.w)) )
	{
		return 1.f;
	}
	else
	{
	
		float4 shadow = shadowData.Position / shadowData.Position.w;
		float2 shadowTexCoords = shadow.xy * 0.5f + 0.5f;
		shadowTexCoords.y = 1-shadowTexCoords.y;

		float2 shadowMap = tex2D( ShadowMapSampler, shadowTexCoords ).rg;
		float shadowMapDepth = shadowMap.r;
		float shadowMapDepthSquared = shadowMap.g;

		if(shadow.z > shadowMapDepth)
		{
			float var = shadowMapDepthSquared - shadowMapDepth*shadowMapDepth;
			var = saturate( var + 0.0001 );

			float d = shadow.z - shadowMapDepth;
			float p = var / (var + d * d);

			//p = pow(p, 5.0f);

			return p;
		}
		else
		{
			return 1.f;
		}
	}
}

#elseif defined(PCF)

float3 GetShadowFactor( ShadowData shadowData )
{

	if( any(saturate(abs(shadowData.Position.xy)-shadowData.Position.w)) )
	{
		return 0.f;
	}
	else
	{
		float4 shadow = shadowData.Position / shadowData.Position.w;
		float2 shadowTexCoords = shadow.xy * 0.5f + 0.5f;
		shadowTexCoords.y = 1-shadowTexCoords.y;

		float2 shadowMap = tex2D( ShadowMapSampler, shadowTexCoords ).rg;

		float2 offset = (float)(frac(shadowData.Position.xy * 0.5) > 0.25);  // mod
		offset.y += offset.x;  // y ^= x in floating point

		if (offset.y > 1.1)
			offset.y = 0;

		float3 depth = 0;
		depth += tex2D( ShadowMapSampler, shadowTexCoords + offset / 1024 + float2(-1.5, 0.5) / 1024);
		depth += tex2D( ShadowMapSampler, shadowTexCoords + offset / 1024 + float2(0.5, 0.5) / 1024);
		depth += tex2D( ShadowMapSampler, shadowTexCoords + offset / 1024 + float2(-1.5, -1.5) / 1024);
		depth += tex2D( ShadowMapSampler, shadowTexCoords + offset / 1024 + float2(0.5, -1.5) / 1024);
			
		depth /= 4.0;

		return shadow.z < depth;



	}
}
#else

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

struct CascadeBlendingInfo
{
	float2 BlendFactor;
	float4 TexCoords;
	int2 CascadeIndices;
	float4 SplitColor;
};

CascadeBlendingInfo GetBlendingInfo(ShadowData shadowData)
{
	float2 shadowTexCoords[NumSplits+1] = 
	{
		shadowData.TexCoords_0_1.xy,
		shadowData.TexCoords_0_1.zw,
		shadowData.TexCoords_2_3.xy,
		shadowData.TexCoords_2_3.zw,
		float2(0,0)
	};

	CascadeBlendingInfo result = (CascadeBlendingInfo)0;
	const float blendingBandSize = 16.0f / ShadowMapSize;

	for( int i=0; i<NumSplits; ++i )
	{		
		if( shadowTexCoords[i].x >= TileBounds[i].x && 
			shadowTexCoords[i].x <= TileBounds[i].y && 
			shadowTexCoords[i].y >= TileBounds[i].z && 
			shadowTexCoords[i].y <= TileBounds[i].w )
		{
			 float minDist = min(min(min(
				 shadowTexCoords[i].x - TileBounds[i].x,
				 TileBounds[i].y - shadowTexCoords[i].x),
				 shadowTexCoords[i].y - TileBounds[i].z),
				 TileBounds[i].w - shadowTexCoords[i].y);
			
			 result.BlendFactor.x = saturate(minDist / blendingBandSize);
			 result.BlendFactor.y = 1.0f - result.BlendFactor.x;
			 
			 result.TexCoords.xy = shadowTexCoords[i];
			 result.TexCoords.zw = shadowTexCoords[i+1];

			 result.CascadeIndices.x = i;
			 result.CascadeIndices.y = i+1;

			return result;
		}
	}

	return result;
}

//#define CASCADE_BLENDING
#ifndef CASCADE_BLENDING

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//														THIS ONE!!!
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
float GetShadowFactor( ShadowData shadowData, float ndotl )
{
	//float4 randomTexCoord3D = float4(shadowData.WorldPosition.xyz*100, 0);
	//float2 randomValues = tex3Dlod(RandomSampler3D, randomTexCoord3D).rg;
	//float2 rotation = randomValues * 2 - 1;

	//float l = saturate(smoothstep(-0.2, 0.2, ndotl));
	//float l = saturate(smoothstep(0, 0.2, ndotl));
	//float t = smoothstep(randomValues.x * 0.5, 1.0f, l);

	//const int numSamples = 2;
	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);
	
	//return lerp(ShadowBrightness, 1.0, (splitInfo.LightSpaceDepth < tex2Dlod(ShadowMapSampler, float4(splitInfo.TexCoords, 0, 0)).r));
	return lerp(ShadowBrightness, 1.0, splitInfo.LightSpaceDepth <  tex2D(ShadowMapSampler, splitInfo.TexCoords).r);
	/*
	float result = 0;
	
	for(int s=0; s<numSamples; ++s)
	{
		float2 poissonOffset = float2(
			rotation.x * PoissonKernel[s].x - rotation.y * PoissonKernel[s].y,
			rotation.y * PoissonKernel[s].x + rotation.x * PoissonKernel[s].y
		);

		const float4 randomizedTexCoords = float4(splitInfo.TexCoords + poissonOffset * PoissonKernelScale[splitInfo.SplitIndex] * 0.75, 0, 0);
		//const float4 randomizedTexCoords = float4(splitInfo.TexCoords, 0, 0);
		result += splitInfo.LightSpaceDepth <  tex2Dlod( ShadowMapSampler, randomizedTexCoords).r;
	}

	float shadowFactor = result / numSamples * t;
	return lerp(ShadowBrightness, 1.0, shadowFactor); 
	*/
}

float GetShadowFactor( ShadowData shadowData )
{
	return GetShadowFactor( shadowData, 1 );
}

#else

float GetShadowFactor( ShadowData shadowData )
{
	const float bias[NumSplits + 1] = { 0.0001, 0.0001, 0.0001, 0.0001, 0.0f };
	const float scale[NumSplits + 1] = { 1.0f, 1.10f, 1.5f, 2.0f, 0.0f };

	float4 randomTexCoord3D = float4(shadowData.WorldPosition.xyz*100, 0);
	float2 rotation = tex3Dlod(RandomSampler3D, randomTexCoord3D).rg * 2 - 1;

	const int numSamples = 12;
	CascadeBlendingInfo blendingInfo = GetBlendingInfo(shadowData);

	float4 TexCoords;
	int2 CascasedIndices;

	const int numSamplesA = min(blendingInfo.BlendFactor.x * numSamples, 12);
	const int numSamplesB = numSamples - numSamplesA;

	float2 rotatedPoissonKernel[12];
	float result = 0;

	for(int i=0; i<12; ++i)
	{
		rotatedPoissonKernel[i] = float2(
			rotation.x * PoissonKernel[i].x - rotation.y * PoissonKernel[i].y,
			rotation.y * PoissonKernel[i].x + rotation.x * PoissonKernel[i].y
		);

//		result += rotatedPoissonKernel[i].x;
	}
//	return result / 12.0f * ShadowMapSize;

//	return rotation.x * 0.5 +0.5;

	for(int s1 = 0; s1 < numSamplesA; ++s1)
	{
		const float4 randomizedTexCoords = float4(blendingInfo.TexCoords.xy + rotatedPoissonKernel[s1] * scale[blendingInfo.CascadeIndices.x], 0, 0);
		result += shadowData.LightSpaceDepth <  tex2Dlod( ShadowMapSampler, randomizedTexCoords).r + bias[blendingInfo.CascadeIndices.x];
	}

//	return numSamplesA>numSamples ? 1 : 0;
//	return blendingInfo.BlendFactor.x;
	return result / (numSamplesA>0 ? numSamplesA : 1.0f);

	for(int s2 = 0; s2 < numSamplesB; ++s2)
	{
		float2 poissonOffset = float2(
			rotation.x * PoissonKernel[s2].x - rotation.y * PoissonKernel[s2].y,
			rotation.y * PoissonKernel[s2].x + rotation.x * PoissonKernel[s2].y
		);

		const float4 randomizedTexCoords = float4(blendingInfo.TexCoords.zw + poissonOffset * scale[blendingInfo.CascadeIndices.y], 0, 0);
		result += shadowData.LightSpaceDepth <  tex2Dlod( ShadowMapSampler, randomizedTexCoords).r + bias[blendingInfo.CascadeIndices.y];
	}

	return result / numSamples;
}
#endif


#endif





