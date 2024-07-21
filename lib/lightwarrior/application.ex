defmodule Lightwarrior.Application do
  # See https://hexdocs.pm/elixir/Application.html
  # for more information on OTP Applications
  @moduledoc false

  use Application

  @impl true
  def start(_type, args) do
    children = [
      LightwarriorWeb.Telemetry,
      {DNSCluster, query: Application.get_env(:lightwarrior, :dns_cluster_query) || :ignore},
      {Phoenix.PubSub, name: Lightwarrior.PubSub},
      # Start the Finch HTTP client for sending emails
      {Finch, name: Lightwarrior.Finch},
      # Start a worker by calling: Lightwarrior.Worker.start_link(arg)
      # {Lightwarrior.Worker, arg},
      # Start to serve requests, typically the last entry
      LightwarriorWeb.Endpoint,
      {DynamicSupervisor, name: Lightwarrior.Imageplayer.GenserverSupervisor, strategy: :one_for_one},
      #{Lightwarrior.Imageplayer.GenserverSupervisor, [name: GenserverSupervisor]},
      #{Lightwarrior.MyTracker, [name: MyTracker, pubsub_server: Lightwarrior.PubSub]}
      #{Lightwarrior.Imageplayer.LayerTracker, []},
      #{Lightwarrior.Worker, args}
    ]

    # See https://hexdocs.pm/elixir/Supervisor.html
    # for other strategies and supported options
    opts = [strategy: :one_for_one, name: Lightwarrior.Supervisor]
    Supervisor.start_link(children, opts)
  end

  # Tell Phoenix to update the endpoint configuration
  # whenever the application is updated.
  @impl true
  def config_change(changed, _new, removed) do
    LightwarriorWeb.Endpoint.config_change(changed, removed)
    :ok
  end
end
