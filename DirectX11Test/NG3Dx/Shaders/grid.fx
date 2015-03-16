//=============================================================================
// grid.fx by NaGa (C) 2014 All Rights Reserved.
//
// Effect used to shade grid.
//=============================================================================

matrix WorldViewProjection;
float4 Color;

RasterizerState WireframeState
{
    FillMode = Wireframe;
};

float4 VShader(float4 position : POSITION) : SV_POSITION
{
	return mul(position, WorldViewProjection);
}

float4 PShader(float4 position : SV_POSITION) : SV_Target
{
	return Color;
}

technique10 Render
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_5_0, VShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, PShader()));
		SetRasterizerState(WireframeState);
	}
}