Shader "Custom/Blur"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_BlurSize("Blur Size", Range(0.0, 0.1)) = 0.05
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 uv  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			uniform float _BlurSize;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				//c.rgb *= c.a;
				//return c;



				fixed4 sum = fixed4(0.0, 0.0, 0.0, 0.0);
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 4.0 * _BlurSize)) * 0.05;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 3.0 * _BlurSize)) * 0.09;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y - 2.0 * _BlurSize)) * 0.12;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y - _BlurSize)) * 0.15;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y)) * 0.16;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y + _BlurSize)) * 0.15;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 2.0 * _BlurSize)) * 0.12;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 3.0 * _BlurSize)) * 0.09;
				sum += tex2D(_MainTex, half2(i.uv.x, i.uv.y + 4.0 * _BlurSize)) * 0.05;

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					sum.a = tex2D(_AlphaTex, i.uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return sum;
			}
		ENDCG
		}
	}
}
