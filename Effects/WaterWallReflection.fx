sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

texture waterReflection;
sampler waterSampler = sampler_state
{
    Texture = (waterReflection);
};

texture noiseTexture;
sampler noiseSampler = sampler_state
{
    Texture = (noiseTexture);
};

float GetNoise(float2 Coord)
{
    return tex2D(noiseSampler, Coord).r;
}

float4 Func1(float2 coords : TEXCOORD0) : COLOR0
{
    float4 wColor = tex2D(waterSampler, coords);
    float2 Sample1 = float2((coords.x / 2 + uIntensity / 1000 + 0.5f) % 1, (coords.y / 20 + uIntensity / 1000 + 0.5f) % 1);
    float2 Sample2 = float2((coords.x / 2 - uIntensity / 500 + 0.5f) % 1, (coords.y / 20 + uIntensity / 2000 + 0.5f) % 1);
    float2 disp = float2(GetNoise(Sample1)*0.02f, GetNoise(Sample2)*0.02f);
    float4 colour = tex2D(uImage0, coords + disp*wColor) + wColor;

    return colour;
}

technique Technique1
{
    pass Func1
    {
        PixelShader = compile ps_2_0 Func1();
    }
}