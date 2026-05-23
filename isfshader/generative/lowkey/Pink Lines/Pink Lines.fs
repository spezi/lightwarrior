/*{
  "DESCRIPTION": "Pink flowing striped abstract pattern with true stripe animation",
  "CREDIT": "OpenAI",
  "ISFVSN": "2",
  "CATEGORIES": [
    "Generator",
    "Pattern"
  ],
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 0.6,
      "MIN": -5.0,
      "MAX": 5.0
    },
    {
      "NAME": "stripeDensity",
      "TYPE": "float",
      "DEFAULT": 48.0,
      "MIN": 5.0,
      "MAX": 140.0
    },
    {
      "NAME": "stripeWidth",
      "TYPE": "float",
      "DEFAULT": 0.70,
      "MIN": 0.01,
      "MAX": 0.99
    },
    {
      "NAME": "flow1",
      "TYPE": "float",
      "DEFAULT": 0.18,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "flow2",
      "TYPE": "float",
      "DEFAULT": 0.08,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "twist",
      "TYPE": "float",
      "DEFAULT": 0.10,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "softness",
      "TYPE": "float",
      "DEFAULT": 0.06,
      "MIN": 0.001,
      "MAX": 0.3
    },
    {
      "NAME": "background",
      "TYPE": "color",
      "DEFAULT": [0.97, 0.93, 0.95, 1.0]
    },
    {
      "NAME": "pinkLight",
      "TYPE": "color",
      "DEFAULT": [0.87, 0.60, 0.72, 1.0]
    },
    {
      "NAME": "pinkDark",
      "TYPE": "color",
      "DEFAULT": [0.68, 0.22, 0.40, 1.0]
    },
    {
      "NAME": "animate",
      "TYPE": "bool",
      "DEFAULT": true
    }
  ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord.xy;

    // espaço centralizado
    vec2 p = uv * 2.0 - 1.0;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    float t = animate ? TIME : 0.0;
    float phase = t * speed;

    // distorção do espaço
    vec2 q = p;
    q.x += flow1 * sin(q.y * 3.2 + t * 0.7);
    q.y += flow2 * sin(q.x * 2.2 - t * 0.45);

    q.x += twist * sin(q.y * 7.0 + q.x * 1.7 + t * 0.35);
    q.y += (twist * 0.45) * sin(q.x * 5.0 - q.y * 1.3 - t * 0.2);

    q.x += 0.05 * sin(length(q * vec2(0.9, 1.3)) * 8.0 - t * 0.4);

    // animação REAL:
    // a fase entra direto na senoide das faixas
    float wave = sin((q.x + 0.22 * sin(q.y * 4.0)) * stripeDensity + phase * 6.28318);
    float bands = wave * 0.5 + 0.5;
    float mask = smoothstep(stripeWidth - softness, stripeWidth + softness, bands);

    // segunda camada também animada
    float wave2 = sin((q.x + 0.12 * sin(q.y * 2.8 - 0.7)) * (stripeDensity * 0.72) - phase * 4.2);
    float bands2 = wave2 * 0.5 + 0.5;
    float mask2 = smoothstep(0.62 - softness, 0.62 + softness, bands2);

    vec3 col = background.rgb;
    col = mix(col, pinkLight.rgb, mask * 0.85);
    col = mix(col, pinkDark.rgb, mask2 * 0.60);

    float vignette = 1.0 - 0.10 * length(p);
    col *= vignette;

    gl_FragColor = vec4(col, 1.0);
}
