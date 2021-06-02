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
    return coords / uZoom + uScreenPosition / uScreenResolution;
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
    float2 WC = Round(WorldCoords(coords),2);
    float4 wColor = tex2D(mapSampler, Round(coords,2));

    int pixelation = 1;

    float4 wColorD = tex2D(mapSampler, Round(coords, 2) + float2(pixelation / uScreenResolution.x, 0));
    float4 wColorR = tex2D(mapSampler, Round(coords, 2) + float2(0, pixelation / uScreenResolution.y));
    float4 wColorU = tex2D(mapSampler, Round(coords, 2) + float2(-pixelation / uScreenResolution.x, 0));
    float4 wColorL = tex2D(mapSampler, Round(coords, 2) + float2(0, -pixelation / uScreenResolution.y));

    float alphaS = abs(wColorL.a - wColor.a) + abs(wColorR.a - wColor.a);
    float alphaV = abs(wColorU.a - wColor.a) + abs(wColorD.a - wColor.a);

    float4 colour = tex2D(uImage0, coords) + clamp((alphaS + alphaV) * 10, 0,1) * float4(1, 1, 1, 1);
    return colour;
}

technique Technique1
{
    pass P1
    {
        PixelShader = compile ps_2_0 P1();
    }
}