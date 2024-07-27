defmodule Lightwarrior.Imageplayer.Thumbnail do
  require Logger

  @sources_dir "priv/static/sources"
  @thumbnails_dir "priv/static/thumbnails"
  @static_url "/thumbnails"

  def generate_thumbnail(input_path, size) do
    dbg(input_path)
    File.mkdir_p!(@thumbnails_dir)

    images_extensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".svg", ".ico"]
    video_extensions = [".mp4", ".mov", ".wmv", ".flv", ".avi", ".mkv", ".webm", ".vob", ".ogv", ".ogg", ".drc", ".mng", ".mts", ".m2ts", ".ts", ".mxf", ".roq", ".nsv", ".f4v", ".f4p", ".f4a", ".f4b"]

    original_file_extension = Path.extname(input_path)
    dbg(original_file_extension)

    out_file_extension  = cond do
      Enum.member?(images_extensions, original_file_extension) ->
        ".jpg"
      Enum.member?(video_extensions, original_file_extension) ->
        ".gif"
    end

    output_path = Path.join(@thumbnails_dir, Path.basename(input_path, Path.extname(input_path)) <> "_thumb" <> out_file_extension )
    static_url = Path.join(@static_url, Path.basename(input_path, Path.extname(input_path)) <> "_thumb" <> out_file_extension )

    cond do
      Enum.member?(images_extensions, original_file_extension) ->
        #".jpg"
        case Thumbnex.create_thumbnail(input_path, output_path, max_width: size.w, max_height: size.h) do
          :ok ->
            Logger.info("Thumbnail created successfully: #{output_path}")
            {:ok, output_path, static_url}
          {:error, reason} ->
            Logger.error("Failed to create thumbnail: #{reason}")
            {:error, reason}
        end
      Enum.member?(video_extensions, original_file_extension) ->
        #".gif"
        case Thumbnex.animated_gif_thumbnail(input_path, output_path, frame_cont: 4, fps: 1) do
          :ok ->
            Logger.info("Thumbnail created successfully: #{output_path}")
            {:ok, output_path, static_url}
          {:error, reason} ->
            Logger.error("Failed to create thumbnail: #{reason}")
            {:error, reason}
        end

    end
    #dbg(static_url)


  end

  def convert_original(input_path, size) do
    dbg(input_path)
    dbg(size)

    #output_path = input_path
    output_path = Path.join(Path.absname(@sources_dir), Path.basename(input_path))

    new_file = Mogrify.open(input_path)
    |> Mogrify.resize_to_fill("#{size.w}x#{size.h}")
    |> Mogrify.save(path: output_path)

    dbg(new_file)

    {:ok, output_path}

  end

end
