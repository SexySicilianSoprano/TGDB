

Shader "Ceto/OceanMask" 
{
	SubShader 
	{
		Tags { "OceanMask"="Ceto_ProjectedGrid_Top" "Queue"="Geometry+1"}
		Pass 
		{
		
			zwrite on
			Fog { Mode Off }
			Lighting off
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "OceanShaderHeader.cginc"
			#include "OceanDisplacement.cginc"
			#include "OceanMasking.cginc"

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float depth : TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
			
				float4 uv = float4(v.vertex.xy, v.texcoord.xy);
			
				float4 oceanPos;
				half3 displacement;
				OceanPositionAndDisplacement(uv, oceanPos, displacement);

				v.vertex.xyz = oceanPos.xyz + displacement;
			
				v2f o;
			
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldPos = v.vertex.xyz;
				o.depth = COMPUTE_DEPTH_01;
				
				return o;
		 	} 
			
			float4 frag(v2f IN) : SV_Target
			{

				return float4(TOP_MASK, IN.depth, 0, 0);

			}	
			
			ENDCG
		}
	}
		

	SubShader 
	{
		Tags { "OceanMask"="Ceto_ProjectedGrid_Under" "Queue"="Geometry+2"}
		Pass 
		{
		
			zwrite on
			Fog { Mode Off }
			cull front
			Lighting off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "OceanShaderHeader.cginc"
			#include "OceanDisplacement.cginc"
			#include "OceanMasking.cginc"

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float depth : TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
			
				float4 uv = float4(v.vertex.xy, v.texcoord.xy);
			
				float4 oceanPos;
				half3 displacement;
				OceanPositionAndDisplacement(uv, oceanPos, displacement);

				v.vertex.xyz = oceanPos.xyz + displacement;
			
				v2f o;
			
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldPos = v.vertex.xyz;
				o.depth = COMPUTE_DEPTH_01;
				
				return o;
		 	} 
			
			float4 frag(v2f IN) : SV_Target
			{
			
			    return float4(UNDER_MASK, IN.depth, 0, 0);
			}	
			
			ENDCG
		}
	}
	
	SubShader 
	{
		
		Tags { "OceanMask"="Ceto_Ocean_Bottom" "Queue"="Background"}
		Pass 
		{
			zwrite off
			Fog { Mode Off }
			Lighting off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "OceanShaderHeader.cginc"
			#include "OceanMasking.cginc"

			struct v2f 
			{
				float4  pos : SV_POSITION;
				float depth : TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.depth = COMPUTE_DEPTH_01;
				
				return o;
		 	} 
			
			float4 frag(v2f IN) : SV_Target
			{
			    return float4(BOTTOM_MASK, IN.depth, 0, 0);
			}	
			
			ENDCG
		}
	}

}











