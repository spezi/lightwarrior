
defmodule Lightwarrior.Imageplayer.GenserverSupervisor do
  use DynamicSupervisor

  def start_link(_) do
    DynamicSupervisor.start_link(__MODULE__, :ok, name: __MODULE__)
  end

  def init(:ok) do
    dbg("init dynamic supervisor")
    DynamicSupervisor.init(strategy: :one_for_one)
  end

  def start_worker(arg, _opts) do
    #spec = {Lightwarrior.Imageplayer.GenserverInstance, arg}
    #DynamicSupervisor.start_child(__MODULE__, spec)
    child_spec = %{
      id: Lightwarrior.Imageplayer.GenserverInstance,
      start: {Lightwarrior.Imageplayer.GenserverInstance, :start_link, [arg]},
      #start: Lightwarrior.Imageplayer.GenserverInstance.start_link(%{command: socket.assigns.command, socket: socket}, name: :layer_one),
      #restart: :transient,
      #shutdown: 5000,
      type: :supervisor
    }

    dbg(DynamicSupervisor.start_child(__MODULE__, child_spec))
  end

  def terminate_worker(pid) do
    DynamicSupervisor.terminate_child(__MODULE__, pid)
  end
end

defmodule Lightwarrior.Imageplayer.GenserverInstance do

  use GenServer
  require Logger

  # GenServer API
  def start_link(args \\ [], opts \\ []) do
    dbg(args)
    dbg(opts)
    GenServer.start_link(__MODULE__, args, name: {:global, "layer_one"})
  end

  def via_tuple(name) do
    {:via, Registry, {Lightwarrior.Imageplayer.Registry, name}}
  end

  def init(args \\ []) do
    Process.flag(:trap_exit, true)
    #dbg(args)
    #port = Port.open({:spawn, @command}, [:binary, :exit_status])
    port = Port.open({:spawn, args.command}, [:binary, :exit_status])
    Port.monitor(port)



    {:ok, %{port: port, latest_output: nil, exit_status: nil} }
    #{:ok, %{}}
  end

  def get_port_info(pid) do
    GenServer.call(pid, :get_port_info)
  end

  def terminate(reason, %{port: port} = state) do
    Logger.info "** TERMINATE: #{inspect reason}. This is the last chance to clean up after this process."
    Logger.info "Final state: #{inspect state}"

    #send(port, {self(), {:command, "SIGINT"}})
    #dbg(Port.call port, SIGINT)
    #dbg(send(port, "SIGINT"))
    dbg(port)

    port_info = Port.info(port)
    os_pid = port_info[:os_pid]

    Logger.warn "Orphaned OS process: #{os_pid}"

     # Send SIGINT signal to the external process
     :os.cmd('kill -s INT #{os_pid}')

     # Optionally, you can handle the exit status of the port
     receive do
       {^port, {:exit_status, status}} ->
         IO.puts("Process exited with status: #{status}")
     end

    :normal
  end

  # This callback handles data incoming from the command's STDOUT
  def handle_info({port, {:data, text_line}}, %{port: port} = state) do
    Logger.info "Data: #{inspect text_line}"
    {:noreply, %{state | latest_output: String.trim(text_line)}}
  end

  # This callback tells us when the process exits
  def handle_info({port, {:exit_status, status}}, %{port: port} = state) do
    Logger.info "Port exit: :exit_status: #{status}"

    new_state = %{state | exit_status: status}

    {:noreply, new_state}
  end

  def handle_info({:DOWN, _ref, :port, port, :normal}, state) do
    Logger.info "Genserver Handled :DOWN message from port: #{inspect port}"
    {:noreply, state}
  end

  def handle_info({:EXIT, port, :normal}, state) do
    Logger.info "handle_info: EXIT"
    {:noreply, state}
  end

  def handle_info(msg, state) do
    Logger.info "Unhandled message: #{inspect msg}"
    {:noreply, state}
  end

end

defmodule Lightwarrior.Imageplayer.GenserverInstance_BAK do

  use GenServer
  require Logger

  # GenServer API
  def start_link(args \\ [], opts \\ []) do
    GenServer.start_link(__MODULE__, args, opts)
  end

  def init(args \\ []) do
    #dbg(args)
    #port = Port.open({:spawn, @command}, [:binary, :exit_status])
    port = Port.open({:spawn, args.command}, [:binary, :exit_status])
    Port.monitor(port)

    {:ok, %{port: port, latest_output: nil, exit_status: nil} }
    #{:ok, %{}}
  end

  # This callback handles data incoming from the command's STDOUT
  def handle_info({port, {:data, text_line}}, %{port: port} = state) do
    Logger.info "Data: #{inspect text_line}"
    {:noreply, %{state | latest_output: String.trim(text_line)}}
  end

  # This callback tells us when the process exits
  def handle_info({port, {:exit_status, status}}, %{port: port} = state) do
    Logger.info "Port exit: :exit_status: #{status}"

    new_state = %{state | exit_status: status}

    {:noreply, new_state}
  end

  def handle_info({:DOWN, _ref, :port, port, :normal}, state) do
    Logger.info "Handled :DOWN message from port: #{inspect port}"
    {:noreply, state}
  end

  def handle_info(msg, state) do
    Logger.info "Unhandled message: #{inspect msg}"
    {:noreply, state}
  end

end

defmodule Lightwarrior.Imageplayer do
  @moduledoc """
  The Imageplayer context.
  """

  import Ecto.Query, warn: false
  alias Lightwarrior.Repo

  alias Lightwarrior.Imageplayer.IPlayer

  require Logger

  @doc """
  Returns the list of iplayer.

  ## Examples

      iex> list_iplayer()
      [%IPlayer{}, ...]

  """
  def list_iplayer do
    raise "TODO"
  end

  @doc """
  Gets a single i_player.

  Raises if the I player does not exist.

  ## Examples

      iex> get_i_player!(123)
      %IPlayer{}

  """
  def get_i_player!(id), do: raise "TODO"

  def get_i_player!() do
    %{ id: "34"}
  end

  @doc """
  Creates a i_player.

  ## Examples

      iex> create_i_player(%{field: value})
      {:ok, %IPlayer{}}

      iex> create_i_player(%{field: bad_value})
      {:error, ...}

  """
  def create_i_player(attrs \\ %{}) do
    raise "TODO"
  end

  @doc """
  Updates a i_player.

  ## Examples

      iex> update_i_player(i_player, %{field: new_value})
      {:ok, %IPlayer{}}

      iex> update_i_player(i_player, %{field: bad_value})
      {:error, ...}

  """
  #def update_i_player(%IPlayer{} = i_player, attrs) do
   # raise "TODO"
  #end

  @doc """
  Deletes a IPlayer.

  ## Examples

      iex> delete_i_player(i_player)
      {:ok, %IPlayer{}}

      iex> delete_i_player(i_player)
      {:error, ...}

  """
  #def delete_i_player(%IPlayer{} = i_player) do
  #  raise "TODO"
  #end

  @doc """
  Returns a data structure for tracking i_player changes.

  ## Examples

      iex> change_i_player(i_player)
      %Todo{...}

  """
  def change_i_player(i_player, _attrs \\ %{}) do
    #raise "TODO"
    %{}
  end

  #@command "gst-launch-1.0 -v filesrc location=/Users/spezi/Pictures/stanzraum.png ! decodebin ! videoconvert ! imagefreeze ! autovideosink"
end
