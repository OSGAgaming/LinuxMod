matrix WorldViewProjection;
int inverse;
float4 colorMod;

float reflectionCoefficient;
float refractionCoefficient;
float distortionCoefficient;

sampler uImage0 : register(s0);

texture noiseTexture;
sampler noiseSampler = sampler_state
{
	Texture = (noiseTexture);
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

texture oclMap;
sampler oclSampler = sampler_state
{
    Texture = (oclMap);
};


float4 Reflect(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);

    float2 distortion = float2(color.r, -color.r) * distortionCoefficient;
    float specular = color.g;
    
    float4 reflection = tex2D(reflectionSampler, float2(coords.x, 1 - coords.y) + distortion);
    float4 refraction = tex2D(refractionSampler, coords + distortion);
    
    float4 map = tex2D(oclSampler, coords);   
    
    return map.a == 1 ? 0 : (((reflection * reflectionCoefficient + refraction * refractionCoefficient) * sign(color.a)) * colorMod + specular);
}

technique BasicColorDrawing
{
	pass Reflect
	{
		PixelShader = compile ps_2_0 Reflect();
	}
}
