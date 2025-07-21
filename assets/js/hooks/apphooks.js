/**
 * @type {import("phoenix_live_view").HooksOptions}
 */

let AppHooks = {}

// local storage in lightwarrior_web/components/layouts/root.html.heex
AppHooks.LocalStorage= { 
  mounted() {
    console.log("localStorageInit")
    this.pushEvent("phx:init-autosave", localStorage.getItem("phx:autosave"));
    this.pushEvent("phx:init-debug", localStorage.getItem("phx:debug"));
    this.pushEvent("phx:init-input_opacity", localStorage.getItem("phx:input_opacity"));
    this.pushEvent("phx:init-output_opacity", localStorage.getItem("phx:output_opacity"));
    this.pushEvent("phx:init-uniform_opacity", localStorage.getItem("phx:uniform_opacity"));

    this.pushEvent("phx:init-input_stripes_color", localStorage.getItem("phx:input_stripes_color"));
    this.pushEvent("phx:init-output_stripes_color", localStorage.getItem("phx:output_stripes_color"));
    this.pushEvent("phx:init-uniform_stripes_color", localStorage.getItem("phx:uniform_stripes_color"));

    this.pushEvent("phx:last_open_tab", { last_open_tab: localStorage.getItem("phx:last_open_tab")});

    this.handleEvent("localstorage", data => this.localstorage_set(data))
  },
  localstorage_set(data) {
    //console.log(data)
    // global items
    if( data.debug != undefined ) {
      //console.log("debug: " + data.debug)
      localStorage.setItem("phx:debug", data.debug);
    }
    if( data.autosave != undefined ) {
      //console.log("autosave: " + data.autosave)
      localStorage.setItem("phx:autosave", data.autosave);
    }
    if( data.last_open_tab != undefined ) {
      //console.log("last_open_tab: " + data.last_open_tab)
      localStorage.setItem("phx:last_open_tab", data.last_open_tab);
    }
    
    // bg opacity
    if( data.input_opacity != undefined ) {
      //console.log("input_opacity: " + data.input_opacity)
      localStorage.setItem("phx:input_opacity", data.input_opacity);
    }
    if( data.output_opacity != undefined ) {
      //console.log("output_opacity: " + data.output_opacity)
      localStorage.setItem("phx:output_opacity", data.output_opacity);
    }
    if( data.uniform_opacity != undefined ) {
      //console.log("uniform_opacity: " + data.uniform_opacity)
      localStorage.setItem("phx:uniform_opacity", data.uniform_opacity);
    }

     // stripes color
    if( data.input_stripes_color != undefined ) {
      //console.log("input_stripes_color " + data.input_stripes_color)
      localStorage.setItem("phx:input_stripes_color", data.input_stripes_color);
    }
    if( data.output_stripes_color != undefined ) {
      //console.log("output_stripes_color " + data.output_stripes_color)
      localStorage.setItem("phx:output_stripes_color", data.output_stripes_color);
    }
    if( data.uniform_stripes_color!= undefined ) {
      //console.log("uniform_stripes_color: " + data.uniform_stripes_color)
      localStorage.setItem("phx:uniform_stripes_color", data.uniform_stripes_color);
    }
    
  }
}

export default AppHooks