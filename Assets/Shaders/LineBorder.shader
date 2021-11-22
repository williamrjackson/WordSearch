Shader "Unlit/Spine Outline Stencil"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 4)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
 
        CGINCLUDE
        #include "UnityCG.cginc"
 
        sampler2D _MainTex;
        float4 _MainTex_ST;
 
        fixed4 _OutlineColor;
        float _OutlineWidth;
 
        struct v2fOutline
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };
 
        v2fOutline vertOutline (appdata_base v, float2 offset)
        {
            v2fOutline o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.pos.xy += offset * 2 * o.pos.w * _OutlineWidth / _ScreenParams.xy;
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            return o;
        }
 
        fixed4 fragOutline (v2fOutline i) : SV_Target
        {
            fixed alpha = tex2D(_MainTex, i.uv).a;
            fixed4 col = _OutlineColor;
            col.a *= alpha;
            return col;
        }
        ENDCG
 
        Pass
        {
            Name "MASK"
            Stencil {
                Ref 1
                Pass Replace
            }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
 
            void frag (v2f i)
            {
                clip(tex2D(_MainTex, i.uv).a - 0.5);
            }
            ENDCG
        }
 
        Pass
        {
            Stencil {
                Ref 1
                Comp NotEqual
            }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOutline
 
            v2fOutline vert (appdata_base v)
            {
                return vertOutline(v, float2( 1, 1));
            }
            ENDCG
        }
 
        Pass
        {
            Stencil {
                Ref 1
                Comp NotEqual
            }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOutline
 
            v2fOutline vert (appdata_base v)
            {
                return vertOutline(v, float2(-1, 1));
            }
            ENDCG
        }
 
        Pass
        {
            Stencil {
                Ref 1
                Comp NotEqual
            }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOutline
 
            v2fOutline vert (appdata_base v)
            {
                return vertOutline(v, float2( 1,-1));
            }
            ENDCG
        }
 
        Pass
        {
            Stencil {
                Ref 1
                Comp NotEqual
            }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOutline
 
            v2fOutline vert (appdata_base v)
            {
                return vertOutline(v, float2(-1,-1));
            }
            ENDCG
        }
    }
}