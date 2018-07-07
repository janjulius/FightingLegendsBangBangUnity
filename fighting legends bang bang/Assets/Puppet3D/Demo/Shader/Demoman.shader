Shader "Puppet3D/Demoman" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	    _Cube ("Cubemap", CUBE) = "" {}
		_RimStrength ("Rim Strength", float) = 3.0
		_RimPower ("Rim Power", Range(0.5,2.0)) = 3.0
		_RimColor ("Rim Color",color) = (1,1,1,1)

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
			float4 vertexColor;
			          float3 worldRefl;
					            float3 viewDir;

		};
		 void vert (inout appdata_full v, out Input o)
         {
             UNITY_INITIALIZE_OUTPUT(Input,o);
             o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
         }
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		      samplerCUBE _Cube;
      float _RimPower,_RimStrength;
	  float4 _RimColor;
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c =IN.vertexColor * _Color;
			o.Albedo = c.rgb;
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			float3 tex3d = texCUBE (_Cube, IN.worldRefl).rgb;
           o.Emission =lerp(rim*_Metallic, _RimColor*_RimStrength*pow (rim, _RimPower)*_Metallic*tex3d , c.r);

			// Metallic and smoothness come from slider variables
			o.Metallic = lerp(1,_Metallic, c.r) ;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
