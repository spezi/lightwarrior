defmodule Lightwarrior do
  @moduledoc """
  Lightwarrior keeps the contexts that define your domain
  and business logic.

  Contexts are also responsible for managing your data, regardless
  if it comes from the database, an external API or others.
  """

  @processes_patched "ossia_score/processes_patched.json"

  @file_path "ossia_score/lightwarrior.score"

  @scorefile_patched "ossia_score/lightwarrior_patched.score"


  alias Lightwarrior.Helper
  alias Lightwarrior.Hyperion

  require Math

  def get_led_size(instance_data, mapping_container_size) do
    #dbg(instance_data)
    firstLed = Enum.fetch!(Map.get(instance_data, "leds"), 0)
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

  def update_selected_instance_data_pixel(num_leds, leds_pixel, selected, points) do
    #dbg(selected)
    #dbg(bounds)
    selected_instance_data_pixel = Enum.fetch!(leds_pixel, selected)
    dbg(points)
    dbg(num_leds)
    if num_leds do
        List.replace_at(leds_pixel,
        selected,
        selected_instance_data_pixel
        |> Map.replace(:leds, interpolate_coords(points, num_leds))
        |> Map.replace(:start, [points.start.x, points.start.y])
        |> Map.replace(:end, [points.end.x, points.end.y])
      )
    end


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

  def save_input(socket) do
      dbg("save input")
      if Lightwarrior.State.get(:instances_with_config_input) do

        # update ossia via osc messages
        #Lightwarrior.update_instance_ossia(leds, socket.assigns.selected, socket.assigns.sc_pid)

        # update ossia via osc messages
        Enum.each(Lightwarrior.State.get(:instances_with_config_input), fn instance ->
          #dbg(get_in(instance, ["settings", "leds"]))
          Lightwarrior.update_instance_ossia(get_in(instance, ["settings", "leds"]), socket.assigns.selected, socket.assigns.sc_pid)
        end)

        #{ :ok, selected_config } = Enum.fetch(Lightwarrior.State.get(:instances_with_config_input), socket.assigns.selected)
        # dbg(Lightwarrior.InputConfigsFileStore.put("instances_with_config_input", Lightwarrior.State.get(:instances_with_config_input)))
        dbg(Lightwarrior.InputConfigsFileStore.put("instances_with_config_input", Lightwarrior.State.get(:instances_with_config_input)))
        #dbg(Lightwarrior.InputConfigsFileStore.persist())
        #dbg(Lightwarrior.InputConfigsFileStore.reload())
        case Lightwarrior.InputConfigsFileStore.persist() do
          :ok ->
            dbg(Lightwarrior.InputConfigsFileStore.reload())
            dbg(Map.keys(Lightwarrior.State.all()))
            %{"success" => true }
          #:error -> %{"success" => false, "error" => "failed to write file" }
        end
      else
        %{"success" => false, "error" => "have no Data to save" }
      end
  end

  def save_output(selected) do
      dbg("save output")
      #https://api.hyperion-project.org/updateconfiguration-17021074e0

      if Lightwarrior.State.get(:instances_with_config_output) do
        { :ok, selected_config } = Enum.fetch(Lightwarrior.State.get(:instances_with_config_output), selected)

        #dbg(selected_config)

        to_save_payload  = %{"instances": [
          selected_config
        ]}
        #to_save_payload = selected_config |> Map.get("config") |> Map.get("info")
        case dbg(Hyperion.save_current_config(to_save_payload)) do
            %{
              "success" => true,
            } ->
              %{"success" => true}
            %{
              "success" => false,
              "error" => error
            } ->
              %{"success" => false, "error" => error }
        end
      else
        %{"success" => false, "error" => "have no Data to save" }
      end
  end

  def save_output_global() do
      dbg("save output")
      #https://api.hyperion-project.org/updateconfiguration-17021074e0

      if Lightwarrior.State.get(:instances_with_config_output) do
        #{ :ok, selected_config } = Enum.fetch(Lightwarrior.State.get(:instances_with_config_output), selected)

        #dbg(selected_config)

        to_save_payload  = %{"instances" =>
          Hyperion.prepare_for_saving_hyperion(Lightwarrior.State.get(:instances_with_config_output))
        }
        #to_save_payload = selected_config |> Map.get("config") |> Map.get("info")
        Hyperion.save_current_config(to_save_payload)
      else
        %{"success" => false, "error" => "have no Data to save" }
      end
  end

  def save_uniform() do

  end

  @doc """
  Save config of current active stripe
  """
  def update_instance_ossia(leds, selected, sc_pid) do

    #dbg(leds)
    start = calculate_center(Enum.at(leds, 0))
    stop = calculate_center(Enum.at(leds, -1))
    #dbg(start)
    #dbg(stop)
    update_ossia_via_osc(start, stop, selected, sc_pid)
  end

  defp calculate_center(coords) do
    h_center = (coords["hmax"] + coords["hmin"]) / 2
    v_center = (coords["vmax"] + coords["vmin"]) / 2
    %{
      "h" => h_center,
      "v" => v_center
    }
  end

  defp update_ossia_via_osc(start, stop, selected, sc_pid) do
    # IP or host and port number for the UDP connection
    #ip_address = '127.0.0.1' # This could be changed to named address, like 'localhost'
    #port_num = 9997 # In this example, this is the default port used by Protokol

    # Open a port
    #{:ok, port} = :gen_udp.open(0, [:binary, {:active, true}])

    start_address = "/start" <> Integer.to_string(selected)
    end_address = "/end" <> Integer.to_string(selected)

    # Encode the message
    osc_message_start = %OSCx.Message{address: start_address, arguments: [start["h"], start["v"]]} |> OSCx.encode()
    osc_message_end = %OSCx.Message{address: end_address, arguments: [stop["h"], stop["v"]]} |> OSCx.encode()

    # Send message
    #dbg(:gen_udp.send(port, ip_address, port_num, osc_message_start))
    #dbg(:gen_udp.send(port, ip_address, port_num, osc_message_end))
    #Lightwarrior.Hyperion.OSC.send_message(osc_message_start, [])
    #Lightwarrior.Hyperion.OSC.send_message(osc_message_end, [])
    dbg(Lightwarrior.Hyperion.SC.send(sc_pid, osc_message_start))
    dbg(Lightwarrior.Hyperion.SC.send(sc_pid, osc_message_end))

    # Close the port
    #:gen_udp.close(port)

  end

  @doc """
  Save config of current active stripe
  """
  def set_ossia_score_osc_adresses(socket) do

      score_file = load_score_file()

      processes = get_in(score_file, ["Document", "BaseScenario", "Constraint", "Processes"])
      updated_processes =
        Enum.map(processes, fn process ->
          updated_inlets =
            Enum.map(process["Inlets"] || [], fn inlet ->
              custom = inlet["Custom"]

              if custom && (String.starts_with?(custom, "start") || String.starts_with?(custom, "end")) do
                Map.put(inlet, "Address", "lightwarrior:/#{custom}")
              else
                inlet
              end
            end)

          Map.put(process, "Inlets", updated_inlets)
        end)

        # for ossia score Address learning
        Enum.each(Lightwarrior.State.get(:instances_with_config_input), fn instance ->
          dbg(Lightwarrior.update_instance_ossia(get_in(instance, ["settings", "leds"]), socket.assigns.selected, socket.assigns.sc_pid))
        end)


        #dbg(updated_processes)
        File.write!(@processes_patched, updated_processes|> Jason.encode!(pretty: true))

        patched_scorefile = put_in(score_file, ["Document", "BaseScenario", "Constraint", "Processes"], updated_processes)
        File.write!(@scorefile_patched, patched_scorefile |> Jason.encode!(pretty: true))
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
