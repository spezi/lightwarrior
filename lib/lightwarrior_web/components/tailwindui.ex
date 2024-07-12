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
        <span class="sr-only"><%= @title %></span>
        <!-- Enabled: "translate-x-5", Not Enabled: "translate-x-0" -->
        <span aria-hidden="true" class={"#{if @switch, do: 'translate-x-5', else: 'translate-x-0' } pointer-events-none inline-block h-5 w-5 translate-x-0 transform rounded-full bg-white shadow ring-0 transition duration-200 ease-in-out"}></span>
      </button>
      <%= render_slot(@inner_block) %>
      </div>
    """
  end

  def set_global(assigns) do
    ~H"""
      <button
        title={@title}
        phx-click={@action}
        type="button"
        class="rounded bg-indigo-500 px-2 py-1 text-sm font-semibold text-white shadow-sm hover:bg-indigo-400 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500"
        >
        <%= render_slot(@inner_block) %>
      </button>
    """
  end

  attr :lockdistance, :boolean, default: true

  def mapping_menue(assigns) do
    ~H"""

      <button
        :if={!@lockdistance}
        title="lock distance"
        type="button"
        phx-click="phx:lock-distance"
        class="rounded bg-zinc-300 px-2 py-1 text-sm font-semibold text-white shadow-sm hover:bg-zinc-400 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500"
        >
        <svg class="h-6 w-6 text-zinc-500"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <rect x="5" y="11" width="14" height="10" rx="2" />  <circle cx="12" cy="16" r="1" />  <path d="M8 11v-4a4 4 0 0 1 8 0v4" /></svg>
      </button>
      <button
        :if={@lockdistance}
        title="unlock distance"
        type="button"
        phx-click="phx:unlock-distance"
        class="rounded bg-zinc-300 px-2 py-1 text-sm font-semibold text-white shadow-sm hover:bg-zinc-400 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500"
        >
        <svg class="h-6 w-6 text-zinc-500"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <rect x="5" y="11" width="14" height="10" rx="2" />  <circle cx="12" cy="16" r="1" />  <path d="M8 11v-5a4 4 0 0 1 8 0" /></svg>
      </button>


      <button
        title="set even y"
        type="button"
        phx-click="phx:set-even-y"
        class="rounded bg-zinc-300 px-2 py-1 text-sm font-semibold text-white shadow-sm hover:bg-zinc-400 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500"
        >
        <svg class="h-6 w-6 text-zinc-500"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <line x1="4" y1="12" x2="9" y2="12" />  <line x1="15" y1="12" x2="20" y2="12" />  <rect x="9" y="6" width="6" height="12" rx="2" /></svg>
      </button>

      <button
        title="set even x"
        type="button"
        phx-click="phx:set-even-x"
        class="rounded bg-zinc-300 px-2 py-1 text-sm font-semibold text-white shadow-sm hover:bg-zinc-400 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500"
        >
        <svg class="h-6 w-6 text-zinc-500"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <line x1="12" y1="4" x2="12" y2="9" />  <line x1="12" y1="15" x2="12" y2="20" />  <rect x="6" y="9" width="12" height="6" rx="2" /></svg>
      </button>



    """
  end

end
