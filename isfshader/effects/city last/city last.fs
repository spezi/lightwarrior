/*{
  "DESCRIPTION": "16:9 apartment window light animation shader for concert backgrounds. Uses the brightness of the source image as a window-light mask.",
  "CATEGORIES": ["Generator", "Stylize"],
  "INPUTS": [
    {
      "NAME": "apartmentImage",
      "TYPE": "image"
    },
    {
      "NAME": "Light_Switch",
      "TYPE": "bool",
      "DEFAULT": true,
      "LABEL": "Light Switch"
    },
    {
      "NAME": "Manual_Light",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Manual Light On/Off"
    },
    {
      "NAME": "Blackout",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Blackout"
    },
    {
      "NAME": "Motion_Amount",
      "TYPE": "float",
      "DEFAULT": 0.65,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Moving Light Amount"
    },
    {
      "NAME": "Speed",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0.0,
      "MAX": 3.0,
      "LABEL": "Animation Speed"
    },
    {
      "NAME": "Wave_Scale",
      "TYPE": "float",
      "DEFAULT": 5.0,
      "MIN": 1.0,
      "MAX": 20.0,
      "LABEL": "Wave Scale"
    },
    {
      "NAME": "Wave_Softness",
      "TYPE": "float",
      "DEFAULT": 0.45,
      "MIN": 0.05,
      "MAX": 1.0,
      "LABEL": "Wave Softness"
    },
    {
      "NAME": "Direction",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Direction X/Y"
    },
    {
      "NAME": "Random_Windows",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Random Window Flicker"
    },
    {
      "NAME": "Flicker",
      "TYPE": "float",
      "DEFAULT": 0.18,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Fine Flicker"
    },
    {
      "NAME": "Light_Boost",
      "TYPE": "float",
      "DEFAULT": 1.25,
      "MIN": 0.0,
      "MAX": 3.0,
      "LABEL": "Light Brightness"
    },
    {
      "NAME": "Light_Warmth",
      "TYPE": "float",
      "DEFAULT": 0.55,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Warm/Cool Color"
    },
    {
      "NAME": "Off_Darkness",
      "TYPE": "float",
      "DEFAULT": 0.82,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Window Off Darkness"
    },
    {
      "NAME": "Glow",
      "TYPE": "float",
      "DEFAULT": 0.22,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Window Glow"
    },
    {
      "NAME": "Mask_Threshold",
      "TYPE": "float",
      "DEFAULT": 0.28,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Light Detection Threshold"
    },
    {
      "NAME": "Mask_Softness",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.01,
      "MAX": 0.8,
      "LABEL": "Light Detection Softness"
    },
    {
      "NAME": "Grid_Columns",
      "TYPE": "float",
      "DEFAULT": 42.0,
      "MIN": 8.0,
      "MAX": 90.0,
      "LABEL": "Window Grid Columns"
    },
    {
      "NAME": "Grid_Rows",
      "TYPE": "float",
      "DEFAULT": 18.0,
      "MIN": 4.0,
      "MAX": 45.0,
      "LABEL": "Window Grid Rows"
    },
    {
      "NAME": "Building_Dim",
      "TYPE": "float",
      "DEFAULT": 0.08,
      "MIN": 0.0,
      "MAX": 0.8,
      "LABEL": "Building Dim"
    },
    {
      "NAME": "Vignette",
      "TYPE": "float",
      "DEFAULT": 0.15,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Vignette"
    },
    {
      "NAME": "Seed",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": 0.0,
      "MAX": 100.0,
      "LABEL": "Random Seed"
    }
  ]
}*/

float hash21(vec2 p) {
  p = fract(p * vec2(123.34, 456.21));
  p += dot(p, p + 45.32);
  return fract(p.x * p.y);
}

vec4 safeImage(vec2 uv) {
  if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) {
    return vec4(0.0, 0.0, 0.0, 1.0);
  }
  return IMG_NORM_PIXEL(apartmentImage, uv);
}

vec2 aspect16x9(vec2 uv) {
  float targetAspect = 16.0 / 9.0;
  float screenAspect = RENDERSIZE.x / RENDERSIZE.y;
  vec2 p = uv;

  if (screenAspect > targetAspect) {
    p.x = (uv.x - 0.5) * (screenAspect / targetAspect) + 0.5;
  } else {
    p.y = (uv.y - 0.5) * (targetAspect / screenAspect) + 0.5;
  }

  return p;
}

float luminance(vec3 c) {
  return dot(c, vec3(0.299, 0.587, 0.114));
}

float lightMaskFromColor(vec3 c) {
  float l = luminance(c);
  float mx = max(max(c.r, c.g), c.b);
  float mn = min(min(c.r, c.g), c.b);
  float sat = mx - mn;

  float brightMask = smoothstep(Mask_Threshold, Mask_Threshold + Mask_Softness, l);
  float colorMask = smoothstep(0.04, 0.26, sat);

  return clamp(brightMask * mix(0.65, 1.0, colorMask), 0.0, 1.0);
}

float windowState(vec2 uv, float mask) {
  vec2 grid = vec2(Grid_Columns, Grid_Rows);
  vec2 cell = floor(uv * grid);
  vec2 cellUV = fract(uv * grid);

  float rnd = hash21(cell + Seed);
  float rnd2 = hash21(cell * 1.73 + Seed * 5.31);

  float axisPos = mix(uv.x, uv.y, Direction);
  float wave = sin((axisPos * Wave_Scale + TIME * Speed + rnd * 1.7) * 6.2831853);
  wave = wave * 0.5 + 0.5;
  float waveGate = smoothstep(1.0 - Wave_Softness, 1.0, wave);

  float randomPulse = sin(TIME * Speed * (2.0 + rnd * 5.0) + rnd2 * 18.0);
  randomPulse = randomPulse * 0.5 + 0.5;
  randomPulse = smoothstep(0.18, 0.92, randomPulse);

  float cellEdge = smoothstep(0.0, 0.08, cellUV.x) *
                   smoothstep(0.0, 0.08, cellUV.y) *
                   smoothstep(0.0, 0.08, 1.0 - cellUV.x) *
                   smoothstep(0.0, 0.08, 1.0 - cellUV.y);

  float animated = mix(1.0, waveGate, Motion_Amount);
  animated *= mix(1.0, randomPulse, Random_Windows);

  float power = Manual_Light;
  if (!Light_Switch) {
    power = 0.0;
  }

  return clamp(animated * power * cellEdge * mask, 0.0, 1.0);
}

void main() {
  vec2 uv = aspect16x9(isf_FragNormCoord);

  vec4 src = safeImage(uv);
  vec3 col = src.rgb;

  float mask = lightMaskFromColor(col);
  float state = windowState(uv, mask);

  vec2 cell = floor(uv * vec2(Grid_Columns, Grid_Rows));
  float flick = hash21(cell + floor(TIME * 24.0 * max(Speed, 0.02)) + Seed);
  flick = mix(1.0, 0.82 + flick * 0.38, Flicker);

  vec3 coolLight = vec3(0.45, 1.0, 0.9);
  vec3 warmLight = vec3(1.0, 0.52, 0.08);
  vec3 tint = mix(coolLight, warmLight, Light_Warmth);

  vec3 offColor = col * vec3(0.08, 0.16, 0.20);
  vec3 windowOff = mix(col, offColor, mask * Off_Darkness);

  vec3 litColor = col + tint * mask * state * Light_Boost * flick;
  vec3 result = mix(windowOff, litColor, state);

  float glowMask = 0.0;
  glowMask += lightMaskFromColor(safeImage(uv + vec2( 0.003,  0.000)).rgb);
  glowMask += lightMaskFromColor(safeImage(uv + vec2(-0.003,  0.000)).rgb);
  glowMask += lightMaskFromColor(safeImage(uv + vec2( 0.000,  0.005)).rgb);
  glowMask += lightMaskFromColor(safeImage(uv + vec2( 0.000, -0.005)).rgb);
  glowMask += lightMaskFromColor(safeImage(uv + vec2( 0.006,  0.004)).rgb);
  glowMask += lightMaskFromColor(safeImage(uv + vec2(-0.006, -0.004)).rgb);
  glowMask /= 6.0;

  result += tint * glowMask * state * Glow * 0.55;

  result *= 1.0 - Building_Dim;

  float d = distance(isf_FragNormCoord, vec2(0.5));
  float vig = smoothstep(0.85, 0.25, d);
  result *= mix(1.0, vig, Vignette);

  result = mix(result, result * 0.025, Blackout);

  gl_FragColor = vec4(clamp(result, 0.0, 1.0), src.a);
}