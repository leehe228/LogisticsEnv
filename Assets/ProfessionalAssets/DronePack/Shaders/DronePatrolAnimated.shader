// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PA/DronePatrolAnimated"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_BeamSpeed("BeamSpeed", Float) = 6
		[HDR]_Emmision("Emmision", Range( 0 , 3)) = 1.5
		_FlashSpeed("FlashSpeed", Float) = 4
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
		#pragma surface surf Unlit alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _GrabTexture;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _FlashSpeed;
		uniform float _Emmision;
		uniform float _BeamSpeed;


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
			float4 screenColor314 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode221 = tex2D( _TextureSample0, uv_TextureSample0 );
			float4 _Color1 = float4(1,0,0,0);
			float4 _Color0 = float4(0,0,1,0);
			float temp_output_322_0 = ( _Time.y * _FlashSpeed );
			float temp_output_300_0 = ceil( sin( temp_output_322_0 ) );
			float4 lerpResult310 = lerp( _Color1 , _Color0 , temp_output_300_0);
			float temp_output_271_0 = round( frac( temp_output_322_0 ) );
			float temp_output_237_0 = ( ( temp_output_271_0 * temp_output_300_0 ) * tex2DNode221.r );
			float temp_output_233_0 = ( ( temp_output_271_0 * ( 1.0 - temp_output_300_0 ) ) * tex2DNode221.b );
			float2 uv_TexCoord213 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float lerpResult214 = lerp( -0.32 , -0.23 , sin( ( _BeamSpeed * _Time.y ) ));
			o.Emission = max( screenColor314 , ( ( ( tex2DNode221.g * lerpResult310 * _Emmision ) + ( temp_output_237_0 * _Color1 * _Emmision ) + ( _Color0 * temp_output_233_0 * _Emmision ) ) * ( ( (0 + (abs( ( uv_TexCoord213.y + lerpResult214 ) ) - 0.03) * (1 - 0) / (0.001 - 0.03)) * tex2DNode221.g ) + temp_output_237_0 + temp_output_233_0 ) * 3.0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15101
342;308;1416;680;-138.862;-588.6973;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;323;453.8619,928.6973;Float;False;Property;_FlashSpeed;FlashSpeed;3;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;138;415.9862,790.7878;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;208;682.8123,429.196;Float;False;Property;_BeamSpeed;BeamSpeed;1;0;Create;True;0;0;False;0;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;849.2918,433.0044;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;322;622.517,816.4558;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;210;981.8234,433.8187;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;284;757.3667,857.9064;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;280;757.6707,789.5596;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;300;874.3268,857.3915;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;214;1097.723,388.5048;Float;False;3;0;FLOAT;-0.32;False;1;FLOAT;-0.23;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;213;1020.288,271.5908;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;299;989.2635,859.9178;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;271;874.6203,789.5889;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;215;1249.175,365.2351;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;238;1217.356,948.3969;Float;False;Constant;_Color1;Color 1;5;1;[HDR];Create;True;0;0;False;0;1,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;232;1022.282,949.3782;Float;False;Constant;_Color0;Color 0;5;1;[HDR];Create;True;0;0;False;0;0,0,1,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;1134.475,856.6114;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;281;1134.44,765.525;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;221;1098.209,503.4392;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;81951b890697e704dac4fcf1f3bdd1ab;81951b890697e704dac4fcf1f3bdd1ab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;216;1366.754,365.901;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;1279.659,857.7228;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;252;1142.06,690.2527;Float;False;Property;_Emmision;Emmision;2;1;[HDR];Create;True;0;0;False;0;1.5;1.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;237;1279.038,765.5084;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;310;1511.116,525.114;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;218;1484.123,365.7264;Float;False;5;0;FLOAT;0;False;1;FLOAT;0.03;False;2;FLOAT;0.001;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;247;1530.596,748.2329;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;250;1528.149,636.3107;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;304;1661.996,459.6221;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;222;1663.616,366.4412;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;311;1777.86,684.7834;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;249;1798.732,458.8943;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;302;1797.666,570.6686;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;312;1934.64,549.8859;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;314;1965.687,385.5149;Float;False;Global;_GrabScreen0;Grab Screen 0;4;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;316;2156.34,444.355;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;321;2277.458,396.1642;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;PA/DronePatrolAnimated;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;209;0;208;0
WireConnection;209;1;138;2
WireConnection;322;0;138;2
WireConnection;322;1;323;0
WireConnection;210;0;209;0
WireConnection;284;0;322;0
WireConnection;280;0;322;0
WireConnection;300;0;284;0
WireConnection;214;2;210;0
WireConnection;299;0;300;0
WireConnection;271;0;280;0
WireConnection;215;0;213;2
WireConnection;215;1;214;0
WireConnection;297;0;271;0
WireConnection;297;1;299;0
WireConnection;281;0;271;0
WireConnection;281;1;300;0
WireConnection;216;0;215;0
WireConnection;233;0;297;0
WireConnection;233;1;221;3
WireConnection;237;0;281;0
WireConnection;237;1;221;1
WireConnection;310;0;238;0
WireConnection;310;1;232;0
WireConnection;310;2;300;0
WireConnection;218;0;216;0
WireConnection;247;0;232;0
WireConnection;247;1;233;0
WireConnection;247;2;252;0
WireConnection;250;0;237;0
WireConnection;250;1;238;0
WireConnection;250;2;252;0
WireConnection;304;0;221;2
WireConnection;304;1;310;0
WireConnection;304;2;252;0
WireConnection;222;0;218;0
WireConnection;222;1;221;2
WireConnection;249;0;304;0
WireConnection;249;1;250;0
WireConnection;249;2;247;0
WireConnection;302;0;222;0
WireConnection;302;1;237;0
WireConnection;302;2;233;0
WireConnection;312;0;249;0
WireConnection;312;1;302;0
WireConnection;312;2;311;0
WireConnection;316;0;314;0
WireConnection;316;1;312;0
WireConnection;321;2;316;0
ASEEND*/
//CHKSM=CF0472872B6F198B919F236EDE9BC2F9DBF3DAA1