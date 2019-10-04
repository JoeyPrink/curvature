Shader "Unlit/ObstacleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _AnimationColor ("Animation Color", Color) = (1,1,1,0.5)
        _Speed("Speed", Float) = 1
        _Frequency("Animation Frequency", Float) = 1
        _Direction("Animation Direction", Float) = 1
        _Thickness("Animation Thickness", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            float _Direction, _Speed, _Frequency, _Thickness;
            float4 _AnimationColor, _OnColor, _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv*2-1;
                float dist = length(pos);
                if (dist > 1) {
                    return half4(0,0,0,0);
                }

                half4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
/*
                float overlay = sin(_Time.y*_Speed*-_Direction+pow(dist,0.5)*_Frequency)*0.5+0.5;
                
                
//                overlay = overlay>(1-_Thickness);
                col.rgb = lerp(col.rgb, _AnimationColor.rgb, overlay*_AnimationColor.a);
 */
 
                float angle = atan2(pos.y, pos.x);
                
 
                if (dist > 1-_Thickness && abs(angle+_Time.y)%0.4 < 0.2) {
                    col.rgb = lerp(col.rgb, _AnimationColor.rgb, _AnimationColor.a);
                } 
                
                /*
                if (dist > 0.9f) {
                    col.rgb = lerp(col.rgb, _OnColor.rgb, _On);
                }
                */
                return col;
            }
            ENDCG
        }
    }
}
