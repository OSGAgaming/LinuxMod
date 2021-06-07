sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);

float2 uScreenResolution = float2(1920, 1080);

float2 Round(float2 coords, int accuracy)
{
    float2 pixel = float2(uScreenResolution.x / accuracy, uScreenResolution.y / accuracy);
    return float2(floor(coords.x * pixel.x), floor(coords.y * pixel.y)) / pixel;
}

float4 FilterMyShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colour = tex2D(uImage0, coords);
    float4 wColour = tex2D(uImage0, Round(coords,2));
    int pixelation = 1;
    int colorlimit = 2;

    float4 ColorD = tex2D(uImage0, Round(coords, 2) + float2(pixelation / uScreenResolution.x, 0));
    float4 ColorR = tex2D(uImage0, Round(coords, 2) + float2(0, pixelation / uScreenResolution.y));
    float4 ColorU = tex2D(uImage0, Round(coords, 2) + float2(-pixelation / uScreenResolution.x, 0));
    float4 ColorL = tex2D(uImage0, Round(coords, 2) + float2(0, -pixelation / uScreenResolution.y));

    float alphaS = abs(ColorL.a - wColour.a) + abs(ColorR.a - wColour.a);
    float alphaV = abs(ColorU.a - wColour.a) + abs(ColorD.a - wColour.a);

    colour.r = floor(colour.r * colorlimit) / colorlimit;
    colour.g = floor(colour.g * colorlimit) / colorlimit;
    colour.b = floor(colour.b * colorlimit) / colorlimit;

    float4 add = clamp((alphaS + alphaV), 0, 10) * float4(1, 1, 1, 1);
    return colour + add;
}

technique Technique1
{
    pass Noise2D
    {
        PixelShader = compile ps_2_0 FilterMyShader();
    }
}