float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

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

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

	return input.TextureCoordinates.x;
}

technique Ambient
{
	pass WorldViewProjection
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}