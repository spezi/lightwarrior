defmodule LightwarriorWeb.IPlayerLive.Show do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Imageplayer

  @impl true
  def mount(_params, _session, socket) do
    {:ok, socket}
  end

  @impl true
  def handle_params(%{"id" => id}, _, socket) do
    {:noreply,
     socket
     |> assign(:page_title, page_title(socket.assigns.live_action))
     |> assign(:i_player, Imageplayer.get_i_player!(id))}
  end

  defp page_title(:show), do: "Show I player"
  defp page_title(:edit), do: "Edit I player"
end
