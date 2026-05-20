/*{
  "DESCRIPTION": "Mondrian Grid — image input, animated grid motion, spatial deformation",
  "CREDIT": "ISF GLSL",
  "ISFVSN": "2",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image",
      "LABEL": "Source Image"
    },
    {
      "NAME": "useImage",
      "TYPE": "bool",
      "DEFAULT": true,
      "LABEL": "Use Image (off = procedural grid)"
    },
    {
      "NAME": "motionSpeed",
      "TYPE": "float",
      "DEFAULT": 0.06,
      "MIN": 0.0,
      "MAX": 0.5,
      "LABEL": "Motion Speed"
    },
    {
      "NAME": "motionAmount",
      "TYPE": "float",
      "DEFAULT": 0.012,
      "MIN": 0.0,
      "MAX": 0.08,
      "LABEL": "Motion Amount"
    },
    {
      "NAME": "gridCols",
      "TYPE": "float",
      "DEFAULT": 3.0,
      "MIN": 1.0,
      "MAX": 8.0,
      "LABEL": "Grid Columns"
    },
    {
      "NAME": "gridRows",
      "TYPE": "float",
      "DEFAULT": 4.0,
      "MIN": 1.0,
      "MAX": 8.0,
      "LABEL": "Grid Rows"
    },
    {
      "NAME": "slideX",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -1.0,
      "MAX": 1.0,
      "LABEL": "Slide X (horizontal shift)"
    },
    {
      "NAME": "slideY",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -1.0,
      "MAX": 1.0,
      "LABEL": "Slide Y (vertical shift)"
    },
    {
      "NAME": "cellScaleX",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.2,
      "MAX": 3.0,
      "LABEL": "Cell Width Stretch"
    },
    {
      "NAME": "cellScaleY",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.2,
      "MAX": 3.0,
      "LABEL": "Cell Height Stretch"
    },
    {
      "NAME": "lineWidth",
      "TYPE": "float",
      "DEFAULT": 0.018,
      "MIN": 0.001,
      "MAX": 0.08,
      "LABEL": "Grid Line Width"
    },
    {
      "NAME": "lineColor",
      "TYPE": "color",
      "DEFAULT": [0.05, 0.04, 0.04, 1.0],
      "LABEL": "Line Color"
    },
    {
      "NAME": "bgColor",
      "TYPE": "color",
      "DEFAULT": [0.94, 0.93, 0.90, 1.0],
      "LABEL": "Background Color"
    },
    {
      "NAME": "accentColor",
      "TYPE": "color",
      "DEFAULT": [0.05, 0.04, 0.04, 1.0],
      "LABEL": "Accent Fill Color"
    },
    {
      "NAME": "accentThreshold",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Accent Threshold (image dark areas)"
    },
    {
      "NAME": "waveDistort",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 0.05,
      "LABEL": "Wave Distortion"
    },
    {
      "NAME": "waveFreq",
      "TYPE": "float",
      "DEFAULT": 4.0,
      "MIN": 0.5,
      "MAX": 20.0,
      "LABEL": "Wave Frequency"
    },
    {
      "NAME": "perspectiveTilt",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -0.3,
      "MAX": 0.3,
      "LABEL": "Perspective Tilt"
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.3,
      "MAX": 3.0,
      "LABEL": "Zoom"
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": -3.14159,
      "MAX": 3.14159,
      "LABEL": "Rotation (rad)"
    },
    {
      "NAME": "invertColors",
      "TYPE": "bool",
      "DEFAULT": false,
      "LABEL": "Invert Colors"
    },
    {
      "NAME": "showGrid",
      "TYPE": "bool",
      "DEFAULT": true,
      "LABEL": "Show Grid Lines"
    },
    {
      "NAME": "cellAnimOffset",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Per-Cell Drift Variation"
    },
    {
      "NAME": "mirrorX",
      "TYPE": "bool",
      "DEFAULT": false,
      "LABEL": "Mirror X"
    },
    {
      "NAME": "mirrorY",
      "TYPE": "bool",
      "DEFAULT": false,
      "LABEL": "Mirror Y"
    }
  ]
}*/

precision highp float;

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(
        mix(hash(i),                    hash(i + vec2(1.0, 0.0)), u.x),
        mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), u.x),
        u.y);
}

// ── Procedural Mondrian grid cell fill ───────────────────────
vec3 proceduralCell(vec2 cellId) {
    float h = hash(cellId);
    if (h < 0.18) return accentColor.rgb;
    return bgColor.rgb;
}

void main() {
    vec2 uv = isf_FragNormCoord;

    // ── Mirror ───────────────────────────────────────────────
    if (mirrorX) uv.x = 1.0 - uv.x;
    if (mirrorY) uv.y = 1.0 - uv.y;

    // ── Zoom + Rotation ──────────────────────────────────────
    vec2 centered = uv - 0.5;
    float cosR = cos(rotation);
    float sinR = sin(rotation);
    centered = vec2(
        centered.x * cosR - centered.y * sinR,
        centered.x * sinR + centered.y * cosR
    );
    centered /= zoom;
    uv = centered + 0.5;

    // ── Perspective tilt (skew Y by X) ───────────────────────
    uv.y += perspectiveTilt * (uv.x - 0.5);

    // ── Wave distortion ──────────────────────────────────────
    float T = TIME * motionSpeed;
    uv.x += waveDistort * sin(uv.y * waveFreq + T * 1.3);
    uv.y += waveDistort * sin(uv.x * waveFreq + T * 0.9);

    // ── Global slow drift ────────────────────────────────────
    vec2 drift = vec2(
        motionAmount * sin(T * 0.7 + 1.1),
        motionAmount * sin(T * 0.5 + 2.3)
    );
    uv += drift;

    // ── Manual slide ─────────────────────────────────────────
    uv += vec2(slideX * 0.5, slideY * 0.5);

    // ── Grid coordinates ─────────────────────────────────────
    vec2 scaled = uv * vec2(gridCols * cellScaleX, gridRows * cellScaleY);
    vec2 cellId  = floor(scaled);
    vec2 cellUV  = fract(scaled);

    // ── Per-cell drift offset ────────────────────────────────
    float cHash = hash(cellId);
    float phase = cHash * 6.2831;
    vec2 cellDrift = cellAnimOffset * motionAmount * vec2(
        sin(T * 0.8 + phase),
        cos(T * 0.6 + phase + 1.0)
    );

    // ── Horizontal sliding: odd rows shift right, even left ───
    float rowParity = mod(cellId.y, 2.0);
    float colParity = mod(cellId.x, 2.0);
    float hSlide = motionAmount * 0.6 *
        sin(T * 0.4 + rowParity * 3.14159 + cellId.y * 0.5);
    float vSlide = motionAmount * 0.6 *
        sin(T * 0.35 + colParity * 3.14159 + cellId.x * 0.5);

    // ── Sample UV for image / procedural ─────────────────────
    vec2 sampleUV = uv + cellDrift + vec2(hSlide, vSlide);
    sampleUV = clamp(sampleUV, 0.0, 1.0);

    // ── Cell base color ───────────────────────────────────────
    vec3 cellColor;
    if (useImage) {
        vec4 img = IMG_NORM_PIXEL(inputImage, sampleUV);
        float luma = 0.299 * img.r + 0.587 * img.g + 0.114 * img.b;
        float dark = 1.0 - luma;
        float accent = smoothstep(accentThreshold, accentThreshold + 0.15, dark);
        cellColor = mix(img.rgb, accentColor.rgb, accent);
    } else {
        cellColor = proceduralCell(cellId);
    }

    // ── Grid lines ────────────────────────────────────────────
    vec3 finalColor = cellColor;
    if (showGrid) {
        float lw = lineWidth;
        float gx = step(cellUV.x, lw) + step(1.0 - lw, cellUV.x);
        float gy = step(cellUV.y, lw) + step(1.0 - lw, cellUV.y);
        float gridMask = clamp(gx + gy, 0.0, 1.0);
        finalColor = mix(cellColor, lineColor.rgb, gridMask);
    }

    // ── Outer border ──────────────────────────────────────────
    vec2 borderUV = isf_FragNormCoord;
    float bw = lineWidth * 0.7;
    float border = step(borderUV.x, bw) + step(1.0 - bw, borderUV.x) +
                   step(borderUV.y, bw) + step(1.0 - bw, borderUV.y);
    finalColor = mix(finalColor, lineColor.rgb, clamp(border, 0.0, 1.0));

    // ── Invert ────────────────────────────────────────────────
    if (invertColors) finalColor = 1.0 - finalColor;

    gl_FragColor = vec4(finalColor, 1.0);
}
