import { fabric } from "fabric"
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

Hooks.Fabric = {
  mounted() {
    this.handleEvent("data-ready", data => this.init_canvas(data))
  },
  init_canvas() {
    console.log("canvas")
    
    // create a wrapper around native canvas element (with id="c")
    var canvas = new fabric.Canvas('mapping_canvas');
    
    //canvas.setDimensions()

    // create a rectangle object
    var rect = new fabric.Rect({
    left: 100,
    top: 100,
    fill: 'red',
    width: 20,
    height: 20
    });

    // "add" rectangle onto canvas
    canvas.add(rect);
  }
}

// Create a PixiJS application.
const app = new PIXI.Application();
const mapping_container = document.getElementById('mapping');

Hooks.Stage = {
  mounted() {
    this.handleEvent("data-ready", data => this.init_stage(data))
  },
  async reconnected(sprite, texture) {
    console.log("reconnected stage")
    console.log(app)
    console.log(sprite)
    //app.stage.removeChildren();
    app.queueResize = true

    app.canvas.width = 1920
    app.canvas.height = 1080
    //sprite.width = 640
    //sprite.height = 360
    //app.stage.addChild(sprite);
    //app.stage.children[0].width = app.screen.width
    //app.stage.children[0].height = app.screen.height
  },
  async init_stage() {
    console.log("init stage")
    console.log(this)

    const texture = await PIXI.Assets.load('/images/stanzraum.png');

    await app.init({ 
      width: 1920, 
      height: 1080, 
    
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




export default Hooks;
