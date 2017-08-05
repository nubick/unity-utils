// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GravitationLensing"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_Rad("Radius", Float) = 0
		_Ratio("Ratio", Float) = 0
		_Distance("Distance", Float) = 0
		_Position("Position", Vector) = (5,5,0,0)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
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
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;// *_Color;
				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D(_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}


			uniform float2 _Position = (5,5);
			uniform float _Rad = 0.5;
			uniform float _Ratio = 1.2;
			uniform float _Distance = 1;

			fixed4 frag(v2f IN) : COLOR
			{
				float2 offset = IN.texcoord - _Position; //Сдвигаем наш пиксель на нужную позицию
				float2 ratio = { _Ratio,1 }; //определяем соотношение сторон экрана
				float rad = length(offset / ratio); //определяем расстояние от условного "центра" экрана.

				float deformation = 1 / pow(rad*pow(_Distance,0.5),2)*_Rad * 2;

				offset = offset*(1 - deformation);

				offset += _Position;

				half4 res = tex2D(_MainTex, offset);
				//if (rad*_Distance<pow(2*_Rad/_Distance,0.5)*_Distance) {res.g+=0.2;} // проверка соблюдения радиуса эйнштейна
				//if (rad*_Distance<_Rad){res.r=0;res.g=0;res.b=0;} //проверка радиуса ЧД
				return res;
			}

			ENDCG
		}
	}
}
