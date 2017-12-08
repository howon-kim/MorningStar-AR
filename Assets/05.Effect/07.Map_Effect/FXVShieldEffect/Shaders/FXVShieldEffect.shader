// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FXV/FXVShieldEffect" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ColorRimMin("ColorRimMin", Range(-2,1)) = 0.6
		_ColorRimMax("ColorRimMax", Range(0,4)) = 1.0

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TextureColor("Color", Color) = (1,1,1,1)
		_TextureScale("TextureScale", Range(0.01,20)) = 1.0
		_TexturePower("TexturePower", Range(0.0,4)) = 1.0
		_TextureRimMin("TextureRimMin", Range(-2,1)) = 0.6
		_TextureRimMax("TextureRimMax", Range(0,4)) = 1.0
		_TextureScrollX("TextureScrollX", Range(-4,4)) = 0.0
		_TextureScrollY("TextureScrollY", Range(-4,4)) = 0.0

		_DistortTex ("Albedo (RGB)", 2D) = "black" {}
		_DistortionFactor ("DistortionFactor", Range(0,5)) = 0.2

		_PatternTex("Albedo (RGB)", 2D) = "black" {}
		_PatternColor("Color", Color) = (1,1,1,1)
		_PatternScale("PatternScale", Range(0.01,100)) = 1.0
		_PatternPower("PatternPower", Range(0.0,4)) = 1.0
		_PatternRimMin("PatternRimMin", Range(-2,1)) = 0.6
		_PatternRimMax("PatternRimMax", Range(0,4)) = 1.0

	    _OverlapRim("OverlapRim", Range(0.0,100.0)) = 0.1

		_DirectionVisibility("DirectionVisibility", Range(-2.0,2.0)) = 0.0

		_ActivationTex("Albedo (RGB)", 2D) = "black" {}
		_ActivationRim("ActivationRim", Range(0.0,1.0)) = 0.0
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

			#pragma multi_compile ACTIVATION_EFFECT_ON ACTIVATION_EFFECT_OFF
			#pragma multi_compile ACTIVATION_TYPE_TEXTURE ACTIVATION_TYPE_TEX_UV ACTIVATION_TYPE_CUSTOM_TEX ACTIVATION_TYPE_UV
			#pragma multi_compile USE_PATTERN_TEXTURE __
			#pragma multi_compile USE_MAIN_TEXTURE __
			#pragma multi_compile USE_DISTORTION_FOR_MAIN_TEXTURE __
			#pragma multi_compile USE_DEPTH_OVERLAP_RIM __
			#pragma multi_compile USE_COLOR_RIM __
			#pragma multi_compile USE_DIRECTION_VISIBILITY __

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

                half3 rimN : TEXCOORD1;
                half3 rimV : TEXCOORD2;

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

                o.rimN = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                float3 p = UnityObjectToViewPos(v.vertex);
                o.rimV = normalize(-p);

				o.depth = -p.z;// mul(UNITY_MATRIX_MV, v.vertex).z;

                return o;
            }
            
            // texture we will sample
            sampler2D _MainTex;
			sampler2D _PatternTex;
            sampler2D _DistortTex;
			sampler2D _CameraDepthTexture;
            fixed4 _Color;
			float _ColorRimMin;
			float _ColorRimMax;
			float _TextureScale;
			fixed4 _TextureColor;
			float _TexturePower;
            float _TextureRimMin;
            float _TextureRimMax;
            float _TextureScrollX;
            float _TextureScrollY;
            float _DistortionFactor;
			fixed4 _PatternColor;
			float _PatternScale;
			float _PatternPower;
			float _PatternRimMin;
			float _PatternRimMax;
			float _OverlapRim;

			float _DirectionVisibility;
			float4 _ShieldDirection;

//#if ACTIVATION_EFFECT_ON
			sampler2D _ActivationTex;
			float _ActivationTime;
			float _ActivationRim;
//#endif

			struct fragOutput 
			{
				fixed4 color0 : SV_Target;
				//fixed4 color1 : SV_Target1;
			};

            // pixel shader; returns low precision ("fixed4" type)
            // color ("SV_Target" semantic)
            fragOutput frag (v2f i)
			{
				float vdn = 1.0 - max(dot(i.rimV, i.rimN), 0.0);

				float depthVisibility = 1.0;
//#if USE_DEPTH_OVERLAP_RIM
				float depthValue = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r);
				float depthDiff = (depthValue - i.depth);
				depthVisibility = step(-depthDiff, 0.0);
				float depthRim = 1.0 - min(1.0, depthDiff / _OverlapRim);
//#endif

				// color rim
				fixed4 basicColor = fixed4(0, 0, 0, 0);
#if USE_COLOR_RIM
				float colorRim = smoothstep(_ColorRimMin, _ColorRimMax, vdn);
	//#if USE_DEPTH_OVERLAP_RIM
				colorRim = max(colorRim, depthRim);
	//#endif
#endif

				//main texture
				fixed4 tex = fixed4(0, 0, 0, 0);
#if USE_MAIN_TEXTURE
				float texRim = smoothstep(_TextureRimMin, _TextureRimMax, vdn);

	//#if USE_DEPTH_OVERLAP_RIM
				texRim = max(texRim, depthRim);
	//#endif

	#if USE_DISTORTION_FOR_MAIN_TEXTURE
				float2 distortCoord = tex2D(_DistortTex, i.uv*2.0+float2(_Time.x, _Time.x)).xy;

				distortCoord -= float2(0.5, 0.5);
				distortCoord *= 2.0 * _DistortionFactor;

				tex = tex2D(_MainTex, float2(i.uv.x*_TextureScale + distortCoord.x + _TextureScrollX*_Time.x,
                							 i.uv.y*_TextureScale + distortCoord.y + _TextureScrollY*_Time.x)) * _TexturePower;
	#else
				tex = tex2D(_MainTex, float2(i.uv.x*_TextureScale + _TextureScrollX*_Time.x,
					i.uv.y*_TextureScale + _TextureScrollY*_Time.x)) * _TexturePower;
	#endif
#endif

				//pattern texture
				fixed4 pattern = fixed4(0, 0, 0, 0);
#if USE_PATTERN_TEXTURE
				float patternRim = smoothstep(_PatternRimMin, _PatternRimMax, vdn);

				pattern = tex2D(_PatternTex, i.uv*_PatternScale) * _PatternPower * (1.0 - patternRim) * _PatternColor;
#endif

				//shield direction visibility
				float dirVisibility = 1.0f;
#if USE_DIRECTION_VISIBILITY
				dirVisibility = clamp(dot(_ShieldDirection.xyz, i.rimN) + _DirectionVisibility, 0.0, 1.0);
#endif

#if ACTIVATION_EFFECT_ON
				const float scaledTime = _ActivationTime*(1.0 + _ActivationRim);
				float activationVal = 0.0;
#if ACTIVATION_TYPE_TEXTURE
				activationVal = tex.r;
#elif ACTIVATION_TYPE_UV
				activationVal = i.uv.y;
#elif ACTIVATION_TYPE_TEX_UV
				activationVal = (tex.r + i.uv.y) * 0.5;
#elif ACTIVATION_TYPE_CUSTOM_TEX
				activationVal = tex2D(_ActivationTex, i.uv).r;
#endif
				float activationVisibility = step(activationVal, scaledTime); 
				float t = 2.0*abs(clamp(((scaledTime - activationVal) / _ActivationRim), 0.0, 1.0) - 0.5);

				float activationRim = lerp(1.0, 0.0, t);

	#if USE_COLOR_RIM
				colorRim = max(colorRim, activationRim);
	#endif
#endif

#if USE_COLOR_RIM
				basicColor = _Color * colorRim;
#endif
 
#if USE_MAIN_TEXTURE
				tex *= _TextureColor * texRim;
#endif
				fragOutput o;
                o.color0.rgb = pattern.rgb + tex.rgb + basicColor.rgb;

#if ACTIVATION_EFFECT_ON
				o.color0.a = max(activationRim, dirVisibility * activationVisibility * (pattern.a + tex.r + basicColor.a)) * depthVisibility;
#else
				o.color0.a = dirVisibility * depthVisibility * (pattern.a + tex.r + basicColor.a);
#endif
                return o;
            }
            ENDCG
        }
		
	}
	CustomEditor "FXVShieldMaterialEditor"

	FallBack "Diffuse"
}
