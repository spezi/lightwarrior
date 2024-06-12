import * as PIXI from 'pixi.js'
import { Layout } from "@pixi/layout";

let Hooks = {}

Hooks.MeasureDiv = {
  mounted() {
    window.addEventListener("resize", () => this.measureAndPushSize());
    this.measureAndPushSize();
    this.getPosition();
  },
  destroyed() {
    window.removeEventListener("resize", () => this.measureAndPushSize());
    window.removeEventListener("points_changed", () => this.messurePointDistance());
  },
  measureAndPushSize() {
    const { width, height } = this.el.getBoundingClientRect();
    this.pushEvent("mapping_div_size", { width, height });
  },
  getPosition() {
    const top = this.el.offsetTop;
    const left = this.el.offsetLeft;
    this.pushEvent("mapping_div_position", { top, left });
  }
}


Hooks.Points = {
  mounted() {
    console.log("points mounted")
    window.addEventListener("phx:pointchange", () => this.messurePointDistance());
  },
  destroyed() {
    window.removeEventListener("phx:pointchange", () => this.messurePointDistance());
  },
  messurePointDistance() {
    const container = document.getElementById('mapping');
    const start = document.getElementById('startpoint');
    const end = document.getElementById('endpoint');
    //console.log(start);
    //console.log(end);
    const startRect = start.getBoundingClientRect();
    const endRect = end.getBoundingClientRect();
    const x1 = startRect.left + startRect.width / 2;
    const y1 = startRect.top + startRect.height / 2;
    const x2 = endRect.left + endRect.width / 2;
    const y2 = endRect.top + endRect.height / 2;

    const verticies = {x1: start.style.left, y1: start.style.top, x2: end.style.left, y2: end.style.top}
    

    const distance = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
    console.log(`The distance between the points is ${distance}px`);
    console.log(verticies);
    this.pushEvent("points_messured", { distance, verticies });  
  }
}

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

const stripe = new PIXI.Container();
stripe.label = 'stripe';

const stripe_start = new PIXI.Graphics();
const stripe_end = new PIXI.Graphics();
const line = new PIXI.Graphics();
path = [
  0, 
  0, 
  0, 
  0
];

Hooks.Stage = {
  mounted() {
    this.handleEvent("data-ready", data => this.init_stage(data))
    console.log(app)
    if(app) {
      app.destroy();
    }
    app = new PIXI.Application();
  },
  // because liveview destroys elements  
  async reconnected(sprite, texture) {
    console.log("reconnected stage")
    console.log(app)
    console.log(sprite)
    console.log(this)

    

    app.canvas.width = mapping_container_wrapper.offsetWidth
    app.canvas.height = mapping_container_wrapper.offsetHeight
    
    stripe_start.label = 'stripe_start';
    stripe_start.position.set(0, 0)
    stripe_start.circle(0, 0, 6);
    stripe_start.fill({color:'blue', alpha:1});
    stripe_start.zIndex = 2;
    stripe_start.cursor = 'pointer';
    stripe_start.eventMode = 'static';
    stripe_start.on('pointerdown', onDragStart, stripe_start);

    stripe_end.label = 'stripe_end';
    stripe_end.position.set(400,0)
    stripe_end.circle(0, 0, 6);
    stripe_end.fill({color:'red', alpha:1});
    stripe_end.zIndex = 2;
    stripe_end.cursor = 'pointer';
    stripe_end.eventMode = 'static';
    stripe_end.on('pointerdown', onDragStart, stripe_end);

    //app.stage.addChild(stripe_start);
    //app.stage.addChild(stripe_end);
    stripe.addChild(stripe_start);
    stripe.addChild(stripe_end);

    stripe.position.set(100,100)

    console.log(app.stage)

    path = [
      stripe_start.x, 
      stripe_start.y, 
      stripe_end.x, 
      stripe_end.y
    ];

    console.log(path)

    line.label = 'line';
    line.poly(path);
    line.stroke({ width: 4, color: 0xffd900 });
    line.zIndex = 1;
    line.cursor = 'pointer';
    line.eventMode = 'static';
    line.on('pointerdown', onDragStart, line);
    
    //app.stage.addChild(line); 
    stripe.addChild(line);

    app.stage.addChild(stripe);

    app.stage.eventMode = 'static';
    app.stage.hitArea = app.screen;
    app.stage.on('pointerup', onDragEnd);
    app.stage.on('pointerupoutside', onDragEnd);

  },
  async init_stage() {
    console.log("init stage")
    console.log(this)
    console.log(mapping_container)
   

    const texture = await PIXI.Assets.load('/images/stanzraum.png');

    await app.init({ 
      width: mapping_container_wrapper.offsetWidth, 
      height: mapping_container_wrapper.offsetHeight, 
    
      canvas: mapping_container
    });

    var sprite = PIXI.Sprite.from(texture);
    sprite.width = app.screen.width
    sprite.height = app.screen.height
    
    app.stage.addChild(sprite);
    
    window.addEventListener("phx:page-loading-stop", 
    _info => this.reconnected(sprite, texture)//headerMenue.hide()
    )

    //window.addEventListener("resize", () => this.resize_stage());
    
    mapping_container.addEventListener("resize", () => this.resize_stage());

  },
  async resize_stage() {
    console.log("resize")
    
  }
}

function onDragStart(event)
{
    // Store a reference to the data
    // * The reason for this is because of multitouch *
    // * We want to track the movement of this particular touch *
    this.alpha = 0.5;
    dragTarget = this;
    

    console.log(event.global);
    console.log(dragTarget.parent.pivot);

    if(dragTarget.label == 'line')
    { 
      dragTarget.parent.toLocal(event.global, null, dragTarget.parent.pivot);
      dragTarget.parent.position = event.global;
    }

    app.stage.on('pointermove', onDragMove);

}

function onDragMove(event)
{
  if (dragTarget)
    console.log(dragTarget.label)
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
          console.log(event.global)
          //dragTarget.parent.toLocal(event.global, null, dragTarget.parent.position);
          dragTarget.parent.position = event.global;
        } else {
          dragTarget.parent.toLocal(event.global, null, dragTarget.position);
        } 
    }
}

function onDragEnd()
{
    if (dragTarget)
    {
        app.stage.off('pointermove', onDragMove);
        dragTarget.alpha = 1;
        dragTarget = null;
    }
}




export default Hooks;
