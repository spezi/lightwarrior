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
    payload = %{
      "command" => "instance",
      "subcommand" => "switchTo",
      "instance" => stripe.instance
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
      {"Authorization", "token 58208a8f-eaaa-4fac-b644-f164fc46ff21"}
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

end
