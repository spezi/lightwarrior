/*{
    "DESCRIPTION": "Generative circuit blueprint with flowing data pulses, glowing nodes, and electrical signals — inspired by technical schematics.",
    "CREDIT": "Generated for circuit blueprint artwork",
    "ISFVSN": "2",
    "CATEGORIES": [
        "Generator",
        "Geometric",
        "Technical"
    ],
    "INPUTS": [
        {
            "NAME": "bgColor",
            "TYPE": "color",
            "DEFAULT": [0.07, 0.18, 0.42, 1.0],
            "LABEL": "Background Color"
        },
        {
            "NAME": "lineColor",
            "TYPE": "color",
            "DEFAULT": [0.85, 0.92, 1.0, 1.0],
            "LABEL": "Line Color"
        },
        {
            "NAME": "pulseColor",
            "TYPE": "color",
            "DEFAULT": [0.4, 1.0, 0.95, 1.0],
            "LABEL": "Pulse / Signal Color"
        },
        {
            "NAME": "glowColor",
            "TYPE": "color",
            "DEFAULT": [0.2, 0.7, 1.0, 1.0],
            "LABEL": "Node Glow Color"
        },
        {
            "NAME": "density",
            "TYPE": "float",
            "DEFAULT": 18.0,
            "MIN": 4.0,
            "MAX": 60.0,
            "LABEL": "Circuit Density"
        },
        {
            "NAME": "lineWidth",
            "TYPE": "float",
            "DEFAULT": 0.04,
            "MIN": 0.005,
            "MAX": 0.15,
            "LABEL": "Line Width"
        },
        {
            "NAME": "pulseSpeed",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 5.0,
            "LABEL": "Pulse Speed"
        },
        {
            "NAME": "pulseDensity",
            "TYPE": "float",
            "DEFAULT": 0.6,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Pulse Density"
        },
        {
            "NAME": "nodeSize",
            "TYPE": "float",
            "DEFAULT": 0.18,
            "MIN": 0.02,
            "MAX": 0.5,
            "LABEL": "Node Size"
        },
        {
            "NAME": "glitch",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Glitch / Noise"
        },
        {
            "NAME": "scanline",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Scanline Sweep"
        },
        {
            "NAME": "zoom",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.2,
            "MAX": 4.0,
            "LABEL": "Zoom"
        },
        {
            "NAME": "panX",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0,
            "LABEL": "Pan X"
        },
        {
            "NAME": "panY",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": -2.0,
            "MAX": 2.0,
            "LABEL": "Pan Y"
        },
        {
            "NAME": "autoPan",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Auto Pan Speed"
        },
        {
            "NAME": "audioReact",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 2.0,
            "LABEL": "Audio Reactivity"
        },
        {
            "NAME": "audioLevel",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 1.0,
            "LABEL": "Audio Level (external)"
        }
    ]
}*/

// ----------------------------------------------------------------------------
// CircuitBlueprint.fs
// A generative blueprint of recursive circuit traces, junctions, and pulses.
// Built around a hash-driven grid that decides, at each cell, what kind of
// trace pattern lives there and how signals propagate through it.
// ----------------------------------------------------------------------------

// --- hashing utilities ------------------------------------------------------

float hash11(float p) {
    p = fract(p * 0.1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

float hash21(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.x + p3.y) * p3.z);
}

vec2 hash22(vec2 p) {
    vec3 p3 = fract(vec3(p.xyx) * vec3(0.1031, 0.1030, 0.0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return fract((p3.xx + p3.yz) * p3.zy);
}

// --- distance primitives ----------------------------------------------------

// distance to a line segment from a to b
float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

// distance to axis-aligned rectangle outline
float sdRectOutline(vec2 p, vec2 size, float thickness) {
    vec2 d = abs(p) - size;
    float outside = length(max(d, 0.0));
    float inside = min(max(d.x, d.y), 0.0);
    return abs(outside + inside) - thickness;
}

// soft "draw" of a line: returns intensity in [0,1], 1 at center
float drawLine(float dist, float width, float feather) {
    return 1.0 - smoothstep(width - feather, width + feather, dist);
}

// --- per-cell circuit drawing -----------------------------------------------
// Each cell of the grid renders a small traced pattern. We pick the pattern
// from a hash so it feels handcrafted rather than uniform.

float cellTrace(vec2 uv, vec2 cellId, float lineW, out vec2 nodePos, out float isNode) {
    nodePos = vec2(0.5);
    isNode = 0.0;

    // local coords inside cell, range [-0.5, 0.5]
    vec2 p = uv - 0.5;

    float h = hash21(cellId);
    float h2 = hash21(cellId + 17.3);
    float h3 = hash21(cellId + 71.7);

    // pick a "pattern type"
    float pattern = floor(h * 6.0);

    float d = 1.0;

    if (pattern < 1.0) {
        // straight horizontal trace with two stubs
        d = min(d, sdSegment(p, vec2(-0.5, 0.0), vec2(0.5, 0.0)));
        d = min(d, sdSegment(p, vec2(h2 - 0.5, 0.0),
                                vec2(h2 - 0.5, (h3 - 0.5) * 0.6)));
    } else if (pattern < 2.0) {
        // L-bend trace
        vec2 corner = vec2(h2 - 0.5, h3 - 0.5) * 0.5;
        d = min(d, sdSegment(p, vec2(-0.5, corner.y), corner));
        d = min(d, sdSegment(p, corner, vec2(corner.x, 0.5)));
        nodePos = corner + 0.5;
        isNode = step(0.7, h);
    } else if (pattern < 3.0) {
        // T-junction with node
        vec2 c = vec2(0.0, 0.0);
        d = min(d, sdSegment(p, vec2(-0.5, 0.0), vec2(0.5, 0.0)));
        d = min(d, sdSegment(p, c, vec2(0.0, 0.5)));
        nodePos = vec2(0.5);
        isNode = 1.0;
    } else if (pattern < 4.0) {
        // diagonal cross
        d = min(d, sdSegment(p, vec2(-0.5, -0.5), vec2(0.5, 0.5)));
        d = min(d, sdSegment(p, vec2(-0.5, 0.5), vec2(0.5, -0.5)));
    } else if (pattern < 5.0) {
        // small IC block
        float w = 0.18 + 0.12 * h2;
        float hgt = 0.22 + 0.15 * h3;
        float box = sdRectOutline(p, vec2(w, hgt), lineW * 0.5);
        d = min(d, box);
        // entry / exit traces
        d = min(d, sdSegment(p, vec2(-0.5, 0.0), vec2(-w, 0.0)));
        d = min(d, sdSegment(p, vec2(w, 0.0), vec2(0.5, 0.0)));
        // pin marks
        for (int i = 0; i < 3; i++) {
            float fi = float(i) - 1.0;
            float yy = fi * hgt * 0.5;
            d = min(d, sdSegment(p, vec2(-w, yy), vec2(-w - 0.05, yy)));
            d = min(d, sdSegment(p, vec2(w, yy), vec2(w + 0.05, yy)));
        }
    } else {
        // capacitor / resistor block: two parallel verticals + leads
        float xSep = 0.08 + 0.05 * h2;
        d = min(d, sdSegment(p, vec2(-xSep, -0.18), vec2(-xSep, 0.18)));
        d = min(d, sdSegment(p, vec2( xSep, -0.18), vec2( xSep, 0.18)));
        d = min(d, sdSegment(p, vec2(-0.5, 0.0), vec2(-xSep, 0.0)));
        d = min(d, sdSegment(p, vec2(xSep, 0.0), vec2(0.5, 0.0)));
    }

    return d;
}

// --- pulse along the cell -------------------------------------------------
// We model a pulse that travels along a horizontal-ish path inside each cell.
// Distance to that traveling point becomes a glow.

float cellPulse(vec2 uv, vec2 cellId, float t) {
    float h = hash21(cellId + 3.7);
    float h2 = hash21(cellId + 9.1);

    // probability this cell carries a pulse at all
    float active = step(1.0 - 0.7 * h2, hash21(cellId + 41.0));
    if (active < 0.5) return 0.0;

    // direction: horizontal or vertical, picked from hash
    float dir = step(0.5, h);
    float speed = 0.4 + h * 1.2;
    float phase = h2;
    float u = fract(t * speed + phase);

    vec2 pulsePos = mix(vec2(u, 0.5), vec2(0.5, u), dir);
    float d = length(uv - pulsePos);
    return exp(-d * 25.0);
}

void main() {
    // ----- normalized coords -----
    vec2 uv = isf_FragNormCoord;
    vec2 res = RENDERSIZE;
    float aspect = res.x / res.y;

    // center, apply zoom & pan
    vec2 p = (uv - 0.5);
    p.x *= aspect;
    p /= zoom;
    p += vec2(panX, panY);
    p += vec2(autoPan * TIME * 0.3, autoPan * TIME * 0.17);

    // audio-reactive scaling (combine external level + reactivity strength)
    float audio = clamp(audioLevel, 0.0, 1.0) * audioReact;

    // ----- grid -----
    float cellsX = max(2.0, density);
    vec2 g = p * cellsX;
    vec2 cellId = floor(g);
    vec2 cellUV = fract(g);

    // sample a 3x3 neighborhood so traces visually connect across cell borders
    float dist = 1e9;
    float pulseGlow = 0.0;
    float nodeGlow = 0.0;

    float lw = lineWidth * (1.0 + audio * 0.5);

    for (int oy = -1; oy <= 1; oy++) {
        for (int ox = -1; ox <= 1; ox++) {
            vec2 off = vec2(float(ox), float(oy));
            vec2 cid = cellId + off;
            vec2 luv = cellUV - off;

            // jitter the cell so it doesn't look like a perfect grid
            vec2 jit = (hash22(cid) - 0.5) * 0.15;
            vec2 luvJ = luv + jit;

            vec2 nodePos;
            float isNode;
            float d = cellTrace(luvJ, cid, lw, nodePos, isNode);
            dist = min(dist, d);

            // pulse traveling along trace
            float pg = cellPulse(luvJ, cid, TIME * pulseSpeed) * pulseDensity;
            pulseGlow = max(pulseGlow, pg);

            // node glow: radial blob at junction
            float dn = length(luv - nodePos);
            float nodeOn = isNode * step(0.4, hash21(cid + 5.0));
            nodeGlow = max(nodeGlow,
                           nodeOn * exp(-dn * (10.0 / max(nodeSize, 0.01))));
        }
    }

    // ----- compose -----
    float feather = lw * 0.6;
    float trace = drawLine(dist, lw, feather);

    // glow halo around traces
    float halo = exp(-dist * 12.0) * 0.35;

    // scanline sweep that re-energizes pulses as it passes
    float sweepPos = fract(TIME * 0.15);
    float sweep = exp(-pow((uv.x - sweepPos) * 6.0, 2.0)) * scanline;

    // small bursts: occasional bright flashes on random cells
    float burst = 0.0;
    {
        float h = hash21(cellId + floor(TIME * 0.7));
        if (h > 0.985) {
            float dn = length(cellUV - 0.5);
            burst = exp(-dn * 8.0) * 1.5;
        }
    }

    // glitch: locally shift bg pattern based on noise & time
    float gl = (hash21(floor(uv * vec2(80.0, 4.0)) + floor(TIME * 12.0)) - 0.5) * glitch;

    // --- color assembly ---
    vec3 col = bgColor.rgb;

    // subtle blueprint texture: faint cross-hatch
    float hatch = sin((uv.x + uv.y) * res.y * 0.25) * 0.5 + 0.5;
    col += hatch * 0.012;

    col += lineColor.rgb * trace;
    col += lineColor.rgb * halo * 0.6;

    col += pulseColor.rgb * pulseGlow * (1.2 + audio);
    col += glowColor.rgb * nodeGlow * (0.9 + audio * 0.6);

    col += pulseColor.rgb * sweep * 0.7;
    col += pulseColor.rgb * burst;

    // glitch tint
    col += vec3(gl * 0.4, gl * 0.2, -gl * 0.3);

    // vignette to push focus inward
    float v = length(uv - 0.5);
    col *= smoothstep(0.95, 0.2, v);

    // a touch of grain
    float grain = (hash21(uv * res + TIME) - 0.5) * 0.025;
    col += grain;

    gl_FragColor = vec4(col, 1.0);
}