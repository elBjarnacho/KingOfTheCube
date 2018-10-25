﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SmokeShader"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_Transparency("Transparency", Range(0.0,0.5)) = 0.25
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent"}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex SmokeVertexProgram
				#pragma fragment SmokeFragmentProgram

				#include "UnityCG.cginc"

				float4 _Tint;
				float _Transparency;

				struct Interpolators
				{
					float4 position : SV_POSITION;
					float3 localPosition : TEXCOORD0;
					float3 worldPosition : TEXCOORD1;
				};

				Interpolators SmokeVertexProgram(float4 position : POSITION) 
				{
					Interpolators i;
					i.localPosition = position.xyz;					
					i.position = UnityObjectToClipPos(position);

					float3 worldPos = mul(unity_ObjectToWorld, position).xyz;
					i.worldPosition = worldPos;

					return i;
				}

				float4 SmokeFragmentProgram(Interpolators i) 
				: SV_TARGET {					
					//float alpha = (1.0 - i.worldPosition.y / 50.0); //For Unity
					float alpha = (0.8 - i.worldPosition.y); //For AR
					
					float3 color = (0.2 - i.localPosition.y) + 0.4;
					float4 col = float4(color, alpha);
					
					//float color2 = (10.0 - i.worldPosition.y) / 10.0;
					//float color1 = (i.worldPosition.y - 10.0) / 10.0;
					//float4 col = float4(color1, color1, color1, 1);					
					//col.a = ((1 - i.position.y) / 2);

					return col;
				}
			ENDCG
		}
	}
}