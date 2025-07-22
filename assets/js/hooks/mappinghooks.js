import * as PIXI from 'pixi.js'
import { Transformer } from '@pixi-essentials/transformer';

/**
 * @type {import("phoenix_live_view").HooksOptions}
 */

let MappingHooks = {}

var liveview = null
var dragTarget = null;
var dragPosition = new PIXI.Point(0,0)

var selected= new PIXI.Container();
var selected_start = new PIXI.Graphics();
var selected_end = new PIXI.Graphics();
var selected_line = new PIXI.Graphics();

var selected_path = [
  0, 
  0, 
  0, 
  0
];

var initialDistance = 0
var lockDistance = true

var step_h = 0
var step_v = 0


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
    this.handleEvent("change_mapping", data => this.reset_stage());
    this.handleEvent("lockdistance", data => lockDistance = data.lockdistance );
    
    window.addEventListener("resize", _info => this.catch_resize(_info));

    // called from backend after render or container size known and after mapping change
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

          this.app.stage.interactive = true;
          this.app.stage.hitArea = this.app.screen;

          this.app.stage.on('pointerup', this.onDragEnd);
          this.app.stage.on('pointerupoutside', this.onDragEnd);

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
        //console.log(stripe)
        let leds = stripe.leds;
        let lines = new PIXI.Graphics();
        lines.label = stripe.instance;
        lines.zIndex = 0;
        //console.log(leds[0].hmin, leds[0].vmin)
        lines.moveTo(leds[0].hmin, leds[0].vmin)
        lines.lineTo(leds[(leds.length - 1)].hmin, leds[(leds.length - 1)].vmin)
        lines.stroke({ width: 4, color: this.stripe_color(), alpha: 1});
        if(this.selected() == stripe.instance) lines.alpha = 0.4;
        lines.cursor = 'pointer';
        lines.eventMode = 'static';
        lines.on('pointerdown', this.onSelectInstance, lines); 
        this.app.stage.addChild(lines);    
      });
    }
    this.render_selected(); 
  },
  async render_selected() {
    if(this.side() != "uniform") {
        this.instances_data_pixel.forEach(stripe => {
          if(this.selected() == stripe.instance) {
            console.log(stripe)
            let leds = stripe.leds;
            
            selected.destroy(true)

            selected = new PIXI.Container();
            selected.label = stripe.instance

            selected_start = new PIXI.Graphics();
            selected_start.label = 'selected_start';
            selected_start.position.set(
              stripe.start[0],
              stripe.start[1]
            )
            selected_start.circle(0, 0, 6);
            selected_start.fill({color:'blue', alpha:1});
            selected_start.zIndex = 3;
            selected_start.cursor = 'grab';
            selected_start.eventMode = 'static';
            selected_start.on('pointerdown', this.onDragStart, selected_start);
            selected.addChild(selected_start);

            selected_end = new PIXI.Graphics();
            selected_end.label = 'selected_end';
            selected_end.position.set(
              stripe.end[0],
              stripe.end[1]
            )
            selected_end.circle(0, 0, 6);
            selected_end.fill({color:'red', alpha:1});
            selected_end.zIndex = 3;
            selected_end.cursor = 'grab';
            selected_end.eventMode = 'static';
            selected_end.on('pointerdown', this.onDragStart, selected_end);
            selected.addChild(selected_end);

            /*
            selected_line = new PIXI.Graphics();
            selected_line.label = 'selected_line';
            selected_line.zIndex = 1;
            selected_line.moveTo(leds[0].hmin, leds[0].vmin)
            selected_line.lineTo(leds[(leds.length - 1)].hmin, leds[(leds.length - 1)].vmin)
            selected_line.stroke({ width: 4, color: 0xf3ccff, alpha: 1});
            selected_line.cursor = 'grab';
            selected_line.eventMode = 'static';
            selected_line.on('pointerdown', this.onDragStart, selected_line);
            selected.addChild(selected_line); 
            */

            selected_path = [
              selected_start.x, 
              selected_start.y, 
              selected_end.x, 
              selected_end.y
            ];

            selected_line = new PIXI.Graphics();
            selected_line.label = 'selected_line';
            selected_line.poly(selected_path);
            selected_line.stroke({ width: 4, color: 0xffd900 });
            selected_line.zIndex = 2;
            selected_line.cursor = 'grab';
            selected_line.eventMode = 'static';
            selected_line.on('pointerdown', this.onDragStart, selected_line);
            selected.addChild(selected_line);


            this.app.stage.addChild(selected);

            this.app.stage.on('pointermove', this.onDragMove);
            this.update_selected_length()

          } 
      });
    }
  },
  onDragStart(event)
  {
      // Store a reference to the data
      // * The reason for this is because of multitouch *
      // * We want to track the movement of this particular touch *
      
      dragTarget = this;

      if(dragTarget.label == 'selected_line')
      { 
        this.parent.alpha = 0.5;
        dragTarget.parent.toLocal(event.global, null, dragTarget.parent.pivot);
        dragTarget.parent.position = event.global;
        dragPosition.set(event.global.x, event.global.y)
      } else {
        this.alpha = 0.5;
      }

      //console.log(this.parent)
      //liveview.app.stage.on('pointermove', this.onDragMove);

  },
  onDragMove(event)
  {
    if (dragTarget != null )
      { 
        //console.log(dragTarget.parent)
        //let dragData = event.data;
        if(dragTarget.label == 'selected_start')
          {
            console.log(initialDistance)
            //preserve distance
            const dx = selected_start.x - selected_end.x;
            const dy = selected_start.y - selected_end.y;
            const currentDistance = Math.sqrt(dx * dx + dy * dy);
            const scaleFactor = initialDistance / currentDistance;
            /*
            path = [
              dragTarget.position.x, 
              dragTarget.position.y, 
              selected_end.x, 
              selected_end.y
            ];*/
            if(lockDistance) {
              selected_path = [
                selected_end.x + dx * scaleFactor, 
                selected_end.y + dy * scaleFactor, 
                selected_end.x, 
                selected_end.y
              ];
            } else {
              selected_path = [
                dragTarget.position.x, 
                dragTarget.position.y, 
                selected_end.x, 
                selected_end.y
              ];
            }
            
            selected_line.clear()
            selected_line.poly(selected_path);
            selected_line.stroke({ width: 4, color: 0xffd900 });
          }
        if(dragTarget.label == 'selected_end')
          {
            //preserve distance
            const dx = selected_end.x - selected_start.x;
            const dy = selected_end.y - selected_start.y;
            const currentDistance = Math.sqrt(dx * dx + dy * dy);
            const scaleFactor = initialDistance / currentDistance;
            /*
            path = [
              selected_start.x, 
              selected_start.y,
              dragTarget.position.x, 
              dragTarget.position.y,
            ];*/

            //console.log(lockDistance)

            if(lockDistance) {
              selected_path = [
                selected_start.x, 
                selected_start.y,
                selected_start.x + dx * scaleFactor, 
                selected_start.y + dy * scaleFactor,
              ];
            } else {
              selected_path = [
                selected_start.x, 
                selected_start.y,
                dragTarget.position.x, 
                dragTarget.position.y,
              ];
            }
            
            selected_line.clear()
            selected_line.poly(selected_path);
            selected_line.stroke({ width: 4, color: 0xffd900 });
          }
        if(dragTarget.label == 'selected_line')
          {
            //console.log(event.global)
            //dragTarget.parent.toLocal(event.global, null, dragTarget.parent.position);
            
            //old way
            dragTarget.parent.position.set(event.global.x, event.global.y);

          } else {
            dragTarget.parent.toLocal(event.global, null, dragTarget.position);
          } 
      }
  },
  onDragEnd()
  {
      if (dragTarget != null)
      {
          if(dragTarget.label == 'selected_line') {
            //console.log(selected_start.position)
            //console.log(dragTarget.parent.groupTransform.tx)
            //console.log(dragTarget.parent.groupTransform.ty)
            //console.log(dragTarget.parent.children)
            //console.log(dragTarget.parent.localTransform)
            dragTarget.parent.children.forEach((child) => {
              child.position.set(
                child.position.x + dragTarget.parent.worldTransform.tx, 
                child.position.y + dragTarget.parent.worldTransform.ty)
            });
            // revert position of container because children are moved
            dragTarget.parent.position.set(dragPosition.x, dragPosition.y);
            
            //console.log(dragTarget.parent.position)
            //console.log(selected_start.position)
          } else {
            // only need on points
            if(lockDistance) {
              // move point to fit line on preserve distance
              selected_start.position.set(
                selected_path[0],
                selected_path[1]
              );
    
              selected_end.position.set(
                selected_path[2],
                selected_path[3]
              );
            }
          }

          console.log(this)
          this.off('pointermove', this.onDragMove);
    
          dragTarget.alpha = 1;
          dragTarget.parent.alpha = 1;
          dragTarget = null;

          MappingHooks.Stage.selected_change_mapping();
          //console.log(this)
          //console.log(dragTarget)
          //console.log(app.stage)
      }
  },
  selected_change_mapping() {
    //console.log(line.getBounds())
    //liveview.pushEvent("phx:selected_change_mapping", line.getBounds());
    
    let points = {
      start: {
        x: selected_start.position.x,
        y: selected_start.position.y
      },
      end: {
        x: selected_end.position.x,
        y: selected_end.position.y
      },
    };
    console.log(points) 
    
    this.update_selected_length()

    liveview.pushEvent("phx:selected_change_mapping", points);
  },
  update_selected_length(){
    //this.update_stripe_length()
    initialDistance = Math.sqrt((selected_end.x - selected_start.x) ** 2 + (selected_end.y - selected_start.y) ** 2);
    //liveview.pushEvent("phx:initial-distance", { initialDistance });
  },
  onSelectInstance(event) {
      liveview.pushEvent("phx:select_instance", {"value": this.label});
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

