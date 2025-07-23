defmodule Lightwarrior.State do
   use GenServer

    @name __MODULE__

    # examples
    # Lightwarrior.State.put(:current_user_count, 5)
    # Lightwarrior.State.get(:current_user_count)
    # Lightwarrior.State.delete(:current_user_count)
    # Lightwarrior.State.all()

    # Public API

    def start_link(_opts) do
      GenServer.start_link(__MODULE__, %{}, name: @name)
    end

    def put(key, value) do
      GenServer.cast(@name, {:put, key, value})
    end

    def get(key) do
      GenServer.call(@name, {:get, key})
    end

    def delete(key) do
      GenServer.cast(@name, {:delete, key})
    end

    def all do
      GenServer.call(@name, :all)
    end

    # Server Callbacks

    def init(state) do
      {:ok, state}
    end

    def handle_call({:get, key}, _from, state) do
      {:reply, Map.get(state, key), state}
    end

    def handle_call(:all, _from, state) do
      {:reply, state, state}
    end

    def handle_cast({:put, key, value}, state) do
      {:noreply, Map.put(state, key, value)}
    end

    def handle_cast({:delete, key}, state) do
      {:noreply, Map.delete(state, key)}
    end

end
