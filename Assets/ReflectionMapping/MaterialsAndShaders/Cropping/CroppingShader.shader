// Upgrade NOTE: replaced 'texRECT' with 'tex2D'

// Upgrade NOTE: replaced 'samplerRECT' with 'sampler2D'
// Upgrade NOTE: replaced 'texRECT' with 'tex2D'

Shader "CopyShader" {
Properties {
	_MainTex ("Base (RGB)", RECT) = "white" {}
    _Region  ("Cropping Region (xy wh)", Vector) = (0,0,1,1)
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
				
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"


uniform sampler2D _MainTex;

uniform float4 _Region;
float4 _TexCoordScale;

float2 VuforiaTexCoordMap(float2 xy)
{
	return xy * _TexCoordScale;
}

float4 GetTexel(float2 uv)
{
	// To index into the light probe region directly
	uv = uv * _Region.zw + _Region.xy;
	
	return tex2D (_MainTex, VuforiaTexCoordMap(uv));
}

float4 frag (v2f_img i) : COLOR
{
	return GetTexel(i.uv);
}
ENDCG

	}
}

Fallback off

}