library CodingFancyBackground;
import 'dart:html';
import 'CodingSection.dart';
import 'dart:math';
import 'dart:convert';

class FancyBackground {
  CodingSection controller;
  CanvasElement backgroundCanvas;
  bool showing = false;
  List<MovingLetter> letters = new List();
  double currentOpacity = 0.0;
  int width = 0,
      height = 0;
  FancyBackground(CodingSection c) {
    controller = c;
    backgroundCanvas = new CanvasElement();
    backgroundCanvas.style
        ..position = "absolute"
        ..display = "block"
        ..left = "0px"
        ..perspective = "1000px"
        ..perspectiveOrigin = "50% 50%"
        ..transformStyle = "preserve-3d"
        ..transform = "translateZ(-250px)"
        ..top = "0px";
  }
  void show() {
    showing = true;
    backgroundCanvas.style.opacity = "0.0";
    currentOpacity = 0.0;
    if (controller.website.behindWebsiteDiv.children.length > 0) {
      controller.website.behindWebsiteDiv.insertBefore(backgroundCanvas, controller.website.behindWebsiteDiv.children[0]);
    } else {
      controller.website.behindWebsiteDiv.append(backgroundCanvas);
    }
  }
  void hide() {
    showing = false;
    backgroundCanvas.style.opacity = "1.0";
  }
  void logic() {
    if (showing) {
      if (currentOpacity < 1.0) {
        currentOpacity += 0.05;
        backgroundCanvas.style.opacity = "${currentOpacity}";
      }
      letterLogic();
    } else {
      if (currentOpacity > 0.0) {
        letterLogic();
        currentOpacity -= 0.05;
        backgroundCanvas.style.opacity = "${currentOpacity}";
        if (currentOpacity <= 0.0) {
          backgroundCanvas.remove();
          letters.clear();
          clearCanvas();
        }
      }
    }
  }
  void letterLogic() {
    Random r = new Random();
    for (int i = 0; i < letters.length; i++) {
      MovingLetter l = letters[i];
      l.y += 2;
      if (r.nextInt(10) == 0) {
        l.letter = ASCII.decode([r.nextInt(94) + 33]);
      }
      if (l.y > backgroundCanvas.height + 10) {
        letters.remove(l);
        i--;
      }
    }
    draw();
    if (r.nextBool()) {
      MovingLetter l = new MovingLetter(ASCII.decode([r.nextInt(94) + 33]), r.nextInt(backgroundCanvas.width + 10) - 5.0, -5.0);
      letters.add(l);
    }
  }
  void clearCanvas() {
    CanvasRenderingContext2D ctx = backgroundCanvas.context2D;
    backgroundCanvas.width = window.innerWidth;
    backgroundCanvas.height = window.innerHeight;
  }
  void draw() {
    CanvasRenderingContext2D ctx = backgroundCanvas.context2D;
    if (width != window.innerWidth || height != window.innerHeight) {
      width = window.innerWidth;
      height = window.innerHeight;
      backgroundCanvas.width = window.innerWidth;
      backgroundCanvas.height = window.innerHeight;
    }
    ctx.fillStyle = "rgba(0,0,0,0.05)";
    ctx.fillRect(0, 0, width, height);
    ctx.fillStyle = "rgba(0,150,0,1.0)";
    ctx.font = '15px Helvetica';
    ctx.textAlign = "center";
    for (MovingLetter l in letters) {
      ctx.fillText(l.letter, l.x, l.y);
    }
  }
}
class MovingLetter {
  double x = 0.0,
      y = 0.0;
  String letter = "";
  MovingLetter(String l, double _x, double _y) {
    letter = l;
    x = _x;
    y = _y;
  }
}
