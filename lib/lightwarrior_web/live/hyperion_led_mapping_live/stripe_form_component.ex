defmodule Lightwarrior.Hyperion.StripeFormComponent do
  use LightwarriorWeb, :live_component

  alias Lightwarrior.Hyperion
  alias Lightwarrior.Hyperion.Device
  alias Lightwarrior.Hyperion.Stripe
  alias Lightwarrior.Helper

  @impl true
  def render(assigns) do
    ~H"""
    <div id={@id} class={@class} >
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
        <.simple_form
          for={@form}
          id="test-form"
          phx-target={@myself}
          phx-change="validate"
          phx-submit="save"
        >

            <.inputs_for :let={device} field={@form[:device]} as={:device} >
              <.input field={device[:host]} label="Host:" type="text" />
              <.input field={device[:port]} label="Port:" type="text" />
            </.inputs_for>

            <.inputs_for :let={smoothing} field={@form[:smoothing]} as={:smoothing} >

                    <.simple_toggle
                      title={"Smoothing"}
                      switch={true}
                      action={

                      }
                    >
                    </.simple_toggle>
            </.inputs_for>

        <:actions>
          <.button phx-disable-with="Saving...">Save Stripe</.button>
        </:actions>
      </.simple_form>
      </div>
    </div>
    """
  end

  @impl true
  def update(%{stripe_data: stripe_data} = assigns, socket) do
    # load initial form with current values
    {:ok,
     socket
     |> assign(assigns)
     |> assign_form(stripe_data)
    }
  end

  @impl true
  def handle_event("validate", params, socket) do

    # load current device
    { max_packet, device } = socket.assigns.stripe_data["device"]
                             |> Map.pop("max-packet")
    device = device
            |> Map.put_new("max_packet", max_packet)

    # prepare for update changes
    device_with_change = Map.merge(device, params["device"], fn _k, v1, v2 ->
      v2
    end)

    # update original params with new params
    params_load = %{
      "_target" => params["_target"],
      "device" => device_with_change
    }

    #dbg(params_load)

    # init struct for updating
    struct = struct(Stripe, socket.assigns.stripe_data)

    #build changeset with updates params
    changeset =
      struct
      |> Stripe.changeset(params_load)
      |> Map.put(:action, :validate)

    dbg(changeset)

    {:noreply, socket
      |> assign_form(changeset)
    }
  end

  def handle_event("save", %{"device" => params}, socket) do
    #dbg(socket.assigns.action)
    save_stripe(socket, socket.assigns.action, params)
  end

  defp assign_form(socket, stripe_data) do
    assign(socket, :form, to_form(stripe_data, id: "stripe_form"))
  end

  defp save_stripe(socket, :edit, params) do
    dbg("save stripe")
    #dbg(socket.assigns.form.params)
    #dbg(socket.assigns.form.params["_target"])

    # only change if something updated by params
    stripe_data = case socket.assigns.form.params["_target"] do
      nil ->
        socket.assigns.stripe_data
      _ ->
        put_in(socket.assigns.stripe_data, socket.assigns.form.params["_target"], get_in(socket.assigns.form.params, socket.assigns.form.params["_target"]))
    end

    #dbg(stripe_data)
    dbg(Map.get(socket.assigns.form.source, :valid? ))

    #validation
    case Map.get(socket.assigns.form.source, :valid? ) do
      true ->
        notify_parent({:save, stripe_data, socket.assigns.form})
      false ->
        notify_parent({:error, :validation, "Form not valid"})
      nil ->
        # if no changes
        notify_parent({:save, stripe_data, socket.assigns.form})
    end

    #notify_parent({:save, stripe_data, socket.assigns.form})

    {:noreply,
      socket
      #|> put_flash(:info, "Stripe update successfully")
      #|> push_patch(to: socket.assigns.patch)
    }
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})
end
