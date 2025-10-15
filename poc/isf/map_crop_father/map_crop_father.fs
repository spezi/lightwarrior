/*{
  "ISFVSN": 2,
  "CATEGORIES": ["Geometry Adjustment"],
  "CREDIT": "ProjectileObjects - Crop & Projection Mapping",
  "DESCRIPTION": "Switch between diagonal crop mode and projection mapping mode with feathering",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image",
      "DEFAULT": null
    },
    {
      "NAME": "mode",
      "LABEL": "Mode",
      "TYPE": "long",
      "VALUES": [0, 1],
      "LABELS": ["Crop", "Projection"],
      "DEFAULT": 0
    },
    {
      "NAME": "topleft",
      "TYPE": "point2D",
      "DEFAULT": [0.0, 1.0],
      "MIN": [0.0, 0.0],
      "MAX": [1.0, 1.0]
    },
    {
      "NAME": "topright",
      "TYPE": "point2D",
      "DEFAULT": [1.0, 1.0],
      "MIN": [0.0, 0.0],
      "MAX": [1.0, 1.0]
    },
    {
      "NAME": "bottomleft",
      "TYPE": "point2D",
      "DEFAULT": [0.0, 0.0],
      "MIN": [0.0, 0.0],
      "MAX": [1.0, 1.0]
    },
    {
      "NAME": "bottomright",
      "TYPE": "point2D",
      "DEFAULT": [1.0, 0.0],
      "MIN": [0.0, 0.0],
      "MAX": [1.0, 1.0]
    },
    {
      "NAME": "TopFeather",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 0.5
    },
    {
      "NAME": "BottomFeather",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 0.5
    },
    {
      "NAME": "LeftFeather",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 0.5
    },
    {
      "NAME": "RightFeather",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 0.5
    }
  ]
}
*/


// FRAGMENT SHADER
void main()
{
    vec2 uv = isf_FragNormCoord;
    vec4 color = IMG_NORM_PIXEL(inputImage, uv);
    
    // CROP MODE - use corner points as diagonal crop boundaries
    if (mode == 0) {
        // Interpolate crop positions based on corner points
        // Top edge: from topleft.y to topright.y
        float topCrop = mix(topleft.y, topright.y, uv.x);
        // Bottom edge: from bottomleft.y to bottomright.y
        float bottomCrop = mix(bottomleft.y, bottomright.y, uv.x);
        // Left edge: from bottomleft.x to topleft.x
        float leftCrop = mix(bottomleft.x, topleft.x, uv.y);
        // Right edge: from bottomright.x to topright.x
        float rightCrop = mix(bottomright.x, topright.x, uv.y);
        
        // Calculate distances
        float topDistance = topCrop - uv.y;
        float bottomDistance = uv.y - bottomCrop;
        float leftDistance = uv.x - leftCrop;
        float rightDistance = rightCrop - uv.x;
        
        // Apply feathering
        float topFeatherFactor = smoothstep(0.0, TopFeather, topDistance);
        float bottomFeatherFactor = smoothstep(0.0, BottomFeather, bottomDistance);
        float leftFeatherFactor = smoothstep(0.0, LeftFeather, leftDistance);
        float rightFeatherFactor = smoothstep(0.0, RightFeather, rightDistance);
        
        float alpha = topFeatherFactor * bottomFeatherFactor * leftFeatherFactor * rightFeatherFactor;
        color.a *= alpha;
    }
    // PROJECTION MODE - apply feathering to edges
    else {
        // Calculate distance from edges (0-1 range)
        float distFromTop = uv.y;
        float distFromBottom = 1.0 - uv.y;
        float distFromLeft = uv.x;
        float distFromRight = 1.0 - uv.x;
        
        // Normalize feather distances
        float topFade = smoothstep(0.0, TopFeather, distFromTop);
        float bottomFade = smoothstep(0.0, BottomFeather, distFromBottom);
        float leftFade = smoothstep(0.0, LeftFeather, distFromLeft);
        float rightFade = smoothstep(0.0, RightFeather, distFromRight);
        
        float alpha = topFade * bottomFade * leftFade * rightFade;
        color.a *= alpha;
    }
    
    gl_FragColor = color;
}