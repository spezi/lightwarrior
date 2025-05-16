/*{
    "CATEGORIES": [
        "Geometry Adjustment",
        "Compositing"
    ],
    "CREDIT": "VIDVOX, modified by Claude",
    "DESCRIPTION": "Maps four video inputs with adjustable blend controls for projection mapping",
    "INPUTS": [
        {
            "NAME": "inputImage1",
            "TYPE": "image",
            "LABEL": "Input 1"
        },
        {
            "NAME": "inputImage2",
            "TYPE": "image",
            "LABEL": "Input 2"
        },
        {
            "NAME": "inputImage3",
            "TYPE": "image",
            "LABEL": "Input 3"
        },
        {
            "NAME": "inputImage4",
            "TYPE": "image",
            "LABEL": "Input 4"
        },
        {
            "NAME": "blend1",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Input 1 Blend"
        },
        {
            "NAME": "blend2",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Input 2 Blend"
        },
        {
            "NAME": "blend3",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Input 3 Blend"
        },
        {
            "NAME": "blend4",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Input 4 Blend"
        },
        {
            "DEFAULT": [
                0,
                1
            ],
            "LABEL": "Top Left",
            "NAME": "topleft",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": [
                0,
                0
            ],
            "LABEL": "Bottom Left",
            "NAME": "bottomleft",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": [
                1,
                1
            ],
            "LABEL": "Top Right",
            "NAME": "topright",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": [
                1,
                0
            ],
            "LABEL": "Bottom Right",
            "NAME": "bottomright",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": [
                0,
                0
            ],
            "LABEL": "Translation",
            "NAME": "translation",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": [
                1,
                1
            ],
            "LABEL": "Scale",
            "NAME": "scale",
            "TYPE": "point2D"
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "TARGET": "bufferVariableNameA",
            "PERSISTENT": true,
            "FLOAT": true
        }
    ]
}
*/

void main()
{
    vec2 loc = vv_FragNormCoord;
    vec2 pt = vec2(0.0);
    float quadBlend = 0.0;
    
    // Calculate the position based on the four corner points
    if (loc.x <= 0.5 && loc.y <= 0.5) {
        // Bottom left quadrant - Input 1
        pt = mix(bottomleft, vec2(0.5), length(loc - vec2(0.0, 0.0)) * 2.0);
        quadBlend = blend1;
    } else if (loc.x <= 0.5 && loc.y > 0.5) {
        // Top left quadrant - Input 2
        pt = mix(topleft, vec2(0.5), length(loc - vec2(0.0, 1.0)) * 2.0);
        quadBlend = blend2;
    } else if (loc.x > 0.5 && loc.y <= 0.5) {
        // Bottom right quadrant - Input 3
        pt = mix(bottomright, vec2(0.5), length(loc - vec2(1.0, 0.0)) * 2.0);
        quadBlend = blend3;
    } else {
        // Top right quadrant - Input 4
        pt = mix(topright, vec2(0.5), length(loc - vec2(1.0, 1.0)) * 2.0);
        quadBlend = blend4;
    }
    
    // Apply scale and translation
    pt = pt * scale + translation;
    
    // Sample all input images at the calculated position
    vec4 color1 = IMG_NORM_PIXEL(inputImage1, pt);
    vec4 color2 = IMG_NORM_PIXEL(inputImage2, pt);
    vec4 color3 = IMG_NORM_PIXEL(inputImage3, pt);
    vec4 color4 = IMG_NORM_PIXEL(inputImage4, pt);
    
    // Calculate the final color based on quadrant and blend values
    vec4 finalColor;
    if (loc.x <= 0.5 && loc.y <= 0.5) {
        finalColor = color1 * quadBlend;
    } else if (loc.x <= 0.5 && loc.y > 0.5) {
        finalColor = color2 * quadBlend;
    } else if (loc.x > 0.5 && loc.y <= 0.5) {
        finalColor = color3 * quadBlend;
    } else {
        finalColor = color4 * quadBlend;
    }
    
    gl_FragColor = finalColor;
}
