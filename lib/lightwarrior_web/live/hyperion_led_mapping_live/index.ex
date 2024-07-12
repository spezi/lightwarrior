defmodule LightwarriorWeb.HyperionLEDMappingLive.Index do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Hyperion
  #alias Lightwarrior.Hyperion.HyperionLEDMapping
  alias Phoenix.LiveView.AsyncResult
  alias Lightwarrior.Helper

  @impl true
  def mount(_params, _session, socket) do

    #dbg(fetched_all_stripes_config)

    { serverinfo, stripes_with_config } = case Hyperion.get_serverinfo() do
      {:ok, serverinfo} ->

        {:ok, stripes} = Hyperion.collect_stripes(serverinfo)
        {:ok, stripes_with_config } = Hyperion.get_all_stripes_config(stripes)
        {serverinfo, stripes_with_config}

      {:error, :econnrefused} ->
        {nil, []}
    end

    socket = case serverinfo do
      nil -> socket |> put_flash(:error, "connection to hyperion failed")
        _ -> socket
    end


    {:ok, socket
      |> assign(:debug, false)
      |> assign(:selected, nil)
      |> assign(:mapping_container_size, nil)
      |> assign(:leds_pixel, [])
      |> assign(:serverinfo, serverinfo)
      |> assign(:stripes, stripes_with_config)
      |> assign(:lockdistance, true)
      |> assign(:stripelength, 0)
    }
  end

  @impl true
  def handle_params(params, _url, socket) do

    socket = case Map.has_key?(params, "selected") do
      true ->
        selected = String.to_integer(Map.get(params, "selected"))
        stripe = Enum.fetch!(socket.assigns.stripes, selected)
        Hyperion.switch_instance(stripe)
        assign(socket, :selected, selected)
        |> push_event("stripe-select", %{select: Map.get(params, "selected")})
        _ -> socket
    end

    socket = socket
      |> push_event("stripes-ready", %{ stripes: socket.assigns.stripes})

    {:noreply, socket
      |> assign(:page_title, page_title(socket.assigns.live_action))
      #|> push_event("stripes-ready", socket.assigns.stripes)
    }
  end


###########################################################################################################

def handle_event("phx:stripe_change", params, socket) do
  # Handle the size information as needed
  #IO.puts("Div width: #{width}, height: #{height}")

  dbg(params)

  stripe = Enum.fetch!(socket.assigns.stripes, socket.assigns.selected)

  stripe = case params do
    %{"smoothing" => smoothing, "value" => ""} -> put_in(stripe, [:config, "info", "smoothing", "enable"], smoothing)
    _ -> stripe
  end

  stripes = List.replace_at(socket.assigns.stripes, socket.assigns.selected, stripe)


  {:noreply, socket
    |> assign(:stripes, stripes)
    |> push_event("stripes-ready", stripes)
    #|> push_event("stripe-select", updated)
  }
end

def handle_event("phx:global_change", params, socket) do
  # Handle the size information as needed
  #IO.puts("Div width: #{width}, height: #{height}")

  dbg(params)

  stripe = Enum.fetch!(socket.assigns.stripes, socket.assigns.selected)

  stripes = Enum.map_every(socket.assigns.stripes, 1, fn stripe ->
    #dbg(stripe)
    case params do
      %{"smoothing" => smoothing, "value" => ""} ->
          dbg(put_in(stripe, [:config, "info", "smoothing", "enable"], smoothing))
      _ -> stripe
    end
  end)


  {:noreply, socket
    |> assign(:stripes, stripes)
    |> push_event("stripes-ready", stripes)
    #|> put_flash(:info, "Values Global updated")
  }
end

  def handle_event("phx:mapping-size", %{"width" => width, "height" => height}, socket) do
    mapping_container_size = %{
      width: width,
      height: height
    }
    IO.puts("size: #{inspect(mapping_container_size)}")
    IO.puts("stripes: #{inspect(length(socket.assigns.stripes))}")

    # check if there be stripes
    check_stripes = case length(socket.assigns.stripes) > 0 do
      true -> true
      false -> false
    end

    # return map with success true without error or error with error
    check_error = case check_stripes do
      true ->
        first_stripe_in_response = Enum.fetch!(socket.assigns.stripes, 0)
        success = Map.get(first_stripe_in_response.config, "success")
        case success do
          true -> %{success: true, error: nil}
          false -> %{success: false, error: Map.get(first_stripe_in_response.config, "error")}
        end
      false ->

        %{success: false, error: Map.get(socket.assigns.flash, "error")}
    end

    # put flash on error to show error to user
    socket = case check_error do
      %{success: true, error: nil} -> socket
      %{success: false, error: error} ->
        # do not overflash existing flashes ;)
        case socket.assigns.flash do
          %{} -> put_flash(
                    socket,
                    :error,
                    "Failed to get Stripe Config. Hyperion Error:" <> error
                  )
          _ -> socket
        end
    end

    # convert leds to pixel if no errors
    leds_pixel = case check_error.success do
      true -> Helper.leds_to_pixel!(socket.assigns.stripes, mapping_container_size)
      false -> socket.assigns.leds_pixel
    end

    #dbg(Enum.fetch!(leds_pixel, socket.assigns.selected))
    socket = case socket.assigns.selected do
      nil -> socket
        _ ->
          step_h = Map.get(Enum.fetch!(Map.get(Enum.fetch!(leds_pixel, socket.assigns.selected), :leds), 0), "hmax") - Map.get(Enum.fetch!(Map.get(Enum.fetch!(leds_pixel, socket.assigns.selected), :leds), 0), "hmin")
          step_v = Map.get(Enum.fetch!(Map.get(Enum.fetch!(leds_pixel, socket.assigns.selected), :leds), 0), "vmax") - Map.get(Enum.fetch!(Map.get(Enum.fetch!(leds_pixel, socket.assigns.selected), :leds), 0), "vmin")
          socket
          |> push_event("set_step_horizontal", %{direction: "h", step: step_h})
          |> push_event("set_step_vertical", %{direction: "v", step: step_v})
    end


    {:noreply, socket
      |> assign(:mapping_container_size, mapping_container_size)
      |> assign(:leds_pixel, leds_pixel)
    }
  end

  def handle_event("phx:get_selected_leds_pixel", %{"width" => width, "height" => height}, socket) do
    mapping_container_size = %{
      width: width,
      height: height
    }

    #dbg(socket.assigns.selected)
    IO.puts("size_for_select: #{inspect(mapping_container_size)} #{socket.assigns.selected}")

    #dbg(Map.get(Enum.fetch!(socket.assigns.stripes, 0), :config))
    dbg(mapping_container_size)


    #dbg(Enum.fetch!(leds_pixel, socket.assigns.selected))

    socket = case length(socket.assigns.leds_pixel) > 0 do
      true -> socket
        |> push_event("stripe-ready", %{select: socket.assigns.selected, leds_pixel: Enum.fetch!(socket.assigns.leds_pixel, socket.assigns.selected)})
      false -> socket
    end

    {:noreply, socket
      |> assign(:mapping_container_size, mapping_container_size)
      #|> assign(:leds_pixel, leds_pixel)
    }
  end

  def handle_event("phx:stripe_change_mapping", points, socket) do
    # Handle the size information as needed
    #IO.puts("Div width: #{width}, height: #{height}")

    dbg(points)
    points = Helper.string_keys_to_atom_keys(points)

    #dbg(Enum.fetch!(socket.assigns.stripes, socket.assigns.selected))
    stripe = Enum.fetch!(socket.assigns.stripes, socket.assigns.selected)
    num_leds = stripe.config["info"]["device"]["hardwareLedCount"]

    #lightwarrior.ex ;)
    updated = Lightwarrior.update_selected_stripe_data_pixel(
      num_leds,
      socket.assigns.leds_pixel,
      socket.assigns.selected,
      points
    )
    #dbg(updated)

    {:noreply, socket
      |> assign(:leds_pixel, updated)
      #|> push_event("stripe-select", updated)
    }
  end

  def handle_event("save", %{ stripe_config: stripe_config, form: form },  socket) do
    # Handle the size information as needed
    #IO.puts("Div width: #{width}, height: #{height}")
    dbg("save")


    {:noreply, socket
      #|> assign(:leds_pixel, updated)
      #|> push_event("stripe-select", updated)
    }
  end

  def handle_event("save", %{ "value" => value },  socket) do
    # Handle the size information as needed
    #IO.puts("Div width: #{width}, height: #{height}")
    dbg("save")

    leds = Helper.leds_to_coordinates!(
      Enum.fetch!(socket.assigns.leds_pixel, socket.assigns.selected),
      socket.assigns.mapping_container_size
    )

    #dbg(Enum.fetch!(socket.assigns.stripes, socket.assigns.selected))
    stripe = Enum.fetch!(socket.assigns.stripes, socket.assigns.selected)
    stripe_config = Map.get(Map.get(stripe, :config), "info")
    #dbg(stripe_config)
    stripe_config_updated = Map.replace(stripe_config, "leds", leds)

    #dbg(stripe_config_updated)
    #save = %{"success"  => true}
    save = Hyperion.save_current_config(stripe_config_updated)
    dbg(save)
    socket = case save do
      %{"success" => true } -> put_flash(socket, :info, "Stripe updated")
      %{"success" => false } -> put_flash(socket, :error, "Failed to update Stripe")
    end

    #{:ok, stripes} = Hyperion.collect_stripes(socket.assigns.serverinfo)
    #{:ok, stripes_with_config } = Hyperion.get_all_stripes_config(stripes)

    {:noreply, socket
      #|> assign(:stripes, stripes_with_config)
      #|> push_event("stripes-ready", %{ stripes: stripes_with_config})
      #|> assign(:leds_pixel, updated)
      #|> push_event("stripe-select", updated)
    }
  end

  def handle_event("save-global", %{ "value" => value },  socket) do
    # Handle the size information as needed
    #IO.puts("Div width: #{width}, height: #{height}")
    dbg("save global")

    stripes = Enum.map_every(socket.assigns.stripes, 1, fn stripe ->
      #dbg(stripe)
      dbg(Hyperion.switch_instance(stripe))
      save = Hyperion.save_current_config(Map.get(Map.get(stripe, :config), "info"))
      dbg(save)
      socket = case save do
        %{"success" => true } -> put_flash(socket, :info, "Stripe updated")
        %{"success" => false } -> put_flash(socket, :error, "Failed to update Stripe")
      end

    end)



    {:noreply, socket
      #|> assign(:leds_pixel, updated)
      #|> push_event("stripe-select", updated)
    }
  end

  @impl true
  def handle_event("phx:debug", %{"debug" => debug, "value" => _value}, socket) do
    #{:noreply, assign(socket, debug: debug)}
    {:noreply, socket
      |> assign(debug: debug)
      |> push_event("save-debug", %{debug: debug})
    }
  end

  @impl true
  def handle_event("phx:lock-distance", _params,  socket) do
    {:noreply,
      socket
      |> assign(:lockdistance, true)
      |> push_event("lock-distance", %{})
    }
  end

  @impl true
  def handle_event("phx:unlock-distance", _params,  socket) do
    {:noreply,
      socket
      |> assign(:lockdistance, false)
      |> push_event("unlock-distance", %{})
    }
  end

  @impl true
  def handle_event("phx:set-even-x", _params,  socket) do
    {:noreply,
      socket
      |> push_event("set-even-x", %{})
    }
  end

  @impl true
  def handle_event("phx:set-even-y", _params,  socket) do
    {:noreply,
      socket
      |> push_event("set-even-y", %{})
    }
  end

  @impl true
  def handle_event("phx:move-stripe", params,  socket) do
    dbg(params)
    {:noreply,
      socket
      |> push_event("move-stripe", %{direction: params["direction"]})
    }
  end



  @impl true
  def handle_event("phx:initial-distance", %{"initialDistance" => length},  socket) do
    #dbg(length)
    {:noreply,
      socket
      |> assign(:stripelength, length)
    }
  end

  def handle_event("set_stripe_length", %{"_target" => ["length"], "length" => stripe_length}, socket) do
    #dbg(Float.parse(stripe_length))
    # handle regular form change
    dbg(Float.parse(stripe_length))
    #{length, rest} = Float.parse(stripe_length)
    case Float.parse(stripe_length) do
      {length, rest} ->
        {:noreply,
          socket
          |> assign(:stripelength, length)
          |> push_event("set_stripe_length", %{length: length})
        }
      :error ->
        {:noreply, socket}
    end
    #dbg(length)
  end

  def handle_event("set_stripe_length", %{"_target" => ["length_copy_select"], "length_copy_select" => instance}, socket) do
    #dbg(Integer.parse(instance))
    #dbg(socket.assigns.leds_pixel)
    {instance_num , ""} = Integer.parse(instance)
    source_stripe = Enum.fetch!(socket.assigns.leds_pixel, instance_num)
    #dbg(source_stripe)

    {:noreply,
      socket
      |> push_event("copy_from_instance", %{instance: instance, stripe: source_stripe})
    }
  end

  def handle_event("set_stripe_length", %{"_target" => ["step_horizontal"], "step_horizontal" => step}, socket) do
    dbg(step)
    {:noreply,
      socket
      |> push_event("set_step_horizontal", %{step: step })
    }
  end

  def handle_event("set_stripe_length", %{"_target" => ["step_vertical"], "step_vertical" => step}, socket) do
    dbg(step)
    {:noreply,
      socket
      |> push_event("set_step_vertical", %{step: step })
    }
  end

  @impl true
  def handle_event("phx:debug", %{"debug" => debug}, socket) do
    case debug do
      nil -> {:noreply, socket}
      "false" -> {:noreply, assign(socket, debug: false)}
      "true" -> {:noreply, assign(socket, debug: true)}
    end

  end

  @impl true
  def handle_info({Lightwarrior.Hyperion.StripeFormComponent, {:save, stripe_config, form}}, socket) do
    #dbg(stripe_config)
    #dbg(form.valid?())

    leds = Helper.leds_to_coordinates!(
      Enum.fetch!(socket.assigns.leds_pixel, socket.assigns.selected),
      socket.assigns.mapping_container_size
    )

    stripe = Enum.fetch!(socket.assigns.stripes, socket.assigns.selected)
    stripe_config_updated = Map.replace(stripe_config, "leds", leds)

    #dbg(stripe_config_updated)
    #save = %{"success"  => true}
    save = Hyperion.save_current_config(stripe_config_updated)
    dbg(save)
    socket = case save do
      %{"success" => true } -> put_flash(socket, :info, "Stripe updated")
      %{"success" => false } -> put_flash(socket, :error, "Failed to update Stripe")
    end



    {:ok, stripes} = Hyperion.collect_stripes(socket.assigns.serverinfo)
    {:ok, stripes_with_config } = Hyperion.get_all_stripes_config(stripes)

    {:noreply, socket
      #|> stream_insert(:tests, test)
      #|> push_patch(to: socket.request_path)
      #|> push_redirect(socket, to: Routes.live_path(socket, __MODULE__, 17))
      #|> push_redirect(socket, to: Routes.live_path(socket, "/hyperion/ledmappings/:selected/edit", 1))
      |> assign(:stripes, stripes_with_config)
      |> push_patch(to: ~p"/hyperion/ledmappings/#{socket.assigns.selected}/edit", replace: true)
      |> push_event("stripes-ready", %{ stripes: stripes_with_config})

    }
  end

  @impl true
  def handle_info({Lightwarrior.Hyperion.StripeFormComponent, {:error, action, error}}, socket) do

    {:noreply,
      socket
      |> put_flash(:error, error)
      #|> push_patch(to: socket.assigns.patch)
    }
  end

  defp page_title(:index), do: "Hyperion Led Mappings"
  defp page_title(:edit), do: "Hyperion Led Mappings Edit"

end
