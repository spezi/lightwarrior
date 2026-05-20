/*{
  "CATEGORIES": ["Video"],
  "INPUTS": [
    { "NAME": "animationMode", "TYPE": "long", "DEFAULT": 0, "VALUES": [0, 1, 2, 3, 4], "LABELS": ["None", "Wave", "Pulse", "Random", "EQ"] },
    { "NAME": "animationSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 20.0 },
    { "NAME": "animationIntensity", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 0.5 },
    
    { "NAME": "height0", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r0", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g0", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b0", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a0", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height1", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r1", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g1", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b1", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a1", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height2", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r2", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g2", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b2", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a2", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height3", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r3", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g3", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b3", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a3", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height4", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r4", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g4", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b4", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a4", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height5", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r5", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g5", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b5", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a5", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height6", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r6", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g6", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b6", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a6", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height7", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r7", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g7", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b7", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a7", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height8", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r8", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g8", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b8", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a8", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height9", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r9", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g9", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b9", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a9", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height10", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r10", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g10", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b10", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a10", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height11", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r11", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g11", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b11", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a11", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height12", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r12", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g12", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b12", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a12", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height13", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r13", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g13", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b13", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a13", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height14", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r14", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g14", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b14", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a14", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height15", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r15", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g15", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b15", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a15", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height16", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r16", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g16", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b16", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a16", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height17", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r17", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g17", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b17", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a17", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height18", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r18", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g18", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b18", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a18", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height19", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r19", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g19", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b19", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a19", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height20", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r20", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g20", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b20", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a20", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height21", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r21", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g21", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b21", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a21", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height22", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r22", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g22", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b22", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a22", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height23", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r23", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g23", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b23", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a23", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height24", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r24", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g24", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b24", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a24", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height25", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r25", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g25", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b25", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a25", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height26", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r26", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g26", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b26", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a26", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height27", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r27", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g27", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b27", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a27", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height28", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r28", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g28", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b28", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a28", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height29", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r29", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g29", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b29", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a29", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height30", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r30", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g30", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b30", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a30", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    
    { "NAME": "height31", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "r31", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "g31", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "b31", "TYPE": "float", "DEFAULT": 255.0, "MIN": 0.0, "MAX": 255.0 },
    { "NAME": "a31", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 }
  ]
}*/

// Pseudo-random number generator
float random(float n) {
  //return fract(sin(n) * 43758.5453123);
  return fract(sin(n * 12.9898 + 78.233) * 43758.5453123);
}

void main() {
  vec2 uv = isf_FragNormCoord;
  float bandWidth = 10.0 / float(RENDERSIZE.x);
  float spacing = 1.0 / 31.0;
  float halfBand = bandWidth / 2.0;
  
  vec4 result = vec4(0.0);
  
  for (int i = 0; i < 31; i++) {
    float cx = (float(i) + 0.5) * spacing;
    
    // Get the base height for this specific band
    float baseHeight;
    vec4 bandColor;
    
    if (i == 0) { 
      baseHeight = height0; 
      bandColor = vec4(r0/255.0, g0/255.0, b0/255.0, a0);
    }
    else if (i == 1) { 
      baseHeight = height1; 
      bandColor = vec4(r1/255.0, g1/255.0, b1/255.0, a1);
    }
    else if (i == 2) { 
      baseHeight = height2; 
      bandColor = vec4(r2/255.0, g2/255.0, b2/255.0, a2);
    }
    else if (i == 3) { 
      baseHeight = height3; 
      bandColor = vec4(r3/255.0, g3/255.0, b3/255.0, a3);
    }
    else if (i == 4) { 
      baseHeight = height4; 
      bandColor = vec4(r4/255.0, g4/255.0, b4/255.0, a4);
    }
    else if (i == 5) { 
      baseHeight = height5; 
      bandColor = vec4(r5/255.0, g5/255.0, b5/255.0, a5);
    }
    else if (i == 6) { 
      baseHeight = height6; 
      bandColor = vec4(r6/255.0, g6/255.0, b6/255.0, a6);
    }
    else if (i == 7) { 
      baseHeight = height7; 
      bandColor = vec4(r7/255.0, g7/255.0, b7/255.0, a7);
    }
    else if (i == 8) { 
      baseHeight = height8; 
      bandColor = vec4(r8/255.0, g8/255.0, b8/255.0, a8);
    }
    else if (i == 9) { 
      baseHeight = height9; 
      bandColor = vec4(r9/255.0, g9/255.0, b9/255.0, a9);
    }
    else if (i == 10) { 
      baseHeight = height10; 
      bandColor = vec4(r10/255.0, g10/255.0, b10/255.0, a10);
    }
    else if (i == 11) { 
      baseHeight = height11; 
      bandColor = vec4(r11/255.0, g11/255.0, b11/255.0, a11);
    }
    else if (i == 12) { 
      baseHeight = height12; 
      bandColor = vec4(r12/255.0, g12/255.0, b12/255.0, a12);
    }
    else if (i == 13) { 
      baseHeight = height13; 
      bandColor = vec4(r13/255.0, g13/255.0, b13/255.0, a13);
    }
    else if (i == 14) { 
      baseHeight = height14; 
      bandColor = vec4(r14/255.0, g14/255.0, b14/255.0, a14);
    }
    else if (i == 15) { 
      baseHeight = height15; 
      bandColor = vec4(r15/255.0, g15/255.0, b15/255.0, a15);
    }
    else if (i == 16) { 
      baseHeight = height16; 
      bandColor = vec4(r16/255.0, g16/255.0, b16/255.0, a16);
    }
    else if (i == 17) { 
      baseHeight = height17; 
      bandColor = vec4(r17/255.0, g17/255.0, b17/255.0, a17);
    }
    else if (i == 18) { 
      baseHeight = height18; 
      bandColor = vec4(r18/255.0, g18/255.0, b18/255.0, a18);
    }
    else if (i == 19) { 
      baseHeight = height19; 
      bandColor = vec4(r19/255.0, g19/255.0, b19/255.0, a19);
    }
    else if (i == 20) { 
      baseHeight = height20; 
      bandColor = vec4(r20/255.0, g20/255.0, b20/255.0, a20);
    }
    else if (i == 21) { 
      baseHeight = height21; 
      bandColor = vec4(r21/255.0, g21/255.0, b21/255.0, a21);
    }
    else if (i == 22) { 
      baseHeight = height22; 
      bandColor = vec4(r22/255.0, g22/255.0, b22/255.0, a22);
    }
    else if (i == 23) { 
      baseHeight = height23; 
      bandColor = vec4(r23/255.0, g23/255.0, b23/255.0, a23);
    }
    else if (i == 24) { 
      baseHeight = height24; 
      bandColor = vec4(r24/255.0, g24/255.0, b24/255.0, a24);
    }
    else if (i == 25) { 
      baseHeight = height25; 
      bandColor = vec4(r25/255.0, g25/255.0, b25/255.0, a25);
    }
    else if (i == 26) { 
      baseHeight = height26; 
      bandColor = vec4(r26/255.0, g26/255.0, b26/255.0, a26);
    }
    else if (i == 27) { 
      baseHeight = height27; 
      bandColor = vec4(r27/255.0, g27/255.0, b27/255.0, a27);
    }
    else if (i == 28) { 
      baseHeight = height28; 
      bandColor = vec4(r28/255.0, g28/255.0, b28/255.0, a28);
    }
    else if (i == 29) { 
      baseHeight = height29; 
      bandColor = vec4(r29/255.0, g29/255.0, b29/255.0, a29);
    }
    else if (i == 30) { 
      baseHeight = height30; 
      bandColor = vec4(r30/255.0, g30/255.0, b30/255.0, a30);
    }
    else{ 
      baseHeight = height31; 
      bandColor = vec4(r31/255.0, g31/255.0, b31/255.0, a31);
    }
    
    // Apply animation to the height based on selected animation mode
    float animatedHeight = baseHeight;
    
    if (animationMode == 1) {
      // Wave animation
      animatedHeight = baseHeight * (0.5 + animationIntensity * sin(TIME * animationSpeed + float(i) * 0.2));
    }
    else if (animationMode == 2) {
      // Pulse animation
      animatedHeight = baseHeight * (0.5 + animationIntensity * sin(TIME * animationSpeed));
    }
    else if (animationMode == 3) {
      // Random animation
      animatedHeight = baseHeight * (0.5 + animationIntensity * sin(TIME * animationSpeed * random(float(i))));
    }
    else if (animationMode == 4) {
      // EQ-style animation (bands increasing from edges to center)
      float center = 15.0;
      float distance = abs(float(i) - center);
      animatedHeight = baseHeight * (0.5 + animationIntensity * sin(TIME * animationSpeed - distance * 0.3));
    }
    
    // Ensure the height stays within bounds
    animatedHeight = clamp(animatedHeight, 0.05, 0.5);
    
    float bandTop = 0.5 - animatedHeight / 2.0;
    float bandBottom = 0.5 + animatedHeight / 2.0;
    
    if (uv.x >= cx - halfBand && uv.x <= cx + halfBand && uv.y >= bandTop && uv.y <= bandBottom) {
      result = bandColor;
      break;
    }
  }
  
  gl_FragColor = result;
}
