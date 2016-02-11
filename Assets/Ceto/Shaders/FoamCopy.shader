Shader "Ceto/FoamCopy" 
{
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	sampler2D Ceto_JacobianXXBuffer, Ceto_JacobianYYBuffer, Ceto_JacobianXYBuffer, Ceto_HeightBuffer;
	float4 Ceto_FoamChoppyness;
	float Ceto_FoamCoverage;

	struct v2f 
	{
		float4  pos : SV_POSITION;
		float2  uv : TEXCOORD0;
	};
	
	struct f2a
	{
	 	float4 col0 : SV_Target0;
	 	float4 col1 : SV_Target1;
	};

	v2f vert(appdata_base v)
	{
		v2f OUT;
		OUT.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		OUT.uv = v.texcoord;
		return OUT;
	}
	
	float4 frag0(v2f IN) : COLOR
	{ 
	
		//Foam map 0 is not used so feel free to add your own method here.
		//This is just a example of using the wave heights to control how much foam is added.
	
		float4 uv = float4(IN.uv,0,0);

		float4 h = max(0.0, tex2Dlod(Ceto_HeightBuffer, uv));
		
		h *= float4(0.1, 2.0, 10.0, 50.0);
		
		h = 1.0 - exp(-h);
		
		return h;
	}
	
	float4 frag1(v2f IN) : COLOR
	{ 
	
		//Foam map 1 uses the jacobians to calculate the foam coverage.
	
		float4 uv = float4(IN.uv,0,0);
		
		float4 choppyness = Ceto_FoamChoppyness;

		float4 Jxx = choppyness*tex2Dlod(Ceto_JacobianXXBuffer, uv);
		float4 Jyy = choppyness*tex2Dlod(Ceto_JacobianYYBuffer, uv);
		float4 Jxy = choppyness*choppyness*tex2Dlod(Ceto_JacobianXYBuffer, uv);
		
		float4 j = Ceto_FoamCoverage + Jxx + Jyy + choppyness*Jxx*Jyy - Jxy*Jxy;
		
		return j;
	}
	
	ENDCG
	
	SubShader 
	{
	
    	Pass 
    	{
			ZTest Always Cull Off ZWrite Off
      		Fog { Mode off }
    		
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag0
			
			ENDCG
    	}
    	
    	Pass 
    	{
			ZTest Always Cull Off ZWrite Off
      		Fog { Mode off }
    		
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag1
			
			ENDCG
    	}
    	
	}
}









