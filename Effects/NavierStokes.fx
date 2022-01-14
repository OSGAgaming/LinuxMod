sampler uImage0 : register(s0);

float visc;
float dT;
int N;
float2 resolution;

texture velocityXField;
sampler velXsampler = sampler_state
{
    Texture = (velocityXField);
};

texture velocityYField;
sampler velYsampler = sampler_state
{
    Texture = (velocityYField);
};

texture bufferTarget;
sampler bufferSample = sampler_state
{
    Texture = (bufferTarget);
};

texture divMap;
sampler divSample = sampler_state
{
    Texture = (divMap);
};

texture pMap;
sampler pSample = sampler_state
{
    Texture = (pMap);
};

texture adDensity;
sampler ADS = sampler_state
{
    Texture = (adDensity);
};

float4x4 MATRIX;

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

VertexShaderOutput MainVS(float4 position : SV_POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = mul(position, MATRIX);
    output.TextureCoordinates = texCoord;

    return output;
}

float round(float number, float precision)
{
    return (float) floor(number / precision) * precision;
}

float2 round(float2 number, float precision)
{
    return float2(round(number.x, precision), round(number.y, precision));
}

float convertToTrue(float4 c)
{
    return c.g - c.r;
}

float convertToTruePrecision(float4 c)
{
    float combine = c.g + c.r;
    return c.g - c.r + (1 - sign(floor(abs(combine * 255.0f)))) * (c.b - 0.5f) * (2.0f / 255.0f);
}

float4 convertToColor(float c)
{
    int s = (sign(c) + 1) / 2;
    return float4((1 - s) * abs(c), s * c, 0, abs(c));
}

float4 convertToColorPrecision(float c)
{
    float s = (sign(c) + 1) / 2.0f;
    int sI = s;
    int p = c == 0 ? 2 : 1 - sign(floor(abs(c * 255.0f)));
    float3 clr = float3((1 - sI) * abs(c), sI * c, p * (s + abs(c * 255.0f)) * 0.5f);
    return float4(clr, 1);
}


float constrict(float x)
{    
    return x;
}

float4 diffuse(VertexShaderOutput input) : COLOR0
{
    float cVisc = visc * N * N * dT;

    float h = 1 / (float) N;
    float2 smallValue = float2(h, h) / 2.0f;

    float2 roundedCoords = float2(((float) floor(input.TextureCoordinates.x / h) * h), ((float) floor(input.TextureCoordinates.y / h) * h)) + smallValue;

    float4 colour;
    float4 pColour = tex2D(bufferSample, input.TextureCoordinates);
    
    float up = convertToTrue(tex2D(uImage0, roundedCoords + float2(0, -h)));
    float down = convertToTrue(tex2D(uImage0, roundedCoords + float2(0, h)));
    float left = convertToTrue(tex2D(uImage0, roundedCoords + float2(-h, 0)));
    float right = convertToTrue(tex2D(uImage0, roundedCoords + float2(h, 0)));

    float totalPressure;
    float numberOfNeighbours;
  
    totalPressure = right + left + down + up;
    numberOfNeighbours = 6;
    
    colour = (pColour + totalPressure * cVisc) / (1 + numberOfNeighbours * cVisc);
    
    return pColour;
}

float4 advect(VertexShaderOutput input) : COLOR0
{
    float h = 1 / (float) N;
    float2 smallValue = float2(h, h) / 2.0f;
    
    float2 roundedCoords = round(input.TextureCoordinates, h);

    float colour;
    float velX = convertToTruePrecision(tex2D(velXsampler, roundedCoords + smallValue));
    float velY = convertToTruePrecision(tex2D(velYsampler, roundedCoords + smallValue));
        
    float visc = dT * N;
    float XGrid, YGrid, XGrid1, YGrid1;
    
    float X = roundedCoords.x - (visc * velX) * h;
    float Y = roundedCoords.y - (visc * velY) * h;
    
    X = clamp(X, h, 1 - h);
    Y = clamp(Y, h, 1 - h);
    
    XGrid = round(X, h);
    XGrid1 = XGrid + h;
    YGrid = round(Y, h);
    YGrid1 = YGrid + h;

    float XRelative1 = (X - XGrid) / h;
    float XRelative0 = 1 - XRelative1;
    float YRelative1 = (Y - YGrid) / h;
    float YRelative0 = 1 - YRelative1;

    float c1 = convertToTruePrecision(tex2D(bufferSample, float2(XGrid, YGrid) + smallValue));
    float c2 = convertToTruePrecision(tex2D(bufferSample, float2(XGrid, YGrid1) + smallValue));
    float c3 = convertToTruePrecision(tex2D(bufferSample, float2(XGrid1, YGrid) + smallValue));
    float c4 = convertToTruePrecision(tex2D(bufferSample, float2(XGrid1, YGrid1) + smallValue));

    colour = XRelative0 * (YRelative0 * c1 + YRelative1 * c2) +
             XRelative1 * (YRelative0 * c3 + YRelative1 * c4);

    return convertToColorPrecision(colour);
}

float4 projectdiv(VertexShaderOutput input) : COLOR0
{
     //use poisson equations to make sure vector field magnitudes are being conserved
    float h = 1 / (float) N;
    float2 smallValue = float2(h, h) / 3.0f;

    float2 roundedCoords = round(input.TextureCoordinates, h) + smallValue;

    float velX1 = convertToTruePrecision(tex2D(velXsampler, roundedCoords + float2(h, 0)));
    float velX2 = convertToTruePrecision(tex2D(velXsampler, roundedCoords + float2(-h, 0)));
    float velY1 = convertToTruePrecision(tex2D(velYsampler, roundedCoords + float2(0, h)));
    float velY2 = convertToTruePrecision(tex2D(velYsampler, roundedCoords + float2(0, -h)));
    
    float colour = -0.5f * h * (velX1 - velX2 +
                     velY1 - velY2);
    
    return convertToColorPrecision(colour);
}

float4 projectp(VertexShaderOutput input) : COLOR0
{
     //use poisson equations to make sure vector field magnitudes are being conserved
    float h = 1 / (float) N;
    float2 smallValue = float2(h, h) / 2.0f;

    float2 roundedCoords = round(input.TextureCoordinates, h) + smallValue;
   
    float up = convertToTruePrecision(tex2D(uImage0, roundedCoords + float2(0, -h)));
    float down = convertToTruePrecision(tex2D(uImage0, roundedCoords + float2(0, h)));
    float left = convertToTruePrecision(tex2D(uImage0, roundedCoords + float2(-h, 0)));
    float right = convertToTruePrecision(tex2D(uImage0, roundedCoords + float2(h, 0)));

    float div = convertToTruePrecision(tex2D(divSample, roundedCoords));
    float c = (div + up + down + left + right) / 6.0f;

    return convertToColorPrecision(c);
}

float4 projectuv(VertexShaderOutput input, uniform bool dir) : COLOR0
{
     //use poisson equations to make sure vector field magnitudes are being conserved
    float h = 1 / (float) N;
    
    float2 smallValue = float2(h, h) / 2.0f;
    
    float2 roundedCoords = round(input.TextureCoordinates, h) + smallValue;

    float colour = convertToTruePrecision(tex2D(uImage0, roundedCoords));
    
    float velX1 = convertToTruePrecision(tex2D(pSample, roundedCoords + float2(h, 0)));
    float velX2 = convertToTruePrecision(tex2D(pSample, roundedCoords + float2(-h, 0)));
    float velY1 = convertToTruePrecision(tex2D(pSample, roundedCoords + float2(0, h)));
    float velY2 = convertToTruePrecision(tex2D(pSample, roundedCoords + float2(0, -h)));
    
    colour -= dir ? 0.5f * (velX1 - velX2) / h : 0.5f * (velY1 - velY2) / h;
    
    return convertToColorPrecision(colour);
}

float4 addsource(VertexShaderOutput input) : COLOR0
{
    float4 add = tex2D(ADS, input.TextureCoordinates);
    
    return add + tex2D(uImage0, input.TextureCoordinates);
}

technique Technique1
{
    pass diffuse
    {
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 diffuse();
    }

    pass advect
    {
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 advect();
    }

    pass projectdiv
    {
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 projectdiv();
    }

    pass projectp
    {
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 projectp();
    }

    pass projectu
    {
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 projectuv(true);
    }

    pass projectv
    {
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 projectuv(false);
    }

    pass projectadd
    {
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 addsource();
    }
}