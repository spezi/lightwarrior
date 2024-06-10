defmodule Lightwarrior.Hyperion.LedInstanceDetails do
  use LightwarriorWeb, :live_component

  alias Lightwarrior.Hyperion
  alias Lightwarrior.Helper

  @impl true
  def render(assigns) do
    ~H"""
      <div id={@id} class={@class}>
        <.header class="rounded-tr-xl mt-3 bg-white antialiased dark:bg-zinc-900 w-2/5 pl-4">
          <h2>Selected</h2>
        </.header>
        <div class="flex flex-wrap rounded-tr-xl rounded-br-xl rounded-bl-xl  shadow-xl ring-1 bg-white antialiased dark:bg-zinc-900 ring-gray-800/5 p-5">
          <%= Jason.encode!(@selected) %>
          <%= Jason.encode!(@stripe_data.instance) %>
          <%= #Jason.encode!(@stripe_data, pretty: true) %>
          <%= #Kernel.inspect(dbg(@selected_stripe_data)) %>
        </div>
      </div>
    """
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})

end
