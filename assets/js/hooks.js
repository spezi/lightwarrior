import * as PIXI from 'pixi.js'
import { Transformer } from '@pixi-essentials/transformer';

/**
 * @type {import("phoenix_live_view").HooksOptions}
 */

let Hooks = {}

// local storage in lightwarrior_web/components/layouts/root.html.heex
Hooks.LocalStorage= { 
  mounted() {
    console.log("localStorageInit")
    this.pushEvent("phx:init-autosave", localStorage.getItem("phx:autosave"));
    this.pushEvent("phx:init-debug", localStorage.getItem("phx:debug"));
    this.pushEvent("phx:init-input_opacity", localStorage.getItem("phx:input_opacity"));
    this.pushEvent("phx:init-output_opacity", localStorage.getItem("phx:output_opacity"));
    this.pushEvent("phx:init-uniform_opacity", localStorage.getItem("phx:uniform_opacity"));
    this.pushEvent("phx:last_open_tab", { last_open_tab: localStorage.getItem("phx:last_open_tab")});

    this.handleEvent("localstorage", data => this.localstorage_set(data))
  },
  localstorage_set(data) {
    console.log(data)
    if( data.debug != undefined ) {
      console.log("debug: " + data.debug)
      localStorage.setItem("phx:debug", data.debug);
    }

    if( data.autosave != undefined ) {
      console.log("autosave: " + data.autosave)
      localStorage.setItem("phx:autosave", data.autosave);
    }

    if( data.input_opacity != undefined ) {
      console.log("input_opacity: " + data.input_opacity)
      localStorage.setItem("phx:input_opacity", data.input_opacity);
    }

    if( data.output_opacity != undefined ) {
      console.log("output_opacity: " + data.output_opacity)
      localStorage.setItem("phx:output_opacity", data.output_opacity);
    }
    if( data.uniform_opacity != undefined ) {
      console.log("uniform_opacity: " + data.uniform_opacity)
      localStorage.setItem("phx:uniform_opacity", data.uniform_opacity);
    }
    if( data.last_open_tab != undefined ) {
      console.log("last_open_tab: " + data.last_open_tab)
      localStorage.setItem("phx:last_open_tab", data.last_open_tab);
    }
  }
}

// Create a PixiJS application.

//const mapping_container = document.getElementById('mapping_input');
//const mapping_container_wrapper = document.getElementById('mapping_wrapper_input');
var dragTarget = null;

var stripe = new PIXI.Container();
var stripe_start = new PIXI.Graphics();
var stripe_end = new PIXI.Graphics();

var stripes = null
//var lines = new PIXI.Graphics();
var lines_wrapper = new PIXI.Container();
var line = new PIXI.Graphics();
var path = [
  0, 
  0, 
  0, 
  0
];

var liveview = null

var dragPosition = new PIXI.Point(0,0)
var initialDistance = 0
var lockDistance = true



var step_h = 0
var step_v = 0

Hooks.Stage= {
  async mounted() {
    console.log("stage mounted")
    console.log(this)
    liveview = this

    this.app = new PIXI.Application();
    this.mapping_container_wrapper = this.el;
    this.mapping_container = this.mapping_container_wrapper.querySelector("canvas")
    this.width = 0;
    this.height = 0;
    this.catchResize = []
    this.waitforResize = false
    //this.mapping_container_wrapper = document.getElementById('mapping_wrapper_input');

    //catchResize 
    window.addEventListener("resize", _info => this.catch_resize(_info));
    this.handleEvent("instance-data-pixel", data => this.get_instance_data_pixel(data));

    await this.init_stage();

    console.log(this.app);

    //this.render_instances();
  },
  updated() {
    if(!this.width && !this.height) {
      this.get_mapping_container_size();
    }
  },
  async init_stage(){
    await this.app.init({
      backgroundAlpha: 0, 
      width: this.mapping_container_wrapper.offsetWidth, 
      height: this.mapping_container_wrapper.offsetHeight, 
      antialias: true,
      canvas: this.mapping_container
    });

    this.app.canvas.width = this.mapping_container_wrapper.offsetWidth
    this.app.canvas.height = this.mapping_container_wrapper.offsetHeight

  },
  get_instance_data_pixel(data){
    console.log(data)

    var instances = []
    /*
    const instances = [
      { start: [100, 100], end: [200, 100] },
      { start: [200, 100], end: [200, 200] },
      { start: [200, 200], end: [100, 200] },
      { start: [100, 200], end: [100, 100] }, // forms a square
    ];
    */
    for (const instance of data.instance_data_pixel) {
      instance_points = { start: instance.start, end: instance.end}
      //console.log(instance_points);
      instances.push(instance_points) 
    }

    //this.render_instances(instances);
  },
  async render_instances(instances) {

    const graphics = new PIXI.Graphics();

    // Draw each line
    for (const line of instances) {
      graphics.moveTo(...line.start);
      graphics.lineTo(...line.end);
    }

    // Apply stroke style and render all lines
    graphics.stroke({
      width: 4,
      color: 0xffffff,
      alpha: 1
    });

    this.app.stage.addChild(graphics);
  },
  catch_resize(_info) {
    const eventTime = new Date().getTime();
    this.catchResize.push(eventTime)
    
    // debounce resize requests
    if(!this.waitforResize) {
      this.waitforResize = true
      let timerId = setInterval(() => {
        const currentTime = new Date().getTime();
        //console.log(currentTime - catchResize[(catchResize.length - 1)])
        if((currentTime - this.catchResize[(this.catchResize.length - 1)]) > 100) {
          clearInterval(timerId);
          this.catchResize = []
          this.waitforResize = false
          this.get_mapping_container_size();
        }
      }, 20);
    }
  },
  get_mapping_container_size() {
    console.log("get mapping size")
    //console.log(this.mapping_container)
    //console.log(this.mapping_container_wrapper)
    //console.log(this.mapping_container_wrapper.getBoundingClientRect())
    const { width, height } = this.mapping_container_wrapper.getBoundingClientRect();
    //console.log({width, height})

    this.width = width
    this.height = height

    if(this.width && this.height) {
      this.pushEvent("phx:mapping-size", { width, height });
    } 

    //initialDistance = Math.sqrt((stripe_end.x - stripe_start.x) ** 2 + (stripe_end.y - stripe_start.y) ** 2);
    //this.pushEvent("phx:initial-distance", { initialDistance });
  }
}

export default Hooks