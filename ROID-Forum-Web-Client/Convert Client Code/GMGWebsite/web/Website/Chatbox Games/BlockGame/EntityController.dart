library EntityController;
import 'BlockGameController.dart';
import 'Entities/Player.dart';

class EntityController {
  BlockGameController blockGame;
  List<Player> players = new List();
  Player mainPlayer;
  EntityController(BlockGameController bg) {
    blockGame = bg;
  }
  void addPlayer(int id, String name, double x, double y) {
    Player p = new Player(blockGame, id, name);
    p.x = x;
    p.y = y;
    p.updateMeshPosition();
    players.add(p);
    blockGame.drawController.addMeshToScene(p.player3DMesh);
  }
  void updatePlayer(int id, double x, double y) {
    Player p = getPlayer(id);
    if (p == null) return;
    p.x = x;
    p.y = y;
    p.updateMeshPosition();
  }
  void setMainPlayer(int id) {
    Player p = getPlayer(id);
    if (p == null) return;
    mainPlayer = p;
  }
  Player getPlayer(int id) {
    for (Player p in players) {
      if (p.id == id) return p;
    }
    return null;
  }
  void removePlayer(int id) {
    Player p = getPlayer(id);
    if (p == null) return;
    players.remove(p);
    blockGame.drawController.removeMeshFromScene(p.player3DMesh);
    if (mainPlayer == p) {
      mainPlayer = null;
    }
  }
}