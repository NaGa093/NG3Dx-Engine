//=============================================================================
// model.fx by NaGa (C) 2014 All Rights Reserved.
//
// Effect used to shade model.
//=============================================================================

#include "light.fx"

Texture2D texColorMap;

cbuffer cbPerFrame
{
	DirectionalLight gDirLights;
	float3 gEyePosW;
}

cbuffer cbPerObject
{
	float4x4 WorldViewProjection;
	float4 modelColor;
	Material gMaterial;
}; 

SamplerState samLinearWrap
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VS_Input
{
    float3 position : POSITION;
	float3 normal : NORMAL;
	float2 texCoord : TEXCOORD;
};

struct VS_Output
{
    float4 position : SV_POSITION;
	float3 normal : NORMAL;
	float2 texCoord : TEXCOORD;
};

VS_Output VShader(VS_Input input)
{
	VS_Output Out; 
	Out.position = mul(float4(input.position,1.0f),WorldViewProjection);
	Out.normal = mul(input.normal,(float3x3)WorldViewProjection);
	Out.texCoord = input.texCoord;
	return Out;
}

float4 PShaderTexture(VS_Output Input) : SV_TARGET
{
	// Interpolating normal can unnormalize it, so normalize it.
    Input.normal = normalize(Input.normal);

	// The toEye vector is used in lighting.
	float3 toEye = gEyePosW - (float3)Input.position;

	// Cache the distance to the eye from this surface point.
	float distToEye = length(toEye);

	// Normalize.
	toEye /= distToEye;


	float4 ambient = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 diffuse = float4(0.0f, 0.0f, 0.0f, 0.0f);
	float4 spec    = float4(0.0f, 0.0f, 0.0f, 0.0f);

	ComputeDirectionalLight(gMaterial, gDirLights, Input.normal, toEye, ambient, diffuse, spec);
	float4 litColor = texColorMap.Sample(samLinearWrap, Input.texCoord);//*(2 + ambient + diffuse) + spec;

	/*float4 litColor = (texColorMap.Sample(samLinearWrap, Input.texCoord) *
					  (gMaterial.Ambient + gMaterial.Diffuse) + gMaterial.Specular) * 
					  float4(modelColor.rgb, gMaterial.TransmissionFilter);*/
	return litColor;
}

float4 PShader(VS_Output Input) : SV_Target
{
	float4 color = float4(modelColor.rgb,gMaterial.TransmissionFilter);
	return color;
}

RasterizerState SolidState
{
	CullMode = None;
	FillMode = Solid;
};

BlendState blend
{	
	BlendEnable[0] = TRUE;
    DestBlend = INV_SRC_ALPHA;
    SrcBlend= SRC_ALPHA;	
	BlendOp=Add;
};

technique10 Render
{
	pass P0
	{		
		SetBlendState(blend, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_5_0, VShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, PShader()));
		SetRasterizerState(SolidState);
	}

	pass P1
	{
		SetBlendState(blend, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_5_0, VShader()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, PShaderTexture()));
		SetRasterizerState(SolidState);
	}
}