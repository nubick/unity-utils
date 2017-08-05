// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Square"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		_Progress("Progress", Range(0.0, 1.0)) = 0.0
		_Size("Size", Vector) = (64.0, 45.0, 0, 0)
		_Smoothness("Smoothness", float) = 0.5
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
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			float rand(float2 co)
			{
				float x = sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453;
				return x - floor(x);
			}

			//fixed4 frag(v2f IN) : SV_Target
			//{
			//	fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
			//	c.rgb *= c.a;
			//	return c;
			//}

			uniform half _Progress;
			uniform float2 _Size;
			uniform float _Smoothness;

			fixed4 frag(v2f_img i) : COLOR
			{
				float r = rand(floor(_Size.xy * i.uv));
				float m = smoothstep(0.0, -_Smoothness, r - (_Progress * (1.0 + _Smoothness)));

				return lerp(tex2D(_MainTex, i.uv), _Color, m);
			}

		ENDCG
		}
	}
}
