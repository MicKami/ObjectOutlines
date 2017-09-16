Shader "Hidden/Blur"
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
			float4 _MainTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 sum = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x * 2, 0)) * 0.06136;
				sum += tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x, 0)) *	0.24477;
				sum += tex2D(_MainTex, i.uv) * 0.38774;
				sum += tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x, 0)) * 0.24477;
				sum += tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x * 2, 0)) * 0.06136;				

				return sum;
			}
			ENDCG
		}
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
			float4 _MainTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 sum = tex2D(_MainTex, i.uv + float2(0, -_MainTex_TexelSize.y * 2)) * 0.06136;
				sum += tex2D(_MainTex, i.uv + float2(0, -_MainTex_TexelSize.y)) *	0.24477;
				sum += tex2D(_MainTex, i.uv) * 0.38774;
				sum += tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y)) * 0.24477;
				sum += tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y * 2)) * 0.06136;				

				return sum;
			}
			ENDCG
		}
	}
}
