
Shader "Ceto/OceanUnderSide_Opaque" 
{
	Properties 
	{

	}
	SubShader 
	{
	
		Tags { "OceanMask"="Ceto_ProjectedGrid_Under" "RenderType"="Ceto_ProjectedGrid_Under" "IgnoreProjector"="True" "Queue"="AlphaTest+49" }
		LOD 200
		
		GrabPass { "Ceto_RefractionGrab" }
		
		zwrite on
		cull front
		
		CGPROGRAM
		#pragma surface surf OceanBRDF noforwardadd nolightmap
		#pragma vertex vert
		#pragma target 3.0
		
		#pragma multi_compile __ CETO_REFLECTION_ON
		#pragma multi_compile __ CETO_UNDERWATER_ON
		#pragma multi_compile __ CETO_USE_OCEAN_DEPTHS_BUFFER
		
		//#define CETO_REFLECTION_ON
		//#define CETO_UNDERWATER_ON
		//#define CETO_USE_OCEAN_DEPTHS_BUFFER
		
		#define CETO_NO_DIFFUSE_IN_REFLECTIONS
		#define CETO_OCEAN_UNDERSIDE
		//Fast BRDF not working on underside. Must use nice BRDF.
		#define CETO_NICE_BRDF
		
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
			v.normal = float3(0,-1,0);
			
			OUT.wPos = float4(v.vertex.xyz, 0);
			OUT.wPos.w = COMPUTE_DEPTH_01;
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
			
			half3 norm;
			fixed4 foam;
			OceanNormalAndFoam(uv, st, worldPos, norm, foam);
			norm.y *= -1.0;
				
			half3 view = normalize(_WorldSpaceCameraPos-worldPos);
			float dist = length(_WorldSpaceCameraPos-worldPos);

			if (dot(view, norm) < 0.0) norm = reflect(norm, view);
			
			fixed3 sky = SkyColorFromBelow(norm, screenUV, worldPos, depth, dist);
			
			fixed3 sea = DefaultUnderSideColor();
			
			float fresnel = FresnelWaterAir(view, norm);

			fixed3 col = fixed3(0,0,0);
			
			col += sea * fresnel;
			
			col += sky * (1.0-fresnel);
			
			col = OceanWithFoamColor(worldPos, col, foam);

			o.Albedo = col;
			o.Normal = TangentSpaceNormal(norm);
			o.DNormal = norm;
			o.Fresnel = fresnel;
			o.Alpha = 1.0;
			o.SpecularStr = 1.0;
			o.IndirectStr = 1.0;
			o.LightMask = 0.0;


		}

		ENDCG
		
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			
			cull front
			
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















