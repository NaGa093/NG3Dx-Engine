//=============================================================================
// grid.fx by NaGa (C) 2014 All Rights Reserved.
//
// Effect used to shade grid.
//=============================================================================

matrix WorldViewProjection;
Texture2D texColorMap;
float4 Color;

struct VS_Output
{
	float4 position : SV_POSITION;
	float4 vNormal : NORMAL;
	float2 Txr : TEXCOORD0;
};

sampler samLinearWrap = sampler_state
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

RasterizerState WireframeState
{
	CullMode = None;
	FillMode = Solid;
};

VS_Output VShader(float3 position : POSITION, float3 vNormal : NORMAL, float2 Txr : TEXCOORD)
{
	VS_Output Out;
	Out.position = mul(float4(position, 1), WorldViewProjection);
	Out.vNormal = mul(float4(vNormal, 1), WorldViewProjection);
	Out.Txr = Txr;
	return Out;
}

float4 PShaderTexture(VS_Output Input) : SV_TARGET
{
	//return Color;
	return texColorMap.Sample(samLinearWrap, Input.Txr);
}

/*float4 VShader(float4 position : POSITION) : SV_POSITION
{
	return mul(position, WorldViewProjection);
}

float4 PShader(float4 position : SV_POSITION) : SV_Target
{
	return Color;
}*/

technique10 Render
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_5_0, VShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, PShaderTexture()));
		SetRasterizerState(WireframeState);
	}
}