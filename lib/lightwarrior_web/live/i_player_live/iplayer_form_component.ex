defmodule LightwarriorWeb.IPlayerLive.IPlayerFormComponent do
  use LightwarriorWeb, :live_component

  alias Lightwarrior.Imageplayer

  @impl true
  def render(assigns) do
    ~H"""
    <div>
      <.header>
        <!--<%= @title %>-->
        <:subtitle>Controll Gstreamer to shmdata transmission</:subtitle>
      </.header>

      <div class="flex flex-row gap-4" >
        <div class="w-1/6 flex-none overflow-">
          <.input name="path" value={@path}/>
          <div id="filesystem" phx-hook="DragArea" class="overflow-x-hidden overflow-y-auto max-h-96 rounded-lg bg-slate-50 dark:bg-slate-800 ring-1 shadow p-3 cursor-pointer" >
            <ul>
              <li phx-click="cd" phx-value-name=".." phx-target={@myself}> <b>..</b> </li>
            </ul>
            <ul :if={@dirs}>
              <li
                :for={dir <- @dirs}
                :if={String.at(dir, 0) != "."}
                phx-click="cd"
                phx-value-name={dir}
                phx-target={@myself}
              >
                <b><%= dir %></b>
              </li>
            </ul>
            <ul :if={@files}>
            <li
                :for={file <- @files}
                :if={String.at(file, 0) != "."}
                phx-click="select"
                phx-value-path={@path}
                phx-value-filename={file}
                phx-target={@myself}
                class="draggable"
                draggable="true"
              >
                <%= file %>
              </li>
            </ul>
          </div>
        </div>
        <div class="basis-5/6">
          <div class="mt-2 overflow-hidden rounded-lg bg-slate-50 dark:bg-slate-800 ring-1 shadow p-3">
            <div class="grid grid-cols-6 gap-4">

              <div class="grid grid-flow-row gap-4 justify-items-center">

                <%= if @layerdata.layer_one.thumbnail do %>
                  <div id="i_player_image_layer_one" class={"h-48 w-48 overflow-hidden bg-center bg-no-repeat bg-cover ring-1 m-2 droptarget"} style={"background-image: url('#{@layerdata.layer_one.thumbnail}');"}>
                      <!--<%= @path %>
                      <%= @filename %>
                      <%= @file %>
                      -->
                  </div>
                <% else %>
                  <div id="i_player_image_layer_one" class="h-48 w-48 overflow-hidden bg-gradient-to-r from-purple-500 to-pink-500 ring-1 m-2 droptarget">
                      <!--<%= @path %>
                      <%= @filename %>
                      <%= @file %>
                      -->
                  </div>
                <% end %>
              <.form
                :if={@layerdata.layer_one.file}
                for={@changesets.layer_one}
                as={:layer_one}
                phx-change="validate"
                phx-submit="start_send_shmdata"
                phx-target={@myself}
                class="w-11/12 gap-4 grid grid-flow-row gap-4"
              >
                <div>
                <.input
                    id="i_player_name_layer_one"
                    field={@changesets.layer_one[:name]}
                    type="hidden"
                    value="layer_one"
                />
                <.input
                    id="i_player_name_layer_one"
                    field={@changesets.layer_one[:type]}
                    type="hidden"
                    value={@layerdata.layer_one.type}
                />
                  <.input
                    id="i_player_output_type_layer_one"
                    field={@changesets.layer_one[:output_type]}
                    type="select"
                    label="output"
                    options={@output_options}
                    selected="shmdatasink"
                    value="shmdatasink"
                    />
                </div>
                <div>
                  <.button type="submit" class="ml-2">
                      <.icon name="hero-play-solid" class="h-5 w-5" />
                  </.button>
                </div>

                <div class="">
                <!--
                  <.button phx-click="start_send_shmdata" phx-value-layer="layer_one" class="ml-2">
                    <.icon name="hero-play-solid" class="h-5 w-5" />
                  </.button>
                -->
                  <div class="info mt-5 w-64 overflow-hidden">
                    <p>File: <%= @layerdata.layer_one.file %></p>
                  </div>
                </div>
                </.form>
              </div>

              <div class="grid grid-flow-row gap-4 w-fit justify-items-center">

                <%= if @layerdata.layer_two.thumbnail do %>
                  <div id="i_player_image_layer_two" class={"h-48 w-48 overflow-hidden bg-center bg-no-repeat bg-cover ring-1 m-2 droptarget"} style={"background-image: url('#{@layerdata.layer_two.thumbnail}');"}>
                      <!--<%= @path %>
                      <%= @filename %>
                      <%= @file %>
                      -->
                  </div>
                <% else %>
                  <div id="i_player_image_layer_two" class="h-48 w-48 overflow-hidden bg-gradient-to-r from-purple-500 to-pink-500 ring-1 m-2 droptarget">
                      <!--<%= @path %>
                      <%= @filename %>
                      <%= @file %>
                      -->
                  </div>
                <% end %>
              <.form
                :if={@layerdata.layer_two.file}
                for={@changesets.layer_two}
                as={:layer_two}
                phx-change="validate"
                phx-submit="start_send_shmdata"
                phx-target={@myself}
                class="w-11/12 gap-4 grid grid-flow-row gap-4"
              >
                <div>
                <.input
                    id="i_player_name_layer_two"
                    field={@changesets.layer_two[:name]}
                    type="hidden"
                    value="layer_two"
                />
                <.input
                    id="i_player_name_layer_two"
                    field={@changesets.layer_two[:type]}
                    type="hidden"
                    value={@layerdata.layer_two.type}
                />
                  <.input
                    id="i_player_output_type_layer_two"
                    field={@changesets.layer_two[:output_type]}
                    type="select"
                    label="output"
                    options={@output_options}
                    selected="shmdatasink"
                    value="shmdatasink"
                    class="dark:text-zinc-900"
                    />
                </div>
                <div>
                  <.button type="submit" class="ml-2">
                      <.icon name="hero-play-solid" class="h-5 w-5" />
                  </.button>
                </div>

                <div class="overflow-hidden">
                  <div class="info mt-5 w-64">
                    <p>File: <%= @layerdata.layer_two.file  %></p>
                  </div>
                </div>
                </.form>
              </div>


              <div class="h-full">03</div>
              <div class="h-full">04</div>
            </div>
          </div>
        </div>
      </div>

    </div>
    """
  end

  @impl true
  def update(%{id: "iplayer_file_form", title: "Listing Iplayer", action: :index} = assigns, socket) do
    #changeset = Imageplayer.change_i_player(i_player)

    #avoid jump back to home on refresh
    path = if Map.has_key?(socket.assigns, :path) do
      socket.assigns.path
    else
      Path.expand("~")
    end



    {:ok,
     socket
     |> assign(assigns)
     |> assign(:path, path)
     |> ls()
     #|> assign_form(changeset)
     }
  end

  @impl true
  def handle_event("validate", %{"_target" => ["i_player", target], "i_player" => params}, socket) do
    dbg(params)
    #changeset =
    #  socket.assigns.i_player
      #|> Imageplayer.change_i_player(value)
     # |> Map.put(:action, :validate)

    #{:noreply, assign_form(socket, changeset)}
    {:noreply, socket}
  end

  def handle_event("cd", param, socket) do
    #dbg(get_path(socket, param["name"]))
    socket =
      socket
      |> assign(path: get_path(socket, param["name"]))
      |> ls()

    {:noreply, socket}
  end

  def handle_event("select", param, socket) do
    dbg(param)
    socket =
      socket
      #|> assign(path: path(socket, param[]))
      |> ls()

      notify_parent({:selected, get_path(socket, param["filename"])})

    {:noreply, socket}
  end

  #"start_send_shmdata", %{"i_player" => %{"output_type" => "autovideosink"}}
  def handle_event("start_send_shmdata", params, socket) do
    #dbg(params)
    notify_parent({:change_player, params})
    {:noreply, socket}
  end



  defp ls(socket) do
    case File.ls(socket.assigns.path) do
      {:ok, entries} ->
        {dirs, files} = Enum.split_with(entries,
          fn x ->
            #IO.puts(socket.assigns.path <> "/" <> x)
            File.dir?(socket.assigns.path <> "/" <> x)
          end
        )

        notify_parent({:change_path, get_path(socket, "")})

        assign(socket, dirs: Enum.sort(dirs), files: Enum.sort(files))
        #{dirs, files} = Enum.split_with(entries, &File.dir?(path(socket, &1)))
        #assign(socket, dirs: Enum.sort(dirs), files: Enum.sort(files))
        #socket
      _ ->
        socket
    end
  end

  defp get_path(socket, param) do
    #dbg(socket.assigns.path <> "/" <> param)
    Path.expand(socket.assigns.path <> "/" <> param)
  end

  #defp assign_form(socket, %Ecto.Changeset{} = changeset) do
  #  assign(socket, :form, to_form(changeset))
  #end

  defp notify_parent(msg), do: send(self(), {__MODULE__, msg})
end
