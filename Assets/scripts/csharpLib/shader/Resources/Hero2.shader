Shader "Custom/Hero2" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "black" {}
//		_EmmiPower ("Self Emmision Power", Range(0,1)) = 0.5
		
		_OutlineColor ("Outline Color", Color) = (1,1,1,0.5)
//		_Outline ("Outline width", Range (0.0, 0.02)) = 0.0002
		
		_BumpMap ("Bumpmap", 2D) = "bump" {}
//		_RimColor ("Rim Color", Color) = (1,1,1,0.0)
//		_RimPower ("Rim Power", Range(0.5,8.0)) = 3
//		
//		_CullNormal("Rim Cull Norm", Range(0,1)) = 0.5
//		_CullSide ("Rim Cull Side", Range(0,1)) = 0.5
		
		_PartIndex("Part Index", Float) = 0.0
		_WeaponIndex("Weapon Index", Float) = 0.0
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"
		
		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};
		struct v2f {
			float4 pos : POSITION;
			float4 color : COLOR;
		};
		uniform float _Outline;
		uniform float4 _OutlineColor;

		v2f vertOutline(appdata v) {
			v2f o;
			
			_Outline = 0.002;
			_OutlineColor = float4(0,0,0,0.3921);
			
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
			float2 offset = TransformViewToProjection(norm.xy);
			o.pos.xy += offset * o.pos.z * _Outline;
			o.color = _OutlineColor;

			return o;
		}
		
	ENDCG
	
	SubShader {

		
	
		Tags {"Queue" = "Geometry"}
//		Blend [_SrcAlpha] [_TarAlpha]  
//		ZWrite [_ZWrite]
		Cull Off
		Offset 0,-20000
         
		CGPROGRAM
		#pragma vertex vertModel
		#pragma surface surf Lambert
		#pragma target 3.0
		 
		sampler2D _MainTex;
		sampler2D _BumpMap;
//		float4 _RimColor;
//		float _RimPower;
//		float _CullSide;
//		float _CullNormal;
		
		int _PartIndex;
		int _WeaponIndex;
		fixed4 _Color;
		
		 
		struct Input {
			float2 uv_MainTex;

			float2 uv_BumpMap;
			float3 viewDir;
			float3 norm;
		};
		//rim
		
		void vertModel (inout appdata_full v, out Input o) {
		
            UNITY_INITIALIZE_OUTPUT(Input,o);
            
            if(v.texcoord1.x > 0 && v.texcoord1.x != _PartIndex){
			
				v.vertex.x = 0;
				v.vertex.y = 0;
				v.vertex.z = 0;
				
			}else if(v.texcoord1.x < 0 && v.texcoord1.x != _WeaponIndex){
			
				v.vertex.x = 0;
				v.vertex.y = 0;
				v.vertex.z = 0;
				
			}
			o.norm  = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
        }
        
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color;
			o.Alpha = c.a;// * _AlphaXXX;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
//			half rim = (1 - dot (normalize(IN.viewDir), o.Normal)) * _CullNormal + abs(_CullSide * dot (normalize(IN.viewDir), IN.norm));
//			rim = saturate(rim);
//			o.Emission = _RimColor.rgb * pow (rim, _RimPower);
		}
		ENDCG
 
 
 		//Outline//
         // note that a vertex shader is specified here but its using the one above
		Pass {
			Name "OUTLINE"
			Tags { "RenderType"="Transparent"}
			
			Cull Off
//			ZWrite Off
			
			ZTest LEqual
			Offset 0,-50
		
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
	
			CGPROGRAM
			#pragma vertex vertOutline
			#pragma fragment frag
			half4 frag(v2f i) :COLOR 
			{ 
				return i.color; 
			}
			ENDCG
		}
     }
	
	FallBack "Diffuse"
}
