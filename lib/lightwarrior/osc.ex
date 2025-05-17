defmodule Lightwarrior.Hyperion.SC do
  use GenServer

  @impl true
  def init(_state) do
    # Open a port and add the UDP socket to the state
    {:ok, socket} = :gen_udp.open(0, [:binary, {:active, true}])
    {:ok, socket}
  end

  @impl true
  def handle_cast({:send, osc_bin_msg}, state) do
    # This could be changed to named address, like 'localhost'
    ip_address = ~c"localhost"
    sc_port_num = 9997
    :gen_udp.send(state, ip_address, sc_port_num, osc_bin_msg)

    {:noreply, state}
  end

  @impl true
  def handle_info(msg, state) do
    case msg do
      {:udp, _process_port, _ip_addr, _port_num, res} ->
        IO.inspect(res, label: "Binary message received")
        IO.inspect(Message.decode(res), label: "\nDecoded message")
        state

      _ ->
        state
    end

    {:noreply, state}
  end

  def start_link() do
    GenServer.start_link(Lightwarrior.Hyperion.SC, nil)
  end

  def send(pid, osc_bin_msg) do
    GenServer.cast(pid, {:send, osc_bin_msg})
  end
end