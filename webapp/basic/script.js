// var json = require('./X0C5yE86.json');
var myjson;
$.ajaxSetup({   async: false });
$.getJSON("/X0C5yE86.json", function(json){
    myjson = json;
});
//console.log(myjson);
c = document.getElementById("canvas");
ctx = c.getContext("2d");

c.width = 210;
c.height = 100;

class Player {
  constructor(x,y,color,speed){
    this.x = x;
    this.y = y;
    this.color = color;
    this.size = 20;
    this.speed = speed
  }
  draw() {
    ctx.fillStyle = this.color;
    ctx.beginPath();
    ctx.fillRect(this.x, this.y, this.size, this.size);
    ctx.fill;
  }
  update(x,y){
    this.y = y;
    this.x = x;
    //if (this.y > c.height + this.size) this.y = 0 - this.size;
  }
}

let players = [
  new Player(25,25, Math.floor(Math.random()*16777215).toString(16), 1),
  new Player(50,50, Math.floor(Math.random()*16777215).toString(16), 1),
  new Player(75,75, Math.floor(Math.random()*16777215).toString(16), 1),
  new Player(100,100, Math.floor(Math.random()*16777215).toString(16), 1)
];
const testArr = [50, 50, 50, 100, 100, 100, 100, 50, 50, 50];

var i = 2; // You need a global variable to track progress
function timeloop() {
  // You have to beginPath and stroke each loop
  // Therefore you need moveTo each loop aswell
  ctx.beginPath();
  ctx.moveTo(testArr[i-2], testArr[i-1]);
  ctx.lineTo(testArr[i++], testArr[i++]);
  ctx.stroke();

  // Check if another iteration should happen
  if (i+1 < testArr.length) { setTimeout(timeloop, 1000); }
}
timeloop();