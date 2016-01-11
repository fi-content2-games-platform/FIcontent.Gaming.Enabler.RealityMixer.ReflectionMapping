#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Custom/AOBumpDiffuseOpaqueVT" 
{
 Properties
 {
        _Shininess ("Shininess", Range(0,1)) = 0.2
        _MainTex ("Main Texture", 2D) = "white" {}
        _BumpMap ("Normal Texture", 2D) = "bump" {}
        _SpecMap ("Specular Map", 2D) = "black" {}
        //_SideShadingMap ("Side Shading Map", 2D) = "white" {}
        _AOMap ("Ambient Occlusion Map", 2D) = "white" {}
        
//        _RSFactor ("RSFactor", Range(0,1)) = 0
//        _LSFactor ("LSFactor", Range(0,1)) = 0
        
 }
 
 SubShader 
 {
    Tags {"IgnoreProjector"="True" }

    Pass 
    {
       // Blend SrcAlpha OneMinusSrcAlpha, One One
        Cull Off
        CGPROGRAM
        #pragma exclude_renderers d3d11 xbox360 d3d11_9x 
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0
        
        #include "UnityCG.cginc"
        
        #include "../VideoTextureShaderDefinitions.cginc"
        
        float _Shininess;
        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _SpecMap;
        //sampler2D _SideShadingMap;
        sampler2D _AOMap;
        
        float4 _MainTex_ST;
        
        float _RSFactor, _LSFactor;
         
        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            half2 texcoord : TEXCOORD1;
       };
        
        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 viewDir	: TEXCOORD1;
			//float3	I		: TEXCOORD2;
			float3	TtoW0 	: TEXCOORD2;
			float3	TtoW1	: TEXCOORD3;
			float3	TtoW2	: TEXCOORD4;
        };
        
        // Vertex shader, only sets normal and view direction to be interpolated
        v2f vert (appdata v)
        {
            v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);                
            o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
            
            // Set viewspace direction (after applying the perspective shift) and normal
           // o.normal =  mul (UNITY_MATRIX_IT_MV, float4(v.normal, 1));   
           // o.viewDir = -o.pos.xyz;

			//o.I = -WorldSpaceViewDir( v.vertex );
			
			TANGENT_SPACE_ROTATION;
			o.TtoW0 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz * 1.0);
			o.TtoW1 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz * 1.0);
			o.TtoW2 = mul(rotation, UNITY_MATRIX_IT_MV[2].xyz * 1.0);
			
            
            //o.normal =  mul (UNITY_MATRIX_IT_MV, mul(_PerspectiveShiftMatrixSpecIT , float4(v.normal, 1)) );   
            //o.viewDir = - mul( UNITY_MATRIX_P, mul( _PerspectiveShiftMatrixSpec, mul (UNITY_MATRIX_MV, v.vertex)));
            return o;
        }
 
//        fixed3 sideShadowing()
//        {
//            return min(
//                       lerp( 1
//                           , mirrorBallIndexing(_SideShadingMap, N, fixed4(0, 0, 1, 1))
//                           , _RSFactor)
//                      ,
//                       lerp( 1
//                           , mirrorBallIndexing(_SideShadingMap, N, fixed4(1, 0, -1, 1))
//                           , _LSFactor)
//                      );
//        }
 
        fixed3 sideShadowing(float3 N)
        {
            
            return min(lerp(fixed3(1,1,1), (1-(N.x)/2), _RSFactor), lerp(fixed3(1,1,1), (1-(-N.x)/2), _LSFactor));
        }
        
        // Fragment shader
        float4 frag (v2f i) : COLOR
        {
            // These need to be set for the global functions to work!
            float3 sN = normalize( UnpackNormal(tex2D(_BumpMap, i.uv) )  ); // normal direction 
            V = normalize(i.viewDir); // view direction
            
			// transform normal to world space 
            float3 wN;
            
            float3x3 NtoW = float3x3(i.TtoW0, i.TtoW1, i.TtoW2);
			float3 N = mul(NtoW, sN);    
           
            R = reflect(V, N); // reflected direction
            T = refract(V, N, 1.00003 / 1.5); // refracted direction
            
            float4 albedoTextureColor = tex2D (_MainTex, i.uv).rgba;
            float3 ambientOcclusion = lerp(tex2D (_AOMap, i.uv).rgb,float3(1,1,1), 0.5);
            float3 t = tex2D(_SpecMap, i.uv).rgb * _Shininess;
            
            //return float4((N+1)/2,albedoTextureColor.a);
            //return float4(N,1);
            //return float4(sideShadowing(),1);
            return float4(reflectionColor(N) * t + (diffuseColor(N) * sideShadowing(N) * albedoTextureColor.rgb * ambientOcclusion), albedoTextureColor.a);
            //return float4(diffuseColor(), albedoTextureColor.a);
        }
        
        ENDCG
 
     }
 }
 
 //Fallback "VertexLit"
} 