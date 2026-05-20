/*{
	"DESCRIPTION": "Pixelation and retro CRT-style treatment for inputImage with tweakable controls.",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Stylize",
		"Pixelate"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "pixelSize",
			"TYPE": "float",
			"LABEL": "Pixel Size",
			"DEFAULT": 12.0,
			"MIN": 1.0,
			"MAX": 64.0
		},
		{
			"NAME": "paletteLevels",
			"TYPE": "float",
			"LABEL": "Palette Levels",
			"DEFAULT": 16.0,
			"MIN": 2.0,
			"MAX": 64.0
		},
		{
			"NAME": "scanlineIntensity",
			"TYPE": "float",
			"LABEL": "Scanline Intensity",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "scanlineDensity",
			"TYPE": "float",
			"LABEL": "Scanline Density",
			"DEFAULT": 2.0,
			"MIN": 0.25,
			"MAX": 4.0
		},
		{
			"NAME": "noiseAmount",
			"TYPE": "float",
			"LABEL": "Noise",
			"DEFAULT": 0.75,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "chromaShift",
			"TYPE": "float",
			"LABEL": "Chroma Shift",
			"DEFAULT": 4,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "vignetteAmount",
			"TYPE": "float",
			"LABEL": "Vignette",
			"DEFAULT": 1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "saturation",
			"TYPE": "float",
			"LABEL": "Saturation",
			"DEFAULT": 1.2,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "contrast",
			"TYPE": "float",
			"LABEL": "Contrast",
			"DEFAULT": 1.5,
			"MIN": 0.5,
			"MAX": 2.0
		},
		{
			"NAME": "retroMix",
			"TYPE": "float",
			"LABEL": "Retro Mix",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

float hash12(vec2 p) {
	return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

vec3 applySaturation(vec3 color, float sat) {
	float luma = dot(color, vec3(0.299, 0.587, 0.114));
	return mix(vec3(luma), color, sat);
}

vec3 applyContrast(vec3 color, float ctr) {
	return ((color - 0.5) * ctr) + 0.5;
}

vec3 posterize(vec3 color, float levels) {
	float l = max(2.0, levels);
	return floor(color * (l - 1.0) + 0.5) / (l - 1.0);
}

vec2 safeUV(vec2 uv) {
	return max(vec2(0.0, 0.0), min(vec2(1.0, 1.0), uv));
}

void main() {
	vec2 uv = isf_FragNormCoord.xy;
	vec2 safeSize = max(RENDERSIZE.xy, vec2(1.0));

	float px = max(1.0, pixelSize);
	vec2 pixelStep = vec2(px) / safeSize;
	vec2 blockUV = (floor(uv / pixelStep) + 0.5) * pixelStep;

	float shift = chromaShift / safeSize.x;
	vec2 shiftVec = vec2(shift, 0.0);

	vec3 src = IMG_NORM_PIXEL(inputImage, uv).rgb;
	vec3 pix;
	vec2 uvR = safeUV(blockUV + shiftVec);
	vec2 uvG = safeUV(blockUV);
	vec2 uvB = safeUV(blockUV - shiftVec);
	pix.r = IMG_NORM_PIXEL(inputImage, uvR).r;
	pix.g = IMG_NORM_PIXEL(inputImage, uvG).g;
	pix.b = IMG_NORM_PIXEL(inputImage, uvB).b;

	pix = applySaturation(pix, saturation);
	pix = applyContrast(pix, contrast);
	pix = posterize(clamp(pix, 0.0, 1.0), paletteLevels);

	float lines = sin(uv.y * safeSize.y * scanlineDensity * 3.14159265);
	float lineMask = mix(1.0, 0.65 + (0.35 * lines), clamp(scanlineIntensity, 0.0, 1.0));
	pix *= lineMask;

	float grain = (hash12((uv * safeSize * 0.5) + TIME * vec2(12.71, 7.13)) - 0.5) * 2.0;
	pix += grain * 0.18 * clamp(noiseAmount, 0.0, 1.0);

	vec2 centered = uv - 0.5;
	float vig = 1.0 - dot(centered, centered) * 2.2;
	vig = mix(1.0, clamp(vig, 0.0, 1.0), clamp(vignetteAmount, 0.0, 1.0));
	pix *= vig;

	vec3 finalColor = mix(src, clamp(pix, 0.0, 1.0), clamp(retroMix, 0.0, 1.0));
	gl_FragColor = vec4(finalColor, 1.0);
}
