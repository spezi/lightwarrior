/*{
  "DESCRIPTION": "Multi-Layer Pulsating Dot Grid with Depth and Color Mixing",
  "CREDIT": "NAVA MICKAEL, Inspired by Lives on KEXPs",
  "ISFVSN": "2",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "numLayers",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": 1.0,
      "MAX": 5.0,
      "LABEL": "Number of Layers"
    },
    {
      "NAME": "gridSize1",
      "TYPE": "float",
      "DEFAULT": 8.0,
      "MIN": 2.0,
      "MAX": 30.0,
      "LABEL": "Layer 1 Grid Size"
    },
    {
      "NAME": "gridSize2",
      "TYPE": "float",
      "DEFAULT": 15.0,
      "MIN": 2.0,
      "MAX": 30.0,
      "LABEL": "Layer 2 Grid Size"
    },
    {
      "NAME": "layerDepth1",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Layer 1 Opacity"
    },
    {
      "NAME": "layerDepth2",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Layer 2 Opacity"
    },
    {
      "NAME": "fadeDistance1",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Layer 1 Edge Fade"
    },
    {
      "NAME": "fadeDistance2",
      "TYPE": "float",
      "DEFAULT": 0.4,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Layer 2 Edge Fade"
    },
    {
      "NAME": "coreColorSmall",
      "TYPE": "color",
      "DEFAULT": [0.2, 0.6, 1.0, 1.0],
      "LABEL": "Core Color (Small)"
    },
    {
      "NAME": "coreColorLarge",
      "TYPE": "color",
      "DEFAULT": [1.0, 0.9, 0.5, 1.0],
      "LABEL": "Core Color (Large)"
    },
    {
      "NAME": "bloomColorSmall",
      "TYPE": "color",
      "DEFAULT": [0.4, 0.8, 1.0, 1.0],
      "LABEL": "Bloom Color (Small)"
    },
    {
      "NAME": "bloomColorLarge",
      "TYPE": "color",
      "DEFAULT": [1.0, 0.9, 0.7, 1.0],
      "LABEL": "Bloom Color (Large)"
    },
    {
      "NAME": "bloomIntensity",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.0,
      "MAX": 5.0,
      "LABEL": "Bloom Intensity"
    },
    {
      "NAME": "bloomRadius",
      "TYPE": "float",
      "DEFAULT": 2.5,
      "MIN": 0.5,
      "MAX": 5.0,
      "LABEL": "Bloom Radius"
    },
    {
      "NAME": "bloomFalloff",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.5,
      "MAX": 3.0,
      "LABEL": "Bloom Falloff"
    },
    {
      "NAME": "bloomPulse",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Bloom Pulse Amount"
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
      "NAME": "minDotSize",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.0,
      "MAX": 0.45,
      "LABEL": "Min Dot Size"
    },
    {
      "NAME": "maxDotSize",
      "TYPE": "float",
      "DEFAULT": 0.35,
      "MIN": 0.1,
      "MAX": 0.45,
      "LABEL": "Max Dot Size"
    }
  ]
}*/

void main() {
    // Get normalized coordinates
    vec2 uv = isf_FragNormCoord.xy;
    
    // Fix aspect ratio to keep dots square
    float aspectRatio = RENDERSIZE.x / RENDERSIZE.y;
    uv.x *= aspectRatio;
    
    // Arrays for layer parameters (max 5 layers)
    float gridSizes[5];
    gridSizes[0] = gridSize1;
    gridSizes[1] = gridSize2;
    gridSizes[2] = 10.0;  // Default values for layers 3-5 (if used)
    gridSizes[3] = 12.0;
    gridSizes[4] = 14.0;
    
    float layerDepths[5];
    layerDepths[0] = layerDepth1;
    layerDepths[1] = layerDepth2;
    layerDepths[2] = 0.3;
    layerDepths[3] = 0.2;
    layerDepths[4] = 0.1;
    
    float fadeDistances[5];
    fadeDistances[0] = fadeDistance1;
    fadeDistances[1] = fadeDistance2;
    fadeDistances[2] = 0.6;
    fadeDistances[3] = 0.5;
    fadeDistances[4] = 0.4;
    
    // Accumulate results from all layers
    vec3 finalColor = vec3(0.0);
    
    int layers = int(floor(numLayers));
    for (int layer = 0; layer < 5; layer++) {
        if (layer >= layers) break;
        
        float gridSize = gridSizes[layer];
        float layerDepth = layerDepths[layer];
        float fadeDistance = fadeDistances[layer];
        
        // Calculate distance-based fade (further from center = more fade)
        vec2 centerVec = uv - 0.5 * vec2(aspectRatio, 1.0);
        float distFromCenter = length(centerVec);
        float fadeFactor = 1.0 - smoothstep(fadeDistance * 0.5, fadeDistance, distFromCenter);
        
        // Accumulate bloom for this layer
        float totalBloom = 0.0;
        float totalCore = 0.0;
        float dotSize = 0.0;
        
        // Sample a 3x3 neighborhood
        for (int i = -1; i <= 1; i++) { 
            for (int j = -1; j <= 1; j++) {
                vec2 offset = vec2(float(i), float(j));
                
                vec2 gridUV = uv * gridSize + offset;
                vec2 cell = floor(gridUV);
                vec2 cellUV = fract(gridUV) - 0.5;
                
                float dist = length(cellUV + offset);
                
                // Different time offset per layer
                float timeOffset = dot(cell, vec2(1.2, 3.7)) * 2.0 + float(layer) * 5.0;
                float pulse = 0.5 + 0.5 * sin(TIME * pulseSpeed + timeOffset);
                
                float currentDotSize = minDotSize + (maxDotSize - minDotSize) * pulse;
                
                if (i == 0 && j == 0) {
                    dotSize = currentDotSize;
                }
                
                if (i == 0 && j == 0) {
                    float core = 1.0 - smoothstep(currentDotSize - 0.03, currentDotSize + 0.03, dist);
                    totalCore = core;
                }
                
                float bloomPulseFactor = 1.0 - bloomPulse + bloomPulse * pulse;
                
                vec2 cellCenter = (cell + 0.5) / gridSize - uv;
                float bloomDist = length(cellCenter * gridSize);
                
                float dotSizeFactor = maxDotSize > 0.0 ? currentDotSize / maxDotSize : 0.0;
                float scaledBloomRadius = bloomRadius * (0.5 + 1.5 * dotSizeFactor);
                float intensityScale = dotSizeFactor * (0.8 + 0.4 * pulse);
                
                float bloom = exp(-pow(bloomDist * scaledBloomRadius, bloomFalloff)) 
                            * bloomIntensity 
                            * bloomPulseFactor 
                            * intensityScale;
                 
                totalBloom += bloom;
            }
        }
        
        // Apply layer depth and distance fade
        totalCore *= layerDepth * fadeFactor;
        totalBloom *= layerDepth * fadeFactor;
        
        // Color mixing based on dot size
        float sizeRange = maxDotSize - minDotSize;
        float sizeRatio = sizeRange > 0.0 ? (dotSize - minDotSize) / sizeRange : 0.5;
        sizeRatio = clamp(sizeRatio, 0.0, 1.0);
        
        vec3 mixedCoreColor = mix(coreColorSmall.rgb, coreColorLarge.rgb, sizeRatio);
        vec3 mixedBloomColor = mix(bloomColorSmall.rgb, bloomColorLarge.rgb, sizeRatio);
        
        // Add this layer to final color
        finalColor += mixedCoreColor * totalCore + mixedBloomColor * totalBloom * 0.8;
    }
    
    // Clamp final color
    finalColor = min(finalColor, 1.2);
    
    gl_FragColor = vec4(finalColor, 1.0);
}