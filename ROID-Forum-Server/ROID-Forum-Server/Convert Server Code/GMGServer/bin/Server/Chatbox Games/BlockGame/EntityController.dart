library EntityController;
import 'BlockGameController.dart';
import 'Entities/Player.dart';
import 'Blocks/Block.dart';
import '../../User.dart';

class EntityController {
  BlockGameController blockGame;
  List<Player> players = new List();
  Player mainPlayer;
  int idCount = 0;
  EntityController(BlockGameController bg) {
    blockGame = bg;
  }
  void addPlayer(User u) {
    Player newPlayer = new Player(blockGame, u, idCount++);
    players.add(newPlayer);
    blockGame.messageSender.sendAddPlayerToAll(newPlayer);
    blockGame.messageSender.sendSelfID(u, newPlayer.id);
  }
  Player getPlayer(User u) {
    for (Player p in players) {
      if (p.user == u) return p;
    }
    return null;
  }
  void removePlayer(User u) {
    Player p = getPlayer(u);
    if (p == null) return;
    players.remove(p);
    blockGame.messageSender.sendRemovePlayerToAll(p.id);
  }


  double jumpVel = 0.5;
  double moveSpeed = 0.06;
  double maxMoveSpeed = 0.3;
  double gravityVel = 0.02;
  double frictionCoef = 0.05;
  double airFrictionCoef = 0.02;
  double maxSpeedLimit = 0.9;
  void movePlayers() {
    for (int i = 0; i < players.length; i++) {
      Player p = players[i];
      double pOldX = p.x;
      double pOldY = p.y;
      if (p.upPressed) {
        if (p.onFlatSurface && p.yVel == 0.0) {
          p.yVel += jumpVel;
        }
      }
      if (p.leftPressed) {
        if (p.xVel > -maxMoveSpeed) {
          p.xVel -= moveSpeed;
        }
      }
      if (p.rightPressed) {
        if (p.xVel < maxMoveSpeed) {
          p.xVel += moveSpeed;
        }
      }
      if (p.onFlatSurface) { //Slow down the x velocity if touching ground
        if (p.xVel.abs() < frictionCoef) {
          p.xVel = 0.0;
        } else if (p.xVel < 0.0) {
          p.xVel += frictionCoef;
        } else {
          p.xVel -= frictionCoef;
        }
      } else { //Fall
        p.yVel -= gravityVel;
        if (p.xVel.abs() < airFrictionCoef) {
          p.xVel = 0.0;
        } else if (p.xVel < 0.0) {
          p.xVel += airFrictionCoef;
        } else {
          p.xVel -= airFrictionCoef;
        }
      }
      if (p.yVel != 0.0) {
        p.onFlatSurface = false;
      }
      //Limit speed so no tunneling. Alternatively can chop movement to fix tunneling
      if (p.xVel > maxSpeedLimit) {
        p.xVel = maxSpeedLimit;
      }
      if (p.xVel < -maxSpeedLimit) {
        p.xVel = -maxSpeedLimit;
      }
      if (p.yVel > maxSpeedLimit) {
        p.yVel = maxSpeedLimit;
      }
      if (p.yVel < -maxSpeedLimit) {
        p.yVel = -maxSpeedLimit;
      }

      p.x += p.xVel;
      p.y += p.yVel;
      //Check if touching any blocks. If so fix position, set velocity and flat surface status
      bool blockTouchingBelow = false;
      for (int j = 0; j < blockGame.blockController.blocks.length; j++) {
        Block b = blockGame.blockController.blocks[j];
        if ((p.x - b.x).abs() < 1.0) { //Touching each other horizontally, next check vertically
          if (p.y - b.y < 1.0 && p.y - b.y >= 0.0) { //Block is touching player from below
            //Only move up if there isn't another block in front
            if (blockGame.blockController.getBlock(b.x.round(), b.y.round() + 1) == null && blockGame.blockController.getBlock(p.x.round(), b.y.round() + 2) == null) {
              p.y = b.y + 1;
            } else {
              //Run into the wall
              if (p.x < b.x && p.xVel > 0) {
                p.xVel = 0.0;
                p.x = b.x - 1;
              }
              if (p.x > b.x && p.xVel < 0) {
                p.xVel = 0.0;
                p.x = b.x + 1;
              }
            }
            if ((p.x - b.x).abs() < 1) {
              blockTouchingBelow = true;
            }
          } else if (b.y - p.y < 2.0 && b.y - p.y > 1.0 && p.onFlatSurface == false) { //Block is touching player from above
            if (blockGame.blockController.getBlock(b.x.round(), b.y.round() - 2) == null) {
              p.y = b.y - 2;
              if (p.yVel > 0.0) {
                p.yVel = 0.0;
              }
            }
          } else if (b.y - p.y <= 1.0 && b.y - p.y > 0.0) { //There's a block at head level
            //Run into the wall
            if (p.x < b.x && p.xVel > 0) {
              p.xVel = 0.0;
              p.x = b.x - 1;
            }
            if (p.x > b.x && p.xVel < 0) {
              p.xVel = 0.0;
              p.x = b.x + 1;
            }
          }
        }
      }
      if (blockTouchingBelow) {
        if (p.yVel < 0.0) {
          p.yVel = 0.0;
        }
        p.onFlatSurface = true;
      } else {
        p.onFlatSurface = false;
      }
      //Send player update
      if (pOldX != p.x || pOldY != p.y) {
        blockGame.messageSender.sendPositionUpdateToAll(p);
      }
    }
  }
  /*void createMainPlayer() {
    mainPlayer = new Player(blockGame);
    mainPlayer.y = 1.0;
    addPlayer(mainPlayer);
  }
  void addPlayer(Player p) {
    players.add(p);
    blockGame.drawController.addMeshToScene(p.player3DMesh);
  }*/
}