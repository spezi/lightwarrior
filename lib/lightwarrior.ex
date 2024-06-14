defmodule Lightwarrior do
  @moduledoc """
  Lightwarrior keeps the contexts that define your domain
  and business logic.

  Contexts are also responsible for managing your data, regardless
  if it comes from the database, an external API or others.
  """

  require Math

  def update_selected_stripe_data_pixel(leds_pixel, selected, bounds) do
    #dbg(selected)
    #dbg(bounds)
    selected_stripe_data_pixel = Enum.fetch!(leds_pixel, selected)

    List.replace_at(leds_pixel,
      selected,
      selected_stripe_data_pixel
      |> Map.replace(:leds, interpolate_coords(bounds, length(selected_stripe_data_pixel.leds)))
      |> Map.replace(:start, [bounds["minX"], bounds["minY"]])
      |> Map.replace(:end, [bounds["maxX"], bounds["maxY"]])
    )

  end

  defp get_distance(bounds) do
    Math.sqrt(Math.pow((bounds["maxX"] - bounds["minX"]), 2) + Math.pow((bounds["maxY"] - bounds["minY"]), 2));
  end

  defp interpolate_coords(bounds, num_leds) do
    #bounds.minX
    #bounds.maxX
    #bounds.minY
    #bounds.maxY

    max_size = get_distance(bounds)/num_leds


    step_x = (bounds["maxX"] - bounds["minX"]) / (num_leds - 1)
    step_y = (bounds["maxY"] - bounds["minY"]) / (num_leds - 1)

    #for i <- 0..(num_leds - 1) do
    #  %{:vmin => y1 + i * step_y, :hmin => x1 + i * step_x, :vmax => y1 + i * step_y + max_size, :hmax => x1 + i * step_x + max_size }
    #end

    for i <- 0..(num_leds - 1) do
      %{"vmin" => bounds["minY"] + i * step_y, "hmin" => bounds["minX"] + i * step_x, "vmax" => bounds["minY"] + i * step_y + max_size, "hmax" => bounds["minX"] + i * step_x + max_size }
    end

  end

end
