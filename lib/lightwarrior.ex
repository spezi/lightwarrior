defmodule Lightwarrior do
  @moduledoc """
  Lightwarrior keeps the contexts that define your domain
  and business logic.

  Contexts are also responsible for managing your data, regardless
  if it comes from the database, an external API or others.
  """

  require Math

  def update_selected_stripe_data_pixel(leds_pixel, selected, points) do
    #dbg(selected)
    #dbg(bounds)
    selected_stripe_data_pixel = Enum.fetch!(leds_pixel, selected)

    List.replace_at(leds_pixel,
      selected,
      selected_stripe_data_pixel
      |> Map.replace(:leds, interpolate_coords(points, length(selected_stripe_data_pixel.leds)))
      |> Map.replace(:start, [points.start.x, points.start.y])
      |> Map.replace(:end, [points.end.x, points.end.y])
    )

  end

  defp get_distance(points) do
    Math.sqrt(Math.pow((points.end.x - points.start.x), 2) + Math.pow((points.end.y - points.start.y), 2));
  end

  defp interpolate_coords(points, num_leds) do
    #bounds.minX
    #bounds.maxX
    #bounds.minY
    #bounds.maxY

    max_size = get_distance(points)/num_leds


    step_x = (points.end.x - points.start.x) / (num_leds - 1)
    step_y = (points.end.y - points.start.y) / (num_leds - 1)

    #for i <- 0..(num_leds - 1) do
    #  %{:vmin => y1 + i * step_y, :hmin => x1 + i * step_x, :vmax => y1 + i * step_y + max_size, :hmax => x1 + i * step_x + max_size }
    #end

    for i <- 0..(num_leds - 1) do
      %{"vmin" => points.start.y + i * step_y, "hmin" => points.start.x + i * step_x, "vmax" => points.start.y + i * step_y + max_size, "hmax" => points.start.x + i * step_x + max_size }
    end

  end

end
