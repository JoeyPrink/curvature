Shader "Unlit/LineAreaShader"
{
    Properties
    {
        [NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
        _Tint("Color Tint", Color) = (1,1,1,1)
        
        _AnimationColor ("Animation Color", Color) = (1,1,1,0.5)
        _Speed("Speed", Float) = 1
        _Frequency("Animation Frequency", Float) = 1
        _Direction("Animation Direction", Float) = 1
        _Thickness("Animation Thickness", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;
            
            float _Direction, _Speed, _Frequency, _Thickness;
            float4 _AnimationColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                col *= i.color;
                col *= _Tint;
                float dist = i.uv.y;
                float overlay = sin(_Time.y*_Speed*-_Direction+pow(dist,0.5)*_Frequency)*0.5+0.5;
//                overlay = overlay>(1-_Thickness);
                col.rgb = lerp(col.rgb, _AnimationColor.rgb, overlay*_AnimationColor.a);
                return col;
            }
            ENDCG
        }
    }
}
