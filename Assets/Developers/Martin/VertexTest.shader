Shader "Custom/VertexTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
			
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_PlanetSize ("Planet Size", float) = 1000
		_DistanceScale ("Distance Scale", float) = 1000
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float _PlanetSize;
		float _DistanceScale;

		float3 WarpPoint(float3 p)
		{
			float PI = 3.14159265359;

			// Position to lat lon
			float latitude = max(PI / 2 - (length(p.xz) / _DistanceScale), -PI / 2);
			float longitude = atan2(p.x, p.z);
			float altitude = p.y;

			// Lat lon to Cartesian (new) coords
			float cosL = cos(latitude);
			float3 cartesian = float3(
				cosL * sin(longitude),
				sin(latitude),
				cosL * cos(longitude));

			return cartesian * (altitude + _PlanetSize) - float3(0, _PlanetSize, 0);
		}

		void vert (inout appdata_full v)
		{
			float3 oldPosition = v.vertex.xyz;

			v.vertex.xyz = WarpPoint(v.vertex.xyz);

			//float3 normal = WarpPoint(oldPosition + v.normal.xyz);
			//v.normal.xyz = normal - v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			//o.Normal = IN.normal;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = 0;//_Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
