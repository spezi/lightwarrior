defmodule LightwarriorWeb.HyperionConfigLive.Index do
  use LightwarriorWeb, :live_view

  alias Phoenix.LiveView.JS
  alias Lightwarrior.Hyperion
  alias Lightwarrior.Helper
  require Logger

  alias Lightwarrior.MappingMenueForm

  use Phoenix.VerifiedRoutes, endpoint: LightwarriorWeb.Endpoint, router: LightwarriorWeb.Router

  # alias OSC SC because of module name collision
  alias Lightwarrior.Hyperion.SC

  @impl true
  def mount(_params, _session, socket) do

    #if connected?(socket) do
    #  Phoenix.PubSub.subscribe(Lightwarrior.PubSub, "state")
    #end

    # start osc session
    {:ok, sc_pid} = SC.start_link()

    #form_data = %MappingMenueForm{}
    #mapping = %{"lockdistance"=> true, "opacity"=> 70}

    mapping_changeset_input  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: "input", lockdistance: "true", stripes_color: "#4adf72", opacity: 70})
    mapping_changeset_output  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: "output", lockdistance: "true", stripes_color: "#4adf72",opacity: 70})
    mapping_changeset_uniform  = MappingMenueForm.changeset(%MappingMenueForm{}, %{side: "uniform", lockdistance: "true", stripes_color: "#4adf72", opacity: 70})

    #dbg(mapping_changeset_input)
    #dbg(mapping_changeset_output)

    # Read and parse the JSON file
    json_content = File.read!("./mappings/stripes_config_dump.json")
    parsed_data = Jason.decode!(json_content)
    #dbg(parsed_data)
    stripes_with_config = Map.get(parsed_data, "stripes_with_config")
    #dbg(stripes_with_config)

    state = Map.replace(Lightwarrior.HyperionApi.get_data(), :stripes_with_config_input, stripes_with_config)

    {:ok,
     socket
     |> assign(:page_title, "Listing Hyperionconfigs")
     #|> assign(:state, Lightwarrior.HyperionApi.get_data())
     |> assign(:state, state)
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
  def handle_params(%{"id" => id}, _referer, socket) do
    dbg("selected: " <> id)
    {:noreply,
      socket
      |> assign(:selected, String.to_integer(id))
      |> push_event("select", %{instance: id})
    }
  end

  @impl true
  def handle_params(%{}, _referer, socket) do
    {:noreply, socket}
  end

  def handle_event("validate", %{"_target" => target, "mapping_tools_form" => mapping_tools_form} = params, socket) do
    #mapping = %{lockdistance: socket.assigns.mapping.lockdistance, opacity: String.to_integer(opacity)}

    dbg(target)
    dbg(mapping_tools_form)

    # to reset stage on color change
    socket = case Enum.at(target,1) do
      "stripes_color" -> socket |> push_event("stripe_color", %{})
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
        |> push_event("localstorage", %{ input_stripes_color: mapping_tools_form["stripes_color"] })
      "output" ->
        socket
        |> assign(:mapping_output, to_form(mapping_changeset, id: :mapping_tools_form_output, as: :mapping_tools_form))
        |> assign(:side, "output")
        |> push_event("localstorage", %{ output_opacity: mapping_tools_form["opacity"] })
        |> push_event("localstorage", %{ output_stripes_color: mapping_tools_form["stripes_color"] })
      "uniform" ->
        socket
        |> assign(:mapping_uniform, to_form(mapping_changeset, id: :mapping_tools_form_uniform, as: :mapping_tools_form))
        |> assign(:side, "uniform")
        |> push_event("localstorage", %{ uniform_opacity: mapping_tools_form["opacity"] })
        |> push_event("localstorage", %{ uniform_stripes_color: mapping_tools_form["stripes_color"] })
      _ ->
        socket
    end

    {:noreply,
      socket
    }
  end

  def handle_event("phx:move-stripe", %{"direction" => direction, "value" => value}, socket) do

    dbg(direction)

    {:noreply,
      socket
    }
  end

  def handle_event("refresh", %{"value" => ""} = _referer, socket) do

    Lightwarrior.HyperionApi.refresh_data()

    {:noreply,
      socket
      |> assign(:selected, nil)
      |> assign(:state, Lightwarrior.HyperionApi.get_data())
      |> push_patch(to: ~p"/hyperion/")
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



  def handle_event("phx:init-input_stripes_color", params, socket) do
    #dbg(params)

    params_concat = Map.merge(socket.assigns.mapping_input.source.params, %{"stripes_color" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)

    #dbg(socket.assigns.mapping_input.source.params)
    {:noreply,
      socket
      |> assign(:mapping_input, to_form(mapping_changeset, id: :mapping_tools_form_input, as: :mapping_tools_form))
    }
  end

  def handle_event("phx:init-output_stripes_color", params, socket) do
    dbg(params)
    #mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, %{"side" => "output", "stripes_color" => params})
    params_concat = Map.merge(socket.assigns.mapping_output.source.params, %{"stripes_color" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    {:noreply,
      socket
      |> assign(:mapping_output, to_form(mapping_changeset, id: :mapping_tools_form_output, as: :mapping_tools_form))
    }
  end

  def handle_event("phx:init-uniform_stripes_color", params, socket) do
    dbg(params)
    #mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, %{"side" => "uniform", "stripes_color" => params})
    params_concat = Map.merge(socket.assigns.mapping_uniform.source.params, %{"stripes_color" => params})
    mapping_changeset = MappingMenueForm.changeset(%MappingMenueForm{}, params_concat)
    {:noreply,
      socket
      |> assign(:mapping_uniform, to_form(mapping_changeset, id: :mapping_tools_form_uniform, as: :mapping_tools_form))
    }
  end



  def handle_event("phx:last_open_tab", params, socket) do
    dbg(params)
    socket = case params do
      %{"tab" => tab, "value" => _value} ->
          dbg(tab)
          socket
          |> assign(:side, tab)
          |> push_event("localstorage", %{ last_open_tab: tab})
          |> push_event("tabchange", %{ last_open_tab: tab})
          #|> assign(:selected, nil)
          #|> push_patch(to: ~p"/hyperion/")
      %{"last_open_tab" => tab} ->
          dbg(tab)
          socket
          |> assign(:side, tab)
       _ -> socket
    end

    {:noreply,
      socket
    }
  end

  def handle_event("phx.toggle_autosave", params, socket) do
    dbg(params)
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
    dbg(params)
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

  def handle_event("phx:select_instance", %{"value" => value} = param, socket) do
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

    instances_data_pixel = Map.get(socket.assigns.instances_data_pixel_map, socket.assigns.side)

    #dbg(Enum.fetch!(socket.assigns.stripes, socket.assigns.selected))
    selected = Enum.fetch!(socket.assigns.state.stripes_with_config, socket.assigns.selected)
    num_leds = selected.config["info"]["device"]["hardwareLedCount"]

    #lightwarrior.ex ;)
    updated = Lightwarrior.update_selected_stripe_data_pixel(
      num_leds,
      instances_data_pixel,
      socket.assigns.selected,
      points
    )

    instances_data_pixel_map = socket.assigns.instances_data_pixel_map
                               |> Map.replace(socket.assigns.side, updated)

    {:noreply,
      socket
      |> assign(:instances_data_pixel_map, instances_data_pixel_map)
      |> push_event("instances-data-pixel", %{instance_data_pixel: updated})
      |> push_event("change_mapping", %{})
    }
  end

  @impl true
  def handle_event("save", %{"value" => ""} = params, socket) do
    dbg("save stripe #{socket.assigns.selected}")

    instances_data_pixel = Map.get(socket.assigns.instances_data_pixel_map, socket.assigns.side)

    leds = Helper.leds_to_coordinates!(
      Enum.fetch!(instances_data_pixel, socket.assigns.selected),
      socket.assigns.mapping_container_size
    )

    selected_config = Enum.fetch!(socket.assigns.state.stripes_with_config, socket.assigns.selected)
    selected_config_updated = Map.replace(selected_config, "leds", leds)
    stripes_with_config = List.update_at(socket.assigns.state.stripes_with_config, socket.assigns.selected, fn stripe -> Map.put(stripe, "config", %{"info" => selected_config_updated}) end)
    dbg(selected_config_updated)

    case socket.assigns.side do
      "input" ->
                        # Dump stripes_with_config to file for debugging
                        backup = %{"stripes_with_config" => stripes_with_config}
                        File.write!("./mappings/stripes_config_dump.json", Jason.encode!(backup, pretty: true))
                        # update ossia via osc messages
                        Lightwarrior.update_stripe_ossia(leds, socket.assigns.selected, socket.assigns.sc_pid)
      "output" ->
        dbg("save output")
      "uniform" ->
        dbg("save uniform")
      _ ->
        dbg("save nothing")
    end

    {:noreply, socket
      |> push_patch(to: ~p"/hyperion/#{socket.assigns.selected}/edit", replace: true)
    }
  end

  def handle_event("phx:get-stripes-config", _referer, socket) do
    {:noreply,
      socket
    }
  end

  def handle_event("phx:set-even", %{"value" => value} = param, socket) do
    {:noreply,
      socket
    }
  end

  def handle_event("phx:toggle-distance-lock", %{"side" => side, "value" => _value} = param, socket) do
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
    #dbg(socket.assigns.state.stripes_with_config)

    if socket.assigns.state.stripes_with_config do
      instances_data_pixel = Helper.leds_to_pixel!(socket.assigns.state.stripes_with_config, mapping_container_size)
      #dbg(instances_data_pixel)
      dbg(socket.assigns.side)

      instances_data_pixel_map = socket.assigns.instances_data_pixel_map
      |> Map.replace(socket.assigns.side, instances_data_pixel)

      #%{"input" => input, "output" => output, "uniform" => uniform} = socket.assigns.instances_data_pixel
      #dbg(output)

      {:noreply, socket
        |> assign(:mapping_container_size, mapping_container_size)
        |> assign(:instances_data_pixel_map, instances_data_pixel_map)
        |> push_event("instances-data-pixel", %{instance_data_pixel: instances_data_pixel})
      }
    else
      {:noreply, socket
        |> assign(:mapping_container_size, mapping_container_size)
      }
    end
  end

end
