library GraphicsIOController;
import 'GraphicsSectionController.dart';
import 'dart:io';
import 'dart:async';
import 'dart:convert';
import 'ThreadInfo.dart';

class IOController {
  GraphicsSectionController controller;
  IOController(GraphicsSectionController c) {
    controller = c;
  }
  Map getDataMap() {
    Map m = new Map();
    m['threadIDs'] = controller.threadController.threadIDs;
    List threads = new List();
    for (int i = 0; i < controller.threadController.threads.length; i++) {
      ThreadInfo t = controller.threadController.threads[i];
      threads.add(t.toMap());
    }
    m['threads'] = threads;
    return m;
  }
  void processDataMap(Map m) {
    int threadIDs = m['threadIDs'];
    controller.threadController.threadIDs = threadIDs;
    for (int i = 0; i < m['threads'].length; i++) {
      Map threadMap = m['threads'][i];
      controller.threadController.threads.add(new ThreadInfo.fromMap(threadMap));
    }
  }
  void saveAllData() {
    try {
      File file = new File('bin/Saved Data/graphicscontroller.json');
      file.exists().then((bool e) {
        if (e) {
          file.delete().then((var f) {
            var output = file.openWrite();
            output.write(JSON.encode(getDataMap()));
            output.close();
          });
        } else {
          var output = file.openWrite();
          output.write(JSON.encode(getDataMap()));
          output.close();
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
  void loadAllData() {
    try {
      File file = new File('bin/Saved Data/graphicscontroller.json');
      file.exists().then((bool e) {
        if (e) {
          Future<String> finishedReading = file.readAsString();
          finishedReading.then((text) {
            processDataMap(JSON.decode(text));
            print("Loaded graphics controller");
          });
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
}
