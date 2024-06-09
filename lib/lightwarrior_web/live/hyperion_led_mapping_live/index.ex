defmodule LightwarriorWeb.HyperionLEDMappingLive.Index do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Hyperion
  alias Lightwarrior.Hyperion.HyperionLEDMapping

  @impl true
  def mount(_params, _session, socket) do

    {:ok,
      stream(socket, :hyperionledmappings, Hyperion.list_hyperionledmappings())
      |> assign(:debug, false)
    }
  end

  @impl true
  def handle_params(params, url, socket) do
    #{:noreply, apply_action(socket, socket.assigns.live_action, params)}
    #dbg(params)
    %{path: path} = URI.parse(url)

    debug = Map.has_key?(params, "debug")
    {:noreply, socket
      |> assign(:debug, debug)
      |> assign(:path, path)
    }

  end

  def handle_event("phx:debug", %{"debug" => debug, "value" => _value}, socket) do
    dbg(socket.assigns.path)
    path = if debug, do: socket.assigns.path <> "?debug", else: socket.assigns.path
    {:noreply, socket
      |> push_patch(to: path)
    }
  end

end
