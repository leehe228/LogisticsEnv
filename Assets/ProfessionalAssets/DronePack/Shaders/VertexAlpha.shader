Shader "Unlit/VertexAlpha"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
		};

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha * 0.8f);
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 temp_output_1_0 = i.vertexColor;
			o.Emission = temp_output_1_0.rgb;
			o.Alpha = temp_output_1_0.r;
		}

		ENDCG
	}
}
