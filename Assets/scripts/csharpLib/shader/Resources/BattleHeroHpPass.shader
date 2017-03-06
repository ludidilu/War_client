﻿Shader "Custom/BattleHeroHpPass" {
	Properties {
	
		_MainTex("Main Texture", 2D) = ""{}
	}
	
	SubShader {
	
		Tags
		{
			"RenderType" = "Transparent"
			"Queue"="Transparent+20"
		}
		
		Pass{
		
			Blend SrcAlpha OneMinusSrcAlpha  
            //ZWrite Off 
            ZTest Off
            
		
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata{
			
				float4 vertex:POSITION;
				float4 tangent:TANGENT;
				float2 uv:TEXCOORD;
			};
			
			struct v2f{
			
				float4 pos:POSITION;
				float2 uv:TEXCOORD;
				float3 color : COLOR0;
			};


			sampler2D _MainTex;
			
			float4 positions[20];
			float4 stateInfo[20];
			float4 fix[20];
			//float4x4 myMatrix[20];
			float4 scaleFix[20];
			
			v2f vert(appdata v){
			
				v2f o;
				
				float index = v.tangent.x;
				float4 vt1;
				float4 vt2;

				vt1.x = v.tangent.y * fix[index].x;
				vt2.x = v.uv.x + vt1.x;
				vt2.y = v.uv.y;
				
				vt1.x = v.tangent.z * fix[index].z;
				vt2.x = vt2.x + vt1.x;
				
				o.uv = vt2.xy;
				
				//坐标
				float4 targetPos;
				
				//血条
				vt1.x = v.tangent.y * fix[index].y;
				vt2.x = v.vertex.x + vt1.x;
				//怒气
				vt1.x = v.tangent.z * fix[index].w;
				vt2.x = vt2.x + vt1.x;
				
				
				vt2.y = v.vertex.y;
				vt2.z = v.vertex.z;
				vt2.w = v.vertex.w;
				
				vt2 = vt2 * stateInfo[index].y;
				
				targetPos = positions[index];
				
				//vt2 = mul(myMatrix[index], vt2);

//				float4x4 tempMatrix;
//
//				tempMatrix[0][0] = scaleFix[index].x; tempMatrix[0][1] = 0; tempMatrix[0][2] = 0; tempMatrix[0][3] = 0;
//				tempMatrix[1][0] = 0; tempMatrix[1][1] = scaleFix[index].y; tempMatrix[1][2] = 0; tempMatrix[1][3] = 0;
//				tempMatrix[2][0] = 0; tempMatrix[2][1] = 0; tempMatrix[2][2] = scaleFix[index].z; tempMatrix[2][3] = 0;
//				tempMatrix[3][0] = 0; tempMatrix[3][1] = 0; tempMatrix[3][2] = 0; tempMatrix[3][3] = 1;

				vt2 = scaleFix[index] * vt2;
               	
               	o.pos = mul(UNITY_MATRIX_P, (mul(UNITY_MATRIX_MV , targetPos) + vt2));
				o.color.xyz = stateInfo[index].x;
				
				
				return o;
			}
			
			half4 frag(v2f o):COLOR{
			
				half4 h = tex2D(_MainTex,o.uv);
			
				h.w = h.w * o.color.x;
				return h;
				//return half4( o.color, 1 );
			}
			
			ENDCG
		
		}
	} 
	
	FallBack "Mobile/Diffuse"
}
