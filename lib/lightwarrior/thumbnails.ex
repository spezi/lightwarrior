defmodule Lightwarrior.Imageplayer.Thumbnail do
  require Logger

  @thumbnails_dir "priv/static/thumbnails"

  def generate_thumbnail(input_path) do
    File.mkdir_p!(@thumbnails_dir)

    output_path = Path.join(@thumbnails_dir, Path.basename(input_path, Path.extname(input_path)) <> "_thumb.jpg")

    case Thumbnex.create_thumbnail(input_path, output_path, max_width: 192, max_height: 192) do
      :ok ->
        Logger.info("Thumbnail created successfully: #{output_path}")
        {:ok, output_path}
      {:error, reason} ->
        Logger.error("Failed to create thumbnail: #{reason}")
        {:error, reason}
    end
  end
end
