defmodule Lightwarrior.InputMappingFileStore do
  use GenServer

  @name __MODULE__
  @file_path "data/global_store.json"

  ## Public API

  # examples
  # Get/set in-memory
  # Lightwarrior.InputMappingFileStore.put(:api_key, "123abc")
  # Lightwarrior.InputMappingFileStore.get(:api_key)
  # Lightwarrior.InputMappingFileStore.all()

  # Write to file
  # Lightwarrior.InputMappingFileStore.persist()

  # Reload from file
  # Lightwarrior.InputMappingFileStore.reload()

  def start_link(_opts) do
    GenServer.start_link(__MODULE__, %{}, name: @name)
  end

  def get(key), do: GenServer.call(@name, {:get, key})
  def put(key, value), do: GenServer.call(@name, {:put, key, value})
  def delete(key), do: GenServer.call(@name, {:delete, key})
  def all, do: GenServer.call(@name, :all)

  def reload, do: GenServer.cast(@name, :reload)
  def persist, do: GenServer.cast(@name, :persist)

  ## Server Callbacks

  def init(_state) do
    state = load_from_file()
    {:ok, state}
  end

  def handle_call({:get, key}, _from, state), do: {:reply, Map.get(state, key), state}
  def handle_call({:put, key, value}, _from, state), do: {:reply, :ok, Map.put(state, key, value)}
  def handle_call({:delete, key}, _from, state), do: {:reply, :ok, Map.delete(state, key)}
  def handle_call(:all, _from, state), do: {:reply, state, state}

  def handle_cast(:reload, _state) do
    new_state = load_from_file()
    {:noreply, new_state}
  end

  def handle_cast(:persist, state) do
    persist_to_file(state)
    {:noreply, state}
  end

  ## Helpers

  defp load_from_file do
    case File.read(@file_path) do
      {:ok, content} ->
        case Jason.decode(content) do
          {:ok, data} when is_map(data) -> data
          _ -> %{}
        end

      _ -> %{}
    end
  end

  defp persist_to_file(state) do
    File.write!(@file_path, Jason.encode!(state, pretty: true))
  end
end
