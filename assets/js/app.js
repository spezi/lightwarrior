// If you want to use Phoenix channels, run `mix help phx.gen.channel`
// to get started and then uncomment the line below.
// import "./user_socket.js"

// You can include dependencies in two ways.
//
// The simplest option is to put them in assets/vendor and
// import them using relative paths:
//
//     import "../vendor/some-package.js"
//
// Alternatively, you can `npm install some-package --prefix assets` and import
// them using a path starting with the package name:
//
//     import "some-package"
//

// Include phoenix_html to handle method=PUT/DELETE in forms and buttons.
import "phoenix_html"
// Establish Phoenix Socket and LiveView configuration.
import {Socket} from "phoenix"
import {LiveSocket} from "phoenix_live_view"
import headerMenue from "./header_menue";

import Hooks from "./hooks";

let csrfToken = document.querySelector("meta[name='csrf-token']").getAttribute("content")
let liveSocket = new LiveSocket("/live", Socket, {hooks: Hooks, params: {_csrf_token: csrfToken}})

// Show progress bar on live navigation and form submits
headerMenue.config({barColors: {0: "#29d"}, shadowColor: "rgba(0, 0, 0, .3)"})
var canvas = document.getElementById('c');
window.addEventListener("phx:page-loading-start", 
_info => console.log("start loading") //headerMenue.show(300)
)

window.addEventListener("phx:page-loading-stop", 
_info => console.log("stop loading") //headerMenue.hide()
)
 
window.addEventListener("phx:save-debug",
    e => localStorage.setItem("debug", e.detail.debug) //console.log(e.detail.debug)
  )

// connect if there are any LiveViews on the page
liveSocket.connect()

// expose liveSocket on window for web console debug logs and latency simulation:
// >> liveSocket.enableDebug()
// >> liveSocket.enableLatencySim(1000)  // enabled for duration of browser session
// >> liveSocket.disableLatencySim()
window.liveSocket = liveSocket

