# Lightwarrior

To start your Phoenix server:

  * Run `mix setup` to install and setup dependencies
  * Start Phoenix endpoint with `mix phx.server` or inside IEx with `iex -S mix phx.server`

Now you can visit [`localhost:4000`](http://localhost:4000) from your browser.

## generators

mix phx.gen.live Hyperion --no-schema
mix phx.gen.live Imageplayer --no-schema

mix phx.gen.live Imageplayer IPlayer iplayer --no-schema


# screenshot ossia

gst-launch-1.0 --gst-plugin-path=/usr/lib/gstreamer-1.0/ \
  shmdatasrc socket-path=/tmp/score_shm_video \
  ! videoconvert \
  ! jpegenc \
  ! multifilesink location=stripes.jpg next-file=key-frame max-files=1