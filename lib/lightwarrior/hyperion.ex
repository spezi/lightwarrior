defmodule Lightwarrior.Hyperion do
  @moduledoc """
  The Hyperion context.
  """

  #import Ecto.Query, warn: false
  alias Lightwarrior.Repo
  alias Lightwarrior.Hyperion.HyperionConfig
  alias Lightwarrior.Helper
  require Logger

  @doc """
  Returns the list of hyperionconfigs.

  ## Examples

      iex> list_hyperionconfigs()
      [%HyperionConfig{}, ...]

  """
  def list_hyperionconfigs do
    raise "TODO"
  end

  @doc """
  Gets a single hyperion_config.

  Raises if the Hyperion config does not exist.

  ## Examples

      iex> get_hyperion_config!(123)
      %HyperionConfig{}

  """
  def get_hyperion_config!(_id), do: raise "TODO"

  @doc """
  Creates a hyperion_config.

  ## Examples

      iex> create_hyperion_config(%{field: value})
      {:ok, %HyperionConfig{}}

      iex> create_hyperion_config(%{field: bad_value})
      {:error, ...}

  """
  def create_hyperion_config(_attrs) do
    raise "TODO"
  end

  @doc """
  Updates a hyperion_config.

  ## Examples

      iex> update_hyperion_config(hyperion_config, %{field: new_value})
      {:ok, %HyperionConfig{}}

      iex> update_hyperion_config(hyperion_config, %{field: bad_value})
      {:error, ...}

  """
  #def update_hyperion_config(%HyperionConfig{} = hyperion_config, attrs) do
  #  raise "TODO"
  #end

  @doc """
  Deletes a HyperionConfig.

  ## Examples

      iex> delete_hyperion_config(hyperion_config)
      {:ok, %HyperionConfig{}}

      iex> delete_hyperion_config(hyperion_config)
      {:error, ...}

  """
  #def delete_hyperion_config(%HyperionConfig{} = hyperion_config) do
  #  raise "TODO"
  #end

  @doc """
  Returns a data structure for tracking hyperion_config changes.

  ## Examples

      iex> change_hyperion_config(hyperion_config)
      %Todo{...}


  #def change_hyperion_config(%HyperionConfig{} = hyperion_config, _attrs \\ %{}) do
  #  raise "TODO"
  #end
  """

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
  def get_all_instances_config(instances) do
    if instances != nil do
      instances = Enum.map_every(instances, 1, fn instance ->
        config = case switch_instance(instance) do
          {:ok, switch} -> get_current_config()
          {:error, error} -> error
        end

        config = case config do
          {:ok, config} -> config
          {:error, error} -> error
        end

        Map.put(instance, "config", config)
      end)

      #dbg(instances)
      {:ok, instances}
    else
      {:error, nil}
    end
  end

  @doc """
  Returns the list of hyperionledmappings.

  ## Examples

      iex> list_hyperionledmappings()
      [%HyperionLEDMapping{}, ...]

  """
  def collect_instances(serverinfo) do
    if serverinfo do
      Logger.info("collect instances")
      %{"info" => info} = serverinfo
      %{"instance" => instances } = info
      #instances = Enum.map_every(instances, 1, fn instance -> Helper.string_keys_to_atom_keys(instance) end)
      #raise "TODO"
      #dbg(instances)
      {:ok, instances}
    else
      {:error, nil}
    end
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
    #url = "http://127.0.0.1:8090/json-rpc"
    url = Application.get_env(:lightwarrior, :hyperion)[:hyperion_jsonrpc_api_url]
    #dbg(url)
      #{"Authorization", "token d894c547-5ca8-449d-8c27-a646102cdeec"}
      #{"Authorization", "token cbe57e29-b42c-490e-81c9-4b9510aa767a"}
    headers = [
      {"Content-Type", "application/json"},
      {"Authorization", "token " <> Application.get_env(:lightwarrior, :hyperion)[:hyperion_jsonrpc_api_token]}
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
