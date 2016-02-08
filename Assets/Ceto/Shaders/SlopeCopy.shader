
Shader "Ceto/SlopeCopy" 
{

	SubShader 
	{ 
		Pass 
		{
 			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D Ceto_SlopeBuffer;
			
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

			float4 frag (v2f i) : SV_Target
			{
			
				float4 slope = tex2D(Ceto_SlopeBuffer, i.texcoord);
				
				return slope;
			}
			ENDCG 

		}
	}
	Fallback Off 
}
