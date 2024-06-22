defmodule Lightwarrior.Hyperion.LedInstanceDetails do
  use LightwarriorWeb, :live_component

  @impl true
  def render(assigns) do
    ~H"""
      <div id={@id} class={@class}>
        <div class="flex flex-row gap-3 w-fit antialiased rounded-tl-xl rounded-tr-xl bg-zinc-50 dark:bg-zinc-900 pr-4 pl-4 ring-1 ring-gray-800/5">
        <h2>Selected: <%= @name %> </h2>
          <button phx-click={
              JS.toggle_class("rotate-180", to: "##{@id} .chevron", time: 300)
              |> JS.toggle_class("scale-y-0", to: "##{@id} .content", time: 300)
              |> JS.toggle_class("h-0", to: "##{@id} .content", time: 300)
            }
            class="cursor-pointer"
          >
            <.icon name="hero-chevron-down" class="chevron h-3 w-3 transition-all duration-300" />
          </button>
        </div>
        <div class="content flex flex-wrap antialiased rounded-tr-xl rounded-br-xl rounded-bl-xl bg-zinc-50 dark:bg-zinc-900 p-5 ring-1 ring-gray-800/5 shadow-xl">
        <.form phx-change="size_change">
          <table class="w-full table-fixed text-sm text-left rtl:text-right text-gray-500 dark:text-gray-400">
            <tbody>
              <tr>
                <td>IP</td>
                <td>
                  <.input type="text" name="ip" id="ip" class="" value={@stripe_data["device"]["host"]} />
                </td>
              </tr>
              <tr>
                <td>LED´s</td>
                <td><%= @stripe_data["device"]["hardwareLedCount"] %></td>
              </tr>
              <tr>
                <td>Copy Mapping</td>
                <td></td>
              </tr>
              <tr>
                <td>Smoothing</td>
                <td>
                <!-- tailwind ui components -->
                  <div class="flex flex-row gap-3">
                    <.simple_toggle
                      title={"Smoothing"}
                      switch={@stripe_data["smoothing"]["enable"]}
                      action={
                        JS.push("phx:stripe_change", value: %{smoothing: !@stripe_data["smoothing"]["enable"]})
                      }
                    >
                    </.simple_toggle>
                    <.set_global
                      title="set global smooting"
                      action={
                        JS.push("phx:global_change", value: %{smoothing: @stripe_data["smoothing"]["enable"]})
                      }
                    >
                      <.icon name="hero-globe-alt" class="h-3 w-3" />
                    </.set_global>
                  </div>
                </td>
              </tr>
              <tr>
                <td>LED Size Pixel</td>
                <td>
                  <div class="w-55 flex">
                    <div class="m-2">
                      <%= Float.round(@ledSize.pixel.width, 3) %>
                    </div>
                    <span class="inline-block m-1 text-lg align-middle">/</span>
                    <div class="m-2">
                    <%= Float.round(@ledSize.pixel.height, 3) %>
                    </div>
                  </div>
                </td>
              </tr>
              <tr>
                <td>LED size Hyperion</td>
                <td>
                  <div class="w-55 flex">
                    <div class="overflow-hidden m-2">
                      <%= Float.round(@ledSize.point.width, 5) %>
                    </div>
                    <span class="inline-block m-1 text-lg align-middle">/</span>
                    <div class="overflow-hidden m-2">
                      <%= Float.round(@ledSize.point.height, 5) %>
                    </div>
                  </div>
                </td>
              </tr>

            </tbody>
          </table>
              <div class="p-5 gap-3 flex flew-row">
              <button phx-disable-with="Saving..." class="px-4 py-2 font-semibold text-sm bg-cyan-500 text-white rounded-full shadow-sm" phx-click="save" >
                Save
              </button>
              <button phx-disable-with="Saving Global ..." class="px-4 py-2 font-semibold text-sm bg-cyan-500 text-white rounded-full shadow-sm" phx-click="save-global" >
                Save Global
              </button>
              </div>
        </.form>



          <div>
              <pre class="hidden">
                <%=

                    pretty_json = Jason.encode!(@stripe_data, pretty: true)
                    raw(pretty_json)

                %>
            </pre>
          </div>

        </div>

      </div>
    """
  end

  @impl true
  #def update(%{test: test} = assigns, socket) do
  def update(assigns, socket) do
    dbg(assigns)
    #changeset = Play.change_test(test)

    {:ok,
     socket
     #|> assign(assigns)
     #|> assign_form(changeset)
    }
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})

end
