library ColorPallet;
import '../BlockGameController.dart';
import 'package:three/three.dart';
import 'dart:html';

class ColorPallet {
  BlockGameController blockGame;
  List<PalletBlock> palletBlocks = new List();
  int selectedBlock = 0;
  ColorPallet(BlockGameController bg) {
    blockGame = bg;

    blockGame.guiController.blockSelector.selectorMesh.material.color.setHex(0xFFFFFF);

    PalletBlock p = new PalletBlock(blockGame, int.parse("FFFFFF", radix: 16));
    palletBlocks.add(p);
    blockGame.drawController.addMeshToScene(p.blockMesh);

    p = new PalletBlock(blockGame, int.parse("000000", radix: 16));
    palletBlocks.add(p);
    blockGame.drawController.addMeshToScene(p.blockMesh);

    p = new PalletBlock(blockGame, int.parse("F6FF00", radix: 16));
    palletBlocks.add(p);
    blockGame.drawController.addMeshToScene(p.blockMesh);

    p = new PalletBlock(blockGame, int.parse("0022FF", radix: 16));
    palletBlocks.add(p);
    blockGame.drawController.addMeshToScene(p.blockMesh);

    p = new PalletBlock(blockGame, int.parse("FF0000", radix: 16));
    palletBlocks.add(p);
    blockGame.drawController.addMeshToScene(p.blockMesh);

    p = new PalletBlock(blockGame, int.parse("00FF15", radix: 16));
    palletBlocks.add(p);
    blockGame.drawController.addMeshToScene(p.blockMesh);

    p = new PalletBlock(blockGame, int.parse("FF00FB", radix: 16));
    palletBlocks.add(p);
    blockGame.drawController.addMeshToScene(p.blockMesh);
  }
  void upOne() {
    selectedBlock += 1;
    if (selectedBlock >= palletBlocks.length) {
      selectedBlock = 0;
    }
    PalletBlock p = palletBlocks[selectedBlock];
    blockGame.guiController.blockSelector.selectorMesh.material.color.setHex(p.color);
  }
  void downOne() {
    selectedBlock -= 1;
    if (selectedBlock < 0) {
      selectedBlock = palletBlocks.length - 1;
    }
    PalletBlock p = palletBlocks[selectedBlock];
    blockGame.guiController.blockSelector.selectorMesh.material.color.setHex(p.color);
  }
  int getColor() {
    PalletBlock p = palletBlocks[selectedBlock];
    return p.color;
  }
  void logic() {
    //Update pallet position relative to player, at the bottom of the screen
    double x = (blockGame.entityController.mainPlayer.x * 20 - window.innerWidth / 2 * 0.7) / 20.0;
    double y = (blockGame.entityController.mainPlayer.y * 20 - window.innerHeight / 2 * 0.65) / 20.0;
    for (int i = 0; i < palletBlocks.length; i++) {
      PalletBlock p = palletBlocks[i];
      p.x = x + i * 2;
      p.y = y;
      if (i == selectedBlock) {
        p.rot += 0.01;
      } else {
        p.rot = 0.0;
      }
      p.updateMeshPosition();
    }

  }
}
class PalletBlock {
  BlockGameController blockGame;
  Mesh blockMesh;
  double x, y, rot;
  num color;
  PalletBlock(BlockGameController bg, num _color) {
    blockGame = bg;
    color = _color;
    blockMesh = blockGame.drawController.createBlock3DMesh(color);
    x = 0.0;
    y = 0.0;
    rot = 0.0;
  }
  void updateMeshPosition() {
    blockMesh.rotation.setValues(blockMesh.rotation.x, rot, blockMesh.rotation.z);
    blockMesh.position.setValues(x * 20, y * 20, 20.0);
  }
}
