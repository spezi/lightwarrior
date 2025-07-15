defmodule LightwarriorWeb.HyperionConfigLive.Index do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Hyperion
  alias Lightwarrior.Helper

  @impl true
  def mount(_params, _session, socket) do

    #if connected?(socket) do
    #  Phoenix.PubSub.subscribe(Lightwarrior.PubSub, "state")
    #end

    {:ok,
     socket
     |> assign(:page_title, "Listing Hyperionconfigs")
     |> assign(:state, Lightwarrior.HyperionApi.get_data())
     |> assign(:selected, nil)
     |> assign(:debug, false)
     |> assign(:mapping_container_size, nil)
     #|> stream(:hyperionconfigs, Hyperion.list_hyperionconfigs())
    }
  end

  @impl true
  def handle_params(%{"id" => id}, _referer, socket) do
    {:noreply, socket}
  end

  def handle_event("refresh", %{"value" => ""} = _referer, socket) do

    Lightwarrior.HyperionApi.refresh_data()

    {:noreply,
      socket
      |> assign(:state, Lightwarrior.HyperionApi.get_data())
    }
  end

  def handle_event("toggle_debug", _referer, socket) do
    {:noreply,
      socket
      |> assign(:debug, !socket.assigns.debug)
    }
  end

  def handle_event("phx:get-stripes-config", _referer, socket) do
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

    instance_data_pixel = nil

    if socket.assigns.state.stripes_with_config do
      instance_data_pixel = Helper.leds_to_pixel!(socket.assigns.state.stripes_with_config, mapping_container_size)
      dbg(instance_data_pixel)
      {:noreply, socket
        |> assign(:mapping_container_size, mapping_container_size)
        |> push_event("instance-data-pixel", %{instance_data_pixel: instance_data_pixel})
      }
    else
      {:noreply, socket
        |> assign(:mapping_container_size, mapping_container_size)
      }
    end
  end

end
