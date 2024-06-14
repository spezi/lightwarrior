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
        <.form phx-change="size_change">
          <table class="w-full text-sm text-left rtl:text-right text-gray-500 dark:text-gray-400">
            <tbody>
              <tr>
                <td>LED´s</td>
                <td><%=  %></td>
              </tr>
              <tr>
                <td>LED size</td>
                <td>
                  <div class="w-55 flex">
                    <div class="">
                      <.input type="number" name="led_width" id="led_width" class="" value={1} />
                    </div>
                    <span class="inline-block pt-3 m-1 text-lg align-middle">/</span>
                    <div class="">
                      <.input type="number" name="led_height" id="led_height" class="" value={1} />
                    </div>
                  </div>
                </td>
              </tr>
              <tr>
                <td>LED size Hyperion</td>
                <td>
                  <div class="w-55 flex">
                    <div class="overflow-hidden pt-5">

                    </div>
                    <span class="inline-block pt-3 m-1 text-lg align-middle">/</span>
                    <div class="overflow-hidden pt-5">

                    </div>
                  </div>
                </td>
              </tr>

              <tr>
                <td>Point Distance</td>
                <td>
                    <%=  %>
                </td>
              </tr>

              <tr>
                <td>max size</td>
                <td>
                    <%=  %>
                </td>
              </tr>

            </tbody>
          </table>
              <button phx-disable-with="Saving..." class="px-4 py-2 font-semibold text-sm bg-cyan-500 text-white rounded-full shadow-sm" phx-click="save" phx-value-instance={@selected} >
                Save
              </button>
        </.form>
        </div>

      </div>
    """
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})

end
