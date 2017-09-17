Shader "Unlit/Outline"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }

		Pass
		{

			ZTest [_ZTestMode]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
						
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
						
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 _Color;
			int _ZTestMode;

			fixed4 frag (v2f i) : SV_Target
			{				
				return _Color;
			}
			ENDCG
		}
	}
}
