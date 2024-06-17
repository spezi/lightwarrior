import * as PIXI from 'pixi.js'

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
var line = new PIXI.Graphics();
var path = [
  0, 
  0, 
  0, 
  0
];

var liveview = null

var dragPosition = new PIXI.Point(0,0)

//var fetched_all_stripes_config = null
//var selected_stripe_data_pixel = null

Hooks.Stage = {
  async mounted() {
    
    liveview = this
    //this.handleEvent("data-ready", data => this.init_stage(data))
    this.handleEvent("stripe-select", _info => this.select_stripe()),
    this.handleEvent("stripe-ready", data => this.stripe_ready(data))
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
  stripe_change() {
    //console.log(line.getBounds())
    //liveview.pushEvent("phx:stripe_change", line.getBounds());
    
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
    liveview.pushEvent("phx:stripe_change", points); 
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
    stripe_start.zIndex = 2;
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
    stripe_end.zIndex = 2;
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
    line.zIndex = 1;
    line.cursor = 'grab';
    line.eventMode = 'static';
    line.on('pointerdown', onDragStart, line);
    
    console.log(line)

    //app.stage.addChild(line); 
    stripe.addChild(line);

    app.stage.addChild(stripe);
  }
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
      if(dragTarget.label == 'stripe_start')
        {
          path = [
            dragTarget.position.x, 
            dragTarget.position.y, 
            stripe_end.x, 
            stripe_end.y
          ];
          line.clear()
          line.poly(path);
          line.stroke({ width: 4, color: 0xffd900 });
        }
      if(dragTarget.label == 'stripe_end')
        {
          path = [
            stripe_start.x, 
            stripe_start.y,
            dragTarget.position.x, 
            dragTarget.position.y,
          ];
          line.clear()
          line.poly(path);
          line.stroke({ width: 4, color: 0xffd900 });
        }
      if(dragTarget.label == 'line')
        {
          //console.log(event.global)
          //dragTarget.parent.toLocal(event.global, null, dragTarget.parent.position);
    
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
        }

        app.stage.off('pointermove', onDragMove);
        dragTarget.alpha = 1;
        dragTarget.parent.alpha = 1;
        dragTarget = null;
        Hooks.Stage.stripe_change();
        //console.log(this)
        //console.log(dragTarget)
        //console.log(app.stage)
    }
}




export default Hooks;
