# ISF Preview — local shader playground

A self-contained tool to preview **ISF** (Interactive Shader Format) fragment
shaders in the browser, feed them an input image, and tweak their `INPUTS`
live. Rendering runs entirely client-side in WebGL via the official
`interactive-shader-format` runtime (vendored in `lib/isf.js`, no network needed).

## Run it

Just open `index.html`. Either:

- **Double-click** `index.html`, or
- Serve the folder (recommended, avoids any `file://` quirks):
  ```
  cd isf-preview
  python3 -m http.server 8000
  # → http://localhost:8000
  ```

`lib/isf.js` must stay next to `index.html`.

## Use

1. **Drop an `.fs` ISF shader** onto the left panel (or *Load example*).
2. **Drop an image** onto the panel or onto the canvas — it's assigned to the
   selected image input (e.g. `inputImage`). Shaders with several image inputs
   show one slot each.
3. Adjust the auto-generated controls: floats/longs → sliders, `long` with
   `VALUES` → dropdown, `bool` → toggle, `color` → swatch + alpha,
   `point2D` → XY pad.

Toolbar: play/pause (`space`), reset time (`R`), output resolution
(fit / 512 / 720p / 1080p), pixelated scaling, and PNG snapshot (`S`).

Built-in uniforms (`TIME`, `TIMEDELTA`, `FRAMEINDEX`, `RENDERSIZE`, `DATE`,
`PASSINDEX`) are driven automatically. Multi-pass shaders and persistent
buffers work.

The reference shader you linked
(`https://editor.isf.video/shaders/5e7a7fea7c113618206de6f4`) is a single-pass
image filter — load its `.fs` and drop an image on `inputImage`.

## Wrapping in Phoenix (optional)

You asked about Elixir. The rendering is pure client-side WebGL, so Phoenix
only ever serves these static files. To drop this into an existing Phoenix app:

```
cp index.html lib/isf.js  →  priv/static/isf/
```

and serve via `Plug.Static` (already on by default for `priv/static`), reaching
it at `/isf/index.html`. If you want **server-side filesystem browsing** of a
shader library (a LiveView that lists `*.fs` files from a directory and streams
the chosen one to the client), say so and I'll add a small LiveView + a JS hook
that calls the same `loadShaderSource()` — the front end here is already
structured around that one entry point. I left it out for now because this
container has no Elixir/Erlang toolchain to compile and verify a Phoenix
project against.
```
