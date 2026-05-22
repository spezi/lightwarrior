({
  inlets: [
    { type: 'message', name: 'r' },
    { type: 'message', name: 'g' },
    { type: 'message', name: 'b' }
  ],
  outlets: [
    { type: 'message', name: 'color' }
  ],

  rVal: 0.0,
  gVal: 0.0,
  bVal: 0.0,

  update: function() {
    if (this.r.length) this.rVal = this.r[this.r.length - 1].value;
    if (this.g.length) this.gVal = this.g[this.g.length - 1].value;
    if (this.b.length) this.bVal = this.b[this.b.length - 1].value;

    return {
      color: [{ value: [this.rVal, this.gVal, this.bVal, 1.0] }]
    };
  }
})