﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CloudSkybox"
{
    Properties
    {
        _SampleCount0("Sample Count (min)", Float) = 30
        _SampleCount1("Sample Count (max)", Float) = 90
        _SampleCountL("Sample Count (light)", Int) = 16

        [Space]
        _NoiseTex1("Noise Volume", 3D) = ""{}
        _NoiseTex2("Noise Volume", 3D) = ""{}
        _NoiseFreq1("Frequency 1", Float) = 3.1
        _NoiseFreq2("Frequency 2", Float) = 35.1
        _NoiseAmp1("Amplitude 1", Float) = 5
        _NoiseAmp2("Amplitude 2", Float) = 1
        _NoiseBias("Bias", Float) = -0.2

        [Space]
        _Scroll1("Scroll Speed 1", Vector) = (0.01, 0.08, 0.06, 0)
        _Scroll2("Scroll Speed 2", Vector) = (0.01, 0.05, 0.03, 0)

        [Space]
        _Altitude0("Altitude (bottom)", Float) = 1500
        _Altitude1("Altitude (top)", Float) = 3500
        _FarDist("Far Distance", Float) = 30000

        [Space]
        _Scatter("Scattering Coeff", Float) = 0.008
        _HGCoeff("Henyey-Greenstein", Float) = 0.5
        _Extinct("Extinction Coeff", Float) = 0.01

        [Space]
        _SunSize ("Sun Size", Range(0,1)) = 0.04
        _AtmosphereThickness ("Atmoshpere Thickness", Range(0,5)) = 1.0
        _SkyTint ("Sky Tint", Color) = (.5, .5, .5, 1)
        _GroundColor ("Ground", Color) = (.369, .349, .341, 1)
        _Exposure("Exposure", Range(0, 8)) = 1.3
    }

    CGINCLUDE

    struct appdata_t
    {
        float4 vertex : POSITION;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 rayDir : TEXCOORD1;
        float3 groundColor : TEXCOORD2;
        float3 skyColor : TEXCOORD3;
        float3 sunColor : TEXCOORD4;
    };

    #include "ProceduralSky.cginc"

    v2f vert(appdata_t v)
    {
        float4 p = UnityObjectToClipPos(v.vertex);

        v2f o;

        o.vertex = p;
        o.uv = (p.xy / p.w + 1) * 0.5;

        vert_sky(v.vertex.xyz, o);

        return o;
    }

    float _SampleCount0;
    float _SampleCount1;
    int _SampleCountL;

    sampler3D _NoiseTex1;
    sampler3D _NoiseTex2;
    float _NoiseFreq1;
    float _NoiseFreq2;
    float _NoiseAmp1;
    float _NoiseAmp2;
    float _NoiseBias;

    float3 _Scroll1;
    float3 _Scroll2;

    float _Altitude0;
    float _Altitude1;
    float _FarDist;

    float _Scatter;
    float _HGCoeff;
    float _Extinct;

    float UVRandom(float2 uv)
    {
        float f = dot(float2(12.9898, 78.233), uv);
        return frac(43758.5453 * sin(f));
    }

    float SampleNoise(float3 uvw)
    {
        const float baseFreq = 1e-5;

        float4 uvw1 = float4(uvw * _NoiseFreq1 * baseFreq, 0);
        float4 uvw2 = float4(uvw * _NoiseFreq2 * baseFreq, 0);

        uvw1.xyz += _Scroll1.xyz * _Time.x;
        uvw2.xyz += _Scroll2.xyz * _Time.x;

        float n1 = tex3Dlod(_NoiseTex1, uvw1).a;
        float n2 = tex3Dlod(_NoiseTex2, uvw2).a;
        float n = n1 * _NoiseAmp1 + n2 * _NoiseAmp2;

        n = saturate(n + _NoiseBias);

        float y = uvw.y - _Altitude0;
        float h = _Altitude1 - _Altitude0;
        n *= smoothstep(0, h * 0.1, y);
        n *= smoothstep(0, h * 0.4, h - y);

        return n;
    }

    float HenyeyGreenstein(float cosine)
    {
        float g2 = _HGCoeff * _HGCoeff;
        return 0.5 * (1 - g2) / pow(1 + g2 - 2 * _HGCoeff * cosine, 1.5);
    }

    float Beer(float depth)
    {
        return exp(-_Extinct * depth);
    }

    float BeerPowder(float depth)
    {
        return exp(-_Extinct * depth) * (1 - exp(-_Extinct * 2 * depth));
    }

    float MarchLight(float3 pos, float rand)
    {
        float3 light = _WorldSpaceLightPos0.xyz;
        float stride = (_Altitude1 - pos.y) / (light.y * _SampleCountL);

        pos += light * stride * rand;

        float depth = 0;
        UNITY_LOOP for (int s = 0; s < _SampleCountL; s++)
        {
            depth += SampleNoise(pos) * stride;
            pos += light * stride;
        }

        return BeerPowder(depth);
    }

    fixed4 frag(v2f i) : SV_Target
    {
        float3 sky = frag_sky(i);

        float3 ray = -i.rayDir;
        int samples = lerp(_SampleCount1, _SampleCount0, ray.y);

        float dist0 = _Altitude0 / ray.y;
        float dist1 = _Altitude1 / ray.y;
        float stride = (dist1 - dist0) / samples;

        if (ray.y < 0.01 || dist0 >= _FarDist) return fixed4(sky, 1);

        float3 light = _WorldSpaceLightPos0.xyz;
        float hg = HenyeyGreenstein(dot(ray, light));

        float2 uv = i.uv + _Time.x;
        float offs = UVRandom(uv) * (dist1 - dist0) / samples;

        float3 pos = _WorldSpaceCameraPos + ray * (dist0 + offs);
        float3 acc = 0;

        float depth = 0;
        UNITY_LOOP for (int s = 0; s < samples; s++)
        {
            float n = SampleNoise(pos);
            if (n > 0)
            {
                float density = n * stride;
                float rand = UVRandom(uv + s + 1);
                float scatter = density * _Scatter * hg * MarchLight(pos, rand * 0.5);
                acc += _LightColor0 * scatter * BeerPowder(depth);
                depth += density;
            }
            pos += ray * stride;
        }

        acc += Beer(depth) * sky;

        acc = lerp(acc, sky, saturate(dist0 / _FarDist));

        return half4(acc, 1);
    }

    ENDCG

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
