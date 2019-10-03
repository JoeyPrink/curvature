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
        _Direction("Direction", Float) = 1
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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
            float _Direction, _Speed;
            float4 _AnimationColor;

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
                float innerDist = _MinRange/_MaxRange;
                float l = saturate((dist-innerDist)/(1-innerDist));
                
                
                half4 col = tex2D(_MainTex, i.uv);
                col *= lerp(_MinRangeColor, _MaxRangeColor, l);
                return col;
            }
            ENDCG
        }
    }
}
