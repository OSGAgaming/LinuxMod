matrix WorldViewProjection;
texture noiseTexture;
sampler noiseSampler = sampler_state
{
	Texture = (noiseTexture);
};
texture spotTexture;
sampler spotSampler = sampler_state
{
	Texture = (spotTexture);
};
texture polkaTexture;
sampler polkaSampler = sampler_state
{
	Texture = (polkaTexture);
};

texture waterMask;
sampler waterSampler = sampler_state
{
	Texture = (waterMask);
};

texture Voronoi;
sampler VoronoiSampler = sampler_state
{
	Texture = (Voronoi);
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
};

float2 coordMultiplier;
float2 coordOffset;
float strength;
float progress;
float progress2;
float widthOfWater;
float baseWidth;
//custom passes
texture imageTexture;
sampler imageSampler = sampler_state
{
	Texture = (imageTexture);
	AddressU = Wrap;
	AddressV = Wrap;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	float4 pos = mul(input.Position, WorldViewProjection);
	output.Position = pos;

	output.Color = input.Color;

	output.TextureCoordinates = input.TextureCoordinates;

	return output;
}

float GetNoise(float2 Coord)
{
	return tex2D(noiseSampler, Coord).r;
}

float hue2rgb(float p, float q, float t) {
	if (t < 0) t += 1;
	if (t > 1) t -= 1;
	if (t < 0.166f) return p + (q - p) * 6 * t;
	if (t < 0.5f) return q;
	if (t < 0.66f) return p + (q - p) * (2 / 3 - t) * 6;
	return p;
}
float4 hslToRgb(float h, float s, float l) {
	float r, g, b;
	float q = l < 0.5 ? l * (1 + s) : (l + s) - (l * s);
	float p = (2 * l) - q;
	r = hue2rgb(p, q, h + 0.33f);
	g = hue2rgb(p, q, h);
	b = hue2rgb(p, q, h - 0.33f);
	return float4(r, g, b, 1);
}

float2 Round(float2 coords, int accuracy)
{
	float pixelX = 1980 / accuracy;
	float pixelY = 1080 / accuracy;
	return float2(floor(coords.x * pixelX) / pixelX, floor(coords.y * pixelY) / pixelY);
}

float2 RoundScaled(float2 coords, int accuracy, float2 D)
{
	float pixelX = 1980 / accuracy;
	float pixelY = 1080 / accuracy;
	return float2(floor(coords.x * pixelX) / pixelX, floor(coords.y * pixelY) / pixelY);
}
float2 dimensions;
float2 averageDimensions;

float4 Example(VertexShaderOutput input) : COLOR
{
	float diff = dimensions.y / averageDimensions.y;
	input.Color += max(0,1 - input.TextureCoordinates.y * (6 / diff));
	float2 coords = input.TextureCoordinates;

	return input.Color;
}

float4 WReflect(VertexShaderOutput input) : COLOR
{
	return input.Color * pow(input.TextureCoordinates.y,3);
}

float4 BasicImage(VertexShaderOutput input) : COLOR
{
	float alpha = abs((1.0 - strength) + tex2D(imageSampler, coordOffset + input.TextureCoordinates * coordMultiplier).r * strength);
	return input.Color * alpha;
}

technique BasicColorDrawing
{
	pass Example
	{
		PixelShader = compile ps_2_0 Example();
	}
	pass WReflect
	{
		PixelShader = compile ps_2_0 WReflect();
	}
	pass BasicImagePass
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 BasicImage();
	}
}
