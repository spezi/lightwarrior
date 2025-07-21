import * as PIXI from 'pixi.js'
import { Transformer } from '@pixi-essentials/transformer';

/**
 * @type {import("phoenix_live_view").HooksOptions}
 */

let MappingHooks = {}

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
          this.get_mapping_container_size();
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
  mounted() {
    console.log(this.el.dataset)
    this.handleEvent("ready", data => this.ready());
    
    this.handleEvent("tabchange", data => this.reset_stage());
    this.handleEvent("stripe_color", data => this.reset_stage());

    window.addEventListener("resize", _info => this.catch_resize(_info));

    this.width = 0;
    this.height = 0;

    this.mapping_container_wrapper = this.el;
    this.mapping_container = this.mapping_container_wrapper.querySelector("canvas")
    console.log(this.mapping_container_wrapper)
    console.log(this.mapping_container)

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

      this.test();
    }
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