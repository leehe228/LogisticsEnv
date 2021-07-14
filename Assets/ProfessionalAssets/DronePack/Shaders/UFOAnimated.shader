// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PA/UFOAnimated"
{
	Properties
	{
		[HDR]_EmissionMap("EmissionMap", 2D) = "white" {}
		_EmissionValue("EmissionValue", Float) = 1
		_LEDspeed("LED speed", Range( 0 , 1.5)) = 0.3
		[HDR]_Tint("Tint", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _GrabTexture;
		uniform sampler2D _EmissionMap;
		uniform float4 _EmissionMap_ST;
		uniform float4 _Tint;
		uniform float _LEDspeed;
		uniform float _EmissionValue;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor7 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			float2 uv_TexCoord26 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float lerpResult29 = lerp( -0.9 , 0 , frac( ( _Time.y * _LEDspeed ) ));
			o.Emission = ( screenColor7 + ( ( ( (0.8 + (sin( ( _Time.y * 2 ) ) - 0) * (1 - 0.8) / (1 - 0)) * tex2D( _EmissionMap, uv_EmissionMap ) ) + ( _Tint * tex2D( _EmissionMap, uv_EmissionMap ).a * ceil( ( uv_TexCoord26.y + lerpResult29 ) ) ) ) * _EmissionValue * 3.0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15101
86;209;1416;680;2132.678;146.0866;1.492988;True;False
Node;AmplifyShaderEditor.TimeNode;40;-1854.616,-233.9257;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;-1922.217,583.3908;Float;False;Property;_LEDspeed;LED speed;2;0;Create;True;0;0;False;0;0.3;0.3;0;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1632.411,566.5098;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;45;-1501.586,566.3087;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-1387.538,-211.5112;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-1464.458,396.4433;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;29;-1370.316,611.1428;Float;False;3;0;FLOAT;-0.9;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-1242.925,498.3945;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;98;-1256.402,-210.4189;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1243.146,-50.2402;Float;True;Property;_EmissionMap;EmissionMap;0;1;[HDR];Create;True;0;0;False;0;c2c45cd6fbd8fc54e89c824b2a06c80f;c2c45cd6fbd8fc54e89c824b2a06c80f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;53;-1155.515,137.4338;Float;False;Property;_Tint;Tint;3;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0.6666667,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;92;-1242.988,304.238;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CeilOpNode;46;-1117.271,493.4141;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;99;-1139.571,-210.1205;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.8;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-930.6959,-67.48456;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-929.0049,142.2155;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-669.1849,236.0572;Float;False;Property;_EmissionValue;EmissionValue;1;0;Create;True;0;0;False;0;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-689.7576,26.687;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-623.0529,307.4931;Float;False;Constant;_HDRvalue;HDR value;1;0;Create;True;0;0;False;0;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;7;-435.1078,51.26828;Float;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-447.5137,220.3258;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-241.4956,148.6489;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;102;-113.5396,102.546;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Custom/UFOanimated;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;23;0;40;2
WireConnection;23;1;22;0
WireConnection;45;0;23;0
WireConnection;97;0;40;2
WireConnection;29;2;45;0
WireConnection;30;0;26;2
WireConnection;30;1;29;0
WireConnection;98;0;97;0
WireConnection;46;0;30;0
WireConnection;99;0;98;0
WireConnection;95;0;99;0
WireConnection;95;1;2;0
WireConnection;47;0;53;0
WireConnection;47;1;92;4
WireConnection;47;2;46;0
WireConnection;101;0;95;0
WireConnection;101;1;47;0
WireConnection;3;0;101;0
WireConnection;3;1;4;0
WireConnection;3;2;6;0
WireConnection;14;0;7;0
WireConnection;14;1;3;0
WireConnection;102;2;14;0
ASEEND*/
//CHKSM=A906D7DB15DFF38B07B4047997F3DBEFE60DE16E