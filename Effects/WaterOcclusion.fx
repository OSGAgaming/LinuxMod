matrix WorldViewProjection;
int inverse;

sampler uImage0 : register(s0);

texture noiseTexture;
sampler noiseSampler = sampler_state
{
	Texture = (noiseTexture);
};

texture occlusionTexture;
sampler occlusionSampler = sampler_state
{
    Texture = (occlusionTexture);
};

float4 WReflect(float2 coords : TEXCOORD0) : COLOR0
{
    float4 map = tex2D(occlusionSampler, coords);
    float4 color = tex2D(uImage0, coords);

    float add = sign(map.r + map.g + map.b);
	
    float mult = inverse == 1 ? 1 - add : add;
	
    return mult == 1 ? color : 0;
}

technique BasicColorDrawing
{
	pass WReflect
	{
		PixelShader = compile ps_2_0 WReflect();
	}
}
