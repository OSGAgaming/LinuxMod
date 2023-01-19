matrix WorldViewProjection;
matrix World;

struct VertexShaderInput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 View : TEXCOORD1;
    //float3x3 worldToTangentSpace : TEXCOORD2;
    float4 clipSpace : TEXCOORD2;
};

texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
{
    Texture = <NormalMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

texture imageTexture;
sampler imageSampler = sampler_state
{
    Texture = (imageTexture);
    AddressU = Wrap;
    AddressV = Wrap;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    output.View = normalize(float4(Target, 1.0) - mul(input.Position, World));
    output.clipSpace = pos;
    
    return output;
}

float4 Noise(VertexShaderOutput input) : COLOR
{
   
}

technique BasicColorDrawing
{
    pass Reflect
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 Noise();
    }
}
