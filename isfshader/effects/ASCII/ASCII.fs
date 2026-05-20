/*{
    "DESCRIPTION": "ASCII with a blendable color background",
    "CREDIT": "Gemini",
    "ISFVSN": "2.0",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "fontSize",
            "TYPE": "float",
            "MIN": 2.0,
            "MAX": 15.0,
            "DEFAULT": 6.0
        },
        {
            "NAME": "colorMix",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0
        },
        {
            "NAME": "threshold",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5
        }
    ]
}*/

float bin(float n, vec2 p) {
    vec2 floorP = floor(p);
    if (floorP.x < 0.0 || floorP.x > 2.0 || floorP.y < 0.0 || floorP.y > 4.0) return 0.0;
    return mod(floor(n / pow(2.0, floorP.y * 3.0 + floorP.x)), 2.0);
}

void main() {
    vec2 uv = isf_FragNormCoord;
    vec2 gridPos = floor(gl_FragCoord.xy / fontSize);
    vec2 charUV = mod(gl_FragCoord.xy, fontSize) / fontSize;
    
    // 1. Sample the original color
    vec2 sampleUV = (gridPos * fontSize + (fontSize * 0.5)) / RENDERSIZE;
    vec4 originalColor = IMG_NORM_PIXEL(inputImage, sampleUV);
    
    // 2. Calculate Grayscale/High Contrast
    float brightness = (originalColor.r + originalColor.g + originalColor.b) / 3.0;
    vec3 bwColor = vec3(step(threshold, brightness));
    
    // 3. Blend between B&W and Original Color
    // colorMix at 0.0 = Pure Black and White | 1.0 = Full Original Color
    vec3 finalBgColor = mix(bwColor, originalColor.rgb, colorMix);
    
    // 4. Character Selection
    float charMask = 0.0;
    if (brightness > 0.1) charMask = 2.0;          // .
    if (brightness > 0.3) charMask = 416.0;        // :
    if (brightness > 0.5) charMask = 14881.0;      // i
    if (brightness > 0.7) charMask = 31725.0;      // o
    if (brightness > 0.8) charMask = 31599.0;      // 8
    if (brightness > 0.9) charMask = 32639.0;      // @

    float pixelMask = bin(charMask, charUV * vec2(3.0, 5.0));
    
    // 5. Apply the character mask to the color
    // This ensures the "text" part takes the color, and the rest is black
    gl_FragColor = vec4(finalBgColor * pixelMask, 1.0);
}