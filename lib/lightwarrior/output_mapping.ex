defmodule Lightwarrior.OutputMapping do

    def automap_instances_pixi(%{width: width, height: height} = mapping_container_size) do
        #dbg(Lightwarrior.State.get(:instances_with_config_output))
        first_instance = Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), 0)
        dbg(first_instance)
        first_instance_leds = get_in(first_instance, ["config", "info", "leds"])
        dbg(first_instance_leds)
        first_led = Enum.fetch!(first_instance_leds, 0)
        #first_led = Enum.fetch!(get_in(Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), 0), ["config", "info", "leds"]), 0)
        dbg(first_led)
        count = length(Lightwarrior.State.get(:instances_with_config_output))
        dbg(get_led_size(first_led))
        new_leds_list = build_led_list(get_led_size(first_led), count)
        #dbg(length(Lightwarrior.State.get(:instances_with_config_output)))
        dbg(update_leds(new_leds_list))
        #Lightwarrior.State.get(:instances_with_config_output)
    end

    def get_led_size(led) do
      %{
        "h_size" => (led["hmax"] - led["hmin"]),
        "v_size" => (led["vmax"] - led["vmin"])
      }
    end

    def build_led_list(%{"h_size" => h_size, "v_size" => v_size} = _led_size, count) do
        centerSpacing = 0.4 / count
        bandWidth = h_size
        halfWidth = bandWidth / 2.0
        max_height = 0.5
        dbg(centerSpacing)
        dbg(bandWidth)

        for i <- 0..(count - 1) do
          cx = (i + 0.5) * centerSpacing
          left = cx - halfWidth
          right = cx + halfWidth

          top = 0.5 - max_height / 2.0;
          bottom = 0.5 + max_height / 2.0;
          [
            %{
              "hmin" => left,
              "hmax" => right,
              "vmin" => top,
              "vmax" => top + v_size
            },
            %{
              "hmin" => left,
              "hmax" => right,
              "vmin" => bottom,
              "vmax" => bottom + v_size
            }
          ]
        end
    end

    def led_builder(hmax, hmin, vmax, vmin) do
      led = %{
          "hmax" => hmax,
          "hmin" => hmin,
          "vmax" => vmax,
          "vmin" => vmin
        }
    end

    def update_leds(new_leds_list) do
      Enum.zip(Lightwarrior.State.get(:instances_with_config_output), new_leds_list)
      |> Enum.map(fn {config_map, new_leds} ->
        final_leds =
          case new_leds do
            nil -> get_in(config_map, ["config", "info", "leds"])
            _ -> new_leds
          end

        put_in(config_map, ["config", "info", "leds"], final_leds)
      end)
    end

  end
