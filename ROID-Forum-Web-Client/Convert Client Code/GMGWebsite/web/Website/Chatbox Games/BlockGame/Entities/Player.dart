library Player;
import '../BlockGameController.dart';
import 'package:three/three.dart';

class Player {
  BlockGameController blockGame;
  bool upPressed = false,
      leftPressed = false,
      rightPressed = false,
      downPressed = false,
      onFlatSurface = true;
  Mesh player3DMesh;
  double x,
      y,
      xVel = 0.0,
      yVel = 0.0;
  int id;
  String name;
  Player(BlockGameController bg, int _id, String _name) {
    blockGame = bg;
    x = 0.0;
    y = 0.0;
    id = _id;
    name = _name;
    player3DMesh = blockGame.drawController.createPlayer3DMesh();
  }
  void updateMeshPosition() {
    //Add 10 to place draw origin at feet of block
    player3DMesh.position.setValues(x * 20, y * 20 + 10, player3DMesh.position.z);
  }
}
