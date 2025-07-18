defmodule LightwarriorWeb.Layouts do
  @moduledoc """
  This module holds different layouts used by your application.

  See the `layouts` directory for all templates available.
  The "root" layout is a skeleton rendered as part of the
  application router. The "app" layout is rendered as component
  in regular views and live views.
  """
  use LightwarriorWeb, :html

  embed_templates "layouts/*"

  def app(assigns) do
    ~H"""
    <header class="navbar px-4 sm:px-6 lg:px-8">
      <div class="flex-1">
        <a href="/" class="flex-1 flex items-center gap-2">
          <img src={~p"/images/logola.svg"} width="36" />
          <!--<span class="text-sm font-semibold">v{Application.spec(:phoenix, :vsn)}</span>-->
        </a>
      </div>
      <div class="flex-none">
        <ul class="flex flex-column px-1 space-x-4 items-center">
          <li>
            <.autosave_toggle autosave={@autosave} />
          </li>
          <li>
            <.debug_toggle debug={@debug} />
          </li>
          <li>
            <.theme_toggle />
          </li>
        </ul>
      </div>
    </header>

    <main class="px-4 py-4 sm:px-6 lg:px-8">
      <div class="mx-auto space-y-4">
        {render_slot(@inner_block)}
      </div>
    </main>

    <.flash_group flash={@flash} />
    """
  end

  @doc """
  Shows the flash group with standard titles and content.

  ## Examples

      <.flash_group flash={@flash} />
  """
  attr :flash, :map, required: true, doc: "the map of flash messages"
  attr :id, :string, default: "flash-group", doc: "the optional id of flash container"

  def flash_group(assigns) do
    ~H"""
    <div id={@id} aria-live="polite">
      <.flash kind={:info} flash={@flash} />
      <.flash kind={:error} flash={@flash} />

      <.flash
        id="client-error"
        kind={:error}
        title={gettext("We can't find the internet")}
        phx-disconnected={show(".phx-client-error #client-error") |> JS.remove_attribute("hidden")}
        phx-connected={hide("#client-error") |> JS.set_attribute({"hidden", ""})}
        hidden
      >
        {gettext("Attempting to reconnect")}
        <.icon name="hero-arrow-path" class="ml-1 h-3 w-3 motion-safe:animate-spin" />
      </.flash>

      <.flash
        id="server-error"
        kind={:error}
        title={gettext("Something went wrong!")}
        phx-disconnected={show(".phx-client-error #client-error") |> JS.remove_attribute("hidden")}
        phx-connected={hide("#client-error") |> JS.set_attribute({"hidden", ""})}
        hidden
      >
        {gettext("Hang in there while we get back on track")}
        <.icon name="hero-arrow-path" class="ml-1 h-3 w-3 motion-safe:animate-spin" />
      </.flash>
    </div>
    """
  end

  @doc """
  Provides dark vs light theme toggle based on themes defined in app.css.

  See <head> in root.html.heex which applies the theme before page load.
  """
  def theme_toggle(assigns) do
    ~H"""
    <div class="card relative flex flex-row items-center border-2 border-base-300 bg-base-300 rounded-full">
      <div class="absolute w-[33%] h-full rounded-full border-1 border-base-200 bg-base-100 brightness-200 left-0 [[data-theme=light]_&]:left-[33%] [[data-theme=dark]_&]:left-[66%] transition-[left]" />

      <button phx-click={JS.dispatch("phx:set-theme", detail: %{theme: "system"})} class="flex p-2">
        <.icon name="hero-computer-desktop-micro" class="size-4 opacity-75 hover:opacity-100" />
      </button>

      <button phx-click={JS.dispatch("phx:set-theme", detail: %{theme: "light"})} class="flex p-2">
        <.icon name="hero-sun-micro" class="size-4 opacity-75 hover:opacity-100" />
      </button>

      <button phx-click={JS.dispatch("phx:set-theme", detail: %{theme: "dark"})} class="flex p-2">
        <.icon name="hero-moon-micro" class="size-4 opacity-75 hover:opacity-100" />
      </button>
    </div>
    """
  end

  @doc """
  Provides debug toggle switch.
  """

  attr :debug, :boolean, doc: "the checked flag for checkbox inputs"

  def autosave_toggle(assigns) do
    ~H"""
    <div id="autosave_switch_wrapper" class="relative flex flex-row gap-2 items-center rounded-full" phx-hook="LocalStorage">

        <span>autosave</span><input id="autosave_switch" phx-click={
          JS.push("phx.toggle_autosave")
          #|>  JS.dispatch("phx:localstorage_toggle", detail: %{ autosave: @autosave } )
        }
        type="checkbox" class="toggle" checked={@autosave}/>

    </div>
    """
  end

  @doc """
  Provides debug toggle switch.
  """

  attr :debug, :boolean, doc: "the checked flag for checkbox inputs"

  def debug_toggle(assigns) do
    ~H"""
    <div id="debug_switch_wrapper" class="relative flex flex-row gap-2 items-center rounded-full" phx-hook="LocalStorage">

        <span>debug</span><input id="debug_switch" phx-click={
          JS.push("phx.toggle_debug")
          #|> JS.dispatch("phx:localstorage_toggle", detail: %{ debug: @debug } )
        }
        type="checkbox" class="toggle" checked={@debug}/>

    </div>
    """
  end

  attr :selected, :integer, doc: "selected id"

  def debug(assigns) do
    ~H"""
      <hr class="mt-10">
      <div class="flex text-xs">
      <div class="basis-1/4">
          <h2>assigns</h2>
          <pre class="w-64">
              <%=
                #pretty_json = Jason.encode!(Map.delete(assigns, :state), pretty: true)
                #raw(pretty_json)
                assigns_no_state = Map.drop(assigns, [:state, :inner_block, :__changed__, :__given__])
                Kernel.inspect(assigns_no_state, pretty: true)
            %>
          </pre>
        </div>
        <div class="basis-1/4">
          <h2>serverinfo</h2>
          <pre>
            <%=
                pretty_json = Jason.encode!(@state.serverinfo, pretty: true)
                raw(pretty_json)
            %>
          </pre>
        </div>
        <div class="basis-1/4">
          <h2>stripes</h2>
          <pre>
            <%=
                pretty_json = Jason.encode!(@state.stripes, pretty: true)
                raw(pretty_json)
            %>
          </pre>
        </div>
        <div class="basis-1/4">
        <h2>stripes_with_config</h2>
          <pre>
            <%=
                pretty_json = Jason.encode!(@state.stripes_with_config, pretty: true)
                raw(pretty_json)
            %>
          </pre>
        </div>
      </div>
    """
  end
end
