
Shader "Custom/CartoonSky" 
{
	Properties 
	{
		[HDR]_TopColor("Top Color", Color) = (0.1, 0.1, 0.5, 0)
		[HDR]_BottomColor ("Bottom Color", Color) = (0.5,0.5,0.8,0)
		_GradientPower ("Gradient Power", Float) = 1
		[HDR]_SunColor ("Sun Color", Color) = (1.5,1.5,1.4,0)
		_SunSize ("Sun Size", Float) = 1
		[HDR]_ScatteringColor("Scattering Color", Color) = (0.1, 0.1, 0.5, 0)
		_ScatteringPower ("Scattering Power", Float) = 1
		
	}
	SubShader
	{
		Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
		Cull Off ZWrite Off

		Pass
	{
		CGPROGRAM
	#pragma target 5.0
	#pragma vertex vert
	#pragma fragment frag

	#include "UnityCG.cginc"
	#include "Lighting.cginc"

	uniform float3 _TopColor;
	uniform float3 _BottomColor;
	uniform float _GradientPower;
	uniform float3 _SunColor;
	uniform float _SunSize;
	uniform float3 _ScatteringColor;
	uniform float _ScatteringPower;

#if defined(UNITY_COLORSPACE_GAMMA)
#define LINEAR_TO_OUTPUT(color) sqrt(color)
#else
#define LINEAR_TO_OUTPUT(color) color
#endif

	struct appdata_t
	{
		float4 vertex : POSITION;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		half3 view : TEXCOORD0;

		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert (appdata_t v)
	{
		v2f OUT;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		OUT.pos = UnityObjectToClipPos(v.vertex);
		OUT.view = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
		return OUT;
	}

	half4 frag (v2f IN) : SV_Target
	{
		float3 view_ray = normalize(IN.view.xyz);
		float3 sun_direction = _WorldSpaceLightPos0.xyz;

		float3 result = lerp(_TopColor, _BottomColor, pow(saturate(1 - view_ray.y), _GradientPower));

		result += lerp(0, _ScatteringColor, pow(saturate(dot(sun_direction, view_ray)), _ScatteringPower));

		bool reflection_capture = any(_LightColor0.xyz == half3(0, 0, 0));
		if (!reflection_capture && dot(sun_direction, view_ray) > _SunSize)
		{
			result += _SunColor;
		}

		result = LINEAR_TO_OUTPUT(result);

		return half4(result, 1.0f);
	}
		ENDCG
	}

	}

		FallBack Off
}
