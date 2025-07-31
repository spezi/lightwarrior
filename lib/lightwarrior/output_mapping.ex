defmodule Lightwarrior.OutputMapping do

    def automap_instances_pixi(%{width: width, height: height} = mapping_container_size) do
          #dbg(Lightwarrior.State.get(:instances_with_config_output))
          if Lightwarrior.State.get(:instances_with_config_output) do
              first_instance = Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), 0)
              #dbg(first_instance)
              if first_instance do
                  first_instance_leds = get_in(first_instance, ["settings", "leds"])
                  if length(first_instance_leds) > 0 do
                    first_led = Enum.fetch!(first_instance_leds, 0)
                    #first_led = Enum.fetch!(get_in(Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), 0), ["config", "info", "leds"]), 0)
                    new_leds_list = build_led_list(get_led_size(first_led))
                    #dbg(new_leds_list)
                    #dbg(length(Lightwarrior.State.get(:instances_with_config_output)))
                    update_leds(new_leds_list)
                    #Lightwarrior.State.set(:instances_with_config_output, new_leds_list)
                  end
              end
        end
    end

    def get_led_size(led) do
      %{
        "h_size" => (led["hmax"] - led["hmin"]),
        "v_size" => (led["vmax"] - led["vmin"])
      }
    end

    def build_led_list(%{"h_size" => h_size, "v_size" => v_size} = _led_size) do

        instances = Lightwarrior.State.get(:instances_with_config_output)
        count_instances = length(instances)

        centerSpacing = 0.4 / count_instances
        bandWidth = h_size
        halfWidth = bandWidth / 2.0
        max_height = 0.5
        dbg(centerSpacing)
        dbg(bandWidth)

        for i <- 0..(count_instances - 1) do

          cx = (i + 0.5) * centerSpacing
          left = cx - halfWidth
          right = cx + halfWidth

          top = 0.5 - max_height / 2.0;
          bottom = 0.5 + max_height / 2.0;

          instance = Enum.fetch!(instances, i)

          if instance do
            dbg(get_in(instance, ["settings", "device", "hardwareLedCount"]))
            num_leds = get_in(instance, ["settings", "device", "hardwareLedCount"])
            points = %{ start: %{x: left, y: bottom}, end: %{x: left, y: top} }
            dbg(get_in(instance, ["name"]))
            new_leds = Lightwarrior.interpolate_coords(points, num_leds)

            # output to automap_instances_pixi


            new_leds

          else
            [
              %{
                "hmin" => left,
                "hmax" => right,
                "vmin" => bottom,
                "vmax" => bottom + v_size
              },
              %{
                "hmin" => left,
                "hmax" => right,
                "vmin" => top,
                "vmax" => top + v_size
              }
            ]
          end
        end
    end

    def update_leds(new_leds_list) do
      instances_with_config_output_new = Enum.zip(Lightwarrior.State.get(:instances_with_config_output), new_leds_list)
      |> Enum.map(fn {config_map, new_leds} ->
        final_leds =
          case new_leds do
            nil -> get_in(config_map, ["settings", "leds"])
            _ -> new_leds
          end

        put_in(config_map, ["settings", "leds"], final_leds)

      end)
      #dbg(instances_with_config_output_new)
      Lightwarrior.State.put(:instances_with_config_output, instances_with_config_output_new)
      #dbg(get_in(Enum.fetch!(instances_with_config_output_new, 0), ["settings", "leds"]))
      instances_with_config_output_new
    end

# modul end
  end
