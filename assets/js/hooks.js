import * as PIXI from 'pixi.js'
import { Transformer } from '@pixi-essentials/transformer';

let Hooks = {}

Hooks.StoreDebug = {
  mounted() {
    console.log("init local storage")
    this.pushEvent("phx:debug", {
      debug: localStorage.getItem("debug"),
    })
  },
}

// Create a PixiJS application.
var app = null
const mapping_container = document.getElementById('mapping');
const mapping_container_wrapper = document.getElementById('mapping_wrapper');
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

//var fetched_all_stripes_config = null
//var selected_stripe_data_pixel = null

Hooks.Stage = {
  async mounted() {
    
    liveview = this
    //this.handleEvent("data-ready", data => this.init_stage(data))
    this.handleEvent("stripe-select", _info => this.select_stripe())
    this.handleEvent("stripe-ready", data => this.stripe_ready(data))
    this.handleEvent("stripes-ready", data => this.stripes_ready(data))
    this.handleEvent("set-even-x", data => this.set_even_x(data))
    this.handleEvent("set-even-y", data => this.set_even_y(data))

    this.handleEvent("lock-distance", data => lockDistance = true )
    this.handleEvent("unlock-distance", data => lockDistance = false )
    
    window.addEventListener("phx:page-loading-stop", 
    _info => this.reconnected()//headerMenue.hide()
    )
   
    //if(app) {
    //  app.destroy();
    //}
    app = new PIXI.Application();


    this.get_mapping_container_size();
    window.addEventListener("resize", _info => this.get_mapping_container_size());

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
    
    app.stage.eventMode = 'static';
    app.stage.hitArea = app.screen;
    app.stage.on('pointerup', onDragEnd);
    app.stage.on('pointerupoutside', onDragEnd);
    
    console.log(app)

  },
  // because liveview destroys elements  
  async reconnected() {
    console.log("reconnected stage")
    //console.log(window.liveSocket)
    console.log(app)
    //console.log(sprite)
    //console.log(this)
    
  },
  get_mapping_container_size() {
    console.log("get mapping size")
    const container = document.getElementById('mapping');
    const { width, height } = container.getBoundingClientRect();
    //console.log({width, height})
    this.pushEvent("phx:mapping-size", { width, height });
  },
  stripe_change_mapping() {
    //console.log(line.getBounds())
    //liveview.pushEvent("phx:stripe_change_mapping", line.getBounds());
    
    let points = {
      start: {
        x: stripe_start.position.x,
        y: stripe_start.position.y
      },
      end: {
        x: stripe_end.position.x,
        y: stripe_end.position.y
      },
    };
    console.log(points) 
    liveview.pushEvent("phx:stripe_change_mapping", points); 
  },
  async select_stripe() {
    console.log("select stripe");
    //console.log(app.stage);
    const container = document.getElementById('mapping');
    const { width, height } = container.getBoundingClientRect();

    this.pushEvent("phx:get_selected_leds_pixel", { width, height });
    //line.clear();
    //this.stripe_to_stage(selected_stripe_data_pixel);
  },
  async stripe_ready(data) {

    console.log(app)

    stripe.destroy({children:true})

    //if (app == null) {
      /*
      await app.init({
        backgroundAlpha: 0, 
        width: mapping_container_wrapper.offsetWidth, 
        height: mapping_container_wrapper.offsetHeight, 
      
        canvas: mapping_container
      });
    
      
      app.canvas.width = mapping_container_wrapper.offsetWidth
      app.canvas.height = mapping_container_wrapper.offsetHeight
      
      app.stage.eventMode = 'static';
      app.stage.hitArea = app.screen;
      app.stage.on('pointerup', onDragEnd);
      app.stage.on('pointerupoutside', onDragEnd);
      */
    //}

    

    stripe.destroy(true)
    stripe = new PIXI.Container();
    stripe_start = new PIXI.Graphics();
    stripe_end = new PIXI.Graphics();
    line = new PIXI.Graphics();

    console.log(data)
    line.clear();
    this.stripe_to_stage(data.leds_pixel);
  },
  stripe_to_stage(selected_stripe_data_pixel) {
    console.log("stripe to stage")
    
    stripe.label = selected_stripe_data_pixel.friendly_name;

    stripe_start.label = 'stripe_start';
    stripe_start.position.set(
      selected_stripe_data_pixel.start[0],
      selected_stripe_data_pixel.start[1]
      )

    stripe_start.circle(0, 0, 6);
    stripe_start.fill({color:'blue', alpha:1});
    stripe_start.zIndex = 3;
    stripe_start.cursor = 'grab';
    stripe_start.eventMode = 'static';
    stripe_start.on('pointerdown', onDragStart, stripe_start);

    stripe_end.label = 'stripe_end';
    stripe_end.position.set(
      selected_stripe_data_pixel.end[0],
      selected_stripe_data_pixel.end[1]
      )
    stripe_end.circle(0, 0, 6);
    stripe_end.fill({color:'red', alpha:1});
    stripe_end.zIndex = 3;
    stripe_end.cursor = 'grab';
    stripe_end.eventMode = 'static';
    stripe_end.on('pointerdown', onDragStart, stripe_end);

    //app.stage.addChild(stripe_start);
    //app.stage.addChild(stripe_end);
    stripe.addChild(stripe_start);
    stripe.addChild(stripe_end);

    //stripe.position.set(100,100)

    //console.log(app.stage)

    path = [
      stripe_start.x, 
      stripe_start.y, 
      stripe_end.x, 
      stripe_end.y
    ];

    //console.log(path)

    line.label = 'line';
    line.poly(path);
    line.stroke({ width: 4, color: 0xffd900 });
    line.zIndex = 2;
    line.cursor = 'grab';
    line.eventMode = 'static';
    line.on('pointerdown', onDragStart, line);
    
    //console.log(line)

    //app.stage.addChild(line); 
    stripe.addChild(line);

    app.stage.addChild(stripe);

    // Calculate initial distance between points
    initialDistance = Math.sqrt((stripe_end.x - stripe_start.x) ** 2 + (stripe_end.y - stripe_start.y) ** 2);
    console.log(initialDistance)
    this.pushEvent("phx:initial-distance", { initialDistance });
    //const length_input = document.getElementById('length');
    //length_input.value = initialDistance
  },
  async stripes_ready(data) {

    //console.log(data)
    /*
    stripes.destroy({children:true})
    stripes.destroy(true)
    stripes = new PIXI.Container();
    stripes.label = "stripes"
    */
    //stripes_stripe_start = new PIXI.Point();
    //stripes_stripe_end = new PIXI.Point();
    //lines = new PIXI.Graphics();
    //lines.clear();
    //app.stage.children.forEach(child => { 
    //  app.stage.removeChild(child)
    //});

    app.stage.removeChildren(0);
    
    this.stripes_to_stage(data);
  },
  stripes_to_stage(data) {
    console.log(data)
    console.log("stripes to stage")
    console.log(stripe)

    //console.log(mapping_container.offsetWidth)
    //console.log(mapping_container.offsetHeight)
    stripes = data.stripes

    data.stripes.forEach(stripe => {
      //console.log(stripe.config.info.leds);
      let leds = stripe.config.info.leds;

      let lines = new PIXI.Graphics();
      lines.label = stripe.friendly_name;
      lines.zIndex = 0;

      //console.log(from)
      //console.log(to)
      //console.log(leds[(leds.length - 1)])
      lines.moveTo((leds[0].hmin * mapping_container.offsetWidth), (leds[0].vmin * mapping_container.offsetHeight))
      lines.lineTo((leds[(leds.length - 1)].hmin * mapping_container.offsetWidth), (leds[(leds.length - 1)].vmin * mapping_container.offsetHeight))
      lines.stroke({ width: 4, color: 0xf3ccff });
      lines.cursor = 'pointer';
      lines.eventMode = 'static';
      lines.on('pointerdown', onSelectStripe, lines); 
      app.stage.addChild(lines);     
    });

    //lines.moveTo(100, 100)
    //lines.lineTo(200, 200)
    

    //stripes.addChild(lines)

    //console.log(stripes)

    //console.log(app.stage);
    //transformer_test();

  },
  set_even_x(data){
    console.log("set even x")
    console.log(stripe_start.position.x)
    console.log(stripe_end.position.x)
    
    stripe_end.position.set(
      stripe_start.position.x,
      stripe_end.position.y
      )
    
    path = [
      stripe_start.position.x, 
      stripe_start.position.y, 
      stripe_start.position.x, 
      stripe_end.position.y
    ];
    line.clear()
    line.poly(path);
    line.stroke({ width: 4, color: 0xffd900 });
    this.stripe_change_mapping();
  },
  set_even_y(data){
    console.log("set even y")
    console.log(stripe_start.position.y)
    console.log(stripe_end.position.y)
    
    stripe_end.position.set(
      stripe_end.position.x,
      stripe_start.position.y
      )

    path = [
      stripe_start.position.x, 
      stripe_start.position.y, 
      stripe_end.position.x, 
      stripe_start.position.y
    ];
    line.clear()
    line.poly(path);
    line.stroke({ width: 4, color: 0xffd900 });
    this.stripe_change_mapping();
  }
}

function onSelectStripe(event)
{
    // Store a reference to the data
    // * The reason for this is because of multitouch *
    // * We want to track the movement of this particular touch *
    //console.log(liveview.liveSocket)
    //console.log(event)
    console.log(liveview)
    console.log(stripes)
    console.log(this.label)
    stripes.forEach(stripe => { 
      //stage.removeChild(c)
      if(stripe.friendly_name == this.label) {
        console.log(stripe.instance)
        //liveview.pushEvent("stripe-select", {
        //  select: stripe.instance
        //})
        //liveview.liveSocket.execJS("patch", `/hyperion/ledmappings/${stripe.instance}/edit`)
        liveview.liveSocket.redirect(`/hyperion/ledmappings/${stripe.instance}/edit`)
      }
    });


}

function onDragStart(event)
{
    // Store a reference to the data
    // * The reason for this is because of multitouch *
    // * We want to track the movement of this particular touch *
    
    dragTarget = this;
    

    //console.log(event.global);
    //console.log(dragTarget.parent.pivot);

    if(dragTarget.label == 'line')
    { 
      this.parent.alpha = 0.5;
      dragTarget.parent.toLocal(event.global, null, dragTarget.parent.pivot);
      dragTarget.parent.position = event.global;

      //console.log(dragPosition)
      dragPosition.set(event.global.x, event.global.y)

    } else {
      this.alpha = 0.5;
    }

    app.stage.on('pointermove', onDragMove);

}

function onDragMove(event)
{
  if (dragTarget)
    //console.log(dragTarget.label)
    { 
      let dragData = event.data;

      if(dragTarget.label == 'stripe_start')
        {
          //preserve distance
          const dx = stripe_start.x - stripe_end.x;
          const dy = stripe_start.y - stripe_end.y;
          const currentDistance = Math.sqrt(dx * dx + dy * dy);
          const scaleFactor = initialDistance / currentDistance;
          /*
          path = [
            dragTarget.position.x, 
            dragTarget.position.y, 
            stripe_end.x, 
            stripe_end.y
          ];*/
          if(lockDistance) {
            path = [
              stripe_end.x + dx * scaleFactor, 
              stripe_end.y + dy * scaleFactor, 
              stripe_end.x, 
              stripe_end.y
            ];
          } else {
            path = [
              dragTarget.position.x, 
              dragTarget.position.y, 
              stripe_end.x, 
              stripe_end.y
            ];
          }
          
          line.clear()
          line.poly(path);
          line.stroke({ width: 4, color: 0xffd900 });
          //maintainFixedDistance(stripe_end, dragTarget);
        }
      if(dragTarget.label == 'stripe_end')
        {
          //preserve distance
          const dx = stripe_end.x - stripe_start.x;
          const dy = stripe_end.y - stripe_start.y;
          const currentDistance = Math.sqrt(dx * dx + dy * dy);
          const scaleFactor = initialDistance / currentDistance;
          /*
          path = [
            stripe_start.x, 
            stripe_start.y,
            dragTarget.position.x, 
            dragTarget.position.y,
          ];*/
          console.log(lockDistance)

          if(lockDistance) {
            path = [
              stripe_start.x, 
              stripe_start.y,
              stripe_start.x + dx * scaleFactor, 
              stripe_start.y + dy * scaleFactor,
            ];
          } else {
            path = [
              stripe_start.x, 
              stripe_start.y,
              dragTarget.position.x, 
              dragTarget.position.y,
            ];
          }
          
          line.clear()
          line.poly(path);
          line.stroke({ width: 4, color: 0xffd900 });
          //maintainFixedDistance(stripe_start, dragTarget);
        }
      if(dragTarget.label == 'line')
        {
          //console.log(event.global)
          //dragTarget.parent.toLocal(event.global, null, dragTarget.parent.position);
          
          //old way
          dragTarget.parent.position.set(event.global.x, event.global.y);

        } else {
          dragTarget.parent.toLocal(event.global, null, dragTarget.position);
        } 
    }
}

function onDragEnd()
{
    if (dragTarget)
    {
        if(dragTarget.label == 'line') {
          //console.log(stripe_start.position)
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
          //console.log(stripe_start.position)
        } else {
          // only need on points
          if(lockDistance) {
            // move point to fit line on preserve distance
            stripe_start.position.set(
              path[0],
              path[1]
            );
  
            stripe_end.position.set(
              path[2],
              path[3]
            );
          }
        }

        

        app.stage.off('pointermove', onDragMove);
        dragTarget.alpha = 1;
        dragTarget.parent.alpha = 1;
        dragTarget = null;
        Hooks.Stage.stripe_change_mapping();
        //console.log(this)
        //console.log(dragTarget)
        //console.log(app.stage)
    }
}

function transformer_test(){
  console.log("transformer test")   

  app.stage.interactive = true;

  // Create a container
  const container = new PIXI.Container();
  app.stage.addChild(container);

  // Create a graphic element (e.g., a simple rectangle)
  const rect = new PIXI.Graphics();
  rect.beginFill(0xde3249);
  rect.drawRect(0, 0, 100, 100);
  rect.endFill();

  // Add the rectangle to the container
  container.addChild(rect);

  // Set initial position of the container
  container.x = 200;
  container.y = 150;


  // Initialize the transformer
  const transformer = new Transformer({
      target: container,
      handles: {
          pivot: true,
          scale: true,
          rotation: true,
          shear: true
      }
  });
  app.stage.addChild(transformer);
}

function maintainFixedDistance(fixedPoint, movingPoint) {
  // Calculate the vector from the fixed point to the moving point
  const dx = movingPoint.x - fixedPoint.x;
  const dy = movingPoint.y - fixedPoint.y;
  const currentDistance = Math.sqrt(dx * dx + dy * dy);
  console.log(currentDistance)

  // Calculate the scale factor to maintain the initial distance
  const scaleFactor = initialDistance / currentDistance;
  //console.log(scaleFactor)
  //console.log(movingPoint)

  // Update the position of the moving point
  movingPoint.x = fixedPoint.x + dx * scaleFactor;
  movingPoint.y = fixedPoint.y + dy * scaleFactor;

  //console.log(movingPoint.x)
}



export default Hooks;
