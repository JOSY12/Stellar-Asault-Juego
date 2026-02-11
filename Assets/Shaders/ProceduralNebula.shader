Shader "Custom/ProceduralNebula2D"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0.2, 0.5, 0.8, 1)
        _SecondaryColor ("Secondary Color", Color) = (0.1, 0.3, 0.6, 1)
        _Scale ("Noise Scale", Float) = 5.0
        _Speed ("Animation Speed", Float) = 0.5
        _Alpha ("Overall Alpha", Range(0, 1)) = 0.5
        _EdgeFade ("Edge Fade", Range(0, 1)) = 0.3
        _Pixelation ("Pixelation", Float) = 64.0
        _Contrast ("Contrast", Range(1, 3)) = 1.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };
            
            float4 _MainColor;
            float4 _SecondaryColor;
            float _Scale;
            float _Speed;
            float _Alpha;
            float _EdgeFade;
            float _Pixelation;
            float _Contrast;
            
            float hash(float2 p)
            {
                p = frac(p * float2(443.897, 441.423));
                p += dot(p, p.yx + 19.19);
                return frac(p.x * p.y);
            }
            
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for (int i = 0; i < 6; i++)
                {
                    value += amplitude * noise(p * frequency);
                    frequency *= 2.13;
                    amplitude *= 0.47;
                }
                
                return value;
            }
            
            float blobs(float2 p, float time)
            {
                float blob = 0.0;
                
                for (int i = 0; i < 4; i++)
                {
                    float2 offset = float2(
                        hash(float2(i * 7.13, time * 0.1)) * 10.0,
                        hash(float2(i * 13.7, time * 0.15)) * 10.0
                    );
                    
                    float2 pos = p + offset;
                    float dist = length(pos);
                    float size = 1.5 + hash(float2(i, time * 0.2)) * 2.0;
                    
                    blob += smoothstep(size, size * 0.5, dist);
                }
                
                return blob;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                uv = floor(uv * _Pixelation) / _Pixelation;
                
                float2 worldUV = i.worldPos.xy * 0.1;
                
                float timeOffset = _Time.y * _Speed;
                float2 noiseCoord = (uv + worldUV) * _Scale + float2(timeOffset * 0.1, timeOffset * 0.05);
                
                float n1 = fbm(noiseCoord);
                float n2 = fbm(noiseCoord + float2(5.2, 1.3));
                float n3 = fbm(noiseCoord * 1.5 + float2(timeOffset * 0.2, timeOffset * 0.1));
                
                float blob = blobs((uv + worldUV - 0.5) * _Scale, timeOffset);
                
                float combined = (n1 * 0.4 + n2 * 0.3 + n3 * 0.2 + blob * 0.1);
                
                combined = pow(combined, _Contrast);
                
                combined = smoothstep(0.3, 0.7, combined);
                
                float2 center = float2(0.5, 0.5);
                float dist = distance(uv, center) * 2.0;
                float mask = smoothstep(1.0, 1.0 - _EdgeFade, dist);
                
                float4 color = lerp(_SecondaryColor, _MainColor, combined);
                
                color.a = combined * mask * _Alpha;
                
                return color;
            }
            ENDCG
        }
    }
}
