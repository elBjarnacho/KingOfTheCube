﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ExplosionShader"
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_Origin("Origin", Vector) = (0, 0, 0, 0)
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
				float4 _Origin;
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
					//float alpha = (0.7 - i.worldPosition.y); //For AR
					//float alpha = (0.7 - ((i.worldPosition.y) - _Origin.y)); //For AR					
					float alpha = 0.7;
					
					float red = 1;
					//float yellow = 0.5 + ((i.worldPosition.x - _Origin.x)*(i.worldPosition.x - _Origin.x));
					float green = 0.5 + (abs(i.worldPosition.x - _Origin.x)
										+abs(i.worldPosition.y - _Origin.y)
										+abs(i.worldPosition.z - _Origin.z)) * 5; // / 6 (good for Unity)
					float4 col = float4(red, green, 0, alpha);
					
					return col;
				}
			ENDCG
		}
	}
}
