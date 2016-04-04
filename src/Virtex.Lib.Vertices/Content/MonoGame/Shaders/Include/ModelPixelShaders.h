//-----------------------------------------------------------------------------
// ModelPixelShaders.h
//
// Virtices Engine
// A collection of common render utilty's and pixel shader types such as 
//		* Cartoon
//		* Lambert
//-----------------------------------------------------------------------------


// The light direction is shared between the Lambert and Toon lighting techniques.
float3 LightDirection = normalize(float3(1, 1, 1));


// Settings controlling the Lambert lighting technique.
float3 DiffuseLight = 0.5;
float3 AmbientLight = 0.5;

// Settings controlling the Toon lighting technique.
float ToonThresholds[2] = { 0.8, 0.4 };
float ToonBrightnessLevels[3] = { 1.3, 0.9, 0.5 };



//Struct too Pass Data between the Main Classes and these here
struct ModelPixelShaderData
{
	float4 PixelColor;
	float3 PixelNormal;
};




//A Basic Toon Pixel Shader
float4 ToonPixelShader(ModelPixelShaderData input)
{
	float LightAmount = dot(input.PixelNormal, LightDirection);
    
    float light;

    if (LightAmount > ToonThresholds[0])
        light = ToonBrightnessLevels[0];
    else if (LightAmount > ToonThresholds[1])
        light = ToonBrightnessLevels[1];
    else
        light = ToonBrightnessLevels[2];
                
    input.PixelColor.rgb *= light  + AmbientLight;
    
    return input.PixelColor;
}





// Pixel shader applies a simple Lambert shading algorithm.
float4 LambertPixelShader(ModelPixelShaderData input)
{   
	float LightAmount = dot(input.PixelNormal, LightDirection);

	float Emissive = 1.75f;

    input.PixelColor.rgb *= (saturate(LightAmount) * DiffuseLight + AmbientLight) * Emissive;
    
    return input.PixelColor;
}