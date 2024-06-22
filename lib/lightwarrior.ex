defmodule Lightwarrior do
  @moduledoc """
  Lightwarrior keeps the contexts that define your domain
  and business logic.

  Contexts are also responsible for managing your data, regardless
  if it comes from the database, an external API or others.
  """

  alias Lightwarrior.Helper

  require Math

  def get_led_size(stripe_data, mapping_container_size) do
    #dbg(stripe_data)
    firstLed = Enum.fetch!(Map.get(stripe_data, "leds"), 0)
    #dbg(firstLed)
    point = {
      (firstLed["hmax"] - firstLed["hmin"]),
      (firstLed["vmax"] - firstLed["vmin"])
    }
    %{
      pixel: %{
        width: Helper.coordinate_to_pixel(elem(point, 0), mapping_container_size.width),
        height: Helper.coordinate_to_pixel(elem(point, 1), mapping_container_size.height)
      },
      point: %{
        width: elem(point, 0),
        height: elem(point, 1)
      },
      mapping_container_size: mapping_container_size
    }
  end

  def update_selected_stripe_data_pixel(num_leds, leds_pixel, selected, points) do
    #dbg(selected)
    #dbg(bounds)
    selected_stripe_data_pixel = Enum.fetch!(leds_pixel, selected)

    List.replace_at(leds_pixel,
      selected,
      selected_stripe_data_pixel
      |> Map.replace(:leds, interpolate_coords(points, num_leds))
      |> Map.replace(:start, [points.start.x, points.start.y])
      |> Map.replace(:end, [points.end.x, points.end.y])
    )

  end

  defp get_distance(points) do
    Math.sqrt(Math.pow((points.end.x - points.start.x), 2) + Math.pow((points.end.y - points.start.y), 2));
  end

  defp interpolate_coords(points, num_leds) do
    dbg(num_leds)
    #bounds.minX
    #bounds.maxX
    #bounds.minY
    #bounds.maxY

    mapping_container_size = get_distance(points)/num_leds


    step_x = (points.end.x - points.start.x) / (num_leds - 1)
    step_y = (points.end.y - points.start.y) / (num_leds - 1)

    #for i <- 0..(num_leds - 1) do
    #  %{:vmin => y1 + i * step_y, :hmin => x1 + i * step_x, :vmax => y1 + i * step_y + mapping_container_size, :hmax => x1 + i * step_x + mapping_container_size }
    #end

    for i <- 0..(num_leds - 1) do
      %{"vmin" => points.start.y + i * step_y, "hmin" => points.start.x + i * step_x, "vmax" => points.start.y + i * step_y + mapping_container_size, "hmax" => points.start.x + i * step_x + mapping_container_size }
    end

  end



end
