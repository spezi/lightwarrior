(function (window, document) {
    "use strict";

    var options = {
        autoRun: true,
        barThickness: 3,
        barColors: {
          0: "rgba(26,  188, 156, .9)",
          ".25": "rgba(52,  152, 219, .9)",
          ".50": "rgba(241, 196, 15,  .9)",
          ".75": "rgba(230, 126, 34,  .9)",
          "1.0": "rgba(211, 84,  0,   .9)",
        },
        shadowBlur: 10,
        shadowColor: "rgba(0,   0,   0,   .6)",
        className: null,
        flyout: document.getElementById('header_flyout'),
    },
    headerMenue = {
        config: function (opts) {
          for (var key in opts)
            if (options.hasOwnProperty(key)) options[key] = opts[key];
        },
      };

      if (typeof module === "object" && typeof module.exports === "object") {
        module.exports = headerMenue;
      } else if (typeof define === "function" && define.amd) {
        define(function () {
          return headerMenue;
        });
      } else {
        this.headerMenue = headerMenue;
      }

    }.call(this, window, document));