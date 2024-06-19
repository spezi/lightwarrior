defmodule Lightwarrior.Hyperion.LedInstanceMenue do
  use LightwarriorWeb, :live_component

  @impl true
  def render(assigns) do
    ~H"""
      <div class="rounded-xl">
        <.header class="w-fit antialiased rounded-tl-xl rounded-tr-xl bg-zinc-50 dark:bg-zinc-900 pr-4 pl-4 ring-1 ring-gray-800/5">
          <h2>Instances</h2>
        </.header>
        <div class="flex flex-wrap antialiased rounded-tr-xl rounded-br-xl rounded-bl-xl bg-zinc-50 dark:bg-zinc-900 p-5 ring-1 ring-gray-800/5 shadow-xl">
            <div :for={instance <- @serverinfo["info"]["instance"]} class="">
              <.led_instance title={instance["friendly_name"]} status={instance["running"]} instance={instance}/>
            </div>
        </div>
      </div>
    """
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})

end
