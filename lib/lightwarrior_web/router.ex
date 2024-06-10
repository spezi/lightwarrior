defmodule LightwarriorWeb.Router do
  use LightwarriorWeb, :router

  pipeline :browser do
    plug :accepts, ["html"]
    plug :fetch_session
    plug :fetch_live_flash
    plug :put_root_layout, html: {LightwarriorWeb.Layouts, :root}
    plug :protect_from_forgery
    plug :put_secure_browser_headers
  end

  pipeline :api do
    plug :accepts, ["json"]
  end

  scope "/", LightwarriorWeb do
    pipe_through :browser

    get "/", PageController, :home

    live "/hyperion/ledmappings", HyperionLEDMappingLive.Index, :index
    live "/hyperion/ledmappings/new", HyperionLEDMappingLive.Index, :new
    live "/hyperion/ledmappings/:selected/edit", HyperionLEDMappingLive.Index, :edit

    #live "/hyperion/ledmappings/:id", HyperionLEDMappingLive.Show, :show
    #live "/hyperion/ledmappings/:id/show/edit", HyperionLEDMappingLive.Show, :edit

  end

  # Other scopes may use custom stacks.
  # scope "/api", LightwarriorWeb do
  #   pipe_through :api
  # end

  # Enable LiveDashboard and Swoosh mailbox preview in development
  if Application.compile_env(:lightwarrior, :dev_routes) do
    # If you want to use the LiveDashboard in production, you should put
    # it behind authentication and allow only admins to access it.
    # If your application does not have an admins-only section yet,
    # you can use Plug.BasicAuth to set up some basic authentication
    # as long as you are also using SSL (which you should anyway).
    import Phoenix.LiveDashboard.Router

    scope "/dev" do
      pipe_through :browser

      live_dashboard "/dashboard", metrics: LightwarriorWeb.Telemetry
      forward "/mailbox", Plug.Swoosh.MailboxPreview
    end
  end
end
