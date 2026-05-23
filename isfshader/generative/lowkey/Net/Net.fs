/*{
    "ISFVSN":"2",
    "TYPE":"IMAGE",
    "LABEL":"Minimal Organic Neural Lines",
    "INPUTS":[
        {
            "NAME":"Zoom",
            "TYPE":"float",
            "DEFAULT":3.2,
            "MIN":0.5,
            "MAX":8.0
        },
        {
            "NAME":"Speed",
            "TYPE":"float",
            "DEFAULT":0.12,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"Motion",
            "TYPE":"float",
            "DEFAULT":0.22,
            "MIN":0.0,
            "MAX":1.0
        },
        {
            "NAME":"LineThickness",
            "TYPE":"float",
            "DEFAULT":0.035,
            "MIN":0.005,
            "MAX":0.12
        },
        {
            "NAME":"Softness",
            "TYPE":"float",
            "DEFAULT":0.055,
            "MIN":0.005,
            "MAX":0.20
        },
        {
            "NAME":"Brightness",
            "TYPE":"float",
            "DEFAULT":1.35,
            "MIN":0.0,
            "MAX":3.0
        },
        {
            "NAME":"Glow",
            "TYPE":"float",
            "DEFAULT":0.65,
            "MIN":0.0,
            "MAX":2.0
        },
        {
            "NAME":"Contrast",
            "TYPE":"float",
            "DEFAULT":1.6,
            "MIN":0.5,
            "MAX":4.0
        },
        {
            "NAME":"Warp",
            "TYPE":"float",
            "DEFAULT":0.28,
            "MIN":0.0,
            "MAX":1.5
        },
        {
            "NAME":"LineColor",
            "TYPE":"color",
            "DEFAULT":[1.0,1.0,1.0,1.0]
        },
        {
            "NAME":"Background",
            "TYPE":"color",
            "DEFAULT":[0.0,0.0,0.0,1.0]
        }
    ]
}*/

float hash11(float p)
{
    return fract(sin(p * 127.1) * 43758.5453123);
}

vec2 hash22(vec2 p)
{
    vec2 q = vec2(
        dot(p, vec2(127.1, 311.7)),
        dot(p, vec2(269.5, 183.3))
    );

    return fract(sin(q) * 43758.5453);
}

float noise(vec2 p)
{
    vec2 i = floor(p);
    vec2 f = fract(p);

    vec2 u = f * f * (3.0 - 2.0 * f);

    float a = hash11(dot(i + vec2(0.0, 0.0), vec2(1.0, 57.0)));
    float b = hash11(dot(i + vec2(1.0, 0.0), vec2(1.0, 57.0)));
    float c = hash11(dot(i + vec2(0.0, 1.0), vec2(1.0, 57.0)));
    float d = hash11(dot(i + vec2(1.0, 1.0), vec2(1.0, 57.0)));

    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

vec2 warpField(vec2 p, float t)
{
    float n1 = noise(p * 0.85 + vec2(t * 0.18, -t * 0.12));
    float n2 = noise(p * 1.15 + vec2(-t * 0.10, t * 0.15));

    return vec2(n1 - 0.5, n2 - 0.5);
}

vec3 voronoiLines(vec2 p, float t)
{
    vec2 cell = floor(p);
    vec2 local = fract(p);

    float d1 = 10.0;
    float d2 = 10.0;
    float brightnessSeed = 0.0;

    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            vec2 offset = vec2(float(x), float(y));
            vec2 h = hash22(cell + offset);

            vec2 animatedPoint = 0.5 + 0.5 * sin(
                t * Speed * 6.2831 +
                h * 6.2831 +
                vec2(h.y, h.x) * Motion * 4.0
            );

            animatedPoint = mix(h, animatedPoint, Motion);

            vec2 r = offset + animatedPoint - local;
            float d = dot(r, r);

            if (d < d1)
            {
                d2 = d1;
                d1 = d;
                brightnessSeed = h.x;
            }
            else if (d < d2)
            {
                d2 = d;
            }
        }
    }

    float edge = abs(sqrt(d2) - sqrt(d1));

    float thinLine = 1.0 - smoothstep(LineThickness, LineThickness + Softness, edge);
    float wideGlow = 1.0 - smoothstep(LineThickness * 2.5, LineThickness * 2.5 + Softness * 4.0, edge);

    float pulse = 0.75 + 0.25 * sin(t * 0.8 + brightnessSeed * 12.0);

    float line = thinLine * pulse;
    float glow = wideGlow * Glow * 0.35;

    float result = line + glow;

    return vec3(result);
}

void main()
{
    vec2 uv = isf_FragNormCoord.xy;

    vec2 aspect = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
    vec2 p = (uv - 0.5) * aspect;

    float t = TIME;

    p *= Zoom;

    vec2 w1 = warpField(p * 0.8, t);
    vec2 w2 = warpField(p * 1.7 + 4.0, t * 0.7);

    p += (w1 * 0.75 + w2 * 0.35) * Warp;

    vec3 linesA = voronoiLines(p, t);
    vec3 linesB = voronoiLines(p * 1.65 + vec2(2.7, -1.4), t * 0.82) * 0.45;
    vec3 linesC = voronoiLines(p * 0.62 + vec2(-3.1, 1.8), t * 0.55) * 0.35;

    vec3 lineValue = linesA + linesB + linesC;

    lineValue = pow(lineValue * Brightness, vec3(Contrast));

    vec3 bg = Background.rgb;
    vec3 fg = LineColor.rgb;

    vec3 color = bg + fg * lineValue;

    color = clamp(color, 0.0, 1.0);

    gl_FragColor = vec4(color, 1.0);
}