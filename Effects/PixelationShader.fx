sampler uImage0 : register(s0);
float2 ScreenResolution = float2(1960, 1080);
int accuracy = 2;

float2 Round(float2 coords, int accuracy)
{
    float2 pixel = float2(ScreenResolution.x / accuracy, ScreenResolution.y / accuracy);
    return float2(floor(coords.x * pixel.x), floor(coords.y * pixel.y)) / pixel;
}

float4 P1(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colour = tex2D(uImage0, Round(coords, accuracy));

    return colour;
}

technique Technique1
{
    pass P1
    {
        PixelShader = compile ps_2_0 P1();
    }
}