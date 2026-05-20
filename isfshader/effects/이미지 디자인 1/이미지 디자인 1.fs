/*{
  "DESCRIPTION": "Living ink wash portrait. Loads an image and turns it into breathing watercolor ink with paper grain, edge bloom, pigment drift, and vertical drips.",
  "CATEGORIES": ["Generator", "Stylize", "Watercolor"],
  "ISFVSN": "2.0",
  "INPUTS": [
    { "NAME": "inputImage", "TYPE": "image" },
    { "NAME": "imageScale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.25, "MAX": 4.0 },
    { "NAME": "imageOffsetX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "imageOffsetY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "imageRotation", "TYPE": "float", "DEFAULT": 0.0, "MIN": -180.0, "MAX": 180.0 },
    { "NAME": "mirrorX", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "mirrorY", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "invertImage", "TYPE": "bool", "DEFAULT": false },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.35, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "gamma", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0 },
    { "NAME": "inkThreshold", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "inkSoftness", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.001, "MAX": 1.0 },
    { "NAME": "inkStrength", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.5 },
    { "NAME": "washAmount", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "edgeBleed", "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "paperGrain", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "pigmentNoise", "TYPE": "float", "DEFAULT": 0.28, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "turbulence", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "breath", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 0.6 },
    { "NAME": "breathSpeed", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "dripAmount", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "dripLength", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.5 },
    { "NAME": "dripWidth", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.02, "MAX": 1.0 },
    { "NAME": "dripSpeed", "TYPE": "float", "DEFAULT": 0.06, "MIN": -1.0, "MAX": 1.0 },
    { "NAME": "evaporation", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "backgroundVignette", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "sourceImageMix", "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "seed", "TYPE": "float", "DEFAULT": 73.0, "MIN": 0.0, "MAX": 999.0 },
    { "NAME": "paperColor", "TYPE": "color", "DEFAULT": [0.72, 0.68, 0.56, 1.0] },
    { "NAME": "inkColor", "TYPE": "color", "DEFAULT": [0.035, 0.032, 0.028, 1.0] },
    { "NAME": "washColor", "TYPE": "color", "DEFAULT": [0.42, 0.39, 0.33, 1.0] }
  ]
}*/

#ifdef GL_ES
precision mediump float;
#endif

float hash12(vec2 p) {
  vec3 p3 = fract(vec3(p.xyx) * 0.1031);
  p3 += dot(p3, p3.yzx + 33.33);
  return fract((p3.x + p3.y) * p3.z);
}

float luminance(vec3 c) {
  return dot(c, vec3(0.299, 0.587, 0.114));
}

vec2 rotate2D(vec2 p, float deg) {
  float r = radians(deg);
  float c = cos(r);
  float s = sin(r);
  return mat2(c, -s, s, c) * p;
}

vec2 imageUV(vec2 uv) {
  vec2 p = uv - 0.5;
  p = rotate2D(p, imageRotation);
  p /= imageScale;
  p += vec2(imageOffsetX, imageOffsetY);
  p += 0.5;

  if (mirrorX) {
    p.x = 1.0 - p.x;
  }
  if (mirrorY) {
    p.y = 1.0 - p.y;
  }

  return p;
}

vec4 sampleImageRaw(vec2 uv) {
  vec2 suv = imageUV(uv);
  if (suv.x < 0.0 || suv.x > 1.0 || suv.y < 0.0 || suv.y > 1.0) {
    return vec4(0.0);
  }
  return IMG_NORM_PIXEL(inputImage, suv);
}

float imageInk(vec2 uv) {
  vec4 src = sampleImageRaw(uv);
  float v = luminance(src.rgb);
  if (invertImage) {
    v = 1.0 - v;
  } else {
    v = 1.0 - v;
  }
  v = (v - 0.5) * contrast + 0.5 + brightness;
  v = clamp(v, 0.0, 1.0);
  v = pow(v, gamma);
  return smoothstep(inkThreshold, inkThreshold + inkSoftness, v);
}

float softInk(vec2 uv) {
  vec2 texel = vec2(1.0) / RENDERSIZE.xy;
  float c = imageInk(uv) * 0.42;
  c += imageInk(uv + vec2(texel.x * 2.0, 0.0)) * 0.145;
  c += imageInk(uv - vec2(texel.x * 2.0, 0.0)) * 0.145;
  c += imageInk(uv + vec2(0.0, texel.y * 2.0)) * 0.145;
  c += imageInk(uv - vec2(0.0, texel.y * 2.0)) * 0.145;
  return clamp(c, 0.0, 1.0);
}

float pigmentField(vec2 uv) {
  vec2 p = uv * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
  float n1 = hash12(floor(p * 170.0) + seed);
  float n2 = hash12(floor(p * 42.0) + seed * 2.0);
  float wave = sin((p.x * 19.0 + p.y * 23.0) + TIME * breathSpeed);
  return mix(n1, n2, 0.65) + wave * 0.12;
}

float edgeBloom(vec2 uv, float ink) {
  vec2 texel = vec2(1.0) / RENDERSIZE.xy;
  float wide = 0.0;
  wide += imageInk(uv + vec2(texel.x * 5.0, 0.0));
  wide += imageInk(uv - vec2(texel.x * 5.0, 0.0));
  wide += imageInk(uv + vec2(0.0, texel.y * 5.0));
  wide += imageInk(uv - vec2(0.0, texel.y * 5.0));
  wide *= 0.25;
  return max(wide - ink, 0.0) * edgeBleed;
}

float dripColumn(vec2 uv, float xSeed, float widthMul) {
  float x = fract(uv.x * mix(34.0, 120.0, dripWidth) + xSeed);
  float center = 0.5 + (hash12(vec2(floor(uv.x * 80.0), seed + xSeed)) - 0.5) * 0.25;
  float column = 1.0 - smoothstep(0.0, 0.18 * widthMul, abs(x - center));
  return column;
}

float inkDrips(vec2 uv) {
  float timeFall = TIME * dripSpeed * 0.08;
  float d = 0.0;

  vec2 up1 = uv + vec2(0.0, 0.08 + dripLength * 0.08 + timeFall);
  vec2 up2 = uv + vec2(0.0, 0.18 + dripLength * 0.18 + timeFall);
  vec2 up3 = uv + vec2(0.0, 0.32 + dripLength * 0.28 + timeFall);
  vec2 up4 = uv + vec2(0.0, 0.48 + dripLength * 0.36 + timeFall);

  float source1 = imageInk(up1);
  float source2 = imageInk(up2) * 0.72;
  float source3 = imageInk(up3) * 0.48;
  float source4 = imageInk(up4) * 0.25;

  float c1 = dripColumn(uv + vec2(0.0, timeFall), 1.7, 1.0);
  float c2 = dripColumn(uv + vec2(0.017, timeFall * 0.6), 5.1, 0.7);
  float c3 = dripColumn(uv + vec2(-0.013, timeFall * 0.3), 8.4, 1.35);

  float taper = smoothstep(1.05, 0.1, uv.y);
  d += source1 * c1;
  d += source2 * c2;
  d += source3 * c3;
  d += source4 * c1 * c2;
  d *= taper * dripAmount;
  return clamp(d, 0.0, 1.0);
}

void main() {
  vec2 uv = isf_FragNormCoord.xy;
  vec2 p = uv - 0.5;

  float pulse = sin(TIME * breathSpeed * 6.2831) * breath;
  vec2 aliveUV = uv + p * pulse * 0.018;
  float turb = pigmentField(uv) - 0.5;
  aliveUV += vec2(turb, -turb) * turbulence * 0.012;

  float ink = imageInk(aliveUV);
  float wash = softInk(aliveUV);
  float bloom = edgeBloom(aliveUV, ink);
  float drips = inkDrips(aliveUV);
  float pigment = pigmentField(uv);

  float paperNoise = (hash12(floor(uv * RENDERSIZE.xy * 0.65) + seed) - 0.5) * paperGrain;
  float fiber = sin((uv.x * RENDERSIZE.x + uv.y * 37.0) * 0.11) * paperGrain * 0.025;

  float wet = clamp(wash * washAmount + bloom + drips, 0.0, 1.0);
  float dense = clamp(ink * inkStrength + drips * 0.85, 0.0, 1.0);
  dense *= 1.0 + pigmentNoise * (pigment - 0.5);
  dense *= mix(1.0, 1.0 - evaporation * 0.45, smoothstep(0.0, 1.0, TIME * 0.03));
  dense = clamp(dense, 0.0, 1.0);

  vec3 src = sampleImageRaw(aliveUV).rgb;
  vec3 col = paperColor.rgb;
  col = mix(col, washColor.rgb, wet * 0.68);
  col = mix(col, inkColor.rgb, dense);
  col = mix(col, src, sourceImageMix * ink);

  float vignette = smoothstep(0.95, 0.25, length((uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0)));
  col = mix(col, paperColor.rgb * 0.82, backgroundVignette * (1.0 - vignette));
  col += paperNoise + fiber;

  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}