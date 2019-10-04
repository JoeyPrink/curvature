Shader "Unlit/AttractorRangePreviewShader"
{
    Properties
    {
        [NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
        _MinRangeColor ("Inner Color", Color) = (1,0,0,0.5)
        _MaxRangeColor ("Outer Color", Color) = (0,0,1,0)
        _MinRange("Inner Range", Float) = 1
        _MaxRange("Outer Range", Float) = 3
        
        
        _AnimationColor ("Animation Color", Color) = (1,1,1,0.5)
        _Speed("Speed", Float) = 1
        _Frequency("Animation Frequency", Float) = 1
        _Direction("Animation Direction", Float) = 1
        _Thickness("Animation Thickness", Float) = 0.2

        _OnFrequency("ON Frequency", Float) = 6
        _OnColor("ON Color", Color) = (1,1,1,0.25)
        _On("ON", Range(0,1)) = 0
        
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
            float4 _MinRangeColor;
            float4 _MaxRangeColor; 
            float _MinRange;
            float _MaxRange;
            float _Direction, _Speed, _Frequency, _Thickness;
            float4 _AnimationColor, _OnColor;
            float _On,_OnFrequency;

            v2f vert (appdata v)
            {
                v2f o;
                float4 pos = v.vertex;
                pos.xyz *= _MaxRange*2;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv*2-1;
                float dist = length(pos);
                if (dist > 1) {
                    return half4(0,0,0,0);
                }
                
                
                float overlay = sin(_Time.y*_Speed*_MaxRange*-_Direction+pow(dist,0.5)*_Frequency*_MaxRange)*0.5+0.5;
                
                float innerDist = _MinRange/_MaxRange;
                float l = saturate((dist-innerDist)/(1-innerDist));
                half4 col = tex2D(_MainTex, i.uv);
                col *= lerp(_MinRangeColor, _MaxRangeColor, l);
                
                overlay = overlay>(1-_Thickness);
                col.rgb = lerp(col.rgb, _AnimationColor.rgb, overlay*_AnimationColor.a);
                
                col.rgb = lerp(col.rgb, _OnColor.rgb, _On*_OnColor.a*(1-0.5*((_OnFrequency*_Time.y)%1.0f)));
                
                return col;
            }
            ENDCG
        }
    }
}
