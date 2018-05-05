library Player;
import '../BlockGameController.dart';
import '../../../User.dart';

class Player {
  BlockGameController blockGame;
  bool upPressed = false,
      leftPressed = false,
      rightPressed = false,
      downPressed = false,
      onFlatSurface = true;
  double x,
      y,
      xVel = 0.0,
      yVel = 0.0;
  User user;
  int id;
  String name;
  Player(BlockGameController bg, User u, int _id) {
    blockGame = bg;
    x = 0.0;
    y = 0.0;
    user = u;
    id = _id;
    name = user.account.name;
  }
}
