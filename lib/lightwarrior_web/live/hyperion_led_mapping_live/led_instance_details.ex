defmodule Lightwarrior.Hyperion.LedInstanceDetails do
  use LightwarriorWeb, :live_component

  @impl true
  def render(assigns) do
    ~H"""
      <div id={@id} class={@class}>
        <.header class="w-fit antialiased rounded-tl-xl rounded-tr-xl bg-zinc-50 dark:bg-zinc-900 pr-4 pl-4 ring-1 ring-gray-800/5">
          <h2>Selected: <%= @stripe_data.friendly_name %> </h2>
        </.header>
        <div class="flex flex-wrap antialiased rounded-tr-xl rounded-br-xl rounded-bl-xl bg-zinc-50 dark:bg-zinc-900 p-5 ring-1 ring-gray-800/5 shadow-xl">
            <h3>

            </h3>
            <div>
              <pre>
                <%=
                  pretty_json = Jason.encode!(@stripe_data.config, pretty: true)
                  raw(pretty_json)
                %>
              </pre>
            </div>
        </div>

      </div>
    """
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})

end
