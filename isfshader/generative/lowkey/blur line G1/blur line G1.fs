/*{
  "CATEGORIES": [
    "Generator",
    "Abstract",
    "Organic"
  ],
  "DESCRIPTION": "Soft organic black tendrils on a white background, inspired by translucent cellular / ink membrane forms. Slowly evolves over time.",
  "ISFVSN": "2.0",
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "LABEL": "Animation Speed",
      "DEFAULT": 0.08,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "LABEL": "Structure Scale",
      "DEFAULT": 2.6,
      "MIN": 0.5,
      "MAX": 8.0
    },
    {
      "NAME": "warpAmount",
      "TYPE": "float",
      "LABEL": "Fluid Warp",
      "DEFAULT": 0.45,
      "MIN": 0.0,
      "MAX": 2.0
    },
    {
      "NAME": "lineDensity",
      "TYPE": "float",
      "LABEL": "Line Density",
      "DEFAULT": 1.35,
      "MIN": 0.4,
      "MAX": 4.0
    },
    {
      "NAME": "lineThickness",
      "TYPE": "float",
      "LABEL": "Line Thickness",
      "DEFAULT": 0.11,
      "MIN": 0.02,
      "MAX": 0.35
    },
    {
      "NAME": "softness",
      "TYPE": "float",
      "LABEL": "Soft Blur Feel",
      "DEFAULT": 0.18,
      "MIN": 0.01,
      "MAX": 0.6
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "LABEL": "Contrast",
      "DEFAULT": 1.65,
      "MIN": 0.4,
      "MAX": 4.0
    },
    {
      "NAME": "blackLevel",
      "TYPE": "float",
      "LABEL": "Ink Darkness",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.5
    },
    {
      "NAME": "background",
      "TYPE": "color",
      "LABEL": "Background Color",
      "DEFAULT": [
        1.0,
        1.0,
        1.0,
        1.0
      ]
    },
    {
      "NAME": "inkColor",
      "TYPE": "color",
      "LABEL": "Ink Color",
      "DEFAULT": [
        0.0,
        0.0,
        0.0,
        1.0
      ]
    },
    {
      "NAME": "vignette",
      "TYPE": "float",
      "LABEL": "Edge Fade",
      "DEFAULT": 0.12,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "driftX",
      "TYPE": "float",
      "LABEL": "Horizontal Drift",
      "DEFAULT": 0.12,
      "MIN": -1.0,
      "MAX": 1.0
    },
    {
      "NAME": "driftY",
      "TYPE": "float",
      "LABEL": "Vertical Drift",
      "DEFAULT": 0.02,
      "MIN": -1.0,
      "MAX": 1.0
    },
    {
      "NAME": "seed",
      "TYPE": "float",
      "LABEL": "Seed",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 100.0
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "LABEL": "Invert",
      "DEFAULT": false
    }
  ]
}*/

float hash(vec2 p)
{
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

float noise(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);

    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(
        mix(a, b, u.x),
        mix(c, d, u.x),
        u.y
    );
}

float fbm(vec2 p)
{
    float v = 0.0;
    float amp = 0.5;

    mat2 rot = mat2(
        0.80, -0.60,
        0.60,  0.80
    );

    for (int i = 0; i < 5; i++)
    {
        v += amp * noise(p);
        p = rot * p * 2.03 + 13.7;
        amp *= 0.5;
    }

    return v;
}

vec2 flowWarp(vec2 p, float t)
{
    float n1 = fbm(p * 1.15 + vec2(t * 0.13, -t * 0.07));
    float n2 = fbm(p * 1.30 + vec2(-t * 0.09, t * 0.11) + 17.0);

    vec2 w = vec2(n1 - 0.5, n2 - 0.5);

    // A second, larger-scale warp gives the slow liquid membrane motion.
    float n3 = fbm(p * 0.45 + vec2(t * 0.035, t * 0.02));
    float a = n3 * 6.2831853;

    w += vec2(cos(a), sin(a)) * 0.35;

    return p + w * warpAmount;
}

float cellularTendrils(vec2 p, float t)
{
    // Slightly anisotropic coordinates create long horizontal / diagonal membranes.
    p.x *= 1.55;
    p.y *= 0.82;

    p += vec2(driftX, driftY) * t;

    p = flowWarp(p, t);

    float field = 0.0;

    // Several soft wave-distance layers create crossing organic boundaries.
    for (int i = 0; i < 4; i++)
    {
        float fi = float(i);

        vec2 q = p;
        q += vec2(
            fbm(p * (0.9 + fi * 0.25) + fi * 11.0 + t * 0.05),
            fbm(p * (1.1 + fi * 0.20) + fi * 19.0 - t * 0.04)
        ) * 0.55;

        float angle = fi * 1.31 + seed * 0.07;
        vec2 dir = vec2(cos(angle), sin(angle));

        float wave = sin(dot(q, dir) * lineDensity * (2.2 + fi * 0.45) + t * (0.12 + fi * 0.03));
        float ridge = 1.0 - abs(wave);

        ridge = smoothstep(
            1.0 - lineThickness - softness,
            1.0 - lineThickness + softness,
            ridge
        );

        field += ridge * (0.32 + 0.16 * fi);
    }

    // Dark knots where membranes intersect.
    float knots = fbm(p * 2.0 + seed * 3.1 + t * 0.05);
    knots = smoothstep(0.48, 0.82, knots);

    field *= mix(0.75, 1.45, knots);

    return field;
}

void main()
{
    vec2 uv = isf_FragNormCoord.xy;
    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

    vec2 p = (uv - 0.5) * aspect;
    float t = TIME * speed + seed * 10.0;

    p *= scale;

    float ink = cellularTendrils(p, t);

    // Broad translucent shadows around the black membrane.
    float shadow = cellularTendrils(p * 0.82 + vec2(4.7, -2.3), t * 0.65);
    shadow = pow(clamp(shadow, 0.0, 1.0), 2.4) * 0.45;

    ink = ink * 0.85 + shadow * 0.65;

    // Contrast shaping.
    ink = pow(clamp(ink, 0.0, 1.0), contrast);
    ink *= blackLevel;

    // Soft edge fade, useful for projection backgrounds.
    float d = distance(uv, vec2(0.5));
    float edge = smoothstep(0.85, 0.25, d);
    ink *= mix(1.0, edge, vignette);

    ink = clamp(ink, 0.0, 1.0);

    vec3 bg = background.rgb;
    vec3 inkCol = inkColor.rgb;

    vec3 col = mix(bg, inkCol, ink);

    if (invert)
    {
        col = 1.0 - col;
    }

    gl_FragColor = vec4(col, background.a);
}