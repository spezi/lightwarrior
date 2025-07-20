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

  }
}

// Create a PixiJS application.
var app = null
const mapping_container = document.getElementById('mapping_input');
const mapping_container_wrapper = document.getElementById('mapping_wrapper_input');
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

var catchResize = []
var waitforResize = false

var step_h = 0
var step_v = 0

Hooks.Stage= {
  async mounted() {
    console.log("stage mounted")
    liveview = this
    app = new PIXI.Application();

    //catchResize 
    this.get_mapping_container_size();
    //window.addEventListener("resize", _info => this.get_mapping_container_size());
    window.addEventListener("resize", _info => this.catch_resize(_info));
    this.handleEvent("instance-data-pixel", data => this.get_instance_data_pixel(data))

    //await this.init_stage();

    await app.init({
      backgroundAlpha: 0, 
      width: mapping_container_wrapper.offsetWidth, 
      height: mapping_container_wrapper.offsetHeight, 
      antialias: true,
      canvas: mapping_container
    });
  
    
    app.canvas.width = mapping_container_wrapper.offsetWidth
    app.canvas.height = mapping_container_wrapper.offsetHeight
    
    
    

    console.log(app)

    //this.render_instances();
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

    this.render_instances(instances);
  },
  async render_instances(instances) {
      // Create a new application
    const app = new PIXI.Application();

    // Initialize the application
    await app.init({
      backgroundAlpha: 0, 
      width: mapping_container_wrapper.offsetWidth, 
      height: mapping_container_wrapper.offsetHeight, 
      antialias: true,
      canvas: mapping_container
    });

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

    app.stage.addChild(graphics);
  },
  catch_resize(_info) {
    const eventTime = new Date().getTime();
    catchResize.push(eventTime)
    
    // debounce resize requests
    if(!waitforResize) {
      waitforResize = true
      let timerId = setInterval(() => {
        const currentTime = new Date().getTime();
        //console.log(currentTime - catchResize[(catchResize.length - 1)])
        if((currentTime - catchResize[(catchResize.length - 1)]) > 100) {
          clearInterval(timerId);
          catchResize = []
          waitforResize = false
          this.get_mapping_container_size();
        }
      }, 20);
    }
  },
  get_mapping_container_size() {
    console.log("get mapping size")

    const container = document.getElementById('mapping_input');
    const { width, height } = container.getBoundingClientRect();
    //console.log({width, height})

    this.pushEvent("phx:mapping-size", { width, height });

    //initialDistance = Math.sqrt((stripe_end.x - stripe_start.x) ** 2 + (stripe_end.y - stripe_start.y) ** 2);
    //this.pushEvent("phx:initial-distance", { initialDistance });
  }
}

export default Hooks