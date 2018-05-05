library LogicController;
import 'BlockGameController.dart';
//import 'dart:async';

class LogicController {
  BlockGameController blockGame;
  //Timer timer;
  LogicController(BlockGameController bg) {
    blockGame = bg;
    
    //timer = new Timer.periodic(new Duration(milliseconds: 8), logic); //Logic runs at 120fps, drawing runs at 60fps
  }
  void logic(/*_*/) {
    blockGame.guiController.guiLogic();
    if (blockGame.drawController.stopwatch.elapsedMilliseconds >= 16) {
      blockGame.drawController.renderScene();
    }
  }
}