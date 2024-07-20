defmodule LightwarriorWeb.IPlayerLive.IPlayerFormComponent do
  use LightwarriorWeb, :live_component

  alias Lightwarrior.Imageplayer

  @impl true
  def render(assigns) do
    ~H"""
    <div>
      <.header>
        <%= @title %>
        <:subtitle>Play Still images via gstreamer</:subtitle>
      </.header>

      <.form phx-change="fileselect" phx-submit="save">
              <select
                id="file_select"
                name="file_select"
                class="mt-2 block w-44 rounded-md border border-gray-300 bg-white shadow-sm focus:border-zinc-400 focus:ring-0 sm:text-sm"
                value={@value}
              >
                <option value=""></option>
                <%= for file <- @files do %>

                  <option :if={ @value == file } value={file} selected><%= file %></option>
                  <option :if={ @value != file } value={file} ><%= file %></option>

                <% end %>
              </select>
              </.form>
    </div>
    """
  end

  @impl true
  def update(%{i_player: i_player} = assigns, socket) do
    #changeset = Imageplayer.change_i_player(i_player)

    {:ok,
     socket
     |> assign(assigns)
     #|> assign_form(changeset)
     }
  end

  @impl true
  def handle_event("validate", %{"i_player" => i_player_params}, socket) do
    changeset =
      socket.assigns.i_player
      |> Imageplayer.change_i_player(i_player_params)
      |> Map.put(:action, :validate)

    #{:noreply, assign_form(socket, changeset)}
    {:noreply, socket}
  end

  def handle_event("save", %{"i_player" => i_player_params}, socket) do
    save_i_player(socket, socket.assigns.action, i_player_params)
  end

  defp save_i_player(socket, :edit, i_player_params) do
    case Imageplayer.update_i_player(socket.assigns.i_player, i_player_params) do
      {:ok, i_player} ->
        notify_parent({:saved, i_player})

        {:noreply,
         socket
         |> put_flash(:info, "I player updated successfully")
         |> push_patch(to: socket.assigns.patch)}

      {:error, %Ecto.Changeset{} = changeset} ->
        #{:noreply, assign_form(socket, changeset)}
        {:noreply, socket}
    end
  end

  defp save_i_player(socket, :new, i_player_params) do
    case Imageplayer.create_i_player(i_player_params) do
      {:ok, i_player} ->
        notify_parent({:saved, i_player})

        {:noreply,
         socket
         |> put_flash(:info, "I player created successfully")
         |> push_patch(to: socket.assigns.patch)}

      {:error, %Ecto.Changeset{} = changeset} ->
        #{:noreply, assign_form(socket, changeset)}
        {:noreply, socket}
    end
  end

  #defp assign_form(socket, %Ecto.Changeset{} = changeset) do
  #  assign(socket, :form, to_form(changeset))
  #end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})
end
