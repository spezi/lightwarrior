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
        <ul id="topmenue" class="flex flex-column px-1 space-x-4 items-center" phx-hook="LocalStorage">
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

  attr(:state, :map, required: false)
  attr(:selected, :integer, required: true)
  attr(:side, :string, required: false)

  def left_top_menue(assigns) do
    ~H"""
      <!-- name of each tab group should be unique -->
      <div id="left_top_menue" class="tabs tabs-lift">
          <input :if={ @side != "uniform" } type="radio" name="left_top_menue_tabs" class="tab" aria-label={"stripe instances (#{length(Lightwarrior.State.get(:instances_with_config_input))})"} checked="checked" />
          <div class="tab-content bg-base-100 border-base-300 p-2">
              <div :if={Lightwarrior.State.get(:instances) != nil } class="flex flex-wrap">
                <div :for={instance <- Lightwarrior.State.get(:instances)}>
                  <.led_instance title={instance["friendly_name"]} status={instance["running"]} instance={instance} selected={@selected}/>
                </div>
              </div>
                <p :if={Lightwarrior.State.get(:instances) == nil }  class="text-red-500">
                  no connection to Hyperion!
                </p>
              <div class="p-2">
                <button phx-click="refresh" class="btn btn-sm btn-neutral btn-square ml-auto">
                  <.icon name="hero-arrow-path-mini" class="size-5 opacity-40 group-hover:opacity-70" />
                </button>
              </div>
          </div>

          <input :if={ @side == "uniform" } type="radio" name="left_top_menue_tabs" class="tab" aria-label="uniform instances" checked="checked" />
          <div class="tab-content bg-base-100 border-base-300 p-2">


          </div>

          <input type="radio" name="left_top_menue_tabs" class="tab" aria-label="global config" />
          <div class="tab-content bg-base-100 border-base-300 p-2">
              <div class="p-2">
                <button phx-click="build-score" class="btn btn-sm btn-primary ml-auto">
                  Build Ossia Score File <.icon name="hero-beaker-mini" class="size-5 opacity-40 group-hover:opacity-70" />
                </button>
              </div>
              <div class="p-2">
                <button phx-click="patch-score" class="btn btn-sm btn-primary ml-auto">
                  Patch score Adresses <.icon name="hero-beaker-mini" class="size-5 opacity-40 group-hover:opacity-70" />
                </button>
              </div>
          </div>

      </div>
    """
  end


  attr(:side, :string, required: false)

  def left_bottom_menue(assigns) do
    ~H"""
            <!-- name of each tab group should be unique -->
            <div :if={ @side != "services" } class="tabs tabs-lift mt-4">
                <input type="radio" name="left_bottom_menue_tabs" class="tab" aria-label="instance config" checked="checked" />
                <div class="tab-content bg-base-100 border-base-300 p-8">
                    <button phx-click="save" class="btn btn-primary ml-auto">
                      <.icon name="hero-arrow-trending-up-mini" class="size-5 opacity-60 group-hover:opacity-70" />
                    </button>
                </div>
            </div>
    """
  end

  @doc """
  Renders a bouttons for LED Instances.

  ## Examples

      <.led_instance>
        <:item title="Title"><%= @post.title %></:item>
        <:item title="Views"><%= @post.views %></:item>
      </.led_instance>
  """

  attr(:title, :string, required: false)
  attr(:instance, :map, required: true)
  attr(:status, :string, required: true)
  attr(:selected, :integer, required: false)

  def led_instance(assigns) do
    ~H"""
        <.link patch={~p"/hyperion/#{@instance["instance"]}/edit"}>
          <button  class={
                  "btn m-1 p-2 rounded-full text-xs font-semibold text-white shadow-xl
                  #{if @status, do: "bg-green-600 ", else: "bg-zinc-600 "}
                  #{if @selected && @selected == @instance["instance"], do: "ring-2 ring-emerald-400 ", else: "" }
                "}
          ><%= @title %>
          </button>
        </.link>
    """
  end

  def services(assigns) do
    ~H"""
    <div class="collapse collapse-arrow bg-base-100 border border-base-300 z-0">
      <input type="radio" name="my-accordion-2" checked="checked" />
      <div class="collapse-title font-semibold">
        <h3>
        hyperion
        </h3>
      </div>
        <.service_player service="hyperion" />
      <div class="collapse-content text-sm">penis</div>
    </div>

    <div class="collapse collapse-arrow bg-base-100 border border-base-300">
      <input type="radio" name="my-accordion-2" />
      <div class="collapse-title font-semibold"><h3>midimonster</h3></div>
      <.service_player service="midimonster" />
      <div class="collapse-content text-sm">penis</div>
    </div>

    <div class="collapse collapse-arrow bg-base-100 border border-base-300">
      <input type="radio" name="my-accordion-2" />
      <div class="collapse-title font-semibold"><h3>Ossia Score</h3></div>
      <.service_player service="ossia-score" />
      <div class="collapse-content text-sm">penis</div>
    </div>

    <div class="collapse collapse-arrow bg-base-100 border border-base-300">
      <input type="radio" name="my-accordion-2" />
      <div class="collapse-title font-semibold"><h3>shm2hyperion</h3></div>
      <.service_player service="shm2hyperion" />
      <div class="collapse-content text-sm">penis</div>
    </div>

    """
  end

  def service_player(assigns) do
      ~H"""
        <div class="absolute right-0 mr-12 p-2 z-1 btn-accent flex gap-2">

          <a href="#" phx-click={"phx:#{assigns.service}-start"} >
            <button class="btn btn-circle shadow-xl">
              <.icon name="hero-play-solid" class="" />
            </button>
          </a>

          <a href="#" phx-click={"phx:#{assigns.service}-stop"} >
          <button class="btn btn-circle shadow-xl">
            <.icon name="hero-stop-solid" class="size-6 shrink-0" />
          </button>
          </a>

          <a :if={assigns.service == "hyperion" } href="http://localhost:8090/#dashboard" target="_blanc">
          <button class="btn btn-circle shadow-xl">
            <.icon name="hero-link-solid" class="size-6 shrink-0" />
          </button>
          </a>

        </div>
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

  attr :autosave, :boolean, doc: "the checked flag for checkbox inputs"

  def autosave_toggle(assigns) do
    ~H"""
    <div id="autosave_switch_wrapper" class="relative flex flex-row gap-2 items-center rounded-full" >

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
    <div id="debug_switch_wrapper" class="relative flex flex-row gap-2 items-center rounded-full" >

        <span>debug</span><input id="debug_switch" phx-click={
          JS.push("phx.toggle_debug")
          #|> JS.dispatch("phx:localstorage_toggle", detail: %{ debug: @debug } )
        }
        type="checkbox" class="toggle" checked={@debug}/>

    </div>
    """
  end

  attr :selected, :integer, doc: "selected id"
  attr :side, :string, doc: "selected side"

  def debug(assigns) do
    ~H"""
      <div class="px-4 py-4 sm:px-6 lg:px-8">
      <hr class="mt-10">
      <div>
          <h2>assigns</h2>
          <pre>
              <%=
                #pretty_json = Jason.encode!(Map.delete(assigns, :state), pretty: true)
                #raw(pretty_json)
                assigns_no_state = Map.drop(assigns, [:state, :inner_block, :__changed__, :__given__])
                Kernel.inspect(assigns_no_state, pretty: true)
            %>
          </pre>
        </div>
      <div class="flex text-xs w-full">
        <div class="basis-1/4">
          <h2>serverinfo</h2>
          <pre>
            <%=
                pretty_json = Jason.encode!(Lightwarrior.State.get(:serverinfo), pretty: true)
                raw(pretty_json)
            %>
          </pre>
        </div>
        <div class="basis-1/4">
          <h2>instances</h2>
          <pre>
            <%=
                pretty_json = Jason.encode!(Lightwarrior.State.get(:instances), pretty: true)
                raw(pretty_json)
            %>
          </pre>
        </div>
        <div class="basis-1/4">
        <h2>instances_with_config_input</h2>
          <pre>
            <%=
                pretty_json = Jason.encode!(Lightwarrior.State.get(:instances_with_config_input), pretty: true)
                raw(pretty_json)
            %>
          </pre>
        </div>
        <div class="basis-1/4">
        <h2>instances_with_config_output</h2>
          <pre>
            <%=
                pretty_json = Jason.encode!(Lightwarrior.State.get(:instances_with_config_output), pretty: true)
                raw(pretty_json)
            %>
          </pre>
        </div>
      </div>
    </div>
    """
  end
end
