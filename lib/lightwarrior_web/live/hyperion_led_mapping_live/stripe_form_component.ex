defmodule Lightwarrior.Hyperion.StripeFormComponent do
  use LightwarriorWeb, :live_component

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
              <.input field={device["host"]} label="Host:" type="text" />
              <.input field={device["port"]} label="Port:" type="text" />
            </.inputs_for>

        <:actions>
          <.button phx-disable-with="Saving...">Save Test</.button>
        </:actions>
      </.simple_form>
      </div>
    </div>
    """
  end

  @impl true
  def update(%{stripe_data: stripe_data} = assigns, socket) do
    {:ok,
     socket
     |> assign(assigns)
     |> assign_form(stripe_data)
    }
  end

  @impl true
  def handle_event("validate", params, socket) do
    #dbg(params)
    #dbg(socket.assigns.stripe_data["device"])

    #dbg(Device.changeset(socket.assigns.stripe_data["device"], params))

    { max_packet, device } = socket.assigns.stripe_data["device"]
                             |> Map.pop("max-packet")
    device = device
            |> Map.put_new("max_packet", max_packet)
            #|> Helper.string_keys_to_atom_keys()

    #struct = struct |> Map.put_new("max_packet", max_packet)
    #struct = struct(Device, Helper.string_keys_to_atom_keys(socket.assigns.stripe_data["device"]))
    #|> Stripe.changeset(params[:device])
    #|> Map.put(:action, :update)
    #dbg(socket.assigns.stripe_data["device"])
    #dbg(device)
    dbg(params)

    params_load = params


    struct = struct(Stripe, socket.assigns.stripe_data)

    Map.replace(struct, :device, device)
    #dbg(struct)

    changeset =
      struct
      |> Stripe.changeset(params_load)
      |> Map.put(:action, :validate)

    #changeset = Map.replace(changeset.changes.device, :action, :update)


    dbg(changeset)

    {:noreply, socket
      #|> assign_form(changeset)
    }
  end

  def handle_event("save", %{"device" => params}, socket) do
    #dbg(socket.assigns.action)
    save_stripe(socket, socket.assigns.action, params)
  end

  defp assign_form(socket, stripe_data) do
    assign(socket, :form, to_form(stripe_data, id: "stripe_form"))
  end

  defp save_stripe(socket, :edit, test_params) do
    dbg("save stripe")
    {:noreply,
      socket
      |> put_flash(:info, "Stripe update successfully")
      #|> push_patch(to: socket.assigns.patch)
    }
  end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})
end
