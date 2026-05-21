/*{
  "DESCRIPTION": "Procedural strips matching the base sampler geometry exactly: 75 vertical 5px columns packed across the left 40% of the frame, centred vertically. No input image. Small blocks travel along each column. Per-band height inputs (OSC-mappable) set the vertical length of that column (centred on y=0.5), exactly as in the base shader. Global controls: block count, size, sharpness/blur, speed, motion mode (linear / sinus / random / ease / pingpong / bounce / pulse / drift / gravity / breathe). Per-block lifecycle: lifeSpan + cycleTime + lifeStagger control how long each block is visible vs paused. density controls how many blocks appear at all (1 = all, lower = sparser); densityReroll re-draws the selection each cycle. sizeRandom varies block size per block (0 = uniform).",
  "CREDIT": "",
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "blockCount",  "TYPE": "float", "DEFAULT": 3.0,  "MIN": 1.0,  "MAX": 32.0 },
    { "NAME": "blockSize",   "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.005,"MAX": 1.0 },
    { "NAME": "sharpness",   "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "speed",       "TYPE": "float", "DEFAULT": 0.3,  "MIN": -3.0, "MAX": 3.0 },
    { "NAME": "phaseSpread", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "lifeSpan",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "cycleTime",   "TYPE": "float", "DEFAULT": 2.0,  "MIN": 0.05, "MAX": 30.0 },
    { "NAME": "lifeStagger", "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "density",     "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "densityReroll","TYPE": "bool", "DEFAULT": false },
    { "NAME": "sizeRandom",  "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0 },
    { "NAME": "mode",        "TYPE": "long",  "DEFAULT": 0,
        "LABELS": ["linear", "sinus", "random", "ease", "pingpong", "bounce", "pulse", "drift", "gravity", "breathe"],
        "VALUES": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9] },
    { "NAME": "blockColor",  "TYPE": "color", "DEFAULT": [1.0, 1.0, 1.0, 1.0] },
    { "NAME": "bgColor",     "TYPE": "color", "DEFAULT": [0.0, 0.0, 0.0, 1.0] },

    { "NAME": "height0", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height1", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height2", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height3", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height4", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height5", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height6", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height7", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height8", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height9", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height10", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height11", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height12", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height13", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height14", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height15", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height16", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height17", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height18", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height19", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height20", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height21", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height22", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height23", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height24", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height25", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height26", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height27", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height28", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height29", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height30", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height31", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height32", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height33", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height34", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height35", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height36", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height37", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height38", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height39", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height40", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height41", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height42", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height43", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height44", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height45", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height46", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height47", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height48", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height49", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height50", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height51", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height52", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height53", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height54", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height55", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height56", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height57", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height58", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height59", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height60", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height61", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height62", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height63", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height64", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height65", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height66", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height67", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height68", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height69", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height70", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height71", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height72", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height73", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "height74", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

// ----- geometry constants (must match the base sampler shader) --------------
//   bandWidthPx   = 5.0 / RENDERSIZE.x   (5 pixel wide columns)
//   centerSpacing = 0.4 / 75.0
//   max_height    = 0.5
//   top    = 0.5 - h * max_height / 2.0
//   bottom = 0.5 + h * max_height / 2.0

float hash11(float n) {
    return fract(sin(n * 12.9898) * 43758.5453);
}

// position along the column (0..1) for one block, given base offset and time
float blockPos(float baseOffset, float tNorm, int m) {
    float x = baseOffset + tNorm;   // raw running coordinate
    float p;

    if (m == 1) {
        // sinus: smooth oscillation between the two ends
        p = 0.5 + 0.5 * sin(x * 6.2831853);

    } else if (m == 2) {
        // random: new pseudo-random target each lap, smoothly interpolated
        float lap   = floor(x);
        float frac  = fract(x);
        float fromP = hash11(lap + baseOffset * 17.0);
        float toP   = hash11(lap + 1.0 + baseOffset * 17.0);
        p = mix(fromP, toP, smoothstep(0.0, 1.0, frac));

    } else if (m == 3) {
        // ease: travel start->end with soft accel/decel, then wrap
        p = smoothstep(0.0, 1.0, fract(x));

    } else if (m == 4) {
        // pingpong: constant-speed triangle wave, sharp turn-arounds
        p = abs(fract(x * 0.5) * 2.0 - 1.0);

    } else if (m == 5) {
        // bounce: accelerates toward the far end, then folds back -> bouncing
        float f = fract(x);
        p = 1.0 - pow(1.0 - f, 2.0);   // accelerate toward end
        p = abs(p * 2.0 - 1.0);        // fold so it returns

    } else if (m == 6) {
        // pulse: hold, then jump to next slot (quantised staccato)
        float steps = 6.0;
        p = floor(fract(x) * steps) / max(steps - 1.0, 1.0);

    } else if (m == 7) {
        // drift: slow smooth random walk (sum of detuned sines per block)
        float s = baseOffset * 23.7;
        p = 0.5
          + 0.30 * sin(tNorm * 2.399 + s)
          + 0.18 * sin(tNorm * 1.131 + s * 2.7)
          + 0.10 * sin(tNorm * 0.577 + s * 4.3);
        p = clamp(p, 0.0, 1.0);

    } else if (m == 8) {
        // gravity: accelerate downward, snap back to top (sawtooth^2)
        float f = fract(x);
        p = f * f;

    } else if (m == 9) {
        // breathe: all blocks contract to centre and expand out together
        float env = 0.5 + 0.5 * sin(tNorm * 6.2831853);   // 0..1 global
        float home = baseOffset;                          // resting slot
        p = mix(0.5, fract(home), env);

    } else {
        // linear: wrap 0 -> 1 repeatedly
        p = fract(x);
    }

    return clamp(p, 0.0, 1.0);
}

void main() {
    vec2 uv = isf_FragNormCoord;

    vec4 color = bgColor;

    // --- exact base-shader geometry ---
    float centerSpacing = 0.4 / 75.0;
    float halfWidth     = (5.0 / RENDERSIZE.x) / 2.0;
    float max_height    = 0.5;

    int   nBlocks = int(blockCount);
    int   m       = int(mode);

    float soft  = mix(1.0, 0.02, clamp(sharpness, 0.0, 1.0));
    float tBase = TIME * speed;

    for (int i = 0; i < 75; ++i) {

        float cx    = (float(i) + 0.5) * centerSpacing;
        float left  = cx - halfWidth;
        float right = cx + halfWidth;

        // horizontal gate: identical to base shader's uv.x >= left && uv.x <= right
        if (uv.x < left || uv.x > right) continue;

        float h = 0.0;
        if (i == 0) { h = height0; }
        else if (i == 1) { h = height1; }
        else if (i == 2) { h = height2; }
        else if (i == 3) { h = height3; }
        else if (i == 4) { h = height4; }
        else if (i == 5) { h = height5; }
        else if (i == 6) { h = height6; }
        else if (i == 7) { h = height7; }
        else if (i == 8) { h = height8; }
        else if (i == 9) { h = height9; }
        else if (i == 10) { h = height10; }
        else if (i == 11) { h = height11; }
        else if (i == 12) { h = height12; }
        else if (i == 13) { h = height13; }
        else if (i == 14) { h = height14; }
        else if (i == 15) { h = height15; }
        else if (i == 16) { h = height16; }
        else if (i == 17) { h = height17; }
        else if (i == 18) { h = height18; }
        else if (i == 19) { h = height19; }
        else if (i == 20) { h = height20; }
        else if (i == 21) { h = height21; }
        else if (i == 22) { h = height22; }
        else if (i == 23) { h = height23; }
        else if (i == 24) { h = height24; }
        else if (i == 25) { h = height25; }
        else if (i == 26) { h = height26; }
        else if (i == 27) { h = height27; }
        else if (i == 28) { h = height28; }
        else if (i == 29) { h = height29; }
        else if (i == 30) { h = height30; }
        else if (i == 31) { h = height31; }
        else if (i == 32) { h = height32; }
        else if (i == 33) { h = height33; }
        else if (i == 34) { h = height34; }
        else if (i == 35) { h = height35; }
        else if (i == 36) { h = height36; }
        else if (i == 37) { h = height37; }
        else if (i == 38) { h = height38; }
        else if (i == 39) { h = height39; }
        else if (i == 40) { h = height40; }
        else if (i == 41) { h = height41; }
        else if (i == 42) { h = height42; }
        else if (i == 43) { h = height43; }
        else if (i == 44) { h = height44; }
        else if (i == 45) { h = height45; }
        else if (i == 46) { h = height46; }
        else if (i == 47) { h = height47; }
        else if (i == 48) { h = height48; }
        else if (i == 49) { h = height49; }
        else if (i == 50) { h = height50; }
        else if (i == 51) { h = height51; }
        else if (i == 52) { h = height52; }
        else if (i == 53) { h = height53; }
        else if (i == 54) { h = height54; }
        else if (i == 55) { h = height55; }
        else if (i == 56) { h = height56; }
        else if (i == 57) { h = height57; }
        else if (i == 58) { h = height58; }
        else if (i == 59) { h = height59; }
        else if (i == 60) { h = height60; }
        else if (i == 61) { h = height61; }
        else if (i == 62) { h = height62; }
        else if (i == 63) { h = height63; }
        else if (i == 64) { h = height64; }
        else if (i == 65) { h = height65; }
        else if (i == 66) { h = height66; }
        else if (i == 67) { h = height67; }
        else if (i == 68) { h = height68; }
        else if (i == 69) { h = height69; }
        else if (i == 70) { h = height70; }
        else if (i == 71) { h = height71; }
        else if (i == 72) { h = height72; }
        else if (i == 73) { h = height73; }
        else if (i == 74) { h = height74; }

        float top    = 0.5 - h * max_height / 2.0;
        float bottom = 0.5 + h * max_height / 2.0;

        // skip empty / zero-height bands
        if (bottom <= top) continue;
        if (uv.y < top || uv.y > bottom) continue;

        // t along the column, exactly as the base shader: (uv.y - top)/(bottom - top)
        float t = (uv.y - top) / (bottom - top);

        // per-band random phase so columns don't move in lockstep
        float bandPhase = hash11(float(i) * 3.137) * phaseSpread;

        float intensity = 0.0;
        for (int j = 0; j < 32; ++j) {
            if (j >= nBlocks) break;
            float baseOffset = float(j) / max(float(nBlocks), 1.0) + bandPhase;
            float p  = blockPos(baseOffset, tBase, m);
            float dT = abs(t - p);

            // --- lifecycle: visible for lifeSpan, then paused, per cycleTime ---
            // each block + band gets its own phase offset so they don't blink in unison
            float lifePhase = fract(baseOffset * (1.0 + lifeStagger * 6.139)
                                    + hash11(float(i) * 7.31 + float(j)) * lifeStagger);
            float cyc  = fract(TIME / max(cycleTime, 0.001) + lifePhase);
            // smooth on/off gate with soft edges (tied to sharpness)
            float edge = mix(0.25, 0.01, clamp(sharpness, 0.0, 1.0)) * max(lifeSpan, 0.001);
            float life = smoothstep(0.0, edge, cyc)
                       * (1.0 - smoothstep(lifeSpan - edge, lifeSpan, cyc));
            // when lifeSpan is at max, keep blocks permanently on
            life = mix(life, 1.0, step(0.999, lifeSpan));

            // cycle id used for re-rolling random selections each cycle
            float cycleId = densityReroll ? floor(TIME / max(cycleTime, 0.001) + lifePhase) : 0.0;

            // --- per-block random size ---
            // sizeRandom 0 -> all blocks use blockSize; higher -> size varies per block.
            float sizeTicket = hash11(float(i) * 5.17 + float(j) * 9.43 + cycleId * 2.3);
            float sizeMul    = mix(1.0, sizeTicket, clamp(sizeRandom, 0.0, 1.0));
            float halfLen    = max(blockSize * h * sizeMul, 0.0001) * 0.5;

            float along = 1.0 - smoothstep(halfLen * (1.0 - soft), halfLen, dT);

            // --- density: only some blocks appear ---
            // each block has a pseudo-random "ticket"; it shows if ticket < density.
            float ticket = hash11(float(i) * 13.91 + float(j) * 4.27 + cycleId * 1.7);
            float gate   = step(ticket, density);   // 1 if visible, 0 if skipped

            intensity = max(intensity, along * life * gate);
        }

        // horizontal soft edge across the 5px column (same sharpness)
        float dx     = abs(uv.x - cx);
        float across = 1.0 - smoothstep(halfWidth * (1.0 - soft), halfWidth, dx);

        float a2 = intensity * across;
        color = mix(color, blockColor, a2 * blockColor.a);
    }

    gl_FragColor = color;
}