/*{
    "DESCRIPTION": "Projection Mapping Shader",
    "CREDIT": "Custom",
    "ISFVSN": "2",
    "CATEGORIES": ["Geometry"],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "srcX",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "LABEL": "Source X"
        },
        {
            "NAME": "srcY",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "LABEL": "Source Y"
        },
        {
            "NAME": "srcWidth",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Source Width"
        },
        {
            "NAME": "srcHeight",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
            "LABEL": "Source Height"
        },
        {
            "NAME": "dstX",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "LABEL": "Destination X"
        },
        {
            "NAME": "dstY",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "LABEL": "Destination Y"
        },
        {
            "NAME": "dstWidth",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Destination Width"
        },
        {
            "NAME": "dstHeight",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Destination Height"
        },
        {
            "NAME": "keystoneH",
            "TYPE": "float",
            "MIN": -1.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "LABEL": "Keystone Horizontal"
        },
        {
            "NAME": "keystoneV",
            "TYPE": "float",
            "MIN": -1.0,
            "MAX": 1.0,
            "DEFAULT": 0.0,
            "LABEL": "Keystone Vertical"
        },
        {
            "NAME": "rotation",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 360.0,
            "DEFAULT": 0.0,
            "LABEL": "Rotation"
        },
        {
            "NAME": "alpha",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 1.0,
            "LABEL": "Opacity"
        }
    ]
}*/

void main() {
    vec2 uv = isf_FragNormCoord;
    vec4 color = vec4(0.0);
    
    // Check if pixel is within destination rectangle
    vec2 dstMin = vec2(dstX, dstY);
    vec2 dstMax = vec2(dstX + dstWidth, dstY + dstHeight);
    
    if (uv.x >= dstMin.x && uv.x <= dstMax.x && 
        uv.y >= dstMin.y && uv.y <= dstMax.y) {
        
        // Normalize to destination space (0-1)
        vec2 localUV = (uv - dstMin) / vec2(dstWidth, dstHeight);
        
        // Apply rotation
        if (rotation != 0.0) {
            float angle = radians(rotation);
            vec2 center = vec2(0.5);
            localUV -= center;
            float s = sin(angle);
            float c = cos(angle);
            localUV = vec2(
                localUV.x * c - localUV.y * s,
                localUV.x * s + localUV.y * c
            );
            localUV += center;
        }
        
        // Apply keystone correction
        if (keystoneH != 0.0 || keystoneV != 0.0) {
            vec2 centered = localUV - 0.5;
            
            // Horizontal keystone
            float xFactor = 1.0 + keystoneH * centered.y;
            centered.x *= xFactor;
            
            // Vertical keystone
            float yFactor = 1.0 + keystoneV * centered.x;
            centered.y *= yFactor;
            
            localUV = centered + 0.5;
        }
        
        // Map to source region
        vec2 srcUV = vec2(
            srcX + localUV.x * srcWidth,
            srcY + localUV.y * srcHeight
        );
        
        // Sample if within bounds
        if (srcUV.x >= 0.0 && srcUV.x <= 1.0 && 
            srcUV.y >= 0.0 && srcUV.y <= 1.0) {
            color = IMG_NORM_PIXEL(inputImage, srcUV);
            color.a *= alpha;
        }
    }
    
    gl_FragColor = color;
}