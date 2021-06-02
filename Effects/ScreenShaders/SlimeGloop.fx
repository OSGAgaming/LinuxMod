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

texture Map;
sampler mapSampler = sampler_state
{
    Texture = (Map);
};

texture Noise;
sampler noiseSampler = sampler_state
{
    Texture = (Noise);
};

texture TileTarget;
sampler tilesampler = sampler_state
{
    Texture = (TileTarget);
};


texture WallTarget;
sampler wallsampler = sampler_state
{
    Texture = (WallTarget);
};


float2 WorldCoords(float2 coords)
{
    return coords/ uZoom + uScreenPosition / uScreenResolution;
}

float GetNoise(float2 Coord)
{
    return tex2D(noiseSampler, Coord).r;
}

float2 Round(float2 coords, int accuracy)
{
    float2 pixel = float2(uScreenResolution.x / accuracy, uScreenResolution.y / accuracy);
    return float2(floor(coords.x * pixel.x), floor(coords.y * pixel.y))/ pixel;
}

float4 P1(float2 coords : TEXCOORD0) : COLOR0
{
    float2 WC = Round(WorldCoords(coords),2)*2;
    float4 wColor = tex2D(mapSampler, Round(coords,2));
    float4 tileColor = tex2D(tilesampler, coords);
    float2 Sample1 = float2((WC.x + uIntensity / 500) % 1, (WC.y - uIntensity / 700) % 1);
    float2 Sample2 = float2((WC.x - uIntensity / 2000) % 1, (WC.y + uIntensity / 1000) % 1);
    float disp = GetNoise(Sample1) * GetNoise(Sample2);

    float4 colour = tex2D(uImage0, coords) + wColor*float4(1 - disp ,1 + disp, disp * 0.2,1) * (1 - tileColor.a);
    return colour;
}

technique Technique1
{
    pass P1
    {
        PixelShader = compile ps_2_0 P1();
    }
}