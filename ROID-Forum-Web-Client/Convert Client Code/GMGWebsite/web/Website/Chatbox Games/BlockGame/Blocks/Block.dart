library Block;
import '../BlockGameController.dart';
import 'package:three/three.dart';

class Block {
  BlockGameController blockGame;
  Mesh blockMesh;
  double x, y;
  int color;
  int id;
  Block(BlockGameController bg, int _id, int _color) {
    blockGame = bg;
    id = _id;
    color = _color;
    x = 0.0;
    y = 0.0;
    blockMesh = blockGame.drawController.createBlock3DMesh(color);
    blockMesh.matrixAutoUpdate = false;
  }
  void updateMeshPosition() {
    blockMesh.position.setValues(x * 20, y * 20, blockMesh.position.z);
    blockMesh.updateMatrix();
  }
}