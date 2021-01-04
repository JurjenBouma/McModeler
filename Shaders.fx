cbuffer cbPerObject
{
	float4x4 WVP;
};

struct VertexIn{
	float3 Pos : POSITION;
	float2 Tex : TEXCOORD;
};

struct VertexOut
{
	float4 Pos : SV_POSITION;
	float2 Tex : TEXCOORD;
};

Texture2D tex;

SamplerState texSampler{ 
	Filter = MIN_MAG_MIP_POINT;
	AddressU = WRAP;
	AddressV = WRAP;
};

VertexOut VShader(VertexIn vIn)
{
	VertexOut output;

	output.Pos = mul(float4(vIn.Pos,1.0f), WVP);
	output.Tex = vIn.Tex;

	return output;
}

float4 PShaderOpaque(VertexOut input) : SV_TARGET
{
	float4 texColor = tex.Sample(texSampler, input.Tex);
	clip(texColor.a -0.999f);
	return texColor;
}
float4 PShaderTransperant(VertexOut input) : SV_TARGET
{
	float4 texColor = tex.Sample(texSampler, input.Tex);
	clip(0.999f - texColor.a);
	return texColor;
}

float4 PShaderOpaqueSelected(VertexOut input) : SV_TARGET
{
	float4 texColor = tex.Sample(texSampler, input.Tex);
	clip(texColor.a - 0.999f);
	texColor.rgb += 0.25f;
	return texColor;
}
float4 PShaderTransperantSelected(VertexOut input) : SV_TARGET
{
	float4 texColor = tex.Sample(texSampler, input.Tex);
	clip(0.999f - texColor.a);
	texColor.rgb += 0.25f;
	return texColor;
}

DepthStencilState DisableDepthWrite
{
	DepthEnable = true;
	DepthWriteMask = false;
	DepthFunc = LESS_EQUAL;
};
DepthStencilState EnableDepth
{
	DepthEnable = true;
	DepthWriteMask = ALL;
	DepthFunc = LESS_EQUAL;
};

technique11 Textured{
	pass P0
	{ 
		SetDepthStencilState(EnableDepth, 0);
		SetVertexShader(CompileShader(vs_4_0, VShader())); 
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderOpaque()));
	}
	pass P1
	{
		SetDepthStencilState(DisableDepthWrite, 0);
		SetVertexShader(CompileShader(vs_4_0, VShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderTransperant()));
	}
}

technique11 Selected{
	pass P0
	{
		SetDepthStencilState(EnableDepth, 0);
		SetVertexShader(CompileShader(vs_4_0, VShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderOpaqueSelected()));
	}
	pass P1
	{
		SetDepthStencilState(DisableDepthWrite, 0);
		SetVertexShader(CompileShader(vs_4_0, VShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderTransperantSelected()));
	}
}


struct VertexColorIn{
	float3 Pos : POSITION;
	float4 Color : COLOR;
};

struct VertexColorOut
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR;
};

VertexColorOut VShaderColor(VertexColorIn vIn)
{
	VertexColorOut output;

	output.Pos = mul(float4(vIn.Pos, 1.0f), WVP);
	output.Color = vIn.Color;

	return output;
}

float4 PShaderColor(VertexColorOut input) : SV_TARGET
{
	return input.Color;
}

technique11 Color{
	pass P0
	{
		SetDepthStencilState(EnableDepth, 0);
		SetVertexShader(CompileShader(vs_4_0, VShaderColor()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderColor()));
	}
}