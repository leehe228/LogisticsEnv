// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PA/SpaceProbeAnimated"
{
	Properties
	{
		_Low_emisson("Low_emisson", 2D) = "white" {}
		[HDR]_EmmissionColor("Emmission Color", Color) = (1,0.8078431,0,1)
		_Mask("Mask", 2D) = "white" {}
		_FrontLEDSpeed("FrontLEDSpeed", Float) = 6
		_FrontLEDRandomize("FrontLEDRandomize", Range( 0 , 0.5)) = 0.3
		_SideLEDSpeed("SideLEDSpeed", Float) = 6
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
		uniform sampler2D _Mask;
		uniform float _FrontLEDRandomize;
		uniform float _FrontLEDSpeed;
		uniform float4 _Mask_ST;
		uniform sampler2D _Low_emisson;
		uniform float4 _Low_emisson_ST;
		uniform float _SideLEDSpeed;
		uniform float4 _EmmissionColor;


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
			float4 screenColor413 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
			float temp_output_239_0 = ( _FrontLEDRandomize * _Time.x );
			float2 panner233 = ( float2( 0.5,0 ) + temp_output_239_0 * float2( 1,1 ));
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode155 = tex2D( _Mask, uv_Mask );
			float2 panner231 = ( float2( 0,0.5 ) + temp_output_239_0 * float2( 1,1 ));
			float2 panner228 = ( float2( 0,0 ) + temp_output_239_0 * float2( 1,1 ));
			float2 uv_Low_emisson = i.uv_texcoord * _Low_emisson_ST.xy + _Low_emisson_ST.zw;
			float4 tex2DNode4 = tex2D( _Low_emisson, uv_Low_emisson );
			float2 uv_TexCoord379 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float temp_output_387_0 = sin( ( _Time.y * _SideLEDSpeed ) );
			float lerpResult383 = lerp( -0.495 , -0.45 , temp_output_387_0);
			float lerpResult394 = lerp( -0.13 , -0.08 , -temp_output_387_0);
			float2 uv_TexCoord392 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			o.Emission = ( screenColor413 + ( max( ( ( round( ( sin( ( ( tex2D( _Mask, panner233 ).a + _Time.z ) * _FrontLEDSpeed ) ) + 0.8 ) ) * tex2DNode155.r ) + ( tex2DNode155.g * round( ( sin( ( _FrontLEDSpeed * ( tex2D( _Mask, panner231 ).a + _Time.z ) ) ) + 0.8 ) ) ) + ( tex2DNode155.b * round( ( sin( ( _FrontLEDSpeed * ( tex2D( _Mask, panner228 ).a + _Time.z ) ) ) + 0.8 ) ) ) ) , max( ( tex2DNode4.a * (0 + (abs( ( uv_TexCoord379.y + lerpResult383 ) ) - 0.03) * (1 - 0) / (0.01 - 0.03)) ) , ( tex2DNode4.a * (0 + (abs( ( lerpResult394 + uv_TexCoord392.y ) ) - 0.03) * (1 - 0) / (0.01 - 0.03)) ) ) ) * 1.5 * _EmmissionColor * 3.0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15101
535;375;1416;680;213.9185;-734.8129;1.287516;True;False
Node;AmplifyShaderEditor.TimeNode;179;-1502.24,1138.115;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;240;-1569.608,1051.563;Float;False;Property;_FrontLEDRandomize;FrontLEDRandomize;4;0;Create;True;0;0;False;0;0.3;0.3;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;384;-1327.448,1668.816;Float;False;Property;_SideLEDSpeed;SideLEDSpeed;5;0;Create;True;0;0;False;0;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;-1299.523,1056.304;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;386;-1139.649,1650.935;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;228;-1122.987,1134.715;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;231;-1125.623,1010.217;Float;False;3;0;FLOAT2;0,0.5;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;233;-1125.131,884.0399;Float;False;3;0;FLOAT2;0.5,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;387;-1011.198,1652.802;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;234;-932.7626,781.0701;Float;True;Property;_TextureSample2;Texture Sample 2;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;155;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;227;-933.4722,1179.898;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;155;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;232;-932.4017,981.9694;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;155;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;400;-897.7,1651.398;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-603.1528,1105.212;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-605.2745,991.3508;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;207;-604.4517,878.6761;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;-833.6393,704.0293;Float;False;Property;_FrontLEDSpeed;FrontLEDSpeed;3;0;Create;True;0;0;False;0;6;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;392;-845.812,1720.753;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;379;-845.44,1378.621;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;394;-769.5969,1606.101;Float;False;3;0;FLOAT;-0.13;False;1;FLOAT;-0.08;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-480.8648,992.8262;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;208;-482.9263,877.5057;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;-474.744,1103.208;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;383;-768.8347,1491.748;Float;False;3;0;FLOAT;-0.495;False;1;FLOAT;-0.45;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;393;-609.817,1606.531;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;182;-346.5108,878.5528;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;380;-614.4502,1467.916;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;180;-343.1849,992.3503;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;188;-346.744,1103.208;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;381;-494.6811,1468.385;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;396;-492.1318,1606.198;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;261;-220.7549,879.0308;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;259;-212.2243,1106.588;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;260;-216.7186,992.4456;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;189;-90.52534,1107.271;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;172;-90.16024,992.2916;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;390;-376.7739,1469.17;Float;False;5;0;FLOAT;0;False;1;FLOAT;0.03;False;2;FLOAT;0.01;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;183;-93.64156,878.7215;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;397;-376.515,1634.149;Float;False;5;0;FLOAT;0;False;1;FLOAT;0.03;False;2;FLOAT;0.01;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;155;-625.4744,1219.937;Float;True;Property;_Mask;Mask;2;0;Create;True;0;0;False;0;3531a04b92a54644a84a1ba02d87ac73;3531a04b92a54644a84a1ba02d87ac73;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1125.381,1376.783;Float;True;Property;_Low_emisson;Low_emisson;0;0;Create;True;0;0;False;0;3cbbff75cf3011540898651c3f94b5a1;3cbbff75cf3011540898651c3f94b5a1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;398;-194.7206,1568.265;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;32.63275,1103.007;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;32.03059,878.4313;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;391;-196.2127,1471.927;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;32.13008,989.9047;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;177;191.5014,965.824;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;401;-48.01715,1486.57;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;259.2023,1152.699;Float;False;Property;_EmmissionColor;Emmission Color;1;1;[HDR];Create;True;0;0;False;0;1,0.8078431,0,1;1,0.8078431,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;408;319.0355,1317.688;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;416;338.6237,965.3248;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;191.3492,1079.058;Float;False;Constant;_EmmissionValue;Emmission Value;2;0;Create;True;0;0;False;0;1.5;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;409;497.5061,1067.795;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;413;530.2278,900.3984;Float;False;Global;_GrabScreen0;Grab Screen 0;6;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;411;725.2629,994.0613;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;875.1203,948.0823;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;SpaceProbeAnimated;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;239;0;240;0
WireConnection;239;1;179;1
WireConnection;386;0;179;2
WireConnection;386;1;384;0
WireConnection;228;1;239;0
WireConnection;231;1;239;0
WireConnection;233;1;239;0
WireConnection;387;0;386;0
WireConnection;234;1;233;0
WireConnection;227;1;228;0
WireConnection;232;1;231;0
WireConnection;400;0;387;0
WireConnection;191;0;227;4
WireConnection;191;1;179;3
WireConnection;185;0;232;4
WireConnection;185;1;179;3
WireConnection;207;0;234;4
WireConnection;207;1;179;3
WireConnection;394;2;400;0
WireConnection;175;0;184;0
WireConnection;175;1;185;0
WireConnection;208;0;207;0
WireConnection;208;1;184;0
WireConnection;187;0;184;0
WireConnection;187;1;191;0
WireConnection;383;2;387;0
WireConnection;393;0;394;0
WireConnection;393;1;392;2
WireConnection;182;0;208;0
WireConnection;380;0;379;2
WireConnection;380;1;383;0
WireConnection;180;0;175;0
WireConnection;188;0;187;0
WireConnection;381;0;380;0
WireConnection;396;0;393;0
WireConnection;261;0;182;0
WireConnection;259;0;188;0
WireConnection;260;0;180;0
WireConnection;189;0;259;0
WireConnection;172;0;260;0
WireConnection;390;0;381;0
WireConnection;183;0;261;0
WireConnection;397;0;396;0
WireConnection;398;0;4;4
WireConnection;398;1;397;0
WireConnection;192;0;155;3
WireConnection;192;1;189;0
WireConnection;165;0;183;0
WireConnection;165;1;155;1
WireConnection;391;0;4;4
WireConnection;391;1;390;0
WireConnection;176;0;155;2
WireConnection;176;1;172;0
WireConnection;177;0;165;0
WireConnection;177;1;176;0
WireConnection;177;2;192;0
WireConnection;401;0;391;0
WireConnection;401;1;398;0
WireConnection;416;0;177;0
WireConnection;416;1;401;0
WireConnection;409;0;416;0
WireConnection;409;1;5;0
WireConnection;409;2;7;0
WireConnection;409;3;408;0
WireConnection;411;0;413;0
WireConnection;411;1;409;0
WireConnection;0;2;411;0
ASEEND*/
//CHKSM=30C1AA12E1F16339527BE15ACA4E4AA06E77CA86