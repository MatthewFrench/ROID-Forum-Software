library GuiController;
import 'dart:html';
import 'BlockGameController.dart';
import 'Gui/BlockSelector.dart';
import 'Blocks/Block.dart';
import 'Gui/ColorPallet.dart';

class GuiController {
  BlockGameController blockGame;
  Element container;
  BlockSelector blockSelector;
  int mouseX = 0,
      mouseY = 0;
  bool mousePressed = false;
  ColorPallet colorPallet;
  GuiController(BlockGameController bg) {
    blockGame = bg;
    container = new DivElement();
    container.style
        ..overflow = "hidden"
        ..position = "absolute"
        ..left = "0px"
        ..top = "0px"
        ..width = "100%"
        ..height = "100%";
    window.onResize.listen(onWindowResize);
    window.onKeyDown.listen(onWindowKeyDown);
    window.onKeyUp.listen(onWindowKeyUp);
    window.onMouseMove.listen(onWindowMouseMove);
    window.onMouseDown.listen(onWindowMouseDown);
    window.onMouseUp.listen(onWindowMouseUp);
  }
  void guiLogic() {
    if (blockGame.entityController.mainPlayer != null) {
      //Selector logic
      double pX = blockGame.entityController.mainPlayer.x;
      double pY = blockGame.entityController.mainPlayer.y;
      double x = ((mouseX - window.innerWidth / 2.0 - 10) / 20.0 * 0.8 + pX);
      double y = (-(mouseY - window.innerHeight / 2.0) / 20.0 * 0.8 + pY);
      blockSelector.x = x.ceil().toDouble();
      blockSelector.y = y.ceil().toDouble();
      blockSelector.updateMeshPosition();
      if (mousePressed) {
  
        Block b = blockGame.blockController.getBlock(blockSelector.x.round(), blockSelector.y.round());
        if (colorPallet.selectedBlock == 0) { //erase
          if (b != null) {
            blockGame.messageSender.sendEraseBlock(b.id);
          }
        } else {
          blockGame.messageSender.sendSetBlockColor(blockSelector.x.round(), blockSelector.y.round(), blockSelector.color);
        }
      }
  
      colorPallet.logic();
    }
  }
  void createGuiElements() {
    blockSelector = new BlockSelector(blockGame, 0xFFFFFF);
    blockSelector.y = 5.0;
    blockSelector.updateMeshPosition();
    blockGame.drawController.addMeshToScene(blockSelector.selectorMesh);

    colorPallet = new ColorPallet(blockGame);
  }
  void onWindowMouseDown(MouseEvent e) {
    mousePressed = true;
  }
  void onWindowMouseUp(MouseEvent e) {
    mousePressed = false;
  }
  void onWindowMouseMove(MouseEvent e) {
    mouseX = e.client.x;
    mouseY = e.client.y;
  }
  void onWindowKeyDown(KeyboardEvent e) {
    if (blockGame.entityController.mainPlayer != null) {
    if (e.keyCode == KeyCode.UP || e.keyCode == KeyCode.W || e.keyCode == KeyCode.SPACE) {
      blockGame.messageSender.sendUpArrowDown();
    }
    if (e.keyCode == KeyCode.DOWN || e.keyCode == KeyCode.S) {
      blockGame.messageSender.sendDownArrowDown();
    }
    if (e.keyCode == KeyCode.LEFT || e.keyCode == KeyCode.A) {
      blockGame.messageSender.sendLeftArrowDown();
    }
    if (e.keyCode == KeyCode.RIGHT || e.keyCode == KeyCode.D) {
      blockGame.messageSender.sendRightArrowDown();
    }
    //Color chooser
    if (e.keyCode == KeyCode.Q) {
      colorPallet.downOne();
    }
    if (e.keyCode == KeyCode.E) {
      colorPallet.upOne();
    }
    }
  }
  void onWindowKeyUp(KeyboardEvent e) {
    if (blockGame.entityController.mainPlayer != null) {
    if (e.keyCode == KeyCode.UP || e.keyCode == KeyCode.W || e.keyCode == KeyCode.SPACE) {
      blockGame.messageSender.sendUpArrowUp();
    }
    if (e.keyCode == KeyCode.DOWN || e.keyCode == KeyCode.S) {
      blockGame.messageSender.sendDownArrowUp();
    }
    if (e.keyCode == KeyCode.LEFT || e.keyCode == KeyCode.A) {
      blockGame.messageSender.sendLeftArrowUp();
    }
    if (e.keyCode == KeyCode.RIGHT || e.keyCode == KeyCode.D) {
      blockGame.messageSender.sendRightArrowUp();
    }
    }
  }
  void onWindowResize(e) {
    blockGame.drawController.camera.aspect = window.innerWidth / window.innerHeight;
    blockGame.drawController.camera.updateProjectionMatrix();
    blockGame.drawController.renderer.setSize(window.innerWidth, window.innerHeight);
  }
}