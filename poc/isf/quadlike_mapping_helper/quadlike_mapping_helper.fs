/*{
    "CATEGORIES": [
        "Generator"
    ],
    "DESCRIPTION": "Draws a quadrilateral (trapezoid, parallelogram, or any four-sided shape) with outline",
    "INPUTS": [
        {
            "NAME": "corner1",
            "TYPE": "point2D",
            "DEFAULT": [0.2, 0.2],
            "MIN": [0.0, 0.0],
            "MAX": [1.0, 1.0]
        },
        {
            "NAME": "corner2",
            "TYPE": "point2D",
            "DEFAULT": [0.8, 0.2],
            "MIN": [0.0, 0.0],
            "MAX": [1.0, 1.0]
        },
        {
            "NAME": "corner3",
            "TYPE": "point2D",
            "DEFAULT": [0.8, 0.8],
            "MIN": [0.0, 0.0],
            "MAX": [1.0, 1.0]
        },
        {
            "NAME": "corner4",
            "TYPE": "point2D",
            "DEFAULT": [0.2, 0.8],
            "MIN": [0.0, 0.0],
            "MAX": [1.0, 1.0]
        },
        {
            "NAME": "fillColor",
            "TYPE": "color",
            "DEFAULT": [0.2, 0.5, 0.8, 1.0]
        },
        {
            "NAME": "outlineColor",
            "TYPE": "color",
            "DEFAULT": [1.0, 1.0, 1.0, 1.0]
        },
        {
            "NAME": "outlineWidth",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.0,
            "MAX": 0.1
        },
        {
            "NAME": "backgroundColor",
            "TYPE": "color",
            "DEFAULT": [0.0, 0.0, 0.0, 1.0]
        }
    ],
    "CREDIT": "Quadrilateral Shape Generator"
}*/

// Calculate the signed distance from point p to a line segment from a to b
float sdSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a;
    vec2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

// Calculate cross product to determine if point is on left side of edge
float cross2d(vec2 a, vec2 b) {
    return a.x * b.y - a.y * b.x;
}

// Check if point is inside quadrilateral using cross products
bool pointInQuad(vec2 p, vec2 c1, vec2 c2, vec2 c3, vec2 c4) {
    // Check if point is on the same side of all edges
    float d1 = cross2d(c2 - c1, p - c1);
    float d2 = cross2d(c3 - c2, p - c2);
    float d3 = cross2d(c4 - c3, p - c3);
    float d4 = cross2d(c1 - c4, p - c4);
    
    // All should have the same sign if point is inside
    return (d1 >= 0.0 && d2 >= 0.0 && d3 >= 0.0 && d4 >= 0.0) ||
           (d1 <= 0.0 && d2 <= 0.0 && d3 <= 0.0 && d4 <= 0.0);
}

// Get minimum distance to quad edges
float distToQuadEdges(vec2 p, vec2 c1, vec2 c2, vec2 c3, vec2 c4) {
    float d1 = sdSegment(p, c1, c2);
    float d2 = sdSegment(p, c2, c3);
    float d3 = sdSegment(p, c3, c4);
    float d4 = sdSegment(p, c4, c1);
    return min(min(d1, d2), min(d3, d4));
}

void main() {
    vec2 uv = isf_FragNormCoord;
    
    // Calculate aspect ratio to correct distance measurements
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    
    // Apply aspect ratio correction to coordinates for uniform distance
    vec2 correctedUV = vec2(uv.x * aspect, uv.y);
    vec2 c1 = vec2(corner1.x * aspect, corner1.y);
    vec2 c2 = vec2(corner2.x * aspect, corner2.y);
    vec2 c3 = vec2(corner3.x * aspect, corner3.y);
    vec2 c4 = vec2(corner4.x * aspect, corner4.y);
    
    // Check if point is inside quadrilateral (use original coordinates)
    bool inside = pointInQuad(uv, corner1, corner2, corner3, corner4);
    
    // Calculate distance to edges with aspect-corrected coordinates
    float dist = distToQuadEdges(correctedUV, c1, c2, c3, c4);
    
    // Scale outline width by aspect for consistency
    float correctedOutlineWidth = outlineWidth * aspect;
    
    // Determine color based on position
    vec4 finalColor = backgroundColor;
    
    if (inside) {
        // Inside the shape - use fill color
        finalColor = fillColor;
        
        // Add outline if within outline width
        if (dist < correctedOutlineWidth) {
            // Smooth transition for antialiasing
            float t = smoothstep(correctedOutlineWidth - 0.001 * aspect, correctedOutlineWidth, dist);
            finalColor = mix(outlineColor, fillColor, t);
        }
    } else {
        // Outside the shape - check if within outline width
        if (dist < correctedOutlineWidth) {
            float t = smoothstep(correctedOutlineWidth - 0.001 * aspect, correctedOutlineWidth, dist);
            finalColor = mix(outlineColor, backgroundColor, t);
        }
    }
    
    gl_FragColor = finalColor;
}