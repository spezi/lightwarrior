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
  def init(state) do
    # Initial API fetch in background
    send(self(), :load_initial_data)

    {:ok,
      %{
        serverinfo: nil,
        stripes: nil,
        stripes_with_config: nil
      }
    }
  end

  @impl true
  def handle_info(:load_initial_data, state) do

    serverinfo = case Hyperion.get_serverinfo() do
      {:ok, serverinfo} -> serverinfo
      {:error, :econnrefused} -> nil
    end

    stripes = case Hyperion.collect_stripes(serverinfo) do
      {:ok, stripes } -> stripes
      {:error, nil} -> nil
    end

    stripes_with_config = case Hyperion.get_all_stripes_config(stripes) do
      {:ok, stripes_with_config } -> stripes_with_config
      {:error, nil} -> nil
    end

    #dbg(serverinfo)
    #dbg(stripes)
    #dbg(stripes_with_config)

    state =  %{
      serverinfo: serverinfo,
      stripes: stripes,
      stripes_with_config: stripes_with_config,
      stripes_with_config_input: nil
    }

    #dbg(state)
    Phoenix.PubSub.broadcast(Lightwarrior.PubSub, "state", state)

    {:noreply, state}
  end

  @impl true
  def handle_call(:get_data, _from, state) do
    {:reply, state, state}
  end

  @impl true
  def handle_cast(:refresh_data, state) do
    #data = Hyperion.get_serverinfo()
    #{:noreply, %{state | serverinfo: data}}
    send(self(), :load_initial_data)
    {:noreply, state}
  end

end
