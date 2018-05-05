library BlockGameIOController;
import 'BlockGameController.dart';
import 'Blocks/Block.dart';
import 'dart:io';
import 'dart:async';
import 'dart:convert';

class BlockGameIOController {
  BlockGameController controller;
  Stopwatch saveWatch;
  BlockGameIOController(BlockGameController c) {
    controller = c;
    saveWatch = new Stopwatch();
    saveWatch.start();
  }
  void logic() {
    if (saveWatch.elapsed.inMinutes == 2) {
      saveWatch.reset();
      saveAllBlocks();
    }
  }
  void saveAllBlocks() {
    try {
      File file = new File('bin/Saved Data/blockgame.json');
      var getData = () {
        Map map = new Map();
        List blockList = new List();
        for (int i = 0; i < controller.blockController.blocks.length; i++) {
          Block b = controller.blockController.blocks[i];
          Map blockMap = new Map();
          blockMap['ID'] = b.id;
          blockMap['x'] = b.x.round();
          blockMap['y'] = b.y.round();
          blockMap['Color'] = b.color;
          blockList.add(blockMap);
        }
        map["Blocks"] = blockList;
        map['Block IDs'] = controller.blockController.blockIDs; 
        return JSON.encode(map);
      };
      file.exists().then((bool e) {
        if (e) {
          file.delete().then((var f) {
            var output = file.openWrite();
            output.write(getData());
            output.close();
          });
        } else {
          var output = file.openWrite();
          output.write(getData());
          output.close();
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
  void loadAllBlocks() {
    try {
      File file = new File('bin/Saved Data/blockgame.json');
      file.exists().then((bool e) {
        if (e) {
          Future<String> finishedReading = file.readAsString();
          finishedReading.then((text) {
            Map map = JSON.decode(text);
            List blockList = map['Blocks'];
            for (int i = 0; i < blockList.length; i++) {
              Map blockMap = blockList[i];
              Block b = new Block(controller, blockMap['ID'], blockMap['Color']);
              b.x = blockMap['x'].toDouble();
              b.y = blockMap['y'].toDouble();
              controller.blockController.addBlock(b);
            }
            controller.blockController.blockIDs = map['Block IDs'];
            print("Loaded blockgame");
          });
        } else {
          controller.blockController.createDefaultBlocks();
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
}
