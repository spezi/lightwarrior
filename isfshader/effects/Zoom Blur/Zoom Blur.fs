/*{
  "DESCRIPTION": "Zoom Blur (safe version for Millumin)",
  "CATEGORIES": [ "Blur" ],
  "INPUTS": [
    { "NAME": "inputImage", "TYPE": "image" },
    { "NAME": "strength", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "samples", "TYPE": "float", "DEFAULT": 20.0, "MIN": 4.0, "MAX": 64.0 },
    { "NAME": "centerX", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "centerY", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

void main()
{
    vec2 uv = isf_FragNormCoord.xy;
    vec2 center = vec2(centerX, centerY);

    vec2 dir = uv - center;

    vec4 sum = vec4(0.0);
    float count = 0.0;

    float s = max(samples, 1.0);

    for (float i = 0.0; i < 64.0; i += 1.0)
    {
        if (i >= s) break;

        float t = i / max(s - 1.0, 1.0);

        vec2 sampleUV = center + dir * (1.0 - strength * t);

        // clamp回避（vec2個別処理）
        sampleUV.x = min(max(sampleUV.x, 0.0), 1.0);
        sampleUV.y = min(max(sampleUV.y, 0.0), 1.0);

        // 安全なサンプリング
        vec4 col = IMG_PIXEL(inputImage, sampleUV * RENDERSIZE);

        sum += col;
        count += 1.0;
    }

    gl_FragColor = sum / max(count, 1.0);
}