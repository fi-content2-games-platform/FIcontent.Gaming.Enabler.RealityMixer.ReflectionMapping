Shader "Custom/GlassVTShader" 
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" { }
	}
	
	SubShader 
	{
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
			
			
			// Refracted directions for each colour channel separately
			float3 Tr, Tg, Tb;
			
			float fresnelTerm(float r, float3 N) // r = refraction indices ratio
			{
				// Schlick 1993 approximation of fresnel term
				float c = dot(V, N);
				float F0 = ( (r - 1.0)*(r - 1.0) ) / ( (r + 1.0)*(r + 1.0) );
				float F = F0 + pow(1.0 - c, 5.0) * (1.0 - F0);
				return F;
			}
			
			// Returns refraction color including chromatic abberation
			float3 refractionColorWithChromaticAbberation()
			{  	
				float3 col;
				col.r = mirrorBallIndexing(_SpecularTex, Tr, _RegionSpecular).r;
				col.g = mirrorBallIndexing(_SpecularTex, Tg, _RegionSpecular).g;
				col.b = mirrorBallIndexing(_SpecularTex, Tb, _RegionSpecular).b;
				
				return col;
			}
			
			// Vertex shader, only sets normal and view direction to be interpolated
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);			    
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    
			    // Set viewspace direction and normal
			    o.viewDir = -o.pos;
			    o.normal =  mul (UNITY_MATRIX_IT_MV, float4(v.normal, 1));	
			    
			    return o;
			}
			
			// Fragment shader
			float4 frag (v2f i) : COLOR
			{
				// Set these as global variables for ease of use
				float3 N = normalize(i.normal);
				V = normalize(i.viewDir);
				R = reflect(V, N);
				
				// Needed for the chromatic abberation effect
				float3 refractionIndices = float3(1.45, 1.5, 1.55);
				Tr = refract(V, N, 1.0003 / refractionIndices.r);
				Tg = refract(V, N, 1.0003 / refractionIndices.g);
				Tb = refract(V, N, 1.0003 / refractionIndices.b);
				
				float4 albedoTextureColor = tex2D (_MainTex, i.uv);
				
				
				// Just compute one Fresnel term and use it for all 3 colours
				float F = fresnelTerm(refractionIndices.g, N);
				float t = 1.0 / (F + 1.0);
	
			    return float4( (1-t) * refractionColorWithChromaticAbberation() + t * reflectionColor(N), 1) * albedoTextureColor;
			}
			
			ENDCG
	
	    }
	}
	
	Fallback "VertexLit"
} 