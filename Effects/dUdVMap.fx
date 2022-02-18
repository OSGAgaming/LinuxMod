matrix WorldViewProjection;

matrix World;

float progress;
float2 coordDensity;
float3 Target;

float SpecularPower;

texture dUdVMap;
sampler dUdVSampler = sampler_state
{
    Texture = (dUdVMap);
};

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
    float3x3 worldToTangentSpace : TEXCOORD2;
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

    output.worldToTangentSpace[0] = mul(normalize(float3(0, -1, 0)), World);
    output.worldToTangentSpace[1] = mul(normalize(float3(1, 0, 0)), World);
    output.worldToTangentSpace[2] = mul(normalize(float3(0, 0, 1)), World);
    
    output.TextureCoordinates = input.TextureCoordinates;
    output.View = normalize(float4(Target, 1.0) - mul(input.Position, World));
    
    return output;
}

float4 GetNoise(float2 Coord)
{
    return tex2D(dUdVSampler, Coord);
}

float GetColor(float4 color)
{
    return (color.g - color.r + 1) * 0.5f;
}

float4 Noise(VertexShaderOutput input) : COLOR
{
    float d1 = GetColor(GetNoise((input.TextureCoordinates * coordDensity.x + float2(progress, progress)) % 1));
    float d2 = GetColor(GetNoise((input.TextureCoordinates * coordDensity.y - float2(progress, progress)) % 1));

    float4 Color = input.Color;
      // Get the Color of the normal. The color describes the direction of the normal vector
      // and make it range from 0 to 1.
    float3 n1 = tex2D(NormalMapSampler, input.TextureCoordinates * coordDensity.x + float2(progress, progress) % 1);
    float3 n2 = tex2D(NormalMapSampler, input.TextureCoordinates * coordDensity.y - float2(progress, progress) % 1);

    float3 N = (2.0 * (n1 * n2)) - 1.0f;
    
    N = normalize(mul(N, input.worldToTangentSpace));
    
      // diffuse
    float3 Light = float3(0, -1, 0);
    float D = saturate(dot(N, Light));
      // reflection
    float3 R = normalize(2 * D * N - Light);
      // specular
    float S = pow(saturate(dot(R, input.View)), SpecularPower);

    float Distortion = d1 * d2;
    
    return float4(Distortion, S, 0, 1);
}


technique BasicColorDrawing
{
    pass Reflect
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 Noise();
    }
}
