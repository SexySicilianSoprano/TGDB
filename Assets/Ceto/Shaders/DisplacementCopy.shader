
Shader "Ceto/DisplacementCopy" 
{
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	sampler2D Ceto_HeightBuffer, Ceto_DisplacementBuffer;
	float4 Ceto_Choppyness;
	
	struct appdata_t 
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
	};

	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = v.texcoord.xy;
		return o;
	}

	float4 frag0(v2f i) : SV_Target
	{
		float4 d = float4(0,0,0,0);
		d.y = tex2D(Ceto_HeightBuffer, i.texcoord).x;
		d.xz = tex2D(Ceto_DisplacementBuffer, i.texcoord).xy;
	
		return d;
	}
	
	float4 frag1(v2f i) : SV_Target
	{
		float4 d = float4(0,0,0,0);
		d.y = tex2D(Ceto_HeightBuffer, i.texcoord).y;
		d.xz = tex2D(Ceto_DisplacementBuffer, i.texcoord).zw;
	
		return d;
	}
	
	float4 frag2(v2f i) : SV_Target
	{
		float4 d = float4(0,0,0,0);
		d.y = tex2D(Ceto_HeightBuffer, i.texcoord).z;
		d.xz = tex2D(Ceto_DisplacementBuffer, i.texcoord).xy;
	
		return d;
	}
	
	float4 frag3(v2f i) : SV_Target
	{
		float4 d = float4(0,0,0,0);
		d.y = tex2D(Ceto_HeightBuffer, i.texcoord).w;
		d.xz = tex2D(Ceto_DisplacementBuffer, i.texcoord).zw;
	
		return d;
	}
	
	ENDCG 
	
	SubShader 
	{ 
		Pass 
		{
 			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag0
			ENDCG
		}
		
		Pass 
		{
 			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag1
			ENDCG
		}
		
		Pass 
		{
 			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag2
			ENDCG
		}
		
		Pass 
		{
 			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag3
			ENDCG
		}
	}
	Fallback Off 
}














