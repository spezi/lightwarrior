defmodule LightwarriorWeb.HyperionConfigLive.Index do
  use LightwarriorWeb, :live_view

  alias Phoenix.LiveView.JS
  alias Lightwarrior.Hyperion
  alias Lightwarrior.Helper
  require Logger

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
     |> assign(:autosave, false)
     |> assign(:mapping_container_size, nil)
     #|> stream(:hyperionconfigs, Hyperion.list_hyperionconfigs())
    }
  end

  @impl true
  def handle_params(%{"id" => id}, _referer, socket) do
    dbg("selected: " <> id)
    {:noreply,
      socket
      |> assign(:selected, String.to_integer(id))
    }
  end

  @impl true
  def handle_params(%{}, _referer, socket) do
    {:noreply, socket}
  end

  def handle_event("refresh", %{"value" => ""} = _referer, socket) do

    Lightwarrior.HyperionApi.refresh_data()

    {:noreply,
      socket
      |> assign(:state, Lightwarrior.HyperionApi.get_data())
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

  def handle_event("phx.toggle_autosave", params, socket) do

    bool_value = case params do
      %{"value" => value} ->
        if value == "on" do true else false end
      %{} -> false
    end

    {:noreply,
      socket
      |> assign(:autosave, bool_value )
      |> push_event("localstorage_toggle", %{ autosave: bool_value})
    }
  end

  def handle_event("phx.toggle_debug", params, socket) do


    bool_value = case params do
      %{"value" => value} ->
        if value == "on" do true else false end
      %{} -> false
    end

    {:noreply,
      socket
      |> assign(:debug, bool_value )
      |> push_event("localstorage_toggle", %{ debug: bool_value})
      #|> JS.dispatch("click", to: ".nav")
      #|> JS.dispatch("phx:localstorage_save", data: %{ debug: !socket.assigns.debug })
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
      #dbg(instance_data_pixel)
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
