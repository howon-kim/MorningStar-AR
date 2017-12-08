// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FXV/FXVShieldHit" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_HitPosU ("HitPosU", float) = 0.5
		_HitPosV ("HitPosV", float) = 0.5
		_PatternTex("Albedo (RGB)", 2D) = "black" {}
		_PatternScale("PatternScale", Range(0.01,100)) = 1.0

		_RippleTex("Albedo (RGB)", 2D) = "black" {}
		_RippleScale("RippleScale", Range(0.1,100)) = 1.0
		_RippleDistortion("RippleDistortion", Range(0.01,1)) = 1.0

		_HitAttenuation("HitAttenuation", Range(0.01,100)) = 1.0
		_HitPower("HitPower", Range(0.001,100)) = 1.0

	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 300

		BlendOp Add
		Blend SrcAlpha One 
		//Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off


		Pass
        {
            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the pixel (fragment) shader
            #pragma fragment frag

			#pragma multi_compile USE_PATTERN_TEXTURE __
			#pragma multi_compile USE_DISTORTION_FOR_PATTERN_TEXTURE __

            #include "UnityCG.cginc"

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0; // texture coordinate
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float2 uv : TEXCOORD0; // texture coordinate
                float4 pos : SV_POSITION; // clip space position
 
                //half3 rimN : TEXCOORD1;
                //half3 rimV : TEXCOORD2;

				float depth : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.pos);

                float3 p = UnityObjectToViewPos(v.vertex);
				o.depth = -p.z;

                return o;
            }
            
			sampler2D _PatternTex;
			sampler2D _RippleTex;
            fixed4 _Color;
			float _PatternScale;
			float _HitPosU;
			float _HitPosV;
			float _HitAttenuation;
			float _HitPower;
			float _RippleScale;
			float _RippleDistortion;

			struct fragOutput 
			{
				fixed4 color0 : SV_Target;
			};

            // pixel shader; returns low precision ("fixed4" type)
            // color ("SV_Target" semantic)
            fragOutput frag (v2f i)
			{
				float2 hitUV = float2(_HitPosU, _HitPosV);

				float diffU = i.uv.x - _HitPosU;
				diffU = sign(diffU)*min(min(abs(diffU), abs(diffU - 1.0)), abs(diffU + 1.0));

				float diffV = i.uv.y - _HitPosV;
				diffV = sign(diffV)*min(min(abs(diffV), abs(diffV - 1.0)), abs(diffV + 1.0));

				float2 diff = float2(diffU, diffV);
				float dist = length(diff);

				fixed4 pattern = fixed4(1.0, 1.0, 1.0, 1.0);
#if USE_PATTERN_TEXTURE
	#if USE_DISTORTION_FOR_PATTERN_TEXTURE
				float2 dir = diff / dist;
				fixed4 ripple = tex2D(_RippleTex, float2(dist*_Color.a*_RippleScale, 0.5));// +diff*_Color.a);
				pattern = tex2D(_PatternTex, i.uv*_PatternScale+_RippleDistortion*dir*ripple.r);
	#else
				pattern = tex2D(_PatternTex, i.uv*_PatternScale);
	#endif
#endif

				float att = 1.0 / (1.0 + 9.0 * _HitAttenuation * dist * dist);

				fragOutput o;

                o.color0 = pattern * _Color * _HitPower * att;

                return o;
            }
            ENDCG
        }
	}
	CustomEditor "FXVShieldHitMaterialEditor"
	
	FallBack "Diffuse"
}
