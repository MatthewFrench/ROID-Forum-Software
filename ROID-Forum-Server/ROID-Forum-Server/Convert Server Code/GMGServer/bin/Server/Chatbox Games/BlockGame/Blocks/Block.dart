library Block;
import '../BlockGameController.dart';

class Block {
  BlockGameController blockGame;
  double x, y;
  int color;
  int id;
  Block(BlockGameController bg, int _id, int _color) {
    blockGame = bg;
    id = _id;
    color = _color;
    x = 0.0;
    y = 0.0;
  }
}