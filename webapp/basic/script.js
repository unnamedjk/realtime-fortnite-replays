// var json = require('./X0C5yE86.json');
var myjson;
$.ajaxSetup({   async: false });
$.getJSON("/X0C5yE86.json", function(json){
    myjson = json;
});
console.log(myjson);
let canvas = $("#canvas");
let ctx = canvas.get(0).getContext("2d");
ctx.fillStyle = "green";
let Width = canvas.width();
let Height = canvas.height();
let playAnimation = true;

playerData = [
  {
    id: 1,
    locs: [{
      time: 300,
      x: 0,
      y: 0,
      state: 10
    },
    {
      time: 400,
      x: 100,
      y: 100,
      state: 0
    }
    ]
  }
]
let Location = function(x,y,time,state){
  this.x      = x,
  this.y      = y,
  this.time   = time;
  this.state  = state;
};

let Player = function (x,y,locs) {

  this.x = x;
  this.y = y;
  this.locs = locs;
}

let players = new Array();
for (i in playerData) {
  let x = 0;
  let y = 0;
  let locs = new Array();
  for (l in playerData[i].locs){
    let x = playerData[i].locs[l].x;
    let y = playerData[i].locs[l].y;
    let time = playerData[i].locs[l].time;
    let state = playerData[i].locs[l].state;
    locs.push(new Location(x,y,time,state));
  }
  players.push(new Player(x,y,locs));
}
var pct = 0.00;
var fps = 60;

animate();
function animate() {
  ctx.clearRect(0,0, Width, Height);
  for (var i = 0; i < players.length; i++) {
    // get reference to next shape
    var player = players[i];
    console.log(player.locs.length);
    locs = player.locs;
    for (var li = 0; li < locs.length; li++) {
      startx  = player.locs[li].x;
      starty  = player.locs[li].y;
      endx    = player.locs[li + 1].x;
      endy    = player.locs[li + 1].y;
      dx = locs[i+1] - player.x;
      dy = locs[i+1] - player.y;
      nx = player.startX + dx * pct;
      ny = player.startY + dx * pct;
      var player = players[i];
      ctx.fillRect(nx, ny, 7, 7);
    }

    /*
    var dx = shape.endX - player.x;
    var dy = shape.endY - shape.y;
    var nextX = shape.startX + dx * pct;
    var nextY = shape.startY + dy * pct;
    var shape = shapes[i];
    ctx.fillStyle = shape.color;
    ctx.fillRect(nextX, nextY, 7, 7);
    */
   }
};
/*
window.requestAnimFrame = (function (callback) {
    return window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame || function (callback) {
        window.setTimeout(callback, 1000 / 60);
    };
})();
var pct = 0.00;
var fps = 60;

animate();
function animate() {
    setTimeout(function () {

        if (pct <= 1.00) {
            requestAnimFrame(animate)
        };

        // increment the percent (from 0.00 to 1.00)
        pct += .01;

        // clear the canvas
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // draw all shapes
        for (var i = 0; i < shapes.length; i++) {

            // get reference to next shape
            var shape = shapes[i];

            // note: dx/dy are fixed values
            // they could be put in the shape object for efficiency
            var dx = shape.endX - shape.startX;
            var dy = shape.endY - shape.startY;
            var nextX = shape.startX + dx * pct;
            var nextY = shape.startY + dy * pct;
            var shape = shapes[i];
            ctx.fillStyle = shape.color;
            ctx.fillRect(nextX, nextY, 7, 7);
        }

    }, 1000 / fps);
}
*/