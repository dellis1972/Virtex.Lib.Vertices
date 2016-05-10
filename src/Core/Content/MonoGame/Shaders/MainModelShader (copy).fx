#define NumSplits 4
#define CSM

// ***** Base Properties **** //
float4x4 World;
float4x4 View;
float4x4 Projection;

// ***** Texture Properties **** //
bool TextureEnabled = true;
bool IsSun = false;

float ShadowBrightness = 0.25f;

float ShadowMapSize = 512.0f;
float4 TileBounds[4];
float4 SplitColors[NumSplits+1];




// ***** Shadow Properties **** //
float4x4 ViewProjection_Sdw;
float2 BlurStep_Sdw;
float2 DepthBias_Sdw;

float4x4 ShadowTransform[NumSplits];

texture2D Texture;
sampler TextureSampler = sampler_state
{
	texture = <Texture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = wrap;
	AddressV  = wrap;
};

texture2D NormalMap;
sampler NormalSampler = sampler_state
{
	texture = <NormalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = wrap;
	AddressV  = wrap;
};

texture2D SpecularMap;
sampler SpecularSampler = sampler_state
{
	texture = <SpecularMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = wrap;
	AddressV  = wrap;
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

	//float4 TileBounds[4];
	//TileBounds[0] = float4(0.005859375, 0.4941406, 0.005859375, 0.4941406);
	//TileBounds[1] = float4(0.5058594, 0.9941406, 0.005859375, 0.4941406);
	//TileBounds[2] = float4(0.005859375, 0.4941406, 0.5058594, 0.9941406);
	//TileBounds[3] = float4(0.5058594, 0.9941406, 0.5058594, 0.9941406);

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

float GetShadowFactor( ShadowData shadowData, float ndotl )
{
	//float l = saturate(smoothstep(-0.2, 0.2, ndotl));
	float l = saturate(smoothstep(0, 0.2, ndotl));

	const int numSamples = 3;
	ShadowSplitInfo splitInfo = GetSplitInfo(shadowData);
	
	return lerp(ShadowBrightness, 1.0, (splitInfo.LightSpaceDepth <  tex2Dlod( ShadowMapSampler, float4(splitInfo.TexCoords, 0, 0)).r)); 

	/*

	float4 randomTexCoord3D = float4(shadowData.WorldPosition.xyz*100, 0);
	float2 randomValues = tex3Dlod(RandomSampler3D, randomTexCoord3D).rg;
	float2 rotation = randomValues * 2 - 1;
	float t = smoothstep(randomValues.x * 0.5, 1.0f, l);

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



struct VertexShaderInput_Sdw
{
    float4 Position			: POSITION0;
};

struct BlurVertexInput_Sdw
{
	float4 Position			: POSITION0;
	float2 TexCoord			: TEXCOORD0;
};

struct VertexShaderOutput_Sdw
{
    float4 Position			: POSITION0;
	float  Depth			: COLOR0;
};

struct BlurVertexOutput_Sdw
{
	float4 Position			: POSITION0;
	float2 TexCoord			: TEXCOORD0;
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










//**************************************************
//			Perp Shader
//**************************************************

/*
	This Technique draws the rendertargets which are used
	in other techniques later on, such as Normal and Depth
	Calculations, Mask for God Rays. It performs all of 
	this in one pass rendering to multiple render targets 
	at once.
*/

// Vertex shader input structure.
struct PrepVSInput
{
    float4 Position : POSITION0;
    float3 Normal 	: NORMAL0;
    float2 TexCoord	: TEXCOORD0;
    float3 Binormal	: BINORMAL0;
    float3 Tangent	: TANGENT0;
};

struct PrepVSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;

	ShadowData Shadow		: TEXCOORD5;
};

struct PrepPSOutput
{
    half4 Color : COLOR0;
    half4 Normal : COLOR1;
    half4 Depth : COLOR2;
    float4 Mask : COLOR3;
};


PrepVSOutput PrepPassVSFunction(PrepVSInput input)
{
    PrepVSOutput output;
    //Calculate Projected Position
    //float4 worldPosition = mul(input.Position, World);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(input.Tangent, World);
    output.tangentToWorld[1] = mul(input.Binormal, World);
    output.tangentToWorld[2] = mul(input.Normal, World);

	output.Shadow = GetShadowData( worldPosition, output.Position);

    return output;  
}

PrepPSOutput PrepPassPSFunction(PrepVSOutput input)
{
    PrepPSOutput output = (PrepPSOutput)0;

    if(IsSun == true)
    {
    	//Give Same Output as Clearing the G Buffer except for Mask Texture
    	//black color
    	output.Color = 0.0f;
    	output.Color.a = 0.0f;
    	//when transforming 0.5f into [-1,1], we will get 0.0f
    	output.Normal.rgb = 0.5f;
    	//no specular power
    	output.Normal.a = 0.0f;
    	//max depth
    	output.Depth = 0.0f;
		output.Mask = tex2D(TextureSampler, input.TexCoord);
	}
	else
	{
    output.Color = tex2D(TextureSampler, input.TexCoord);

	float4 specularAttributes = tex2D(SpecularSampler, input.TexCoord);

    //specular Intensity
    output.Color.a = specularAttributes.r;
    
    // read the normal from the normal map
    float3 normalFromMap = tex2D(NormalSampler, input.TexCoord);
    //tranform to [-1,1]
    normalFromMap = 2.0f * normalFromMap - 1.0f;
    //transform into world space
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
    //normalize the result
    normalFromMap = normalize(normalFromMap);
    //output the normal, in [0,1] space
    output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);

    //specular Power
    output.Normal.a = specularAttributes.a;

    //Depth
    output.Depth = input.Depth.x / input.Depth.y;
    float c = 0;
    //Mask Colour
	output.Mask = float4( c, c, c, 1 );


    float shadow =  GetShadowFactor( input.Shadow, 1 );
    output.Color *= shadow;
	}
    return output;
}

technique Technique_PrepPass
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 PrepPassVSFunction();
        PixelShader = compile ps_3_0 PrepPassPSFunction();
    }
}