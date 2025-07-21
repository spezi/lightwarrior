import * as PIXI from 'pixi.js'
import { Transformer } from '@pixi-essentials/transformer';

/**
 * @type {import("phoenix_live_view").HooksOptions}
 */

let MappingHooks = {}

var liveview = null

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

// Create a PixiJS application.

MappingHooks.Stage= {
  selected() { return this.el.dataset.selected },
  side() { return this.el.dataset.side },
  stripe_color() { return this.el.dataset.stripes_color },
  async poll_mapping_container_size() {
    for (let i = 0; i < 10; i++) {
        //console.log("Loop iteration", i);
        this.get_mapping_container_size()
        if (this.size.width > 0 && this.size.width > 0) {
          console.log("mapping container size: " + this.size.width + " x " + this.size.height )
          break
        }
        await sleep(500); // Pause for 500 milliseconds
      }
    return true
  },
  get_mapping_container_size() {
    //console.log("get mapping size")
    const { width, height } = this.mapping_container_wrapper.getBoundingClientRect();
    this.size = { width, height }
    if (this.size.width > 0 && this.size.width > 0) {
      this.pushEvent("phx:mapping-size", this.size);
    }
  },
  catch_resize(_info) {
    var catchResize = []
    var waitforResize = false

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
          this.reset_stage();
        }
      }, 20);
    }
  },
  reset_stage() {
    this.app.stage.removeChildren(0);
    this.ready() 
    //this.app.destroy(false, { children: true, texture: true, baseTexture: true });
    //poll_mapping_container_size()
    //this.test();
  },
  set_instances_data_pixel(data) {
    //console.log(data)
    this.instances_data_pixel = data.instance_data_pixel
    //console.log(this.instances_data_pixel)
  },
  mounted() {
    console.log(this.el.dataset)
    
    liveview = this

    // wait for ready event after mount -> init app or use in ready() -> render function
    this.handleEvent("ready", data => this.ready());
    
    // have toreset stage
    this.handleEvent("tabchange", data => this.reset_stage());
    this.handleEvent("select", data => this.reset_stage());
    this.handleEvent("stripe_color", data => this.reset_stage());
    window.addEventListener("resize", _info => this.catch_resize(_info));

    // called from backend after render or container size known 
    this.handleEvent("instances-data-pixel", data => this.set_instances_data_pixel(data));

    this.width = 0;
    this.height = 0;

    this.mapping_container_wrapper = this.el;
    this.mapping_container = this.mapping_container_wrapper.querySelector("canvas")
    this.instances_data_pixel = []
    //console.log(this.mapping_container_wrapper)
    //console.log(this.mapping_container)

    this.app = new PIXI.Application();
    
  },
  updated() {
    if (this.selected() != undefined){
      //console.log(this.selected())
    }
    if (this.side() != undefined){
      //console.log(this.side())
    }
    if (this.stripe_color() != undefined){
      //console.log(this.stripe_color())
    }
  },
  async ready() {
    console.log("ready")
    console.log(this.selected())
    console.log(this.side())
    console.log(this.stripe_color())

    let stage_canvas_ready = await this.poll_mapping_container_size();
    if(stage_canvas_ready && this.size.width > 0 && this.size.height > 0) {
      console.log("stage canvas ready");

      // if allready initialized on tabchange
      if ( this.app.renderer == undefined) {
          await this.app.init({
            backgroundAlpha: 0, 
            width: this.size.width, 
            height: this.size.height, 
            antialias: true,
            canvas: this.mapping_container
          });
      }
       
      //this.app.canvas.width = this.size.width;
      //this.app.canvas.height = this.size.height;

      // render functions
      this.test();
      this.wait_for_instance_data_and_render()
    }
  },
  async wait_for_instance_data_and_render() {
    for (let i = 0; i < 10; i++) {
        //console.log("Loop iteration", i);
        if ( this.instances_data_pixel.length > 0 ) {
              console.log(this.instances_data_pixel)
              this.render_instances();
          break
        }
        await sleep(500); // Pause for 500 milliseconds
      }
    return true
  },
  async render_instances() {
      if(this.side() != "uniform") {
        this.instances_data_pixel.forEach(stripe => {

        console.log(stripe)
        let leds = stripe.leds;

        let lines = new PIXI.Graphics();
        lines.label = stripe.instance;
        lines.zIndex = 0;
        //console.log(leds[0].hmin, leds[0].vmin)
        lines.moveTo(leds[0].hmin, leds[0].vmin)
        lines.lineTo(leds[(leds.length - 1)].hmin, leds[(leds.length - 1)].vmin)
        lines.stroke({ width: 4, color: this.stripe_color(), alpha: 1});
        if(this.selected() == stripe.instance) lines.alpha = 0.5;
        lines.cursor = 'pointer';
        lines.eventMode = 'static';
        lines.on('pointerdown', this.onSelectInstance, lines); 
        this.app.stage.addChild(lines);     
      });
    }
  },
  onSelectInstance(event) {
      // Store a reference to the data
      // * The reason for this is because of multitouch *
      // * We want to track the movement of this particular touch *
      //console.log(liveview.liveSocket)
      //console.log(event)
      //console.log(this.label)

      // this. is here instance context and not the parent liveview
      //console.log(liveview.liveSocket)
      liveview.pushEvent("phx:select_instance", {"value": this.label});
      //liveview.liveSocket.redirect(`/hyperion/${this.label}/edit`)
      //this.alpha = 0.5;

  },
  async test() {
  
       // Append the application canvas to the document body
        //document.body.appendChild(app.canvas);
  
        const instances = [
            { start: [100, 100], end: [200, 100] },
            { start: [200, 100], end: [200, 200] },
            { start: [200, 200], end: [100, 200] },
            { start: [100, 200], end: [100, 100] }, // forms a square
          ];
  
        const graphics = new PIXI.Graphics();
        
        // Draw each line
        for (const line of instances) {
          graphics.moveTo(...line.start);
          graphics.lineTo(...line.end);
        }
  
        graphics.stroke({
            width: 4,
            color: this.stripe_color(),
            alpha: 1
          });
  
        this.app.stage.addChild(graphics);
  
    }
}

export default MappingHooks

