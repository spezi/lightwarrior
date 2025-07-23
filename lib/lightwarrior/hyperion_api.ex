defmodule Lightwarrior.HyperionApi do
  use GenServer
  alias Lightwarrior.Hyperion

  def start_link(_opts) do
    GenServer.start_link(__MODULE__, %{}, name: __MODULE__)
  end

  def get_data do
    GenServer.call(__MODULE__, :get_data)
  end

  def refresh_data do
    GenServer.cast(__MODULE__, :refresh_data)
  end

  ## GenServer Callbacks

  @impl true
  def init(hyperion_state) do
    # Initial API fetch in background
    send(self(), :load_data)

    {:ok,
      %{
        serverinfo: nil,
        instances: nil,
      }
    }
  end

  @impl true
  def handle_info(:load_data, hyperion_state) do

    serverinfo = case Hyperion.get_serverinfo() do
      {:ok, serverinfo} ->
        Lightwarrior.State.put(:serverinfo, serverinfo)
        serverinfo
      {:error, :econnrefused} -> nil
    end

    instances = case Hyperion.collect_instances(serverinfo) do
      {:ok, instances } ->
        Lightwarrior.State.put(:instances, instances)
        instances
        instances
      {:error, nil} -> nil
    end

    hyperion_state =  %{
      serverinfo: serverinfo,
      instances: instances,
    }

    #dbg(hyperion_state)
    Phoenix.PubSub.broadcast(Lightwarrior.PubSub, "hyperion_ready", %{ "hyperion_ready" => true })

    {:noreply, hyperion_state}
  end

  @impl true
  def handle_call(:get_data, _from, hyperion_state) do
    {:reply, hyperion_state, hyperion_state}
  end

  @impl true
  def handle_cast(:refresh_data, hyperion_state) do
    #data = Hyperion.get_serverinfo()
    #{:noreply, %{hyperion_state | serverinfo: data}}
    send(self(), :load_data)
    {:noreply, hyperion_state}
  end

end
