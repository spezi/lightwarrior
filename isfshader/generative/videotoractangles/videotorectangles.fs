//	passthru.fs
/*{
	"DESCRIPTION": "Divides video into 40 planes with individual texture mapping",
	"CREDIT": "by VIDVOX, modified",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "configTexture",
			"TYPE": "image",
			"DEFAULT": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFAAAAAUCAYAAAAa2LrXAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjEuNv1OCegAAABgSURBVFhH7dYxDQAhEMXQMwYrWMEKVrCCFaxgBStYwQpWsIIVrGAFK1jBClawghWsYAUrWMEKVrCCFaxgBStYwQpWsIIVrGAFK1jBClawghWsYAUrWMEKVrCCFXzc7AHxewX6EhdnUAAAAABJRU5ErkJggg==",
			"LABEL": "Config Texture (RGBA: scale.xy, offset.xy)"
		},
		{
			"NAME": "numPlanes",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 40.0,
			"DEFAULT": 40.0
		},
		{
			"NAME": "gridColumns",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 40.0,
			"DEFAULT": 8.0
		}
	]
}*/

//	Click on the VS button to view the vertex shader

varying vec2 translated_coord;

// Get configuration for a specific plane from the config texture
vec4 getPlaneConfig(int planeIndex) {
    // Convert planeIndex to x coordinate in config texture (0-1 range)
    float x = (float(planeIndex) - floor(float(planeIndex) / 40.0) * 40.0) / 40.0;
    return IMG_PIXEL(configTexture, vec2(x, 0.5));
}

void main() {
    float cols = gridColumns;
    float rows = ceil(numPlanes / cols);
    
    // Calculate block size
    vec2 blockSize = vec2(1.0 / cols, 1.0 / rows);
    
    // Calculate which block we're in
    vec2 blockPos = floor(translated_coord / blockSize);
    int blockIndex = int(blockPos.y * cols + blockPos.x);
    
    // Only process if we're in a valid block
    if (blockIndex < int(numPlanes)) {
        // Calculate local coordinates within the block (0 to 1)
        vec2 localCoord = fract(translated_coord / blockSize);
        
        // Get configuration for this plane
        vec4 config = getPlaneConfig(blockIndex);
        vec2 scale = config.xy * 2.0; // Scale range 0-2
        vec2 offset = (config.zw * 2.0) - 1.0; // Offset range -1 to 1
        
        // Apply plane-specific scaling and offset
        vec2 adjustedCoord = localCoord * scale + offset;
        
        // Calculate final texture coordinates
        vec2 finalCoord = (blockPos + adjustedCoord) * blockSize;
        
        // Sample the texture with the modified coordinates
        gl_FragColor = IMG_NORM_PIXEL(inputImage, finalCoord);
    } else {
        // For blocks beyond numPlanes, show black
        gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
    }
}