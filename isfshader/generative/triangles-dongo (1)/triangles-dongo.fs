/*{
  "DESCRIPTION": "Glowing triangle with adjustable pulse speed, continuous rotation, number of clones, kaleidoscope tie-dye background, line thickness, background rotation speed, stop and reverse rotation, and in/out animation for each triangle with audio-reactive background color inversion",
  "CATEGORIES": [ "graphics", "geometry" ],
  "INPUTS": [
    {
      "NAME": "pulseSpeed",
      "TYPE": "float",
      "DEFAULT": 3.0,
      "MIN": 0.1,
      "MAX": 10.0,
      "LABEL": "Pulse Speed (Seconds)"
    },
    {
      "NAME": "rotationSpeed",
      "TYPE": "float",
      "DEFAULT": 30.0,
      "MIN": 1.0,
      "MAX": 180.0,
      "LABEL": "Rotation Speed (Degrees/Second)"
    },
    {
      "NAME": "rotationDirection",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": -1.0,
      "MAX": 1.0,
      "LABEL": "Rotation Direction (1=Clockwise, -1=Counterclockwise)"
    },
    {
      "NAME": "stopRotation",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Stop Rotation (0=Resume, 1=Stop)"
    },
    {
      "NAME": "numClones",
      "TYPE": "float",
      "DEFAULT": 20.0,
      "MIN": 1.0,
      "MAX": 20.0,
      "LABEL": "Number of Clones"
    },
    {
      "NAME": "lineThickness",
      "TYPE": "float",
      "DEFAULT": 0.02,
      "MIN": 0.01,
      "MAX": 0.1,
      "LABEL": "Line Thickness"
    },
    {
      "NAME": "bgRotationSpeed",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Background Rotation Speed"
    },
    {
      "NAME": "numKaleidoscopeMirrors",
      "TYPE": "float",
      "DEFAULT": 6.0,
      "MIN": 1.0,
      "MAX": 12.0,
      "LABEL": "Kaleidoscope Mirrors"
    },
    {
      "NAME": "audio",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Audio Input"
    }
  ]
}*/

float triangleOutline(vec2 p, float thickness) {
    const float sqrt3 = 1.73205;

    // Scale down for visibility
    float size = 0.4;
    p /= size;

    // Signed distance function for equilateral triangle
    p.x = abs(p.x) - 1.0;
    p.y += 1.0 / sqrt3;

    if (p.x + sqrt3 * p.y > 0.0) {
        p = vec2(p.x - sqrt3 * p.y, -sqrt3 * p.x - p.y) / 2.0;
    }

    p.x -= clamp(p.x, -2.0, 0.0);

    // Adjusting the edge sharpness: controlling the distance and applying thickness directly
    float dist = length(p) * sign(p.y);
    return dist;
}

vec3 kaleidoscopeBackground(vec2 uv, float rotationSpeed, float numMirrors, float audio) {
    // Rotate the UV coordinates to create kaleidoscope effect
    vec2 center = vec2(0.5); // Center of the image
    uv -= center;

    // Determine angle per mirror segment
    float angleStep = 3.14159 * 2.0 / numMirrors;

    // Create symmetrical reflection-based kaleidoscope
    vec2 uvRotated = uv;
    vec3 kaleidoscopeColor = vec3(0.0);

    for (int i = 0; i < 12; i++) {  // We can use a constant loop count (like 12) for kaleidoscope reflections
        float angle = float(i) * angleStep;
        mat2 rotation = mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
        uvRotated = rotation * uv;

        // Apply a tie-dye color pattern with time-based animation
        float r = 0.5 + 0.5 * sin(uvRotated.x * 5.0 + TIME * 0.3);
        float g = 0.5 + 0.5 * sin(uvRotated.y * 5.0 + TIME * 0.5);
        float b = 0.5 + 0.5 * sin(uvRotated.x * 5.0 + TIME * 0.7);

        // Blend the colors to create kaleidoscopic symmetry
        kaleidoscopeColor += vec3(r, g, b);
    }

    // Normalize the kaleidoscope effect to avoid excess brightness
    kaleidoscopeColor /= 12.0;

    // Apply audio-reactive inversion effect
    float invertFactor = audio; // Use audio input to determine the inversion effect (0 = no inversion, 1 = full inversion)
    kaleidoscopeColor = mix(kaleidoscopeColor, vec3(1.0) - kaleidoscopeColor, invertFactor);

    return kaleidoscopeColor;
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 p = (uv - 0.5) * 2.0;

    // Adjust for aspect ratio first (before rotation)
    p.x *= RENDERSIZE.x / RENDERSIZE.y;

    // If the stopRotation toggle is 1, stop the rotation by setting speed to 0
    float actualRotationSpeed = stopRotation == 1.0 ? 0.0 : rotationSpeed;

    // Calculate continuous rotation using TIME and rotationDirection
    float angle = radians(TIME * actualRotationSpeed * rotationDirection);  // Multiply time by rotation speed
    float s = sin(angle);
    float c = cos(angle);

    // Apply rotation to the adjusted coordinates
    p = mat2(c, -s, s, c) * p;

    // Create kaleidoscope background effect with audio-reactive inversion
    vec3 bgColor = kaleidoscopeBackground(uv, bgRotationSpeed, numKaleidoscopeMirrors, audio);
    
    // Initialize the final color
    vec3 finalColor = vec3(0.0);  // Start with a black background

    // Create triangles with line thickness
    float thickness = lineThickness;

    // Loop through the clones and apply independent movement to each
    for (int i = 0; i < 20; i++) {  // Max 20 clones
        if (i >= int(floor(numClones))) break;  // Stop drawing more clones than selected

        // Calculate angle offset for circular positioning
        float angleOffset = float(i) * 2.0 * 3.14159 / float(numClones);  // Angular offset for clones
        vec2 offset = vec2(cos(angleOffset), sin(angleOffset)) * 0.5;  // Circular offset
        vec2 pWithOffset = p + offset;

        // Add independent pulse for each clone (in and out animation)
        float pulse = 0.5 + 0.5 * sin(TIME * pulseSpeed + float(i) * 0.2);  // Add different phase for each clone

        // Scale each triangle based on its own pulse value
        pWithOffset *= pulse;  // Apply individual scaling to the triangle's position

        // Calculate the distance to the outline of the triangle
        float dist = triangleOutline(pWithOffset, thickness);

        // Apply the glowing effect and color for each triangle
        finalColor += vec3(1.0, 0.8, 0.3) * smoothstep(thickness, 0.0, abs(dist));  // Glowing edges
    }

    // Blend background and triangle colors
    finalColor = mix(bgColor, finalColor, 0.5);  // Combine background and triangles

    // Set the final output color
    gl_FragColor = vec4(finalColor, 1.0);
}
