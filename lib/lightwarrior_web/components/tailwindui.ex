defmodule LightwarriorWeb.TailwindUiComponents do
  use Phoenix.Component

  alias Phoenix.LiveView.JS
  import LightwarriorWeb.Gettext

  @doc """
  Simple toggle switch.

  TODO

  ## Examples

      <.simple_toggle title={"Title"}>

      </.simple_toggle>
  """


  attr :title, :string, required: true
  attr :switch, :boolean, default: false
  attr :action, :string, required: true

  slot :inner_block, doc: "the optional inner block that renders the label"


  def simple_toggle(assigns) do
    ~H"""
      <div class="flex items-center">
      <button
        type="button"
        class={"#{if @switch, do: 'bg-indigo-600', else: 'bg-gray-200 dark:bg-gray-600' } relative inline-flex h-6 w-11 flex-shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors duration-200 ease-in-out focus:outline-none focus:ring-2 focus:ring-indigo-600 focus:ring-offset-2"}
        role="switch"
        aria-checked={@switch}
        phx-click={@action}
      >
        <span class="sr-only">Toogle Debug</span>
        <!-- Enabled: "translate-x-5", Not Enabled: "translate-x-0" -->
        <span aria-hidden="true" class={"#{if @switch, do: 'translate-x-5', else: 'translate-x-0' } pointer-events-none inline-block h-5 w-5 translate-x-0 transform rounded-full bg-white shadow ring-0 transition duration-200 ease-in-out"}></span>
      </button>
      <%= render_slot(@inner_block) %>
      </div>
    """
  end
end
