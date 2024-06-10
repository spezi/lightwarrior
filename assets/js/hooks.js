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
    this.pushEvent("div_size", { width, height });
  },
  getPosition() {
    const top = this.el.offsetTop;
    const left = this.el.offsetLeft;
    this.pushEvent("mapping_position", { top, left });
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


export default Hooks;
