/*{
    "DESCRIPTION": "Renders input image pixels as dots in 3D space. Brighter pixels appear closer (larger), darker pixels appear farther (smaller). Creates a depth illusion from luminance.",
    "CREDIT": "Claude / Anthropic",
    "ISFVSN": "2",
    "CATEGORIES": ["3D Effect", "Stylize"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "dotGridSize",
            "LABEL": "Grid Size",
            "TYPE": "float",
            "MIN": 4.0,
            "MAX": 64.0,
            "DEFAULT": 16.0
        },
        {
            "NAME": "maxDotSize",
            "LABEL": "Max Dot Size",
            "TYPE": "float",
            "MIN": 0.1,
            "MAX": 1.0,
            "DEFAULT": 0.85
        },
        {
            "NAME": "minDotSize",
            "LABEL": "Min Dot Size",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.5,
            "DEFAULT": 0.05
        },
        {
            "NAME": "depthScale",
            "LABEL": "Depth Scale",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 2.0,
            "DEFAULT": 0.6
        },
        {
            "NAME": "perspective",
            "LABEL": "Perspective Strength",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.4
        },
        {
            "NAME": "focalLength",
            "LABEL": "Focal Length",
            "TYPE": "float",
            "MIN": 0.5,
            "MAX": 5.0,
            "DEFAULT": 2.0
        },
        {
            "NAME": "bgColor",
            "LABEL": "Background Color",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.0, 0.0, 1.0]
        },
        {
            "NAME": "tiltX",
            "LABEL": "Tilt X",
            "TYPE": "float",
            "MIN": -1.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "tiltY",
            "LABEL": "Tilt Y",
            "TYPE": "float",
            "MIN": -1.0,
            "MAX": 1.0,
            "DEFAULT": -0.2
        },
        {
            "NAME": "colorize",
            "LABEL": "Use Source Color",
            "TYPE": "bool",
            "DEFAULT": true
        },
        {
            "NAME": "dotColor",
            "LABEL": "Dot Color (if not colorized)",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0]
        }
    ]
}*/

// Luminance helper
float luma(vec3 c) {
    return dot(c, vec3(0.2126, 0.7152, 0.0722));
}

void main() {
    // Normalized UV [0,1]
    vec2 uv = isf_FragNormCoord.xy;

    // Aspect ratio
    float aspect = RENDERSIZE.x / RENDERSIZE.y;

    // Cell grid: which cell are we in?
    vec2 cellUV = uv * RENDERSIZE / dotGridSize;
    vec2 cellIndex = floor(cellUV);          // integer cell
    vec2 cellLocal = fract(cellUV) - 0.5;   // local [-0.5, 0.5]

    // Sample the source image at the cell center
    vec2 sampleUV = (cellIndex + 0.5) * dotGridSize / RENDERSIZE;
    sampleUV = clamp(sampleUV, 0.0, 1.0);
    vec4 srcColor = IMG_NORM_PIXEL(inputImage, sampleUV);

    // Depth from luminance: bright=near (depth=0), dark=far (depth=1)
    float brightness = luma(srcColor.rgb);
    float depth = 1.0 - brightness;  // 0=near, 1=far

    // 3D position of this dot center (normalized device-like coords)
    // X in [-aspect, aspect], Y in [-1, 1], Z driven by depth
    float cellNormX = (sampleUV.x - 0.5) * 2.0 * aspect;
    float cellNormY = (sampleUV.y - 0.5) * 2.0;
    float cellNormZ = depth * depthScale;  // Z: 0=front, depthScale=back

    // Apply tilt (simple rotation offset in X and Y based on Z)
    float tiltOffsetX = cellNormZ * tiltX;
    float tiltOffsetY = cellNormZ * tiltY;
    cellNormX += tiltOffsetX;
    cellNormY += tiltOffsetY;

    // Perspective projection: project back to screen
    float perspFactor = focalLength / (focalLength + cellNormZ * perspective);
    vec2 projectedCenter;
    projectedCenter.x = (cellNormX * perspFactor) / (2.0 * aspect) + 0.5;
    projectedCenter.y = (cellNormY * perspFactor) / 2.0 + 0.5;

    // Dot size: brighter = larger (closer), darker = smaller (farther)
    // Also scale by perspective for convincing 3D foreshortening
    float sizeFraction = mix(maxDotSize, minDotSize, depth);
    sizeFraction *= perspFactor;

    // To determine if current pixel is inside this projected dot,
    // we need to find which projected cell we belong to — but since projection
    // moves cells around, we use a search over neighboring cells.
    // For each neighbor cell, project its center and check if we're inside its dot.

    vec4 resultColor = bgColor;
    float closestDepth = 9999.0;

    for (int dy = -2; dy <= 2; dy++) {
        for (int dx = -2; dx <= 2; dx++) {
            vec2 neighborCell = cellIndex + vec2(float(dx), float(dy));
            vec2 neighborUV = (neighborCell + 0.5) * dotGridSize / RENDERSIZE;
            neighborUV = clamp(neighborUV, 0.0, 1.0);

            vec4 nColor = IMG_NORM_PIXEL(inputImage, neighborUV);
            float nLuma = luma(nColor.rgb);
            float nDepth = 1.0 - nLuma;
            float nZ = nDepth * depthScale;

            // Tilt
            float nNormX = (neighborUV.x - 0.5) * 2.0 * aspect + nZ * tiltX;
            float nNormY = (neighborUV.y - 0.5) * 2.0 + nZ * tiltY;

            // Perspective
            float nPersp = focalLength / (focalLength + nZ * perspective);
            vec2 nProj;
            nProj.x = (nNormX * nPersp) / (2.0 * aspect) + 0.5;
            nProj.y = (nNormY * nPersp) / 2.0 + 0.5;

            float nSize = mix(maxDotSize, minDotSize, nDepth) * nPersp;

            // Distance from current pixel to this projected dot center (in pixel space)
            vec2 diff = uv - nProj;
            diff.x *= aspect;  // correct for aspect
            float dist = length(diff);

            // Dot radius in UV space
            float radius = nSize * 0.5 * (dotGridSize / RENDERSIZE.y);

            if (dist < radius) {
                // Depth sort: closer dots (smaller nZ) paint on top
                if (nZ < closestDepth) {
                    closestDepth = nZ;
                    // Smooth edge antialiasing
                    float edge = smoothstep(radius, radius * 0.85, dist);
                    vec4 dColor = colorize ? vec4(nColor.rgb, nColor.a) : dotColor;
                    // Depth-based shading: closer = brighter dot
                    float shade = mix(0.3, 1.0, 1.0 - nDepth);
                    resultColor = vec4(dColor.rgb * shade * edge + bgColor.rgb * (1.0 - edge), 1.0);
                }
            }
        }
    }

    gl_FragColor = resultColor;
}