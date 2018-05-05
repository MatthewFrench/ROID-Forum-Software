library BlockController;
import 'BlockGameController.dart';
import 'Blocks/Block.dart';

class BlockController {
  BlockGameController blockGame;
  List<Block> blocks = new List();
  int blockIDs = 0;
  BlockController(BlockGameController bg) {
    blockGame = bg;
  }
  void createDefaultBlocks() {
    for (int i = -5; i < 5; i++) {
      Block b = new Block(blockGame, blockIDs++, int.parse("000000", radix: 16));
      b.x = i.toDouble();
      addBlock(b);
    }
    for (int i = -10; i < -5; i++) {
      Block b = new Block(blockGame, blockIDs++, int.parse("000000", radix: 16));
      b.x = i.toDouble();
      b.y = 1.0;
      addBlock(b);
    }
    for (int i = 5; i < 10; i++) {
      Block b = new Block(blockGame, blockIDs++, int.parse("000000", radix: 16));
      b.x = i.toDouble();
      b.y = -1.0;
      addBlock(b);
    }
  }
  void setBlockColor(int x, int y, num color) {
    Block b = getBlock(x, y);
    if (b == null) { //Create new block
      Block b = new Block(blockGame, blockIDs++, color);
      b.x = x.toDouble();
      b.y = y.toDouble();
      addBlock(b);
    } else {
      b.color = color;
      blockGame.messageSender.sendUpdateBlockColorToAll(b);
    }
  }
  void addBlock(Block b) {
    blocks.add(b);
    blockGame.messageSender.sendAddBlockToAll(b);
  }
  Block getBlock(int x, int y) {
    for (int i = 0; i < blocks.length; i++) {
      Block b = blocks[i];
      if (b.x.round() == x && b.y.round() == y) {
        return b;
      }
    }
    return null;
  }
  Block getBlockByID(int id) {
    for (int i = 0; i < blocks.length; i++) {
      Block b = blocks[i];
      if (id == b.id) {
        return b;
      }
    }
    return null;
  }
  void removeBlock(int id) {
    Block b = getBlockByID(id);
    if (b == null) return;
    blocks.remove(b);
    blockGame.messageSender.sendRemoveBlockToAll(id);
  }
}