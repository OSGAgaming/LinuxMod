float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;
float Progress;

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
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.Color = input.Color;

	output.TextureCoordinates = input.TextureCoordinates;

	return output;
}

texture noiseTexture;
sampler noiseSampler = sampler_state
{
	Texture = (noiseTexture);
};

float GetNoise(float2 Coord)
{
	return tex2D(noiseSampler, Coord).r;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float2 Sample1 = float2((input.TextureCoordinates.x / 2 + Progress / 4000) % 1, (input.TextureCoordinates.y / 2 - Progress / 5000) % 1);
	float2 Sample2 = float2((input.TextureCoordinates.x / 2 - Progress / 20000) % 1, (input.TextureCoordinates.y / 2 + Progress / 10000) % 1);
	float disp = GetNoise(Sample1) * GetNoise(Sample2);

	float4 add = float4(0, 0, 0, 0);
	if (disp > 0.38 && disp < 0.43)
		add = float4(1, 1, 1, 1);

	if (disp < 0.4f)
		disp = 0;
	else
		disp = 1;
	float4 cloudColour = float4(1,1,1,1);
	cloudColour *= disp * 1;
	return cloudColour + add;
}

technique Ambient
{
	pass WorldViewProjection
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}