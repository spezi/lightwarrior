defmodule Lightwarrior.OutputShader do

  @isf_shader_file_path "ossia_score/output_shader.fs"

  @processes "ossia_score/processes.json"

  @file_path "ossia_score/lightwarrior.score"

  vertex_shader = """
#version 150

out vec2 left_coord;
out vec2 right_coord;
out vec2 above_coord;
out vec2 below_coord;

out vec2 lefta_coord;
out vec2 righta_coord;
out vec2 leftb_coord;
out vec2 rightb_coord;

void main()
{
    isf_vertShaderInit();
    vec2 texc = vec2(isf_FragNormCoord[0], isf_FragNormCoord[1]) * RENDERSIZE;

    left_coord  = texc + vec2(-1.0,  0.0);
    right_coord = texc + vec2( 1.0,  0.0);
    above_coord = texc + vec2( 0.0,  1.0);
    below_coord = texc + vec2( 0.0, -1.0);

    lefta_coord = texc + vec2(-1.0,  1.0);
    righta_coord = texc + vec2( 1.0,  1.0);
    leftb_coord = texc + vec2(-1.0, -1.0);
    rightb_coord = texc + vec2( 1.0, -1.0);
}
  """


  frangemt_shader = """
/*{
  "CATEGORIES": ["Video"],
  "INPUTS": [
  { "NAME": "inputImage", "TYPE": "image" },

  { "NAME": "start0", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end0", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height0", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start1", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end1", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height1", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start2", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end2", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height2", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start3", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end3", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height3", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start4", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end4", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height4", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start5", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end5", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height5", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start6", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end6", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height6", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start7", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end7", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height7", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start8", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end8", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height8", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start9", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end9", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height9", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start10", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end10", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height10", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start11", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end11", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height11", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start12", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end12", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height12", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start13", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end13", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height13", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start14", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end14", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height14", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start15", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end15", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height15", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start16", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end16", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height16", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start17", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end17", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height17", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start18", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end18", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height18", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start19", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end19", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height19", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start20", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end20", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height20", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start21", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end21", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height21", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start22", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end22", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height22", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start23", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end23", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height23", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start24", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end24", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height24", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start25", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end25", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height25", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start26", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end26", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height26", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start27", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end27", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height27", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start28", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end28", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height28", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start29", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end29", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height29", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },

  { "NAME": "start30", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
  { "NAME": "end30", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
  { "NAME": "height30", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 }
]
}*/

void main() {
  vec2 uv = isf_FragNormCoord;
  float bandWidth = 5.0 / float(RENDERSIZE.x);
  float centerSpacing = 0.4 / 30.0;
  float halfWidth = bandWidth / 2.0;
  float max_height = 0.5;

  vec4 color = vec4(0.0);

  for (int i = 0; i < 31; ++i) {
    float cx = (float(i) + 0.5) * centerSpacing;

    float left = cx - halfWidth;
    float right = cx + halfWidth;


    // Per-band top and bottom
    float h = 0.0;
    vec2 start = vec2(0.0);
    vec2 end = vec2(1.0);

    // Access uniforms manually (hardcoded since dynamic indexing of inputs isn't allowed)
    if (i == 0) { start = start0; end = end0; h = height0; }
    else if (i == 1) { start = start1; end = end1; h = height1; }
    else if (i == 2) { start = start2; end = end2; h = height2; }
    else if (i == 3) { start = start3; end = end3; h = height3; }
    else if (i == 4) { start = start4; end = end4; h = height4; }
    else if (i == 5) { start = start5; end = end5; h = height5; }
    else if (i == 6) { start = start6; end = end6; h = height6; }
    else if (i == 7) { start = start7; end = end7; h = height7; }
    else if (i == 8) { start = start8; end = end8; h = height8; }
    else if (i == 9) { start = start9; end = end9; h = height9; }
    else if (i == 10) { start = start10; end = end10; h = height10; }
    else if (i == 11) { start = start11; end = end11; h = height11; }
    else if (i == 12) { start = start12; end = end12; h = height12; }
    else if (i == 13) { start = start13; end = end13; h = height13; }
    else if (i == 14) { start = start14; end = end14; h = height14; }
    else if (i == 15) { start = start15; end = end15; h = height15; }
    else if (i == 16) { start = start16; end = end16; h = height16; }
    else if (i == 17) { start = start17; end = end17; h = height17; }
    else if (i == 18) { start = start18; end = end18; h = height18; }
    else if (i == 19) { start = start19; end = end19; h = height19; }
    else if (i == 20) { start = start20; end = end20; h = height20; }
    else if (i == 21) { start = start21; end = end21; h = height21; }
    else if (i == 22) { start = start22; end = end22; h = height22; }
    else if (i == 23) { start = start23; end = end23; h = height23; }
    else if (i == 24) { start = start24; end = end24; h = height24; }
    else if (i == 25) { start = start25; end = end25; h = height25; }
    else if (i == 26) { start = start26; end = end26; h = height26; }
    else if (i == 27) { start = start27; end = end27; h = height27; }
    else if (i == 28) { start = start28; end = end28; h = height28; }
    else if (i == 29) { start = start29; end = end29; h = height29; }
    else if (i == 30) { start = start30; end = end30; h = height30; }

    float top = 0.5 - h * max_height / 2.0;
    float bottom = 0.5 + h * max_height / 2.0;

    if (uv.x >= left && uv.x <= right && uv.y >= top && uv.y <= bottom) {
      float t = (uv.y - top) / (bottom - top);
      vec2 sampleUV = mix(start, end, t);
      color = IMG_NORM_PIXEL(inputImage, sampleUV);
      break;
    }
  }

  gl_FragColor = color;
}


  """

  def gen_inputs() do

    header = """
  /*{
    "CATEGORIES": ["Video"],
    "INPUTS": [
    { "NAME": "inputImage", "TYPE": "image" },

  """
      input_list = for i <- 0..(length(Lightwarrior.State.get(:instances_with_config_output)) - 1) do
  """
    { "NAME": "start#{i}", "TYPE": "point2D", "DEFAULT": [0.25, 0.0] },
    { "NAME": "end#{i}", "TYPE": "point2D", "DEFAULT": [0.75, 1.0] },
    { "NAME": "height#{i}", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 }#{if i != (length(Lightwarrior.State.get(:instances_with_config_output)) - 1) do "," else "" end}

  """
      end

      footer = """
  ]
  }*/
  """
      IO.inspect((header <> Enum.join(input_list, "\n") <> footer), pretty: true)
  end

  def gen_code() do
    header = """

void main() {
  vec2 uv = isf_FragNormCoord;
  float bandWidth = 5.0 / float(RENDERSIZE.x);
  float centerSpacing = 0.4 / 30.0;
  float halfWidth = bandWidth / 2.0;
  float max_height = 0.5;

  vec4 color = vec4(0.0);

  for (int i = 0; i < #{length(Lightwarrior.State.get(:instances_with_config_output)) + 1}; ++i) {
  float cx = (float(i) + 0.5) * centerSpacing;

  float left = cx - halfWidth;
  float right = cx + halfWidth;


  // Per-band top and bottom
  float h = 0.0;
  vec2 start = vec2(0.0);
  vec2 end = vec2(1.0);
  """

      isf_for = for i <- 0..(length(Lightwarrior.State.get(:instances_with_config_output)) - 1) do
  """
      #{if i == 0 do "if (i == 0) { start = start0; end = end0; h = height0; }" else "" end}
      #{if i > 0 do "else if (i == #{i}) { start = start#{i}; end = end#{i}; h = height#{i}; }" else "" end}
  """
      end

      rest = """

    float top = 0.5 - h * max_height / 2.0;
    float bottom = 0.5 + h * max_height / 2.0;

    if (uv.x >= left && uv.x <= right && uv.y >= top && uv.y <= bottom) {
      float t = (uv.y - top) / (bottom - top);
      vec2 sampleUV = mix(start, end, t);
      color = IMG_NORM_PIXEL(inputImage, sampleUV);
      break;
    }
  }

  gl_FragColor = color;
}
  """
      IO.inspect((header <> Enum.join(isf_for, "\n") <> rest))
  end

  def build() do
    output_isf_shader = gen_inputs() <> gen_code()
    File.write!(@isf_shader_file_path, output_isf_shader)
    output_isf_shader
  end

  def patch_scorefile(output_isf_shader) do
      #dbg(output_isf_shader)
      #dbg(get_in(load_score_file(), ["Document", "BaseScenario", "Constraint", "Processes"]))
      File.write!(@processes, get_in(load_score_file(), ["Document", "BaseScenario", "Constraint", "Processes"]) |> Jason.encode!(pretty: true))
  end

  defp load_score_file do
    case File.read(@file_path) do
      {:ok, content} ->
        case Jason.decode(content) do
          {:ok, data} when is_map(data) -> data
          _ -> %{}
        end

      _ -> %{}
    end
  end



  end
