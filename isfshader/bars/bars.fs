/*{
  "DESCRIPTION": "Horizontal and vertical bars with controllable thickness, length, position, and color.",
  "CATEGORIES": ["Bars", "Utility"],
  "INPUTS": [
    {
      "NAME": "barColor",
      "TYPE": "color",
      "DEFAULT": [1.0, 1.0, 1.0, 1.0]
    },
    {
      "NAME": "hPos",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Horizontal Bar Y Position"
    },
    {
      "NAME": "hThickness",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 0.001,
      "MAX": 1.0,
      "LABEL": "Horizontal Bar Thickness"
    },
    {
      "NAME": "hLength",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Horizontal Bar Length"
    },
    {
      "NAME": "vPos",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Vertical Bar X Position"
    },
    {
      "NAME": "vThickness",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 0.001,
      "MAX": 1.0,
      "LABEL": "Vertical Bar Thickness"
    },
    {
      "NAME": "vLength",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 1.0,
      "LABEL": "Vertical Bar Length"
    }
  ]
}*/

void main() {
  vec2 uv = isf_FragNormCoord;
  vec4 color = vec4(0.0);

  // Horizontal bar
  float hTop = hPos + hThickness / 2.0;
  float hBottom = hPos - hThickness / 2.0;
  float hLeft = 0.5 - hLength / 2.0;
  float hRight = 0.5 + hLength / 2.0;

  // Vertical bar
  float vLeft = vPos - vThickness / 2.0;
  float vRight = vPos + vThickness / 2.0;
  float vTop = 0.5 + vLength / 2.0;
  float vBottom = 0.5 - vLength / 2.0;

  bool inHorizontalBar = uv.y >= hBottom && uv.y <= hTop && uv.x >= hLeft && uv.x <= hRight;
  bool inVerticalBar = uv.x >= vLeft && uv.x <= vRight && uv.y >= vBottom && uv.y <= vTop;

  if (inHorizontalBar || inVerticalBar) {
    color = barColor;
  }

  gl_FragColor = color;
}
