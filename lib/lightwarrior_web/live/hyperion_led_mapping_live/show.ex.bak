defmodule LightwarriorWeb.HyperionLEDMappingLive.Show do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Hyperion

  @impl true
  def mount(_params, _session, socket) do
    {:ok, socket}
  end

  @impl true
  def handle_params(%{"id" => id}, _, socket) do
    {:noreply,
     socket
     |> assign(:page_title, page_title(socket.assigns.live_action))
     |> assign(:hyperion_led_mapping, Hyperion.get_hyperion_led_mapping!(id))}
  end

  defp page_title(:show), do: "Show Hyperion led mapping"
  defp page_title(:edit), do: "Edit Hyperion led mapping"
end
