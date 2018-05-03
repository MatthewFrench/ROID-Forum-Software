library Server.NetworkingController;
import 'dart:io';
import '../Server.dart';
import '../User/User.dart';
import 'dart:typed_data';
import 'dart:async';
import 'MessageReader.dart';
import 'MessageWriter.dart';

class NetworkingController {
  Server server;
  List<User> users = new List();
  int bytesSent = 0;
  int totalBytesSent = 0;
  Stopwatch stopwatch;
  int secondsBetweenByteUpdates = 80;
  NetworkingController(Server s) {
    server = s;
    HttpServer.bind(InternetAddress.ANY_IP_V6, 7777).then((HttpServer serv) {
      serv.listen((HttpRequest req) {
        WebSocketTransformer.upgrade(req).then((socket) {
          onConnection(socket);
        }, onError: (e) {
          print("Caught Websocket upgrade error: ${e}");
        });
      }, onError: (e) => print("An error occurred: ${e}"));
    });
    stopwatch = new Stopwatch();
    new Timer(new Duration(seconds: secondsBetweenByteUpdates), resetWatch);
  }
  //This watch prints out data usage every certain amount of seconds
  void resetWatch() {
    stopwatch.stop();
    stopwatch.reset();
    print('Bytes sent per second: ${(bytesSent/secondsBetweenByteUpdates).round()} as Kilobytes: ${bytesSent/1000.0/secondsBetweenByteUpdates} as Megabytes: ${bytesSent/1000000.0/secondsBetweenByteUpdates}');

    totalBytesSent += bytesSent;
    print('Total bytes send: ${totalBytesSent/1000000.0} megabytes as Kilobytes: ${totalBytesSent/1000.0}');
    bytesSent = 0;
    stopwatch.start();
    new Timer(new Duration(seconds: secondsBetweenByteUpdates), resetWatch);
  }
  /*
  void logic() {
    for (int i = 0; i < users.length; i++) {
      User u = users.elementAt(i);
      u.flushBinary();
    }
  }*/
  void onConnection(WebSocket conn) {
    User u = new User(conn, this, server);
    users.add(u);
    u.eventOnOpen();
    conn.listen((m) {
      if (m is Uint8List) {
        try {
          ByteData byteData = m.buffer.asByteData(m.offsetInBytes, m.lengthInBytes);
          //Lets separate the byte data into individual messages
          int i = 0;
          while (i < byteData.lengthInBytes) {
            MessageReader m = new MessageReader(byteData, i);
            u.onBinaryMessage(m);
            i += m.getLength();
          }
        } catch (exception, stackTrace) {
          print(exception);
          print(stackTrace);
        }
      }
    }, onDone: () {
      users.remove(u);
      u.eventOnClose(u);
    }, onError: (e) {
      users.remove(u);
      u.eventOnClose(u);
    });
  }
  void sendMessageToAllConnected(MessageWriter data, [bool immediately = false]) {
    for (int i = 0; i < users.length; i++) {
      User u = users.elementAt(i);
      u.sendBinary(data);
    }
  }
}
