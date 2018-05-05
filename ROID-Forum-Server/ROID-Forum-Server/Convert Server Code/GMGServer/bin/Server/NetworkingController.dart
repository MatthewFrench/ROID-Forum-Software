library Server.NetworkingController;
import 'dart:io';
import 'Server.dart';
import 'User.dart';

class NetworkingController {
  Server server;
  List<User> users = new List();
  NetworkingController(Server s) {
    server = s;
    HttpServer.bind(InternetAddress.ANY_IP_V4, 7777).then((HttpServer serv) {
      serv.listen((HttpRequest req) {
        WebSocketTransformer.upgrade(req).then((socket) {
          User p = new User(socket);
                users.add(p);
                server.onOpen(p);
          socket.listen((m) {
            try {
              server.onMessage(p, m);
            } catch (exception, stackTrace) {
              print(exception);
              print(stackTrace);
            }
          }, onDone: () {
            server.onClose(p);
            users.remove(p);
          }, onError: (e) {
            print("Client got an error: ${e}");
            server.onClose(p);
            users.remove(p);
          });
        }, onError: (e) {print("Websocket upgrade error: ${e}");});
      }, onError: (e) => print("An error occurred: ${e}"));
    });
  }
}