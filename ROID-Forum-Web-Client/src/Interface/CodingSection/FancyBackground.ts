import {Section} from "./Section";
import {Interface} from "../../Utility/Interface";

export class FancyBackground {
  controller : Section;
  backgroundCanvas : HTMLCanvasElement;
  showing = false;
  letters : MovingLetter[] = [];
  currentOpacity = 0.0;
  width = 0;
  height = 0;
  constructor(c : Section) {
    this.controller = c;
    this.backgroundCanvas = Interface.Create({type: 'canvas'});
        this.backgroundCanvas.style.position = "absolute";
        this.backgroundCanvas.style.display = "block";
        this.backgroundCanvas.style.left = "0px";
        this.backgroundCanvas.style.perspective = "1000px";
        this.backgroundCanvas.style.perspectiveOrigin = "50% 50%";
        this.backgroundCanvas.style.transformStyle = "preserve-3d";
        this.backgroundCanvas.style.transform = "translateZ(-250px)";
        this.backgroundCanvas.style.top = "0px";
  }
  show() {
    this.showing = true;
    this.backgroundCanvas.style.opacity = "0.0";
    this.currentOpacity = 0.0;
    if (this.controller.website.behindWebsiteDiv.children.length > 0) {
      this.controller.website.behindWebsiteDiv.insertBefore(this.backgroundCanvas, this.controller.website.behindWebsiteDiv.children[0]);
    } else {
      this.controller.website.behindWebsiteDiv.appendChild(this.backgroundCanvas);
    }
  }
  hide() {
    this.showing = false;
    this.backgroundCanvas.style.opacity = "1.0";
  }
  logic() {
    if (this.showing) {
      if (this.currentOpacity < 1.0) {
        this.currentOpacity += 0.05;
        this.backgroundCanvas.style.opacity = `${this.currentOpacity}`;
      }
      this.letterLogic();
    } else {
      if (this.currentOpacity > 0.0) {
        this.letterLogic();
        this.currentOpacity -= 0.05;
        this.backgroundCanvas.style.opacity = `${this.currentOpacity}`;
        if (this.currentOpacity <= 0.0) {
          this.backgroundCanvas.remove();
          this.letters = [];
          this.clearCanvas();
        }
      }
    }
  }
  letterLogic() {
    for (let i = 0; i < this.letters.length; i++) {
      let l : MovingLetter = this.letters[i];
      l.y += 2;
      if (Math.floor(Math.random() * 10) == 0) {
        l.letter = String.fromCharCode(Math.floor(Math.random() * 94) + 33);
      }
      if (l.y > this.backgroundCanvas.height + 10) {
        this.letters.splice(this.letters.indexOf(l), 1);
        i--;
      }
    }
    this.draw();
    if (Math.floor(Math.random() * 2) == 0) {
      let l : MovingLetter = new MovingLetter(String.fromCharCode(Math.floor(Math.random() * 94)  + 33), Math.floor(Math.random() * (this.backgroundCanvas.width + 10)) - 5.0, -5.0);
      this.letters.push(l);
    }
  }
  clearCanvas() {
    this.backgroundCanvas.width = window.innerWidth;
    this.backgroundCanvas.height = window.innerHeight;
  }
  draw() {
    let ctx : CanvasRenderingContext2D = this.backgroundCanvas.getContext("2d");
    if (this.width != window.innerWidth || this.height != window.innerHeight) {
      this.width = window.innerWidth;
      this.height = window.innerHeight;
      this.backgroundCanvas.width = window.innerWidth;
      this.backgroundCanvas.height = window.innerHeight;
    }
    ctx.fillStyle = "rgba(0,0,0,0.05)";
    ctx.fillRect(0, 0, this.width, this.height);
    ctx.fillStyle = "rgba(0,150,0,1.0)";
    ctx.font = '15px Helvetica';
    ctx.textAlign = "center";
    for (let l of this.letters) {
      ctx.fillText(l.letter, l.x, l.y);
    }
  }
}
class MovingLetter {
  x = 0.0;
  y = 0.0;
  letter = "";
  constructor(l : string, _x : number, _y : number) {
    this.letter = l;
    this.x = _x;
    this.y = _y;
  }
}