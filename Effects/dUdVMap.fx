matrix WorldViewProjection;

matrix World;

float progress;
float2 coordDensity;
float3 Target;
float3 LightDirection;

float SpecularPower;
float distortionCoefficient;

float4 tint;
float lerptint;
float reflectivity;

texture dUdVMap;
sampler dUdVSampler = sampler_state
{
    Texture = (dUdVMap);
};

texture reflectionMap;
sampler reflectionSampler = sampler_state
{
    Texture = (reflectionMap);
};

texture refractionMap;
sampler refractionSampler = sampler_state
{
    Texture = (refractionMap);
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

    //output.worldToTangentSpace[0] = mul(normalize(float3(0, 1, 0)), World);
    //output.worldToTangentSpace[1] = mul(normalize(float3(1, 0, 0)), World);
    //output.worldToTangentSpace[2] = mul(normalize(float3(0, 0, 1)), World);
   
    output.TextureCoordinates = input.TextureCoordinates;
    output.View = normalize(float4(Target, 1.0) - mul(input.Position, World));
    output.clipSpace = pos;
    
    return output;
}

float2 GetNoise(float2 Coord)
{
    return tex2D(dUdVSampler, Coord).rg;
}

float GetColor(float4 color)
{
    return (color.g - color.r + 1) * 0.5f;
}

float4 Noise(VertexShaderOutput input) : COLOR
{    
    float2 d1 = GetNoise((input.TextureCoordinates * coordDensity.x + float2(progress, progress)) % 1) * 2.0f - 1;
    float2 d2 = GetNoise((input.TextureCoordinates * coordDensity.y - float2(progress, progress)) % 1) * 2.0f - 1;

    float4 Color = input.Color;
      // Get the Color of the normal. The color describes the direction of the normal vector
      // and make it range from 0 to 1.
        
    float2 n1 = GetNoise((input.TextureCoordinates * coordDensity + float2(progress, 0)) % 1).rg * 0.1f;
    n1 = input.TextureCoordinates * coordDensity + float2(n1.x, n1.y + progress);
    
    float4 normal = tex2D(NormalMapSampler, n1 % 1);
    float3 N = float3(normal.r * 2 - 1, normal.b, normal.g * 2 - 1);

    float3x3 worldToTangentSpace;
    
    worldToTangentSpace[0] = float3(0, 1, 0);
    worldToTangentSpace[1] = float3(1, 0, 0);
    worldToTangentSpace[2] = float3(0, 0, 1);
    
    N = normalize(mul(N, worldToTangentSpace));
    
      // diffuse
    float3 Light = LightDirection;
    float D = saturate(dot(N, Light));
      // reflection
    float3 R = reflect(normalize(D), N);
      // specular
    float S = pow(saturate(dot(R, input.View)), SpecularPower) * reflectivity;

    float2 Distortion = d1 + d2;
    float2 distortionVec = Distortion * distortionCoefficient;

    float2 screenPos;
    screenPos.x = input.clipSpace.x / input.clipSpace.w / 2.0f + 0.5f;
    screenPos.y = -input.clipSpace.y / input.clipSpace.w / 2.0f + 0.5f;
    
    float4 reflection = tex2D(reflectionSampler, float2(screenPos.x, 1 - screenPos.y) + distortionVec) + S;
    float4 refraction = tex2D(refractionSampler, float2(screenPos.x, screenPos.y) + distortionVec) + S;
    
    float4 fresnelMix = lerp(reflection, refraction, abs(dot(input.View, float3(0, 1, 0))));
    
    return lerp(fresnelMix, tint, lerptint);
}


technique BasicColorDrawing
{
    pass Reflect
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 Noise();
    }
}
