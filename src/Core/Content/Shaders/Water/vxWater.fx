#include "../Include/Shadow.h"

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