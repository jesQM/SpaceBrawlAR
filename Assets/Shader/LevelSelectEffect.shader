Shader "Custom/LevelSelectEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Amount ("Noise Amount", Range(0,1)) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _Noise;
            float _Amount;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 offset = tex2D(_Noise, i.uv);

				float x = i.uv.x + (offset.r - 0.5) * _Amount;
				float y = i.uv.y + (offset.r - 0.5) * _Amount;
                fixed4 col = tex2D(_MainTex, float2(x,y) );
                return col;
            }
            ENDCG
        }
    }
}
