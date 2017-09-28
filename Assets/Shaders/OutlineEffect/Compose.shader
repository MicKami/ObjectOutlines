Shader "Hidden/Compose"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _OutlinePrepass;
			sampler2D _Blurred;
			float _AlphaMultiplier;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 screen = tex2D(_MainTex, i.uv);
				fixed4 blurred = tex2D(_Blurred, i.uv);
				fixed4 prepass = tex2D(_OutlinePrepass, i.uv);
				fixed4 outline = max(0, blurred - prepass);
				float alpha = saturate(outline.a * _AlphaMultiplier);
				fixed4 final = outline + screen * (1 - alpha);
				final.a = alpha;
				return final;
			}
			ENDCG
		}
	}
}
