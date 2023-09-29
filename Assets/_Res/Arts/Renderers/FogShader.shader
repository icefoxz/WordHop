Shader "Custom/FogShader"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _MaskTex("Mask (A)", 2D) = "white" {}
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque"}

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MaskTex;
        };

        sampler2D _MainTex;
        sampler2D _MaskTex;

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * tex2D(_MaskTex, IN.uv_MaskTex).a;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    Fallback "Diffuse"
}