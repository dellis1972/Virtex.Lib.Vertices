//-----------------------------------------------------------------------------
// Distorters.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

// common parameters
float4x4 WorldViewProjection;
float4x4 WorldView;
float DistortionScale;
float Time;

float offset = 0;
texture2D DisplacementMap;
sampler2D DisplacementMapSampler = sampler_state
{
    texture = <DisplacementMap>;
};

//-----------------------------------------------------------------------------
//
// Displacement Mapping
//
//-----------------------------------------------------------------------------

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

float4 Textured_PixelShader(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(DisplacementMapSampler, texCoord + float2(0,offset));
    
    // Ignore the blue channel    
    return float4(color.rgb, color.a) * DistortionScale;
}

technique DisplacementMapped
{
    pass
    {
        VertexShader = compile vs_3_0 TransformAndTexture_VertexShader();
        PixelShader = compile ps_3_0 Textured_PixelShader();
    }
}
