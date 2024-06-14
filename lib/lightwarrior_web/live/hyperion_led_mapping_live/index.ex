defmodule LightwarriorWeb.HyperionLEDMappingLive.Index do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Hyperion
  #alias Lightwarrior.Hyperion.HyperionLEDMapping
  alias Phoenix.LiveView.AsyncResult
  alias Lightwarrior.Helper

  @impl true
  def mount(_params, _session, socket) do

    #dbg(fetched_all_stripes_config)

    {:ok, serverinfo} = Hyperion.get_serverinfo()
    {:ok, stripes} = Hyperion.collect_stripes(serverinfo)
    {:ok, stripes_with_config } = Hyperion.get_all_stripes_config(stripes)

    {:ok, socket
      |> assign(:debug, false)
      |> assign(:selected, nil)
      |> assign(:mapping_container_size, nil)
      |> assign(:leds_pixel, nil)
      |> assign(:serverinfo, serverinfo)
      |> assign(:stripes, stripes_with_config)
    }
  end

  @impl true
  def handle_params(params, _url, socket) do

    socket = case Map.has_key?(params, "selected") do
      true ->
        assign(socket, :selected, Map.get(params, "selected"))
        |> push_event("stripe-select", %{select: Map.get(params, "selected")})
        _ -> socket
    end

    {:noreply, socket
      |> assign(:page_title, page_title(socket.assigns.live_action))
      #|> assign(:leds_pixel, Helper.leds_to_pixel!(fetched_all_stripes_config, socket.assigns.mapping_container_size))
    }
  end


###########################################################################################################

  def handle_event("phx:mapping-size", %{"width" => width, "height" => height}, socket) do
    mapping_container_size = %{
      width: width,
      height: height
    }
    IO.puts("size: #{inspect(mapping_container_size)}")
    {:noreply, socket
      |> assign(:mapping_container_size, mapping_container_size)
      |> assign(:leds_pixel, Helper.leds_to_pixel!(socket.assigns.stripes, mapping_container_size))
    }
  end

  def handle_event("phx:get_selected_leds_pixel", %{"width" => width, "height" => height}, socket) do
    mapping_container_size = %{
      width: width,
      height: height
    }

    dbg(socket.assigns.selected)
    IO.puts("size_for_select: #{inspect(mapping_container_size)} #{socket.assigns.selected}")
    leds_pixel = Helper.leds_to_pixel!(socket.assigns.stripes, mapping_container_size)

    #dbg(Enum.fetch!(leds_pixel, String.to_integer(socket.assigns.selected)))

    {:noreply, socket
      |> assign(:mapping_container_size, mapping_container_size)
      |> assign(:leds_pixel, leds_pixel)
      |> push_event("stripe-ready", %{select: socket.assigns.selected, leds_pixel: Enum.fetch!(leds_pixel, String.to_integer(socket.assigns.selected))})
    }
  end

  def handle_event("phx:stripe_change", bounds, socket) do
    # Handle the size information as needed
    #IO.puts("Div width: #{width}, height: #{height}")
    dbg(bounds)
    #lightwarrior.ex ;)
    #lightwarrior.ex ;)
    updated = Lightwarrior.update_selected_stripe_data_pixel(
      socket.assigns.leds_pixel,
      String.to_integer(socket.assigns.selected),
      bounds
    )
    {:noreply, socket
      |> assign(:leds_pixel, updated)
      #|> push_event("stripe-select", updated)
    }
  end

  def handle_event("save", %{ "value" => value },  socket) do
    # Handle the size information as needed
    #IO.puts("Div width: #{width}, height: #{height}")
    dbg("save")

    dbg(Helper.leds_to_coordinates!(
      Enum.fetch!(socket.assigns.leds_pixel, String.to_integer(socket.assigns.selected)),
      socket.assigns.mapping_container_size
    ))

    dbg(socket.assigns.selected)

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
  def handle_event("phx:debug", %{"debug" => debug}, socket) do
    case debug do
      nil -> {:noreply, socket}
      "false" -> {:noreply, assign(socket, debug: false)}
      "true" -> {:noreply, assign(socket, debug: true)}
    end

  end

  defp page_title(:index), do: "Hyperion Led Mappings"
  defp page_title(:edit), do: "Hyperion Led Mappings Edit"

end
