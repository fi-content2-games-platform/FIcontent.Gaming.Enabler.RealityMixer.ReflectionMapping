Shader "Custom/DiffuseVT" 
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" { }
	}
	
	SubShader 
	{
        Tags{"foo"="bar"}
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			
			#include "../VideoTextureShaderDefinitions.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
				
			struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD1;
            };
            
			struct v2f {
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			    float3 normal : TEXCOORD2;
			    float3 viewDir : TEXCOORD1;
			};
			
			// Vertex shader, only sets normal and view direction to be interpolated
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);			    
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    
			    // Set viewspace direction (after applying the perspective shift) and normal
			    o.normal =  mul (UNITY_MATRIX_IT_MV, float4(v.normal, 1));	
			    o.viewDir = -o.pos;
			    
			    //o.normal =  mul (UNITY_MATRIX_IT_MV, mul(_PerspectiveShiftMatrixSpecIT , float4(v.normal, 1)) );	
			    //o.viewDir = - mul( UNITY_MATRIX_P, mul( _PerspectiveShiftMatrixSpec, mul (UNITY_MATRIX_MV, v.vertex)));
			    return o;
			}
			
			// Fragment shader
			float4 frag (v2f i) : COLOR
			{
				// These need to be set for the global functions to work!
				float3 N = normalize(i.normal); // normal direction
				V = normalize(i.viewDir); // view direction
				R = reflect(V, N); // reflected direction
				T = refract(V, N, 1.00003 / 1.5); // refracted direction
				
				
				float3 albedoTextureColor = tex2D (_MainTex, i.uv).rgb;
				
				return float4(diffuseColor(N) * albedoTextureColor, 1);
			}
			
			ENDCG
	
	    }
	}
	
	Fallback "VertexLit"
} 