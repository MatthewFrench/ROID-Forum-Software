library BlockController;
import 'BlockGameController.dart';
import 'Blocks/Block.dart';

class BlockController {
  BlockGameController blockGame;
  List<Block> blocks = new List();
  BlockController(BlockGameController bg) {
    blockGame = bg;
  }
  void addBlock(int id, double x, double y, num color) {
    Block b = new Block(blockGame, id, color);
    b.x = x;
    b.y = y;
    b.updateMeshPosition();
    blocks.add(b);
    blockGame.drawController.addMeshToScene(b.blockMesh);
  }
  void updateBlockColor(int id, num color) {
    Block b = getBlockByID(id);
    if (b == null) return;
    //Cause materials are cached, just kill and recreate block
    double x = b.x;
    double y = b.x;
    blocks.remove(b);
    blockGame.drawController.removeMeshFromScene(b.blockMesh);
    addBlock(id, x, y, color);
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
    blockGame.drawController.removeMeshFromScene(b.blockMesh);
  }
}
