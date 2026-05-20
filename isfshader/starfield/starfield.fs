/*
{
  "CATEGORIES": [
    "Generator",
    "Stars"
  ],
  "DESCRIPTION": "A dense star field with larger stars and high density",
  "INPUTS": [
    {
      "NAME": "starDensity",
      "TYPE": "float",
      "DEFAULT": 0.6,
      "MIN": 0.3,
      "MAX": 1.5,
      "LABEL": "Star Density"
    },
    {
      "NAME": "starSize",
      "TYPE": "float",
      "DEFAULT": 0.002,
      "MIN": 0.002,
      "MAX": 0.015,
      "LABEL": "Star Size"
    },
    {
      "NAME": "largeStarRatio",
      "TYPE": "float",
      "DEFAULT": 0.15,
      "MIN": 0.0,
      "MAX": 0.4,
      "LABEL": "Large Star Ratio"
    },
    {
      "NAME": "twinkleAmount",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Twinkle Amount"
    },
    {
      "NAME": "twinkleSpeed",
      "TYPE": "float",
      "DEFAULT": 0.62,
      "MIN": 0.01,
      "MAX": 1.0,
      "LABEL": "Twinkle Speed"
    },
    {
      "NAME": "starBrightness",
      "TYPE": "float",
      "DEFAULT": 0.9,
      "MIN": 0.2,
      "MAX": 1.0,
      "LABEL": "Star Brightness"
    },
    {
      "NAME": "backgroundColor",
      "TYPE": "color",
      "DEFAULT": [0.0, 0.0, 0.05, 1.0],
      "LABEL": "Background Color"
    }
  ]
}
*/

// Fast hash function
float hash(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

// Optimized star function
float drawStar(vec2 uv, vec2 pos, float size, float brightness) {
    float dist = distance(uv, pos);
    
    // Glow effect for larger stars
    float glow = 0.0;
    if (size > 0.008) {
        glow = smoothstep(size * 3.0, size * 0.5, dist) * 0.5 * brightness;
    }
    
    // Core of the star
    float core = smoothstep(size, 0.0, dist) * brightness;
    
    return core + glow;
}

// Star field function
vec3 starField(vec2 uv, float density, float sizeScale, float brightScale) {
    vec3 color = vec3(0.0);
    
    // Cell size determines density - smaller cells = more stars
    float cellSize = (1.0 / (300.0 * density));
    vec2 cellCoord = floor(uv / cellSize);
    vec2 cellUv = fract(uv / cellSize);
    
    // Check surrounding cells for improved star rendering near edges
    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 offset = vec2(float(x), float(y));
            vec2 cell = cellCoord + offset;
            
            // Determine if this cell has a star (threshold controls density)
            float h = hash(cell);
            float thresh = 1.0 - (density * 0.6);
            
            if (h > thresh) {
                // Star position within the cell
                vec2 starPos = vec2(
                    hash(cell + 0.1),
                    hash(cell + 0.2)
                );
                
                // Global position of the star
                vec2 pos = (cell + starPos) * cellSize;
                
                // Determine if this is a large star
                bool isLarge = hash(cell + 0.3) < largeStarRatio;
                
                // Star size with variation
                float sizeMod = starSize * sizeScale * (0.7 + 0.6 * hash(cell + 0.4));
                if (isLarge) {
                    sizeMod *= 2.0; // Large stars are twice as big
                }
                
                // Twinkle effect
                float twinkle = 1.0;
                if (twinkleAmount > 0.0) {
                    float t = TIME * twinkleSpeed;
                    float speed = 1.0 + hash(cell + 0.5) * 3.0;
                    twinkle = 1.0 - twinkleAmount + twinkleAmount * sin(t * speed);
                }
                
                // Star brightness with variation
                float brightness = starBrightness * brightScale * (0.7 + 0.3 * hash(cell + 0.6)) * twinkle;
                
                // Draw the star if it's in range of our fragment
                float maxDist = sizeMod * (isLarge ? 4.0 : 2.0); // Only process nearby stars
                if (distance(uv, pos) < maxDist) {
                    // Star color - slight variation
                    vec3 starColor = vec3(
                        1.0,
                        0.9 + 0.1 * hash(cell + 0.7),
                        0.8 + 0.2 * hash(cell + 0.8)
                    );
                    
                    // Add the star to our color
                    color += drawStar(uv, pos, sizeMod, brightness) * starColor;
                }
            }
        }
    }
    
    return color;
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // Initialize with background color
    vec4 color = backgroundColor;
    
    // Render multiple star layers for greater density and variety
    color.rgb += starField(uv, starDensity * 1.0, 1.0, 1.0);                  // Main layer
    color.rgb += starField(uv, starDensity * 0.8, 0.8, 0.7) * 0.7;            // Medium stars
    color.rgb += starField(uv * 1.5 + 0.5, starDensity * 0.6, 0.5, 0.5) * 0.5; // Small background stars
    
    // Prevent overbright stars
    color.rgb = min(color.rgb, vec3(1.0));
    
    gl_FragColor = color;
}
