defmodule LightwarriorWeb.HyperionComponents do
  use Phoenix.Component
  use Gettext, backend: LightwarriorWeb.Gettext

  import LightwarriorWeb.CoreComponents
  use Phoenix.VerifiedRoutes, endpoint: LightwarriorWeb.Endpoint, router: LightwarriorWeb.Router

  alias Phoenix.LiveView.JS

  attr :state, :map, required: false

  def led_instances_menue(assigns) do
    ~H"""
    <div class="card card-border bg-base-100 w-96">
      <div class="card-body">
        <div class="flex gap-2">
          <h2 class="card-title">Instances</h2>
          <button phx-click="refresh" class="btn btn-square">
            <.icon name="hero-arrow-path-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
        </div>
        <div class="flex flex-wrap">
          <div :for={instance <- @state.stripes}>
            <.led_instance title={instance.friendly_name} status={instance.running} instance={instance} selected={@selected}/>
          </div>
        </div>
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
                  #{if @selected && @selected == @instance.instance, do: "ring-offset-2 ring-2 ring-emerald-400 ", else: "" }
                "}
          ><%= @title %>
          </button>
        </.link>
    """
  end

end
