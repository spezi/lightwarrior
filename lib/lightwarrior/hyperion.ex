defmodule Lightwarrior.Hyperion do
  @moduledoc """
  The Hyperion context.
  """

  #import Ecto.Query, warn: false
  #alias Lightwarrior.Repo

  alias Lightwarrior.Hyperion.HyperionLEDMapping
  alias Lightwarrior.Helper
  require Logger

  @doc """
  Returns the list of hyperionledmappings.

  ## Examples

      iex> list_hyperionledmappings()
      [%HyperionLEDMapping{}, ...]

  """
  def list_hyperionledmappings do
    #raise "TODO"
    %{}
  end

  @doc """
  Get initial Hyperion Server Info
  """
  def get_serverinfo do

    payload = %{
      "command" => "serverinfo",
      "subscribe" => ["all"],
      "tan" => 1
    }

    post_json(payload)

  end

  @doc """
  Get config of any stripe
  """
  def switch_instance(stripe) do

    #dbg("switch instance #{stripe.instance}" )

    instance = case Map.has_key?(stripe, :instance) do
      true -> Map.get(stripe, :instance)
      false -> Map.get(stripe, "instance")
    end

    payload = %{
      "command" => "instance",
      "subcommand" => "switchTo",
      "instance" => instance
    }
    post_json(payload)

  end

  @doc """
  Get config of current active stripe
  """
  def get_current_config do

    payload = %{
      "command" => "config",
      "subcommand" => "getconfig",
      "tan" => 1
    }

    post_json(payload)

  end

  @doc """
  Save config of current active stripe
  """
  def save_current_config(config) do

    payload = %{
      "command" => "config",
      "subcommand" => "setconfig",
      "config" => config,
      "tan" => 1
    }

    case post_json(payload) do
      {:ok, response} ->
        response
      {:error, reason} ->
        IO.inspect(reason, label: "Error")
        {:error, reason}
    end

  end

  @doc """
  Get config of any stripe
  """
  def get_all_stripes_config(stripes) do

    stripes = Enum.map_every(stripes, 1, fn stripe ->
      config = case switch_instance(stripe) do
        {:ok, switch} -> get_current_config()
        {:error, error} -> error
      end

      config = case config do
        {:ok, config} -> config
        {:error, error} -> error
      end

      Map.put(stripe, :config, config)
    end)

    #dbg(stripes)
    {:ok, stripes}
  end

  @doc """
  Returns the list of hyperionledmappings.

  ## Examples

      iex> list_hyperionledmappings()
      [%HyperionLEDMapping{}, ...]

  """
  def collect_stripes(serverinfo) do
    Logger.info("collect stripes")
    %{"info" => info} = serverinfo
    %{"instance" => stripes } = info
    stripes = Enum.map_every(stripes, 1, fn stripe -> Helper.string_keys_to_atom_keys(stripe) end)
    #raise "TODO"
    {:ok, stripes}
  end

  def get_instance_leds(current_config) do
    case current_config["success"] do
      true ->
        info = Map.get(current_config, "info", %{})
        Map.get(info, "leds", [])
      false -> []
      end
  end

  defp post_json(payload) do
    url = "http://127.0.0.1:8090/json-rpc"
    headers = [
      {"Content-Type", "application/json"},
      {"Authorization", "token d894c547-5ca8-449d-8c27-a646102cdeec"}
    ]
    body = Jason.encode!(payload)
    #dbg(headers)

    case HTTPoison.post(url, body, headers) do
      {:ok, %HTTPoison.Response{status_code: 200, body: response_body}} ->
        case Jason.decode(response_body) do
          {:ok, json} ->
            #Logger.info("Request successful: #{Jason.encode_to_iodata!(json, pretty: true)}")
            Logger.info("Request to #{url} successful!")
            {:ok, json}

          {:error, decode_error} ->
            Logger.error("Failed to decode JSON response: #{inspect(decode_error)}")
            {:error, :invalid_json}
        end

      {:ok, %HTTPoison.Response{status_code: status_code, body: response_body}} ->
        Logger.error("Request failed with status #{status_code}: #{response_body}")
        {:error, {:http_error, status_code, response_body}}

      {:error, %HTTPoison.Error{reason: reason}} ->
        Logger.error("HTTP request failed: #{inspect(reason)}")
        {:error, reason}
    end
  end

    @doc """
  Save config of current active stripe
  """
  def update_stripe_ossia(leds, selected) do

    #dbg(leds)
    start = calculate_center(Enum.at(leds, 0))
    stop = calculate_center(Enum.at(leds, -1))
    #dbg(start)
    #dbg(stop)
    update_ossia_via_osc(start, stop, selected)
  end

  defp calculate_center(coords) do
    h_center = (coords["hmax"] + coords["hmin"]) / 2
    v_center = (coords["vmax"] + coords["vmin"]) / 2
    %{
      "h" => h_center,
      "v" => v_center
    }
  end

  defp update_ossia_via_osc(start, stop, selected) do
    # IP or host and port number for the UDP connection
    ip_address = '127.0.0.1' # This could be changed to named address, like 'localhost'
    port_num = 9997 # In this example, this is the default port used by Protokol

    # Open a port
    {:ok, port} = :gen_udp.open(0, [:binary, {:active, true}])

    start_address = "/start" <> Integer.to_string(selected)
    end_address = "/end" <> Integer.to_string(selected)

    # Encode the message
    osc_message_start = %OSCx.Message{address: start_address, arguments: [start["h"], start["v"]]} |> OSCx.encode()
    osc_message_end = %OSCx.Message{address: end_address, arguments: [stop["h"], stop["v"]]} |> OSCx.encode()

    # Send message
    dbg(:gen_udp.send(port, ip_address, port_num, osc_message_start))
    dbg(:gen_udp.send(port, ip_address, port_num, osc_message_end))

    # Close the port
    :gen_udp.close(port)

  end

end
