defmodule LightwarriorWeb.IPlayerLive.Index do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Imageplayer
  alias Lightwarrior.Imageplayer.IPlayer

  alias Lightwarrior.Imageplayer.Thumbnail
  alias Lightwarrior.MyTracker

  @impl true
  def mount(_params, _session, socket) do
    #{:ok, stream(socket, :iplayer, Imageplayer.list_iplayer())}
    #path = Path.absname("~")
    #{:ok, files} = File.ls(path)

    #dbg(Phoenix.Tracker.list(Lightwarrior.MyTracker, "player"))
    #dbg(Process.whereis(Lightwarrior.Imageplayer.GenserverSupervisor))
    dbg(Process.whereis(:layer_one))

    if Process.whereis(:layer_one) do
      dbg(Process.info(Process.whereis(:layer_one)))
    end


    #dbg(DynamicSupervisor.count_children(GenserverSupervisor))
    dbg(DynamicSupervisor.count_children(Lightwarrior.Imageplayer.GenserverSupervisor))
    dbg(DynamicSupervisor.which_children(Lightwarrior.Imageplayer.GenserverSupervisor))
    dbg(Process.registered())

    #{:ok, ws_pid} = Lightwarrior.WebSocketClient.start_link("ws://127.0.0.1:9999")
    {:ok, ws_pid} = Lightwarrior.WebSocketClient.start_link("ws://127.0.0.1:10212")
    dbg(ws_pid)
    #https://ossia.io/score-docs/in-depth/remote.html
    #/Users/spezi/git/score/src/plugins/score-plugin-remotecontrol/js-remote

    #Lightwarrior.WebSocketClient.send_message_ossia_score(ws_pid, %{ Message: "Play" } )

    Enum.each(DynamicSupervisor.which_children(Lightwarrior.Imageplayer.GenserverSupervisor), fn x ->
      {:undefined, pid, :supervisor, [Lightwarrior.Imageplayer.GenserverInstance]} = x
      dbg(x)
      dbg(Process.info(pid))
      #dbg(x)
      #Lightwarrior.Imageplayer.GenserverInstance.get_port_info(pid)
      #Lightwarrior.Imageplayer.GenserverInstance.terminate(:shutdown)
      #Process.exit(pid, :normal)
      #dbg(Process.monitor(pid))
      #dbg(Process.monitor(pid))
      #dbg(Process.info(pid))
      #dbg(Process.info(pid, :registered_name))
      #dbg(Process.get_keys())
    end)

    #dbg(self())

    output_options = [
      autovideosink: "autovideosink",
      shmdatasink: "shmdatasink"
    ]

    form = %Lightwarrior.Imageplayer.IPlayer{}
    |> Ecto.Changeset.change()
    |> to_form()

    changesets = %{layer_one: form, layer_two: form}

    layerdata_inner = %{
      file: nil,
      thumbnail: nil,
      type: nil,
      command: nil
    }

    layerdata = %{
      layer_one: layerdata_inner,
      layer_two: layerdata_inner,
    }

    dbg(layerdata)

    {:ok, socket
      |> assign(:debug, false)
      |> assign(:file, nil)
      |> assign(:filename, nil)
      |> assign(:ws_pid, ws_pid)
      |> assign(:output_options, output_options)
      |> assign(changesets: changesets)
      |> assign(layerdata: layerdata)
    }
  end

  @impl true
  def handle_params(params, _url, socket) do
    {:noreply, apply_action(socket, socket.assigns.live_action, params)}
  end

  defp apply_action(socket, :edit, %{"id" => id}) do
    socket
    |> assign(:page_title, "Edit I player")
    |> assign(:i_player, Imageplayer.get_i_player!(id))
  end

  defp apply_action(socket, :new, _params) do
    socket
    |> assign(:page_title, "New I player")
    #|> assign(:i_player, %IPlayer{})
  end

  defp apply_action(socket, :index, _params) do
    socket
    |> assign(:page_title, "Listing Iplayer")
    |> assign(:i_player, nil)
  end

  @impl true
  def handle_info({LightwarriorWeb.IPlayerLive.IPlayerFormComponent, {:change_path, change_path}}, socket) do
    {:noreply, socket
      |> push_event("change_path", %{path: change_path})
    }
  end

  #change player
  def handle_info({LightwarriorWeb.IPlayerLive.IPlayerFormComponent, {:change_player, params}}, socket) do

    player_values = params["i_player"]
    dbg(player_values)

    {output, layer_name} = case params["i_player"] do
      %{"name" => layer_name, "output_type" => output_type} ->
        case output_type do
          "shmdatasink" -> {"#{output_type} socket-path=/tmp/lightwarrior_#{layer_name}", layer_name}
          _ -> {output_type, layer_name}
        end
      _ -> nil
    end

    command_list = cond do
      player_values["type"] == "image" ->
              [
                "gst-launch-1.0 filesrc location=" <> socket.assigns.layerdata[String.to_atom(layer_name)].file,
                "decodebin",
                "videoconvert",
                "imagefreeze",
                "videoscale",
                "video/x-raw,width=1920,height=1080",
                output
              ]
      player_values["type"] == "video" ->
              [
                "gst-launch-1.0 filesrc location=" <> socket.assigns.layerdata[String.to_atom(layer_name)].file,
                "decodebin",
                "videoconvert",
                "videoscale",
                "video/x-raw,width=1920,height=1080",
                output
              ]
    end

    #command_list = [
    #  "gst-launch-1.0 filesrc location=" <> socket.assigns.layerdata[String.to_atom(layer_name)].file,
    #  "decodebin",
    #  "videoconvert",
    #  "imagefreeze",
    #  "videoscale",
    #  "video/x-raw,width=1920,height=1080",
    #  output
    #]

    command = Enum.join(command_list, " ! ")

    dbg(command)

    #############################
    Lightwarrior.WebSocketClient.send_message_ossia_score(socket.assigns.ws_pid, %{ Message: "Stop" } )

    dbg(Process.whereis(String.to_atom(player_values["name"])))


    case Process.whereis(String.to_atom(player_values["name"])) do
      nil -> IO.puts("first start " <> player_values["name"])
      pid ->
        #dbg(Process.unregister(String.to_atom(player_values["name"])))
        dbg(Lightwarrior.Imageplayer.GenserverSupervisor.terminate_worker(pid))
        dbg(Process.info(pid))
    end

    ready = Enum.each(DynamicSupervisor.which_children(Lightwarrior.Imageplayer.GenserverSupervisor), fn x ->
      {:undefined, pid, :supervisor, [Lightwarrior.Imageplayer.GenserverInstance]} = x
      dbg(x)
      info = Process.info(pid)
      if info[:registered_name] == String.to_atom(player_values["name"]) do
        dbg(Lightwarrior.Imageplayer.GenserverSupervisor.terminate_worker(pid))
      end
    end)

    socket = case ready do
       :ok ->
          {:ok, new_pid} = Lightwarrior.Imageplayer.GenserverSupervisor.start_worker(%{command: command}, %{name: String.to_atom(player_values["name"])})
          Process.register(new_pid, String.to_atom(player_values["name"]))
          assign(socket, :pid, new_pid)
        _ -> socket
    end

    #websocket connection to ossia score
    dbg(socket.assigns.ws_pid)
    dbg(Lightwarrior.WebSocketClient.send_message_ossia_score(socket.assigns.ws_pid, %{ Message: "Play" } ))

    #############################

    {:noreply, socket
      #|> assign(:command, command)
    }
  end


  @impl true
  def handle_info({LightwarriorWeb.IPlayerLive.IPlayerFormComponent, {:selected, file}}, socket) do
    IO.puts("select file")
    #IO.puts(filename)



    command_list = [
      "gst-launch-1.0 filesrc location=" <> file,
      "decodebin",
      "videoconvert",
      "imagefreeze",
      "videoscale",
      "video/x-raw,width=1280,height=720",
      "autovideosink"
    ]

    command_list = [
      "gst-launch-1.0 --gst-plugin-path=/usr/local/lib/gstreamer-1.0/ filesrc location=" <> file,
      "decodebin",
      "videoconvert",
      "imagefreeze",
      "videoscale",
      "video/x-raw,width=1280,height=720",
      "shmdatasink socket-path=/tmp/lightwarrior_layer1"
    ]

    #command = Enum.join(command_list, " ! ")

    #output = :os.cmd('ls -l')
    #IO.puts(output)

    {:noreply, socket
      #|> assign(:command, command)
      |> assign(:value, file)
    }
  end

  @impl true
  def handle_info({LightwarriorWeb.IPlayerLive.FormComponent, {:saved, i_player}}, socket) do
    {:noreply, stream_insert(socket, :iplayer, i_player)}
  end

  @impl true
  def handle_info({:DOWN, ref, :process, pid, :shutdown}, socket) do
    IO.puts("Process went down")
    {:noreply, socket}
  end

  @impl true
  def handle_event("phx:debug", %{"debug" => debug, "value" => _value}, socket) do
    #{:noreply, assign(socket, debug: debug)}
    {:noreply, socket
      |> assign(debug: debug)
      |> push_event("save-debug", %{debug: debug})
    }
  end

  @impl true
  def handle_event("phx:debug", %{"debug" => debug}, socket) do
    case debug do
      "null" -> {:noreply, socket}
      nil -> {:noreply, socket}
      "false" -> {:noreply, assign(socket, debug: false)}
      "true" -> {:noreply, assign(socket, debug: true)}
    end

  end

  @impl true
  def handle_event("delete", %{"id" => id}, socket) do
    i_player = Imageplayer.get_i_player!(id)
    {:ok, _} = Imageplayer.delete_i_player(i_player)

    {:noreply, stream_delete(socket, :iplayer, i_player)}
  end

  @impl true
  def handle_event("phx:get_layerdata_cache", %{"layerdata" => layerdata_cache}, socket) do
    #{:noreply, assign(socket, debug: debug)}
    {:ok, layerdata } = Jason.decode(layerdata_cache, [keys: :atoms])

    {:noreply, socket
    |> assign(layerdata: layerdata)
      #|> assign(debug: debug)
      #|> push_event("save-debug", %{debug: debug})
    }
  end


  @impl true
  def handle_event("dropped", %{"filename" => filename, "path" => path, "target" => target}, socket) do
    #{:noreply, assign_form(socket, changeset)}
    #gst-launch-1.0 -v filesrc location=./stanzraum.png ! decodebin ! imagefreeze ! videoconvert ! autovideosink
    #gst-launch-1.0 --gst-plugin-path=/usr/lib/gstreamer-1.0/ shmdatasrc socket-path=/tmp/blender_shmdata_camera_Camera ! videoconvert ! shmdatasink socket-path=/tmp/blender_shmdata_camera_converted
    IO.puts("dropped file")

    command_list = [
      "gst-launch-1.0 filesrc location=" <> path <> "/" <> filename,
      "decodebin",
      "videoconvert",
      "imagefreeze",
      "videoscale",
      "video/x-raw,width=1280,height=720",
      "autovideosink"
    ]

    command_list = [
      "gst-launch-1.0 --gst-plugin-path=/usr/local/lib/gstreamer-1.0/ filesrc location=" <> path <> "/" <> filename,
      "decodebin",
      "videoconvert",
      "imagefreeze",
      "videoscale",
      "video/x-raw,width=1280,height=720",
      "shmdatasink socket-path=/tmp/lightwarrior_layer1"
    ]

    #command = Enum.join(command_list, " ! ")
    original_file = path <> "/" <> filename

    images_extensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".webp", ".svg", ".ico"]
    video_extensions = [".mp4", ".mov", ".wmv", ".flv", ".avi", ".mkv", ".webm", ".vob", ".ogv", ".ogg", ".drc", ".mng", ".mts", ".m2ts", ".ts", ".mxf", ".roq", ".nsv", ".f4v", ".f4p", ".f4a", ".f4b"]

    original_file_extension = Path.extname(original_file)

    {:ok, file, type} = cond do
      Enum.member?(images_extensions, original_file_extension) ->
        #convert to fit output video caps
        {:ok, file} = Thumbnail.convert_original(original_file, %{w: 1920, h: 1080})
        {:ok, file, :image}
      Enum.member?(video_extensions, original_file_extension) ->
        {:ok, original_file, :video}
    end

    dbg(type)

    #dbg(Thumbnail.generate_thumbnail(path))
    #dbg(command)

    {:noreply, socket} = case Thumbnail.generate_thumbnail(file, %{w: 192, h: 192}) do
      {:ok, thumbnail_path, static_url} ->
        {:noreply, assign(socket, thumbnail_path: static_path(socket, static_url))}
      {:error, _reason} ->
        {:noreply, put_flash(socket, :error, "Failed to create thumbnail.")}
    end

    layer = case target do
      "i_player_image_layer_one" ->
        dbg(Map.get(socket.assigns.layerdata, :layer_one))
        #Map.replace(socket.assigns.layerdata.layer_one, :file, file)
        :layer_one
      "i_player_image_layer_two" ->
        #Map.replace(socket.assigns.layerdata.layer_two, :file, file)
        dbg(Map.get(socket.assigns.layerdata, :layer_two))
        :layer_two
    end

    data = socket.assigns.layerdata

    this_layer = Map.get(socket.assigns.layerdata, layer)
    |> Map.replace(:file, file)
    |> Map.replace(:thumbnail, socket.assigns.thumbnail_path)
    |> Map.replace(:type, type)

    new_layerdata = Map.replace(data, layer, this_layer)

    dbg(new_layerdata)

    {:noreply, socket
      #|> assign(:command, command)
      |> assign(:filename, filename)
      |> assign(:layerdata, new_layerdata)
      |> push_event("set-layer-data", new_layerdata)
    }

  end

  @impl true
  def handle_event("start_send_shmdata", %{"layer" => layer}, socket) do
    #{:noreply, assign_form(socket, changeset)}
    #gst-launch-1.0 -v filesrc location=./stanzraum.png ! decodebin ! imagefreeze ! videoconvert ! autovideosink

    #stop player workaround
    Lightwarrior.WebSocketClient.send_message_ossia_score(socket.assigns.ws_pid, %{ Message: "Stop" } )

    IO.puts("start shmdata transmission")
    IO.puts(layer)

    terminate_current = Enum.each(DynamicSupervisor.which_children(Lightwarrior.Imageplayer.GenserverSupervisor), fn x ->
      {:undefined, pid, :supervisor, [Lightwarrior.Imageplayer.GenserverInstance]} = x
      #dbg(x)

      {:registered_name, layer_name} = Process.info(pid, :registered_name)
      IO.puts(layer_name)
      IO.puts("#{layer}" == "#{layer_name}")

      if "#{layer}" == "#{layer_name}" do
        Process.unregister(layer_name)
        dbg(Lightwarrior.Imageplayer.GenserverSupervisor.terminate_worker(pid))

      end

    end)

    socket = case terminate_current do
      :ok ->
        {:ok, new_pid} = Lightwarrior.Imageplayer.GenserverSupervisor.start_worker(%{command: socket.assigns.command}, name: {:global, String.to_atom(layer)})
          Process.register(new_pid, String.to_atom(layer))
          assign(socket, :pid, new_pid)
      _  ->
          IO.puts("terminate current not ok!")
          socket
    end

    #websocket connection to ossia score
    dbg(socket.assigns.ws_pid)
    Lightwarrior.WebSocketClient.send_message_ossia_score(socket.assigns.ws_pid, %{ Message: "Play" } )

    {:noreply, socket
      #|> assign(:pid, "Pid: #{inspect pid}")
      #|> assign(:pid, pid)
    }

  end
end
