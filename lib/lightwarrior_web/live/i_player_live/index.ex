defmodule LightwarriorWeb.IPlayerLive.Index do
  use LightwarriorWeb, :live_view

  alias Lightwarrior.Imageplayer
  alias Lightwarrior.Imageplayer.IPlayer

  @impl true
  def mount(_params, _session, socket) do
    #{:ok, stream(socket, :iplayer, Imageplayer.list_iplayer())}
    #path = Path.absname("~")
    #{:ok, files} = File.ls(path)

    {:ok, socket
      |> assign(:debug, false)
      |> assign(:command, ["gst-launch"])
      |> assign(:pid, nil)
      |> assign(:file, nil)
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


  @impl true
  def handle_info({LightwarriorWeb.IPlayerLive.IPlayerFormComponent, {:selected, file}}, socket) do
    IO.puts("select file")
    #IO.puts(filename)

    command_list = [
      "gst-launch-1.0 --gst-plugin-path=/usr/lib/gstreamer-1.0/ filesrc location=" <> file,
      "decodebin",
      "videoconvert",
      "imagefreeze",
      "shmdatasink socket-path=/tmp/lightwarrior_layer1"
    ]

    command_list = [
      "gst-launch-1.0 filesrc location=" <> file,
      "decodebin",
      "videoconvert",
      "imagefreeze",
      "videoscale",
      "video/x-raw,width=1280,height=720",
      "autovideosink"
    ]

    command = Enum.join(command_list, " ! ")

    #output = :os.cmd('ls -l')
    #IO.puts(output)

    {:noreply, socket
      |> assign(:command, command)
      |> assign(:value, file)
    }
  end

  @impl true
  def handle_info({LightwarriorWeb.IPlayerLive.FormComponent, {:saved, i_player}}, socket) do
    {:noreply, stream_insert(socket, :iplayer, i_player)}
  end

  @impl true
  def handle_info({:DOWN, ref, :process, {nil, :nonode@nohost}, :noproc}, socket) do
    IO.puts("Process went down")
    {:noreply, socket}
  end

  @impl true
  def handle_event("phx:debug", %{"debug" => debug}, socket) do
    #{:noreply, assign(socket, debug: debug)}
    {:noreply, socket
      |> assign(debug: debug)
      |> push_event("save-debug", %{debug: debug})
    }
  end

  @impl true
  def handle_event("delete", %{"id" => id}, socket) do
    i_player = Imageplayer.get_i_player!(id)
    {:ok, _} = Imageplayer.delete_i_player(i_player)

    {:noreply, stream_delete(socket, :iplayer, i_player)}
  end


  @impl true
  def handle_event("dropped", %{"filename" => filename, "path" => path}, socket) do
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

    command = Enum.join(command_list, " ! ")

    {:noreply, socket
    |> assign(:command, command)
    |> assign(:value, filename)
    |> push_event("file-drag", %{path: path,filename: filename})
    }

  end

  @impl true
  def handle_event("start_send_shmdata", %{"value" => value}, socket) do
    #{:noreply, assign_form(socket, changeset)}
    #gst-launch-1.0 -v filesrc location=./stanzraum.png ! decodebin ! imagefreeze ! videoconvert ! autovideosink
    IO.puts("start shmdata transmission")
    #output = :os.cmd('ls -l')
    #output = :os.cmd(socket.assigns.command)
    #{output, exit_code} = System.cmd(socket.assigns.command, [])
    #IO.puts(output)
    #IO.puts("Exit code: #{exit_code}")

    #exit_status = if socket.assigns.pid != nil do
      #dbg(:sys.get_state(socket.assigns.pid))
    #  %{
    #    exit_status: exit_status,
     #   port: port,
     #   latest_output: latest_output
     # } = :sys.get_state(socket.assigns.pid)
     # exit_status
    #else
    #  1
    #end

    #dbg(exit_status)

    #socket = case exit_status do
    #  nil -> socket
    #  _ -> assign(socket, :pid, nil)
    #end

    #dbg(socket.assigns.pid)

    #{:ok, pid} = case socket.assigns.pid do
    #  nil -> Lightwarrior.Imageplayer.start_link(%{command: socket.assigns.command, socket: socket})
    #  _ ->
    #    {:ok, nil}
    #end

    dbg(socket.assigns.pid)

    pid = case socket.assigns.pid do
      nil ->
        {:ok, pid} = Lightwarrior.Imageplayer.start_link(%{command: socket.assigns.command, socket: socket})
        pid
      _ ->
        %{
          exit_status: exit_status,
          port: port,
          latest_output: latest_output
         } = :sys.get_state(socket.assigns.pid)
        if exit_status == nil do
          socket.assigns.pid
        else
          {:ok, pid} = Lightwarrior.Imageplayer.start_link(%{command: socket.assigns.command, socket: socket})
          pid
        end

    end

    dbg(pid)

    {:noreply, socket
      #|> assign(:pid, "Pid: #{inspect pid}")
      |> assign(:pid, pid)
    }

  end
end
