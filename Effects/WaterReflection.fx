matrix WorldViewProjection;
float4 colorMod;

sampler uImage0 : register(s0);

texture oclMap;
sampler oclSampler = sampler_state
{
    Texture = (oclMap);
};


float4 Reflect(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 map = tex2D(oclSampler, coords);   
    
    return map.a == 1 ? 0 : color;
}

technique BasicColorDrawing
{
	pass Reflect
	{
		PixelShader = compile ps_2_0 Reflect();
	}
}
