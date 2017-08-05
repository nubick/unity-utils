// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Transparency"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_AlphaTex("Alpha Texture", 2D) = "white" {}
		_Amount("Amount", Range(0,1)) = 1.0
		_Amount2("Amount2", Range(0,1)) = 1.0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			//"Queue" = "Transparent+1"
			"IgnoreProjector"="True" 
			//"RenderType" = "Opaque"
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
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
			};
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			sampler2D _MainTex;			
			sampler2D _AlphaTex;
			float _Amount;
			float _Amount2;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord);
				c.rgb *= c.a;

				fixed4 t = tex2D(_AlphaTex, IN.texcoord);
	
	
				if (t.a < _Amount)
					return fixed4(0, 0, 0, 0);
				else
					return c;

				//return lerp(c, c*t.a, _Amount);

				//return lerp(c, c*pow(t.a, 1 / _Amount2), _Amount);
			}
		ENDCG
		}
	}
}
