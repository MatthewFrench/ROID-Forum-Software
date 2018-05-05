library BlockSelector;
import '../BlockGameController.dart';
import 'package:three/three.dart';

class BlockSelector {
  BlockGameController blockGame;
  Mesh selectorMesh;
  double x, y;
  int color;
  BlockSelector(BlockGameController bg, int _color) {
    blockGame = bg;
    color = _color;
    selectorMesh = blockGame.drawController.createBlockSelector3DMesh(color);
    x = 0.0;
    y = 0.0;
  }
  void updateMeshPosition() {
    selectorMesh.position.setValues(x * 20, y * 20, 1.0);
  }
}