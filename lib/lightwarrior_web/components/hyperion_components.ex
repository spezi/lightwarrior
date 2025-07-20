defmodule LightwarriorWeb.HyperionComponents do
  use Phoenix.Component
  use Gettext, backend: LightwarriorWeb.Gettext

  import LightwarriorWeb.CoreComponents
  use Phoenix.VerifiedRoutes, endpoint: LightwarriorWeb.Endpoint, router: LightwarriorWeb.Router

  alias Phoenix.LiveView.JS

  attr :state, :map, required: false

  def left_top_menue(assigns) do
    ~H"""
          <!-- name of each tab group should be unique -->
          <div class="tabs tabs-lift">
              <input type="radio" name="left_top_menue_tabs" class="tab" aria-label="instances" checked="checked" />
              <div class="tab-content bg-base-100 border-base-300 p-2">
                <div class="w-full text-right">
                  <button phx-click="refresh" class="btn btn-sm btn-neutral btn-square">
                    <.icon name="hero-arrow-path-mini" class="size-5 opacity-40 group-hover:opacity-70" />
                  </button>
                </div>
                  <div :if={@state.stripes != nil } class="flex flex-wrap">
                    <div :for={instance <- @state.stripes}>
                      <.led_instance title={instance.friendly_name} status={instance.running} instance={instance} selected={@selected}/>
                    </div>
                  </div>
                  <div :if={@state.stripes == nil } class="flex flex-wrap">
                    <p class="text-red-500">
                      there seems to be no connection to Hyperion!
                    </p>
                  </div>

              </div>

              <input type="radio" name="left_top_menue_tabs" class="tab" aria-label="global config" />
              <div class="tab-content bg-base-100 border-base-300 p-2">

              </div>
          </div>
  """
  end

    attr :state, :map, required: false

  def left_bottom_menue(assigns) do
    ~H"""
          <!-- name of each tab group should be unique -->
          <div class="tabs tabs-lift mt-4">
              <input type="radio" name="left_bottom_menue_tabs" class="tab" aria-label="instance config" checked="checked" />
              <div class="tab-content bg-base-100 border-base-300 p-2">

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

  attr :title, :string, required: false
  attr :instance, :map, required: true
  attr :selected, :integer, required: false


  def led_instance(assigns) do
    ~H"""
        <.link patch={~p"/hyperion/#{@instance.instance}/edit"}>
          <button  class={
                  "btn m-1 p-2 rounded-full text-xs font-semibold text-white shadow-xl
                  #{if @status, do: "bg-green-600 ", else: "bg-red-600 "}
                  #{if @selected && @selected == @instance.instance, do: "ring-2 ring-emerald-400 ", else: "" }
                "}
          ><%= @title %>
          </button>
        </.link>
    """
  end

  def mapping_tool_menue(assigns) do
    ~H"""
    <.form for={@form} phx-change="validate" >
    <div class="flex flex-row m-3 gap-1 text-xs">

        <.input name="side" type="hidden" value={@side} placeholder="Stripe length" />

        <div class="w-26">
            <.input type="text" field={@form[:stripe_length]}  placeholder="Stripe length" />
        </div>

        <div>
          <div class="flex flex-row w-fit p-1">
            <select type="select" class="select select-sm">
            <!--
              <option selected></option>
              <option>Crimson</option>
              <option>Amber</option>
              <option>Velvet</option>-->
              <option disabled selected>copy from</option>
                <%= for stripe <- @state.stripes do %>
                  <option value={stripe.instance}><%= stripe.friendly_name %></option>
                <% end %>
            </select>
          </div>
        </div>

        <div class="flex flex-row gap-1 m-1">
          <span class="py-1">move: </span>
          <button phx-click="phx:move-stripe" phx-value-direction="left" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-left-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button phx-click="phx:move-stripe" phx-value-direction="right" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-right-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button phx-click="phx:move-stripe" phx-value-direction="up" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-up-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button phx-click="phx:move-stripe" phx-value-direction="down" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-down-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <input type="text" placeholder="steps" class="input input-sm w-16" />
        </div>

        <.input type="range" label="opacity" field={@form[:opacity]} class="range range-sm" value={@mapping[:opacity]}/>

        <div class="flex flex-row-reverse flex-end gap-1 w-64 grow m-1">


          <button :if={!@mapping["lockdistance"]} phx-click="phx:toggle-distance-lock" phx-value-side={@side} phx-value-value={@mapping["lockdistance"]} class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-lock-closed-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button :if={@mapping["lockdistance"]} phx-click="phx:toggle-distance-lock" phx-value-side={@side} phx-value-value={!@mapping["lockdistance"]} class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-lock-open-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>

          <button
            title="set even y"
            type="button"
            phx-click="phx:set-even-y"
            class="btn btn-sm btn-neutral btn-square"
            >
            <svg class="size-5 opacity-40 group-hover:opacity-70"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <line x1="4" y1="12" x2="9" y2="12" />  <line x1="15" y1="12" x2="20" y2="12" />  <rect x="9" y="6" width="6" height="12" rx="2" /></svg>
          </button>

          <button
            title="set even x"
            type="button"
            phx-click="phx:set-even-x"
            class="btn btn-sm btn-neutral btn-square"
            >
            <svg class="size-5 opacity-40 group-hover:opacity-70"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <line x1="12" y1="4" x2="12" y2="9" />  <line x1="12" y1="15" x2="12" y2="20" />  <rect x="6" y="9" width="12" height="6" rx="2" /></svg>
          </button>

        </div>
    </div>
    </.form>
    """
  end

end
