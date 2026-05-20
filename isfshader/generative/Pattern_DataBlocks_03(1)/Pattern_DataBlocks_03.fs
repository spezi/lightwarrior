/*{
    "CREDIT": "AxiomCrux.net - Versão Corrigida",
    "DESCRIPTION": "Padrões de Dados Digitais",
    "CATEGORIES": [ "generator", "2d" ],
    "INPUTS": [
        { "NAME": "grid", "TYPE": "point2D", "MIN": [6,6], "MAX": [50,50], "DEFAULT": [45,50] },
        { "NAME": "grid2", "TYPE": "point2D", "MIN": [6,6], "MAX": [50,50], "DEFAULT": [6,50] },
        { "NAME": "center", "TYPE": "point2D", "MIN": [-1.0,-1.0], "MAX": [1.0,1.0], "DEFAULT": [0,0] },
        { "NAME": "curve", "TYPE": "point2D", "MIN": [-1.0,-1.0], "MAX": [1.0,1.0], "DEFAULT": [0,0] },
        { "NAME": "shift", "TYPE": "float", "MIN": 0, "MAX": 1, "DEFAULT": 0.20 },
        { "NAME": "density", "TYPE": "float", "MIN": -900, "MAX": 1800, "DEFAULT": 270.0 },
        { "NAME": "rate", "TYPE": "float", "MIN": -3, "MAX": 3, "DEFAULT": -0.05 },
        { "NAME": "seed", "TYPE": "float", "MIN": 8, "MAX": 233, "DEFAULT": 71.34 },
        { "NAME": "offset", "TYPE": "float", "DEFAULT": -0.05, "MIN": -5.0, "MAX": 10.0 },
        { "NAME": "polar_on", "TYPE": "bool", "DEFAULT": false }
    ]
}*/

#ifdef GL_ES
precision highp float;
#endif

#define TWO_PI 6.28318530718

// Funções de números aleatórios
float random(float x) {
    return fract(sin(x * 12.9898) * 43758.5453);
}

float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

float random2(vec2 st, float s) {
    return fract(sin(dot(st.xy, vec2(s, s * 2.0))) * s);
}

// Função para criar padrão de dados
float pattern(vec2 st, vec2 v, float t) {
    vec2 p = floor(st + v);
    return step(t, random2(p * 0.001, seed) * 0.8);
}

// Função para calcular offsets baseados no parâmetro offset
vec2 calcularOffset(float offsetVal, int canal) {
    float off1, off2;
    
    if (offsetVal < 5.0) {
        off1 = offsetVal * 5.1;
        off2 = 0.0;
    } else {
        off1 = 1.0;
        off2 = (offsetVal - 5.0) * 0.01;
    }
    
    if (canal == 0) return vec2(off1, 0.0);      // Vermelho
    if (canal == 1) return vec2(0.0, 0.0);       // Verde
    return vec2(-off2, 0.0);                      // Azul
}

void main() {
    // Normalizar coordenadas
    vec2 st = gl_FragCoord.xy / RENDERSIZE.xy;
    st.x *= RENDERSIZE.x / RENDERSIZE.y;
    st -= center;
    
    // Coordenadas polares (opcional)
    vec2 polar;
    polar.x = atan(st.y, st.x) / TWO_PI;
    polar.y = length(st);
    
    // Misturar coordenadas cartesianas com polares baseado no parâmetro curve
    if (polar_on) {
        st = mix(st, polar, curve);
    }
    
    // Aplicar grade
    st *= grid;
    
    // Separar parte inteira e fracionária
    vec2 ipos = floor(st);
    vec2 fpos = fract(st);
    
    // Calcular velocidade baseada no tempo e posição
    float velocidadeX = TIME * rate * max(grid.x, grid.y);
    float randomVel = random(ipos.y + 1.0) * 2.0 - 1.0;
    vec2 vel = vec2(velocidadeX * randomVel, 0.0);
    vel *= grid2;
    
    // Calcular offsets para cada canal de cor
    vec2 offsetVermelho = calcularOffset(offset, 0);
    vec2 offsetVerde = calcularOffset(offset, 1);
    vec2 offsetAzul = calcularOffset(offset, 2);
    
    // Gerar padrões para cada canal
    float limite = 0.5 + density / RENDERSIZE.x;
    
    float r = pattern(st + offsetVermelho, vel, limite);
    float g = pattern(st + offsetVerde, vel, limite);
    float b = pattern(st + offsetAzul, vel, limite);
    
    // Aplicar máscara vertical baseada no shift
    float mascaraVertical = step(shift, fpos.y);
    
    vec3 color = vec3(r, g, b) * mascaraVertical;
    
    gl_FragColor = vec4(color, 1.0);
}