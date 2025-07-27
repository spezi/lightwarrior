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

    this.pushEvent("phx:init-input_automap", localStorage.getItem("phx:input_automap"));
    this.pushEvent("phx:init-output_automap", localStorage.getItem("phx:output_automap"));
    this.pushEvent("phx:init-uniform_automap", localStorage.getItem("phx:uniform_automap"));

    this.pushEvent("phx:init-input_instances_color", localStorage.getItem("phx:input_instances_color"));
    this.pushEvent("phx:init-output_instances_color", localStorage.getItem("phx:output_instances_color"));
    this.pushEvent("phx:init-uniform_instances_color", localStorage.getItem("phx:uniform_instances_color"));

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

     // instances color
    if( data.input_instances_color != undefined ) {
      //console.log("input_instances_color " + data.input_instances_color)
      localStorage.setItem("phx:input_instances_color", data.input_instances_color);
    }
    if( data.output_instances_color != undefined ) {
      //console.log("output_instances_color " + data.output_instances_color)
      localStorage.setItem("phx:output_instances_color", data.output_instances_color);
    }
    if( data.uniform_instances_color != undefined ) {
      //console.log("uniform_instances_color: " + data.uniform_instances_color)
      localStorage.setItem("phx:uniform_instances_color", data.uniform_instances_color);
    }

    // automap side
    if( data.input_automap != undefined ) {
      //console.log("input_instances_color " + data.input_instances_color)
      localStorage.setItem("phx:input_automap", data.input_automap);
    }
    if( data.output_automap != undefined ) {
      //console.log("output_instances_color " + data.output_instances_color)
      localStorage.setItem("phx:output_automap", data.output_automap);
    }
    if( data.uniform_automap != undefined ) {
      //console.log("uniform_instances_color: " + data.uniform_instances_color)
      localStorage.setItem("phx:uniform_automap", data.uniform_automap);
    }
    
  }
}

export default AppHooks