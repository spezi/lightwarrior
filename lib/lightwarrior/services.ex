defmodule Lightwarrior.Services do
  use GenServer

  ## Public API

  def start_link(opts) do
    GenServer.start_link(__MODULE__, opts, name: __MODULE__)
  end

  def start_process(cmd, args \\ []) do
    GenServer.call(__MODULE__, {:start, cmd, args})
  end

  def stop_process do
    GenServer.call(__MODULE__, :stop)
  end

  def get_state do
    GenServer.call(__MODULE__, :state)
  end

  ## Server Callbacks

  def init(_opts) do
    {:ok, %{port: nil, output: "", status: :idle, exit_code: nil}}
  end

  def handle_call({:start, cmd, args}, _from, state) do
    if state.status == :running do
      {:reply, {:error, :already_running}, state}
    else
      port = Port.open(
        {:spawn_executable, System.find_executable(cmd)},
        [:binary, :exit_status, :stderr_to_stdout, args: args]
      )

      new_state = %{state | port: port, output: "", status: :running, exit_code: nil}
      {:reply, {:ok, port}, new_state}
    end
  end

  def handle_call(:stop, _from, %{port: nil} = state) do
    {:reply, :not_running, state}
  end

  def handle_call(:stop, _from, %{port: port} = state) do
    Port.close(port)
    {:reply, :ok, %{state | port: nil, status: :stopping}}
  end

  def handle_call(:state, _from, state) do
    {:reply, state, state}
  end

  # Handle async output
  def handle_info({port, {:data, data}}, state) do
    IO.puts(">> #{data}")
    {:noreply, %{state | output: state.output <> data}}
  end

  def handle_info({port, {:exit_status, code}}, state) do
    IO.puts("Process exited with code #{code}")
    {:noreply, %{state | status: :exited, exit_code: code, port: nil}}
  end
end
