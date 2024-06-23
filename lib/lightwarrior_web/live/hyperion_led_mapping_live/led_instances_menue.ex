defmodule Lightwarrior.Hyperion.LedInstanceMenue do
  use LightwarriorWeb, :live_component

  @impl true
  def render(assigns) do
    ~H"""
      <div id="led_instances_menue" class="rounded-xl">
        <div class="flex flex-row gap-3 w-fit antialiased rounded-tl-xl rounded-tr-xl bg-zinc-50 dark:bg-zinc-900 pr-4 pl-4 ring-1 ring-gray-800/5">
          <h2>Instances</h2>
          <button phx-click={
              JS.toggle_class("rotate-180", to: "#led_instances_menue .chevron", time: 300)
              |> JS.toggle_class("scale-y-0", to: "#led_instances_menue .content", time: 300)
              |> JS.toggle_class("h-0", to: "#led_instances_menue .content", time: 300)
            }
            class="cursor-pointer"
          >
            <.icon name="hero-chevron-down" class="chevron h-3 w-3 transition-all duration-300" />
          </button>
        </div>
        <div class="content overflow-hidden shadow-xl origin-top transition-all duration-300 antialiased rounded-tr-xl rounded-br-xl rounded-bl-xl bg-zinc-50 dark:bg-zinc-900 ring-1 ring-gray-800/5">
          <div class="flex flex-wrap p-5">
              <div :for={instance <- @serverinfo["info"]["instance"]} class="">
                <.led_instance title={instance["friendly_name"]} status={instance["running"]} instance={instance} selected={@selected}/>
              </div>
          </div>
        </div>
      </div>
    """
  end

  @impl true
  #def update(%{test: test} = assigns, socket) do
  def update(assigns, socket) do
    #dbg(assigns)
    #changeset = Play.change_test(test)

    {:ok,
     socket
     |> assign(assigns)
     #|> assign_form(changeset)
    }
  end


  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})

end
