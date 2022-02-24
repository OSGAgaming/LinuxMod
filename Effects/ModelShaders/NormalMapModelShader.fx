float4x4 matWorldViewProj;
float4x4 matWorld;
float4 vecLightDir = float4(0, 1, 0.5f, 0);
float4 vecEye;
float4 vDiffuseColor = float4(1, 1, 1, 1);
float4 vSpecularColor = float4(1, 1, 1, 1);
float3 vAmbient = float3(0.075f, 0.075f, 0.075f);
float YCull = -1000000;

int FogStart = 1000;
int FogEnd = 10000;

texture2D FogMap;
sampler2D FogMapSampler = sampler_state
{
    Texture = <FogMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

struct IN
{
    float4 Position : POSITION;
    float3 Normal : NORMAL;
};

struct OUT
{
    float4 Pos : SV_POSITION;
    float4 OutPos : TEXCOORD0;
    float3 N : TEXCOORD1;
    float3 V : TEXCOORD2;
};

OUT VS(in IN In)
{
    OUT Out = (OUT) 0;
   
    Out.Pos = mul(In.Position, matWorldViewProj);
    Out.N = mul(In.Normal, matWorld);
   
    float4 PosWorld = mul(In.Position, matWorld);
    
    Out.OutPos = In.Position;
    Out.V = vecEye - PosWorld;
    //Out.C = In.Color;

    return Out;
}
float4 PS(OUT input) : COLOR
{
    float4 fullOut = mul(input.OutPos, matWorld);
    
    if (fullOut.y < YCull)
        discard;
    
    float dist = distance(fullOut.xyz, vecEye.xyz);
    float fogfunction = saturate((dist - FogStart) / (float) (FogEnd - FogStart));
    
    float3 Normal = normalize(input.N);
    float3 LightDir = normalize(vecLightDir);
    float3 ViewDir = normalize(input.V);
       
    float Diff = saturate(dot(Normal, LightDir));
   
    float4 clip = mul(input.OutPos, matWorldViewProj);
    
    float2 screenPos;
    screenPos.x = clip.x / clip.w / 2.0f + 0.5f;
    screenPos.y = -clip.y / clip.w / 2.0f + 0.5f;
    
    float4 FogStrip = tex2D(FogMapSampler, float2(0.5f, 0.9f));
    FogStrip.a = 1;
    
    // R = 2 * (N.L) * N – L
    float3 Reflect = normalize(2 * Diff * Normal - LightDir);
    float Specular = pow(saturate(dot(Reflect, ViewDir)), 15); // R.V^n
    // I = A + Dcolor * Dintensity * N.L + Scolor * Sintensity * (R.V)n
    
    float4 diffuseColor = float4(vAmbient, 1);
    float4 final = lerp((diffuseColor * Diff + vSpecularColor * Specular), FogStrip, fogfunction);
    return float4(final.xyz, 1);
}
technique SpecularLight
{
    pass P1
    {
        // compile shaders
        VertexShader = compile vs_1_1 VS();

        PixelShader = compile ps_2_0 PS();
    }
}
