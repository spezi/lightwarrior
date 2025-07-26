defmodule LightwarriorWeb.HyperionConfigLive.Index do
  use LightwarriorWeb, :live_view

  #alias Phoenix.LiveView.JS
  alias Lightwarrior.Hyperion
  alias Lightwarrior.Helper
  require Logger

  alias Lightwarrior.MappingMenueForm

  use Phoenix.VerifiedRoutes, endpoint: LightwarriorWeb.Endpoint, router: LightwarriorWeb.Router

  # alias OSC SC because of module name collision
  alias Lightwarrior.Hyperion.SC

  @impl true
  def mount(_params, _session, socket) do

    if connected?(socket) do
      Phoenix.PubSub.subscribe(Lightwarrior.PubSub, "hyperion")
    end

    # start osc session
    {:ok, sc_pid} = SC.start_link()

    #form_data = %MappingMenueForm{}
    #mapping = %{"lockdistance"=> true, "opacity"=> 70}

    mapping_changeset_input  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: "input", lockdistance: "true", instances_color: "#4adf72", opacity: 70})
    mapping_changeset_output  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: "output", lockdistance: "true", instances_color: "#4adf72",opacity: 70})
    mapping_changeset_uniform  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: "uniform", lockdistance: "true", instances_color: "#4adf72", opacity: 70})

    #dbg(mapping_changeset_input)
    #dbg(mapping_changeset_output)

    # state in own datastore by genserver in state.ex
    # TODO cleanup lightwarrior.ex not needed functions
    dbg(Map.keys(Lightwarrior.State.all()))
    dbg(Map.keys(Lightwarrior.InputConfigsFileStore.all()))
    dbg(Lightwarrior.State.put(:instances_with_config_input, Lightwarrior.InputConfigsFileStore.get("instances_with_config_input")))

    {:ok,
     socket
     |> assign(:page_title, "Hyperionconfig")
     #|> assign(:state, Lightwarrior.HyperionApi.get_data())
     |> assign(:selected, nil)
     |> assign(:debug, false)
     |> assign(:autosave, false)
     |> assign(:mapping_input, to_form(mapping_changeset_input, id: :mapping_tools_form_input, as: :mapping_tools_form))
     |> assign(:mapping_output, to_form(mapping_changeset_output, id: :mapping_tools_form_output, as: :mapping_tools_form))
     |> assign(:mapping_uniform, to_form(mapping_changeset_uniform, id: :mapping_tools_form_uniform, as: :mapping_tools_form))
     #|> assign(form: to_form(Map.from_struct(form_data)))
     |> assign(:side, nil)
     |> assign(:mapping_container_size, %{width: 0.0, height: 0.0})
     |> assign(:instances_data_pixel_map, %{"input" => nil, "output" => nil, "uniform" => nil})
     |> assign(:sc_pid, sc_pid)
     #|> stream(:hyperionconfigs, Hyperion.list_hyperionconfigs())
     |> push_event("ready", %{})
    }
  end

  @impl true
  def handle_params(params, uri, socket) do

    # set selected
    socket = case params do
      %{"id" => id} ->
        dbg("selected: " <> id)
        dbg(Lightwarrior.State.get(:instances_with_config_output))
        dbg(Hyperion.switch_instance(Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), String.to_integer(id))))
        socket
        |> assign(:selected, String.to_integer(id))
        |> push_event("select", %{instance: id})
      _ -> socket
    end

    # check for data and copy from output mapping if none

    socket = if Lightwarrior.State.get(:instances_with_config_output) != nil && length(Lightwarrior.State.get(:instances_with_config_output)) > 0 do
        #dbg(Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), 0))
        #dbg(Map.get(Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), 0), :config) )
        case Map.get(Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), 0), "config") do
          %{
            "command" => "config",
            "error" => error,
            "instance" => 0,
            "success" => _success,
            "tan" => 1
          } ->
            put_flash(socket, :error, "Hyperion error getting Instances configuration: " <> error)
          _ ->
              socket = if length(Map.keys(Lightwarrior.InputConfigsFileStore.all())) == 0 do
                dbg(Lightwarrior.InputConfigsFileStore.put("instances_with_config_input", Lightwarrior.State.get(:instances_with_config_output)))
                dbg(Lightwarrior.InputConfigsFileStore.persist())
                dbg(Lightwarrior.InputConfigsFileStore.reload())
                put_flash(socket, :info, "No input configs available. create new from current output mapping ..")
                |> push_navigate(to: ~p"/hyperion/", replace: true)
              else
                socket
              end
            socket
        end
      else
      socket
    end

    # Extract the path from the full URI
    path = URI.parse(uri).path
    #dbg(path)

    {:noreply, assign(socket, :current_path, path)}
  end

  def handle_event("validate", %{"_target" => target, "mapping_tools_form" => mapping_tools_form} = _params, socket) do
    #mapping = %{lockdistance: socket.assigns.mapping.lockdistance, opacity: String.to_integer(opacity)}

    #dbg(target)
    #dbg(mapping_tools_form)

    # to reset stage on color change
    socket = case Enum.at(target,1) do
      "instances_color" -> socket |> push_event("instances_color", %{})
        _ -> socket
    end

    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, mapping_tools_form)
    #dbg(mapping_changeset)

    socket = case mapping_tools_form["side"] do
      "input" ->
        socket
        |> assign(:mapping_input, to_form(mapping_changeset, id: :mapping_tools_form_input, as: :mapping_tools_form))
        |> assign(:side, "input")
        |> push_event("localstorage", %{ input_opacity: mapping_tools_form["opacity"] })
        |> push_event("localstorage", %{ input_instances_color: mapping_tools_form["instances_color"] })
      "output" ->
        socket
        |> assign(:mapping_output, to_form(mapping_changeset, id: :mapping_tools_form_output, as: :mapping_tools_form))
        |> assign(:side, "output")
        |> push_event("localstorage", %{ output_opacity: mapping_tools_form["opacity"] })
        |> push_event("localstorage", %{ output_instances_color: mapping_tools_form["instances_color"] })
      "uniform" ->
        socket
        |> assign(:mapping_uniform, to_form(mapping_changeset, id: :mapping_tools_form_uniform, as: :mapping_tools_form))
        |> assign(:side, "uniform")
        |> push_event("localstorage", %{ uniform_opacity: mapping_tools_form["opacity"] })
        |> push_event("localstorage", %{ uniform_instances_color: mapping_tools_form["instances_color"] })
      _ ->
        socket
    end

    {:noreply,
      socket
    }
  end

  def handle_event("phx:move-stripe", %{"direction" => direction, "value" => _value}, socket) do
    dbg(direction)
    {:noreply,
      socket
    }
  end

  def handle_event("phx:hyperion-start", %{} = _params, socket) do
    dbg("start hyperion")
    dbg(Lightwarrior.Services.start_process("/Applications/Hyperion.app/Contents/MacOS/Hyperion"))
    dbg(Lightwarrior.Services.get_state())
    {:noreply,
      socket
    }
  end

  def handle_event("phx:hyperion-stop", %{} = _params, socket) do
    dbg("stop hyperion")
    dbg(Lightwarrior.Services.stop_process())
    {:noreply,
      socket
    }
  end

  def handle_event("phx:ossia-score-start", %{} = _params, socket) do
    dbg("start hyperion")
    dbg(Lightwarrior.Services.start_process("/Applications/ossia score.app/Contents/MacOS//ossia\ score"))
    dbg(Lightwarrior.Services.get_state())
    {:noreply,
      socket
    }
  end

  def handle_event("phx:ossia-score-stop", %{} = _params, socket) do
    dbg("stop hyperion")
    dbg(Lightwarrior.Services.stop_process())
    {:noreply,
      socket
    }
  end

  @impl true
  def handle_info(%{ "hyperion_ready" => true } = _data, socket) do
    #send_update(LightwarriorWeb.HyperionComponents, id: "left_top_menue", refresh: true)
    #dbg(socket.assigns.current_path)
    {:noreply,
      socket
      |> put_flash(:info, "Hyperion connected !!")
      |> push_navigate(to: socket.assigns.current_path, replace: true)
    }
  end

  @impl true
  def handle_info(%{ "hyperion_ready" => false } = _data, socket) do
    #send_update(LightwarriorWeb.HyperionComponents, id: "left_top_menue", refresh: true)
    #dbg(socket.assigns.current_path)
    {:noreply,
      socket
      |> put_flash(:error, "Hyperion not connected !!")
    }
  end

  def handle_event("refresh", %{"value" => ""} = _referer, socket) do

    dbg(Lightwarrior.HyperionApi.refresh_data())

    {:noreply,
      socket
      |> push_event("refresh", %{})
    }
  end

  def handle_event("phx:init-autosave", params, socket) do
    case params do
        "true" -> {:noreply, socket |> assign(:autosave, true)}
        _ ->  {:noreply, socket}
    end
  end

  def handle_event("phx:init-debug", params, socket) do
    case params do
        "true" -> {:noreply, socket |> assign(:debug, true)}
        _ ->  {:noreply, socket}
    end
  end

  def handle_event("phx:init-input_opacity", params, socket) do
    dbg(params)
    params_concat = Map.merge(socket.assigns.mapping_input.source.params, %{"opacity" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    {:noreply,
      socket
      |> assign(:mapping_input, to_form(mapping_changeset, id: :mapping_tools_form_input, as: :mapping_tools_form))
    }
  end

  def handle_event("phx:init-output_opacity", params, socket) do
    #dbg(params)
    params_concat = Map.merge(socket.assigns.mapping_output.source.params, %{"opacity" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    {:noreply,
      socket
      |> assign(:mapping_output, to_form(mapping_changeset, id: :mapping_tools_form_output, as: :mapping_tools_form))
    }
  end

  def handle_event("phx:init-uniform_opacity", params, socket) do
    #dbg(params)
    params_concat = Map.merge(socket.assigns.mapping_uniform.source.params, %{"opacity" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    {:noreply,
      socket
      |> assign(:mapping_uniform, to_form(mapping_changeset, id: :mapping_tools_form_uniform, as: :mapping_tools_form))
    }
  end



  def handle_event("phx:init-input_instances_color", params, socket) do
    #dbg(params)
    params_concat = Map.merge(socket.assigns.mapping_input.source.params, %{"instances_color" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    #dbg(mapping_changeset)
    {:noreply,
      socket
      |> assign(:mapping_input, to_form(mapping_changeset, id: :mapping_tools_form_input, as: :mapping_tools_form))
    }
  end

  def handle_event("phx:init-output_instances_color", params, socket) do
    #dbg(params)
    #mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, %{"side" => "output", "instances_color" => params})
    params_concat = Map.merge(socket.assigns.mapping_output.source.params, %{"instances_color" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    #dbg(mapping_changeset)
    {:noreply,
      socket
      |> assign(:mapping_output, to_form(mapping_changeset, id: :mapping_tools_form_output, as: :mapping_tools_form))
    }
  end

  def handle_event("phx:init-uniform_instances_color", params, socket) do
    #dbg(params)
    params_concat = Map.merge(socket.assigns.mapping_uniform.source.params, %{"instances_color" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    #dbg(mapping_changeset)
    {:noreply,
      socket
      |> assign(:mapping_uniform, to_form(mapping_changeset, id: :mapping_tools_form_uniform, as: :mapping_tools_form))
    }
  end

  def handle_event("phx:last_open_tab", params, socket) do
    #dbg(params)
    socket = case params do
      %{"tab" => tab, "value" => _value} ->
          #dbg(tab)
          socket
          |> assign(:side, tab)
          |> push_event("instances-data-pixel", %{instances_data_pixel: []})
          |> push_event("localstorage", %{ last_open_tab: tab})
          |> push_event("tabchange", %{ last_open_tab: tab})
          #|> assign(:selected, nil)
          #|> push_patch(to: ~p"/hyperion/")
      %{"last_open_tab" => tab} ->
          #dbg(tab)
          socket
          |> assign(:side, tab)
       _ -> socket
    end

    {:noreply,
      socket
    }
  end

  def handle_event("phx.toggle_autosave", params, socket) do
    #dbg(params)
    bool_value = case params do
      %{"value" => value} ->
        if value == "on" do true else false end
      %{} -> false
    end

    {:noreply,
      socket
      |> assign(:autosave, bool_value )
      |> push_event("localstorage", %{ autosave: bool_value})
    }
  end

  def handle_event("phx.toggle_debug", params, socket) do
    #dbg(params)
    bool_value = case params do
      %{"value" => value} ->
        if value == "on" do true else false end
      %{} -> false
    end

    {:noreply,
      socket
      |> assign(:debug, bool_value )
      |> push_event("localstorage", %{ debug: bool_value})
      #|> JS.dispatch("click", to: ".nav")
      #|> JS.dispatch("phx:localstorage_save", data: %{ debug: !socket.assigns.debug })
    }
  end

  def handle_event("phx:select_instance", %{"value" => value} = _param, socket) do
    dbg("select: #{value}")
    #dbg(Enum.fetch!(Lightwarrior.State.get(:instances_with_config_output), value))
    {:noreply,
      socket
      |> push_patch(to: ~p"/hyperion/#{value}/edit")
    }
  end

  def handle_event("phx:selected_change_mapping", points, socket) do
    # Handle the size information as needed
    #IO.puts("Div width: #{width}, height: #{height}")

    dbg(points)
    points = Helper.string_keys_to_atom_keys(points)

    dbg(socket.assigns.side)
    #dbg(Lightwarrior.State.all())

    instances_data_config = case socket.assigns.side do
      "input" ->
          Lightwarrior.State.get(:instances_with_config_input)
      "output" ->
        Lightwarrior.State.get(:instances_with_config_output)
        _ -> []
    end


    instances_data_pixel = Lightwarrior.Helper.leds_to_pixel!(instances_data_config, socket.assigns.mapping_container_size)
    instance_data_config =  Enum.fetch!(instances_data_config, socket.assigns.selected)
    #dbg(instance_data_config)
    num_leds = instance_data_config["config"]["info"]["device"]["hardwareLedCount"]


    if num_leds > 1 do
          #lightwarrior.ex ;)
          instances_data_pixel_new = Lightwarrior.update_selected_instance_data_pixel(
            num_leds,
            instances_data_pixel,
            socket.assigns.selected,
            points
          )

          ### update data

          leds = Helper.leds_to_coordinates!(
            Enum.fetch!(instances_data_pixel_new, socket.assigns.selected),
            socket.assigns.mapping_container_size
          )

          #dbg(instance_data_config)
          #instance_data_config_new = Map.replace(instance_data_config, "leds", %{"penis" => true})
          instance_data_config_new = put_in(instance_data_config,["config", "info", "leds"], leds)


          #dbg(instance_data_config_new)
          instances_data_config_new = List.replace_at(instances_data_config, socket.assigns.selected, instance_data_config_new)

          dbg(socket.assigns.autosave)

          case socket.assigns.side do
            "input" ->
                dbg(Lightwarrior.State.put(:instances_with_config_input, instances_data_config_new))
                if socket.assigns.autosave, do: Lightwarrior.save_input()
            "output" ->
                dbg(Lightwarrior.State.put(:instances_with_config_output, instances_data_config_new))
              _ -> []
          end


          {:noreply,
            socket
            |> push_event("instances-data-pixel", %{instances_data_pixel: instances_data_pixel_new})
            |> push_event("change_mapping", %{})
          }
    else
          {:noreply,
            put_flash(socket, :error, "Device LED count is #{num_leds}, no interpolation possible")
          }
    end
  end

  @impl true
  def handle_event("save", %{"value" => ""} = _params, socket) do
    dbg("save stripe #{socket.assigns.selected}")

    #dbg(Enum.fetch(Lightwarrior.State.get(:instances_with_config_output), socket.assigns.selected))

    #instances_data_pixel = Map.get(socket.assigns.instances_data_pixel_map, socket.assigns.side)

    #leds = Helper.leds_to_coordinates!(
    #  Enum.fetch!(instances_data_pixel, socket.assigns.selected),
    #  socket.assigns.mapping_container_size
    #)

    #dbg(selected_config_updated)

    save = case socket.assigns.side do
      "input" ->
            Lightwarrior.save_input()
            # update ossia via osc messages
            #Lightwarrior.update_stripe_ossia(leds, socket.assigns.selected, socket.assigns.sc_pid)

      "output" ->
            Lightwarrior.save_output(socket.assigns.selected)
      "uniform" ->
            dbg("save uniform")
            { :ok, _selected_config } = Enum.fetch(Lightwarrior.State.get(:instances_with_config_output), socket.assigns.selected)
            #dbg(selected_config)
            %{"success" => true }
      _ ->
        dbg("save nothing")
        {:error, :econnrefused}
    end

    socket = case save do
      %{"success" => true } -> put_flash(socket, :info, "Instance updated on #{socket.assigns.side} mapping")
      %{"success" => false, "error" => error } -> put_flash(socket, :error, "Failed to update Stripe: " <> error)
      {:error, :econnrefused} -> put_flash(socket, :error, "Failure to save Instance, Hyperion is not reachable")
    end

    {:noreply, socket
      |> push_patch(to: ~p"/hyperion/#{socket.assigns.selected}/edit", replace: true)
    }
  end

  def handle_event("phx:get-instances-config", _referer, socket) do
    {:noreply,
      socket
    }
  end

  def handle_event("phx:set-even", %{"value" => _value} = _param, socket) do
    {:noreply,
      socket
    }
  end

  def handle_event("phx:toggle-distance-lock", %{"side" => side, "value" => _value} = _param, socket) do
    # params_concat because of failing db
    socket = case side do
      "input" ->
        params_concat = Map.merge(socket.assigns.mapping_input.source.params, %{"lockdistance" => !socket.assigns.mapping_input.params["lockdistance"]})
        mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
        #mapping_changeset  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: side, lockdistance: !socket.assigns.mapping_input.params["lockdistance"], opacity: socket.assigns.mapping_input.params["opacity"]})
        #dbg(mapping_changeset)
        socket
        |> assign(:mapping_input, to_form(mapping_changeset, id: :mapping_tools_form_input, as: :mapping_tools_form))
        |> push_event("lockdistance", %{lockdistance: !socket.assigns.mapping_input.params["lockdistance"]})
      "output" ->
        params_concat = Map.merge(socket.assigns.mapping_output.source.params, %{"lockdistance" => !socket.assigns.mapping_output.params["lockdistance"]})
        mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
        #mapping_changeset  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: side, lockdistance: !socket.assigns.mapping_output.params["lockdistance"], opacity: socket.assigns.mapping_output.params["opacity"]})
        #dbg(mapping_changeset)
        socket
        |> assign(:mapping_output, to_form(mapping_changeset, id: :mapping_tools_form_output, as: :mapping_tools_form))
        |> push_event("lockdistance", %{lockdistance: !socket.assigns.mapping_output.params["lockdistance"]})
      "uniform" ->
        params_concat = Map.merge(socket.assigns.mapping_uniform.source.params, %{"lockdistance" => !socket.assigns.mapping_uniform.params["lockdistance"]})
        mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
        #mapping_changeset  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: side, lockdistance: !socket.assigns.mapping_uniform.params["lockdistance"], opacity: socket.assigns.mapping_uniform.params["opacity"]})
        #dbg(mapping_changeset)
        socket
        |> assign(:mapping_uniform, to_form(mapping_changeset, id: :mapping_tools_form_uniform, as: :mapping_tools_form))
        |> push_event("lockdistance", %{lockdistance: !socket.assigns.mapping_uniform.params["lockdistance"]})
      _ ->
        socket
    end

    {:noreply,
      socket
    }
  end


  def handle_event("phx:mapping-size", %{"width" => width, "height" => height}, socket) do
    mapping_container_size = %{
      width: width,
      height: height
    }
    #dbg(mapping_container_size)

    instances_data_with_config = case socket.assigns.side do
      "input" -> Lightwarrior.State.get(:instances_with_config_input)
      "output" -> Lightwarrior.OutputMapping.automap_instances_pixi(mapping_container_size)
      #"output" -> Lightwarrior.State.get(:instances_with_config_output)
      _ -> nil
    end

    socket = if instances_data_with_config != nil && length(instances_data_with_config) > 0 do
        #dbg(Enum.fetch(instances_data_with_config, 0))
        instances_data_with_config_pixel = Lightwarrior.Helper.leds_to_pixel!(instances_data_with_config, mapping_container_size)
        #dbg(instances_data_with_config_pixel)
        socket
        |> push_event("instances-data-pixel", %{instances_data_pixel: instances_data_with_config_pixel})
    else
      socket
    end





    if true do
      {:noreply, socket
        |> assign(:mapping_container_size, mapping_container_size)
        #|> push_event("instances-data-pixel", %{instance_data_pixel: instances_data_with_config_pixel })
      }
    else
      {:noreply, socket
        |> assign(:mapping_container_size, mapping_container_size)
        #|> push_event("instances-data-pixel", %{instance_data_pixel: instances_data_with_config_pixel })
      }
    end
  end

end
