Shader "Custom/RevealShaderPercentage"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" { }
        _RevealPercentage("Reveal Percentage", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        float _RevealPercentage;

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

            // Calculate the reveal effect horizontally based on the percentage
            fixed revealFactor = step(_RevealPercentage, IN.uv_MainTex.x);

            // Apply the new color
            o.Albedo = c.rgb * revealFactor;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}