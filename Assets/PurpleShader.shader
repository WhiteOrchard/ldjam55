Shader "Custom/FancyGlowingPurpleShader"
{
    Properties
    {
        _GlowStrength("Glow Strength", Range(0, 1)) = 0.5
        _GlowFrequency("Glow Frequency", Range(0.1, 20)) = 5 // Increased range for faster oscillation
        _NoiseTex("Noise Texture", 2D) = "white" {} // Additional noise texture
    }
        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Blend SrcAlpha One // Additive blending to simulate glow
        ZWrite Off // Turn off depth writing for transparency
        AlphaToMask On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex, _NoiseTex; // Main texture and noise texture
            float4 _MainTex_ST;
            float _GlowStrength;
            float _GlowFrequency;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Pulsating effect on texture coordinates
                float time = _Time.y * _GlowFrequency;
                float pulse = sin(time * 2) * 0.1 + 1; // Multiply by 2 to double the pulsating speed
                o.uv = v.uv * _MainTex_ST.xy * pulse + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y * _GlowFrequency;
                float glow = sin(time) * 0.5 + 0.5; // Oscillates between 0 and 1
                glow = pow(glow, 3); // Exaggerate the glow effect

                // Introduce noise into the glow
                fixed4 noiseColor = tex2D(_NoiseTex, i.uv);
                float noiseFactor = noiseColor.r * 0.5 + 0.5; // Normalize to 0-1 range

                // Calculate color with glow effect and noise
                fixed4 color = fixed4(0.5, 0, 0.5, 1) * _GlowStrength * glow * noiseFactor;
                color += fixed4(0.1, 0, 0.1, 1) * (1 - _GlowStrength); // Base color to prevent complete darkness
                return color;
            }
            ENDCG
        }
    }
}
