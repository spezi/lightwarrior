defmodule Lightwarrior.WebSocketClient do
  use WebSockex

  def start_link(url) do
    WebSockex.start_link(url, __MODULE__, %{})
  end

  @impl true
  def handle_connect(conn, state) do
    IO.puts "Connected!"
    dbg(state)
    dbg(conn)

    {:ok, state}
  end

  @impl true
  def handle_frame({:text, msg}, state) do
    #IO.puts "Received message: #{msg}"
    {:ok, state}
  end

  @impl true
  def handle_cast({:send, {type, msg} = frame}, state) do
    IO.puts "Sending #{type} frame with payload: #{msg}"
    {:reply, frame, state}
  end

  @impl true
  def handle_disconnect(_reason, state) do
    IO.puts "Disconnected!"
    {:ok, state}
  end

  def send_message(pid, message) do
    WebSockex.send_frame(pid, {:text, message})
  end

  def send_message_ossia_score(pid, message) do
    {status, result} = Jason.encode(message)
    WebSockex.send_frame(pid, {:text, result})
  end

end
