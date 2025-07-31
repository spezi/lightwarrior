# Lightwarrior

## Prerequisites

```
npm install --prefix assets
```

### Arch Linux

```
pacman -S \
    elixir \
    erlang-asn1 \
    erlang-parsetools \
    erlang-public_key \
    erlang-ssl \
    erlang-syntax_tools \
    erlang-xmerl
```

## Setup


```
cp .env.template .env
```
-> and set Hyperion URL and API Key 

Ossia score osc adress 127.0.0.1 port 9997

To start your Phoenix server:

* Run `mix setup` to install and setup dependencies
* Start Phoenix endpoint with `mix phx.server` or inside IEx with `iex -S mix phx.server`

Now you can visit [`localhost:4000`](http://localhost:4000) from your browser.

Ready to run in production? Please [check our deployment guides](https://hexdocs.pm/phoenix/deployment.html).

## Learn more

* Official website: https://www.phoenixframework.org/
* Guides: https://hexdocs.pm/phoenix/overview.html
* Docs: https://hexdocs.pm/phoenix
* Forum: https://elixirforum.com/c/phoenix-forum
* Source: https://github.com/phoenixframework/phoenix

## Hyperion 
* Network Services
    * set API Authentication
    * Local API Authentication

    * generate token 

-> cp .env.template .env and set env variables

### Code Navigator

#### for restore on init localstorage

* application.ex -> starten des hyperion api clients im hintergrund 

* add field to: lib/lightwarrior/structs/mapping_menue.ex
    * add field to cast
* set default on mount in lib/lightwarrior_web/live/hyperion_config_live/index.ex
* push_event if wants to save in local storage :
    * |> push_event("localstorage", %{ input_opacity: mapping_tools_form["opacity"] })
    * catch event in js hooks and push data to localstorage 

#### stripes update ->


"global config" -> "Build Ossia Score File" (build processes json and outputshader.fs) -> manual copy shader code to score ->
"Patch score Addresses" -> set osc addresses and generate lightwarrior_patched.score 