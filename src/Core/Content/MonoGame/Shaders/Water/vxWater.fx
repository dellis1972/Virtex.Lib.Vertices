//			Shadow Properties
//*********************************************************************************************
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
#define NumSplits 4
#define CSM

#ifdef CSM
float4x4 ShadowTransform[NumSplits];
#else
float4x4 ShadowTransform;
#endif

//float ShadowMapSize = 512.0f;
float4 TileBounds[NumSplits];
float4 SplitColors[NumSplits+1];


float ShadowBrightness = 0.25f;


//			Main Properties
//*********************************************************************************************

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 ReflectionView;

float xWaveLength;
float xWaveHeight;

float2 WaterFlowOffset = float2(0,0);
float3 xCamPos;

float3 xLightDirection;


float fFresnelBias
<
    string UIName 	= "Fresnel Bias";
	string UIWidget = "Slider";
	float  UIMin 	= 0.0f;
	float  UIMax 	= 1.0f; 
	float  UIStep 	= 0.1f;
> = 0.025f;

float fFresnelPower
<
    string UIName 	= "Fresnel Exponent";
	string UIWidget = "Slider";
	float  UIMin 	= 1.0f;
	float  UIMax 	= 10.0f;
	float  UIStep 	= 0.1f;
> = 1.0f;


float fHDRMultiplier // To exaggerate HDR glow effects!
<
    string UIName 	= "HDR Multiplier";
	string UIWidget = "Slider";
	float  UIMin 	= 0.0f;
	float  UIMax 	= 100.0f;
	float  UIStep 	= 1.0f;
> = 0.450f;

float4 vDeepColor : Diffuse
<
    string UIName 	= "Deep Water Color";
	string UIWidget = "Vector";
> = { 0.0f, 0.20f, 0.4150f, 1.0f };

float4 vShallowColor : Diffuse
<
    string UIName 	= "Shallow Water Color";
	string UIWidget = "Vector";
> = { 0.35f, 0.55f, 0.55f, 1.0f };


float fReflectionAmount
<
    string UIName 	= "Reflection Amount";
	string UIWidget = "Slider";    
	float  UIMin 	= 0.0f;
	float  UIMax 	= 2.0f;
	float  UIStep 	= 0.1f;    
> = 0.5f;

float fWaterAmount
<
    string UIName 	= "Water Color Amount";
	string UIWidget = "Slider";    
	float  UIMin 	= 0.0f;
	float  UIMax 	= 2.0f;
	float  UIStep 	= 0.1f;    
> = 0.5f;


texture2D ReflectionMap;
sampler ReflectionSampler = sampler_state
{
	texture = <ReflectionMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = mirror;
	AddressV  = mirror;
};

Texture xWaterBumpMap;
sampler WaterBumpMapSampler = sampler_state 
{ 
	texture = <xWaterBumpMap>; 
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = mirror;
	AddressV  = mirror;
};








//bool DoShadow = true;
bool ShadowDebug = false;

texture Texture;
sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture SpecularMap;
sampler specularSampler = sampler_state
{
    Texture = (SpecularMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};



texture2D ShadowMap;
sampler ShadowMapSampler = sampler_state 
{	
	texture = <ShadowMap>;
	magfilter = POINT;	
	minfilter = POINT;	
	mipfilter = POINT;	
	AddressU  = clamp;	
	AddressV  = clamp;
};

texture3D RandomTexture3D;
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


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//														THIS ONE!!!
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
float GetShadowFactor( ShadowData shadowData, float ndotl )
{
	float  PoissonKernelScale[NumSplits + 1] = { 1.1f, 1.10f, 1.2f, 1.3f, 0.0f };

	float2 PoissonKernel[12] = { float2(-0.326212f, -0.405810f),
		float2(-0.840144f, -0.073580f),
		float2(-0.695914f, 0.457137f),
		float2(-0.203345f, 0.620716f),
		float2(0.962340f, -0.194983f),
		float2(0.473434f, -0.480026f),
		float2(0.519456f, 0.767022f),
		float2(0.185461f, -0.893124f),
		float2(0.507431f, 0.064425f),
		float2(0.896420f, 0.412458f),
		float2(-0.321940f, -0.932615f),
		float2(-0.791559f, -0.597710f) };


	float4 randomTexCoord3D = float4(shadowData.WorldPosition.xyz*100, 0);
	float2 randomValues = tex3Dlod(RandomSampler3D, randomTexCoord3D).rg;
	float2 rotation = randomValues * 2 - 1;

	//float l = saturate(smoothstep(-0.2, 0.2, ndotl));
	float l = saturate(smoothstep(0, 0.2, ndotl));
	float t = smoothstep(randomValues.x * 0.5, 1.0f, l);

	const int numSamples = 2;
	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);
	
	//return lerp(0.25f, 1.0, (splitInfo.LightSpaceDepth < tex2Dlod(ShadowMapSampler, float4(splitInfo.TexCoords, 0, 0)).r));
	return lerp(0.25f, 1.0, splitInfo.LightSpaceDepth <  tex2D(ShadowMapSampler, splitInfo.TexCoords).r);
	
	float result = 0;
	
	for(int s=0; s<numSamples; ++s)
	{
		
		float2 poissonOffset = float2(
			rotation.x * PoissonKernel[s].x - rotation.y * PoissonKernel[s].y,
			rotation.y * PoissonKernel[s].x + rotation.x * PoissonKernel[s].y
		);
		
		//float2 poissonOffset = float2(
		//	rotation.x - rotation.y,
		//	rotation.y + rotation.x 
		//	);

		const float4 randomizedTexCoords = float4(splitInfo.TexCoords + poissonOffset * PoissonKernelScale[splitInfo.SplitIndex] * 0.75, 0, 0);
		//const float4 randomizedTexCoords = float4(splitInfo.TexCoords, 0, 0);
		result += splitInfo.LightSpaceDepth <  tex2Dlod( ShadowMapSampler, randomizedTexCoords).r;
	}

	float shadowFactor = result / numSamples * t;
	return lerp(0.25f, 1.0, shadowFactor); 
	
}

float GetShadowFactor( ShadowData shadowData )
{
	return GetShadowFactor( shadowData, 1 );
}










struct WaterVSIn
{
    float4 Position : POSITION0;
	float2 inTex: TEXCOORD0;
};

struct WaterVSOut
{
    float4 Position : POSITION0;
	float4 ReflectionMapSamplingPos : TEXCOORD1;
    float2 BumpMapSamplingPos        : TEXCOORD2;	
    float4 Position3D                : TEXCOORD3;
	ShadowData Shadow : TEXCOORD4;
};

struct WaterPixelToFrame
{
	float4 Color : COLOR0;
};


WaterVSOut WaterVS(WaterVSIn input)
{
	WaterVSOut Output = (WaterVSOut)0;

	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    Output.Position = mul(viewPosition, Projection);

	Output.ReflectionMapSamplingPos = mul(input.Position, mul(World, mul(ReflectionView, Projection)));
	Output.BumpMapSamplingPos = input.inTex/xWaveLength;
	Output.Position3D = mul(input.Position, World);


	Output.Shadow = GetShadowData(worldPosition, Output.Position);
	
	return Output;
}

WaterPixelToFrame WaterPS(WaterVSOut PSIn)
{
	WaterPixelToFrame Output = (WaterPixelToFrame)0; 
	
	float2 ProjectedTexCoords;
    ProjectedTexCoords.x = PSIn.ReflectionMapSamplingPos.x/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;
    ProjectedTexCoords.y = -PSIn.ReflectionMapSamplingPos.y/PSIn.ReflectionMapSamplingPos.w/2.0f + 0.5f;    


	float shadow = GetShadowFactor(PSIn.Shadow, 1);

	float2 perturbatedTexCoords = ProjectedTexCoords;
	float3 normalVector = float3(0,1,0);

	float3 dist = xCamPos - PSIn.Position3D;

	//Get Bump Map
	if(abs(length(dist)) < 5000)
	{
		float4 bumpColorSample1 = tex2D(WaterBumpMapSampler, PSIn.BumpMapSamplingPos + WaterFlowOffset + float2(WaterFlowOffset.x+50, -WaterFlowOffset.x/2));
		float4 bumpColorSample2 = tex2D(WaterBumpMapSampler, PSIn.BumpMapSamplingPos + WaterFlowOffset - float2(WaterFlowOffset.x/2, WaterFlowOffset.x/10));
		float4 bumpColor = (bumpColorSample1 + bumpColorSample2)/2;
		float2 perturbation = xWaveHeight*(bumpColor.rg - 0.5f)*2.0f;
		perturbatedTexCoords = ProjectedTexCoords + perturbation;
	 
		normalVector = (bumpColor.rbg-0.5f)*2.0f;
	 }
	 float4 reflectiveColor = tex2D(ReflectionSampler, perturbatedTexCoords);

	 float3 eyeVector = normalize(xCamPos - PSIn.Position3D);

     float fresnelTerm = dot(eyeVector, normalVector);    

	// Compute the Fresnel term
    float fFacing  = 1.0 - max( dot( eyeVector, normalVector ), 0 );
    float fFresnel = fFresnelBias + ( 1.0 - fFresnelBias ) * pow( fFacing, fFresnelPower);
	
	// Compute the final water color
	float4 vWaterColor = lerp( vDeepColor, vShallowColor, fFacing );

	reflectiveColor.rgb *= ( 1.0 + reflectiveColor.a * fHDRMultiplier );

	Output.Color = vWaterColor * fWaterAmount + reflectiveColor * fFresnel;

	//Apply Specular
     float3 reflectionVector = -reflect(xLightDirection, normalVector);
     float specular = dot(normalize(reflectionVector), normalize(eyeVector));
     specular = pow(specular, 256);


     Output.Color.rgb += specular * specular;
	 Output.Color.rgb *= shadow;
	 //Output.Color.a = 0.5;

	return Output;
}

technique Water
{
pass Pass0
	{
		//CullMode		= None;
		//AlphaBlendEnable = TRUE;
  //      DestBlend = INVSRCALPHA;
  //      SrcBlend = SRCALPHA;
		VertexShader = compile vs_3_0 WaterVS();
		PixelShader = compile ps_3_0 WaterPS();
	}
}


//-----------------------------------------------------------------------------
//
// Displacement Mapping
//
//-----------------------------------------------------------------------------

float4x4 WorldViewProjection;
float4x4 WorldView;
float DistortionScale;
float Time;

struct PositionTextured
{
   float4 Position : POSITION;
   float2 TexCoord : TEXCOORD;
};

PositionTextured TransformAndTexture_VertexShader(PositionTextured input)
{
    PositionTextured output;
    
    output.Position = mul(input.Position, WorldViewProjection);
    output.TexCoord = input.TexCoord;
    
    return output;
}
float offset = 0;

texture2D DisplacementMap;
sampler2D DisplacementMapSampler = sampler_state
{
    texture = <DisplacementMap>;
};

float4 Textured_PixelShader(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(DisplacementMapSampler, texCoord + float2(0,offset));
    
    // Ignore the blue channel    
    return float4(color.rgb, color.a) * DistortionScale;
}

technique Distortion
{
    pass
    {
        VertexShader = compile vs_3_0 TransformAndTexture_VertexShader();
        PixelShader = compile ps_3_0 Textured_PixelShader();
    }
}