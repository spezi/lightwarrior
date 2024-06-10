defmodule Lightwarrior.Hyperion.LedInstanceDetails do
  use LightwarriorWeb, :live_component

  alias Lightwarrior.Hyperion
  alias Lightwarrior.Helper

  @impl true
  def render(assigns) do
    ~H"""
      <div>

      </div>
    """
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})

end
