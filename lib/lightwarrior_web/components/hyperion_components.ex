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

end
