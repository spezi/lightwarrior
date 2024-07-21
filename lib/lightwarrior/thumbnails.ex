defmodule Lightwarrior.Imageplayer.Thumbnail do
  require Logger

  @thumbnails_dir "priv/static/thumbnails"
  @static_url "/thumbnails"

  def generate_thumbnail(input_path, size) do
    File.mkdir_p!(@thumbnails_dir)

    output_path = Path.join(@thumbnails_dir, Path.basename(input_path, Path.extname(input_path)) <> "_thumb.jpg")
    static_url = Path.join(@static_url, Path.basename(input_path, Path.extname(input_path)) <> "_thumb.jpg")

    #dbg(static_url)

    case Thumbnex.create_thumbnail(input_path, output_path, max_width: size.w, max_height: size.h) do
      :ok ->
        Logger.info("Thumbnail created successfully: #{output_path}")
        {:ok, output_path, static_url}
      {:error, reason} ->
        Logger.error("Failed to create thumbnail: #{reason}")
        {:error, reason}
    end
  end
end
