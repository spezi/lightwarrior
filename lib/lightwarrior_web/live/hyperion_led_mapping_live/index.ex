defmodule LightwarriorWeb.HyperionLEDMappingLive.Index do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Hyperion
  alias Lightwarrior.Hyperion.HyperionLEDMapping
  #alias Phoenix.LiveView.AsyncResult

  @impl true
  def mount(_params, _session, socket) do

    {:ok,
      stream(socket, :stripes, [])
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
      |> assign_async(:serverinfo, fn -> {:ok, serverinfo } = Hyperion.get_serverinfo(); {:ok, %{serverinfo: serverinfo}} end)
      |> assign_async(:current_config, fn -> {:ok, current_config } = Hyperion.get_current_config(); {:ok, %{current_config: current_config}}  end)
    }
  end

  defp apply_action(socket, :select, %{"id" => id}) do
    dbg("select instance")
    socket
    #|> assign(:page_title, "Edit Mapping")
    #|> assign(:mapping, Hyperion.get_mapping!(id))
  end

  def handle_event("phx:debug", %{"debug" => debug, "value" => _value}, socket) do
    dbg(socket.assigns.path)
    path = if debug, do: socket.assigns.path <> "?debug", else: socket.assigns.path
    {:noreply, socket
      |> push_patch(to: path)
    }
  end

end
