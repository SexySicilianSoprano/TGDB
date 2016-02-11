
Shader "Ceto/OceanTopSide_Opaque" 
{
	Properties 
	{
		
	}
	SubShader 
	{

		Tags { "OceanMask"="Ceto_ProjectedGrid_Top" "RenderType"="Ceto_ProjectedGrid_Top" "IgnoreProjector"="True" "Queue"="AlphaTest+50" }
		LOD 200
		
		GrabPass { "Ceto_RefractionGrab" }
		
		zwrite on
		cull back
		
		CGPROGRAM
		#pragma surface surf OceanBRDF noforwardadd nolightmap
		//#pragma surface surf OceanBRDF nolightmap fullforwardshadows
		#pragma vertex vert
		#pragma target 3.0
		
		#pragma multi_compile __ CETO_REFLECTION_ON
		#pragma multi_compile __ CETO_UNDERWATER_ON
		#pragma multi_compile __ CETO_USE_OCEAN_DEPTHS_BUFFER
		
		//#define CETO_REFLECTION_ON
		//#define CETO_UNDERWATER_ON
		//#define CETO_USE_OCEAN_DEPTHS_BUFFER
		
		#define CETO_NO_DIFFUSE_IN_REFLECTIONS
		//#define CETO_BRDF_FRESNEL
		//#define CETO_NICE_BRDF
		
		#define CETO_USE_SLOPE_GRID_0
		#define CETO_USE_SLOPE_GRID_1
		#define CETO_USE_SLOPE_GRID_2
		#define CETO_USE_SLOPE_GRID_3

		#define CETO_USE_FOAM_GRID_0
		#define CETO_USE_FOAM_GRID_1
		#define CETO_USE_FOAM_GRID_2
		#define CETO_USE_FOAM_GRID_3
		
		#define CETO_USE_NORMAL_OVERLAYS
		#define CETO_USE_FOAM_OVERLAYS
		
		#include "OceanShaderHeader.cginc"
		#include "OceanDisplacement.cginc"
		#include "OceanBRDF.cginc"
		#include "OceanUnderWater.cginc"
		
		struct Input 
		{
			float4 wPos;
			float4 screenUV;
			float4 grabUV;
			float4 texUV;
		};

		void vert(inout appdata_full v, out Input OUT) 
		{
		
       		UNITY_INITIALIZE_OUTPUT(Input, OUT);
       		
       		float4 uv = float4(v.vertex.xy, v.texcoord.xy);

			float4 oceanPos;
			float3 displacement;
			OceanPositionAndDisplacement(uv, oceanPos, displacement);
			
			v.vertex.xyz = oceanPos.xyz + displacement;
			
			v.tangent = float4(1,0,0,1);
			v.normal = float3(0,1,0);
	
			OUT.wPos = float4(v.vertex.xyz, COMPUTE_DEPTH_01);
			OUT.texUV = uv;
			
			float4 screenPos = mul(UNITY_MATRIX_MVP, v.vertex);

			float4 screenUV = ComputeScreenPos(screenPos);
			screenUV = UNITY_PROJ_COORD(screenUV);

			float4 grabUV = ComputeGrabScreenPos(screenPos);
			grabUV = UNITY_PROJ_COORD(grabUV);

			OUT.screenUV = screenUV;
			OUT.grabUV = grabUV;
			
     	} 

		void surf (Input IN, inout SurfaceOutputOcean o) 
		{
		
			float4 uv = IN.texUV;
			float3 worldPos = IN.wPos.xyz;
			float depth = IN.wPos.w;

			float4 screenUV;
			screenUV.xy = IN.screenUV.xy / IN.screenUV.w;
			screenUV.zw = IN.grabUV.xy / IN.grabUV.w;
			
			float4 st = WorldPosToProjectorSpace(worldPos);
			OceanClip(st);
								
			half3 norm1, norm2, norm3;
			fixed4 foam;
			OceanNormalAndFoam(IN.texUV, st, worldPos, norm1, norm2, norm3, foam);
				
			half3 view = normalize(_WorldSpaceCameraPos-worldPos);
			float dist = length(_WorldSpaceCameraPos-worldPos);
			
			if(dot(view, norm1) < 0.0) norm1 = reflect(norm1, view);
			if(dot(view, norm2) < 0.0) norm2 = reflect(norm2, view);
			if(dot(view, norm3) < 0.0) norm3 = reflect(norm3, view);
			
			fixed3 sky = ReflectionColor(norm2, screenUV.xy);
			
			fixed3 sea = OceanColorFromAbove(norm3, screenUV, worldPos, depth, dist);
			
			sea += SubSurfaceScatter(view, norm1, worldPos.y);
			
			fixed fresnel = FresnelAirWater(view, norm3);
			
			fixed3 col = fixed3(0,0,0);
			
			col += sky * fresnel;
			
			col += sea * (1.0-fresnel);

			col = OceanWithFoamColor(worldPos, col, foam);

			fixed3 grab = tex2D(Ceto_RefractionGrab, screenUV.zw).rgb;

			float edgeFade = EdgeFade(screenUV, view, worldPos);
			col = lerp(grab, col, edgeFade);

			o.Albedo = col;
			o.Normal = TangentSpaceNormal(norm3);
			o.DNormal = norm2;
			o.Fresnel = fresnel;
			o.Alpha = 1.0;
			o.SpecularStr = 1.0;
			o.IndirectStr = 1.0;
			o.LightMask = 1.0-edgeFade;

		}

		ENDCG
		
		
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
		
			#pragma multi_compile_shadowcaster
	
			#include "UnityCG.cginc"
			
			#include "OceanShaderHeader.cginc"
			#include "OceanDisplacement.cginc"
			
			struct v2f { 
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
			};
			
			v2f vert( appdata_base v )
			{
				v2f o;
				
				float4 uv = float4(v.vertex.xy, v.texcoord.xy);
				
				float4 oceanPos;
				float3 displacement;
				OceanPositionAndDisplacement(uv, oceanPos, displacement);

				v.vertex.xyz = oceanPos.xyz + displacement;
				o.worldPos = v.vertex.xyz;
				
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}
					
			float4 frag( v2f i ) : SV_Target
			{
			
				half4 st = WorldPosToProjectorSpace(i.worldPos);
				OceanClip(st);
			
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG	
		}
		
	} 
	FallBack Off
}















