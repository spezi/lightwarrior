defmodule LightwarriorWeb.HyperionComponents do
  use Phoenix.Component
  use Gettext, backend: LightwarriorWeb.Gettext

  import LightwarriorWeb.CoreComponents
  use Phoenix.VerifiedRoutes, endpoint: LightwarriorWeb.Endpoint, router: LightwarriorWeb.Router

  #alias Phoenix.LiveView.JS

  def mapping_tool_menue(assigns) do
    #dbg(assigns.mapping_changeset)
    ~H"""
    <.form id={"mapping-menue-form-#{@side}"} for={@mapping_changeset} phx-change="validate">
    <div class="flex flex-row m-3 gap-1 text-xs">

        <.input type="hidden" field={@mapping_changeset[:side]} />

        <div class="w-fit p-1">
          <button :if={!@mapping_changeset[:lockdistance].value} title="lock distance" type="button" phx-click="phx:toggle-distance-lock" phx-value-side={@side} class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-lock-open-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button :if={@mapping_changeset[:lockdistance].value} title="unlock distance" type="button" phx-click="phx:toggle-distance-lock" phx-value-side={@side} class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-lock-closed-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
        </div>
        <!-- stripe length -->
        <div class="w-26">
            <.input type="text" field={@mapping_changeset[:instance_length]}  placeholder="Stripe length" />
        </div>
        <div>
          <div :if={Lightwarrior.State.get(:instances)} class="flex flex-row w-fit p-1">
            <select type="select" class="select select-sm">
            <!--
              <option selected></option>
              <option>Crimson</option>
              <option>Amber</option>
              <option>Velvet</option>-->
              <option disabled selected>copy from</option>
                <%= for instance <- Lightwarrior.State.get(:instances) do %>
                  <option value={instance["instance"]}><%= instance["friendly_name"] %></option>
                <% end %>
            </select>
          </div>
        </div>

        <!-- stripe functions  -->
        <div class="flex flex-row gap-1 w-64 grow m-1">
          <button
            title="set even y"
            type="button"
            phx-click="phx:set-even"
            value="y"
            class="btn btn-sm btn-neutral btn-square"
            >
            <svg class="size-5 opacity-40 group-hover:opacity-70"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <line x1="4" y1="12" x2="9" y2="12" />  <line x1="15" y1="12" x2="20" y2="12" />  <rect x="9" y="6" width="6" height="12" rx="2" /></svg>
          </button>
          <button
            title="set even x"
            type="button"
            phx-click="phx:set-even"
            value="x"
            class="btn btn-sm btn-neutral btn-square"
            >
            <svg class="size-5 opacity-40 group-hover:opacity-70"  width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">  <path stroke="none" d="M0 0h24v24H0z"/>  <line x1="12" y1="4" x2="12" y2="9" />  <line x1="12" y1="15" x2="12" y2="20" />  <rect x="6" y="9" width="12" height="6" rx="2" /></svg>
          </button>

          <span class="py-1">move: </span>
          <!--<input type="text" placeholder="steps" class="input input-sm w-16" />-->
          <button title="move left" type="button" phx-click="phx:move-stripe" phx-value-direction="left" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-left-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button title="move right" type="button" phx-click="phx:move-stripe" phx-value-direction="right" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-right-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button title="move up" type="button" phx-click="phx:move-stripe" phx-value-direction="up" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-up-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
          <button title="move down" type="button" phx-click="phx:move-stripe" phx-value-direction="down" class="btn btn-sm btn-neutral btn-square">
            <.icon name="hero-arrow-long-down-mini" class="size-5 opacity-40 group-hover:opacity-70" />
          </button>
        </div>

        <div class="p-4">
        <button :if={@side == "output"} title="refresh_background" type="button" phx-click="phx:refresh-output-image" class="btn btn-sm btn-secondary btn-square">
            <.icon name="hero-arrow-path-mini" class="size-5 opacity-40 group-hover:opacity-70" />
        </button>
        </div>

        <.input type="toggle" label="automap" field={@mapping_changeset[:automap]} disabled={@mapping_changeset[:side].value == "input" && "disabled"}/>

        <div class="w-24">
          <.input
            type="color"
            label="instances color"
            field={@mapping_changeset[:instances_color]}
          />
          <!-- <%= Kernel.inspect(@mapping_changeset[:instances_color].value)%> -->
        </div>

        <!-- opacity -->
        <.input type="range" label="Stripes opacity" phx-change="phx:stripe-opacity" field={@mapping_changeset[:stripes_opacity]} />

        <!-- opacity -->
        <.input type="range" label="Bg opacity" field={@mapping_changeset[:opacity]} />
    </div>
    </.form>
    """
  end

end
