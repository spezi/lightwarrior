// VERTEX SHADER
void main()
{
    isf_vertShaderInit();
    
    vec4 position = gl_Position;
    
    // PROJECTION MODE - modify vertex positions
    if (mode == 1) {
        vec2 newPos;
        
        if ((position.x < 0.0) && (position.y < 0.0)) {
            // Bottom-left corner
            newPos = bottomleft;
        }
        else if ((position.x > 0.0) && (position.y < 0.0)) {
            // Bottom-right corner
            newPos = bottomright;
        }
        else if (position.x < 0.0) {
            // Top-left corner
            newPos = topleft;
        }
        else {
            // Top-right corner
            newPos = topright;
        }
        
        // Convert from 0-1 space to -1 to 1 clip space
        gl_Position.xy = newPos * 2.0 - 1.0;
    }
}