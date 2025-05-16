defmodule Lightwarrior.Helper do
  @moduledoc """
  litle helper functions
  """
  alias Phoenix.LiveView.AsyncResult

  @doc """
  convert map string keys to atoms
  """
  def string_keys_to_atom_keys(map) when is_map(map) do
    map
    |> Enum.reduce(%{}, fn {key, value}, acc ->
      atom_key = String.to_existing_atom(key)
      new_value =
        if is_map(value) do
          string_keys_to_atom_keys(value)
        else
          value
        end
      Map.put(acc, atom_key, new_value)
    end)
  end

  #{:ok, leds_pixel} = Helper.leds_to_pixel(Map.get(config, "info"))

  #config_ = Map.put(config, :leds_pixel, leds_pixel)
  #dbg(final_config.leds_pixel)

  @doc """
  return a Map of Stripe Instances with
  converted LED coordinates to pixel coordinates
  """
  def leds_to_pixel!(fetched_all_stripes_config, mapping_container_size) do
    dbg(mapping_container_size)
    #dbg(Enum.at(fetched_all_stripes_config, 0))
    configs = case is_list(fetched_all_stripes_config) do
      true -> fetched_all_stripes_config
      false -> []
    end

    #leds = Map.get(config, "leds")
    #dbg(leds)
    result = Enum.map_every(configs, 1, fn config ->
      leds = Map.get(config["config"], "info")
             |> Map.get("leds")

      leds_pixel = Enum.map_every(leds, 1, fn led ->
              %{
                "hmax" => coordinate_to_pixel(led["hmax"], mapping_container_size.width),
                "hmin" => coordinate_to_pixel(led["hmin"], mapping_container_size.width),
                "vmax" => coordinate_to_pixel(led["vmax"], mapping_container_size.height),
                "vmin" => coordinate_to_pixel(led["vmin"], mapping_container_size.height),
              }
             end)
      %{
        instance: config["instance"],
        friendly_name: config["friendly_name"],
        leds: leds_pixel,
        start: get_stripe_start_pixipoint!(leds_pixel),
        end: get_stripe_end_pixipoint!(leds_pixel),
      }
    end)

    result
  end

  def leds_to_coordinates!(leds_pixel, mapping_container_size) do
    dbg(mapping_container_size)

    #leds = Map.get(config, "leds")
    #dbg(leds)
    result = Enum.map_every(leds_pixel.leds, 1, fn led->
              %{
                "hmax" => pixel_to_coordinate(led["hmax"], mapping_container_size.width),
                "hmin" => pixel_to_coordinate(led["hmin"], mapping_container_size.width),
                "vmax" => pixel_to_coordinate(led["vmax"], mapping_container_size.height),
                "vmin" => pixel_to_coordinate(led["vmin"], mapping_container_size.height),
              }
    end)

    result
  end

  defp get_stripe_start_pixipoint!(leds_pixel) do
    led = Enum.fetch!(leds_pixel, 0)
    [led["hmin"], led["vmin"]]
  end

  defp get_stripe_end_pixipoint!(leds_pixel) do
    led = Enum.fetch!(leds_pixel, -1)
    [led["hmin"], led["vmin"]]
  end

  def coordinate_to_pixel(point, max_pixel) do
    point * max_pixel
  end

  def pixel_to_coordinate(pixel, max_pixel) do
    #dbg(pixel)
    #dbg(max_pixel)
    pixel/max_pixel
  end

end
