library ChatController;
import '../Server.dart';
import '../User.dart';
import 'dart:convert';
import '../User.dart';
import 'ChatIOController.dart';

class ChatController {
  List<String> chats = new List();
  Server server;
  List<User> users = new List();
  ChatIOController chatIOController;
  ChatController(Server s) {
    chatIOController = new ChatIOController(this);
    server = s;
    chatIOController.loadAllChats();
  }
  void addUser(User u) {
    users.add(u);
    sendListUpdateToAll();
    sendAllChats(u);
  }
  void removeUser(User u) {
    users.remove(u);
    sendListUpdateToAll();
  }
  void sendListUpdateToAll() {
    String list = "Online(${users.length}): ";
    int guests = 0;
    bool addComma = false;
    for (User u in users) {
      if (u.account != null) {
        if (addComma) {
          list += ", ";
        } else {
          addComma = true;
        }
        list += u.account.name;
      } else {
        guests += 1;
      }
    }
    if (addComma) {
      list += " and ${guests} Guests";
    } else {
      list += "${guests} Guests";
    }

    Map m = new Map();
    m['Controller'] = 'Chat';
    m['Title'] = 'Online List';
    m['Data'] = list;
    for (User u in users) {
      u.sendMap(m);
    }
    print(list);
  }
  void logic() {
    chatIOController.logic();
  }
  void addChat(User u, String chat) {
    chat = '${u.account.name}: ' + chat;
    chats.add(chat);
    //Send chat to all connected websockets
    Map m = new Map();
    m['Controller'] = 'Chat';
    m['Title'] = 'Msg';
    m['Data'] = chat;
    String message = JSON.encode(m);
    for (int i = 0; i < server.networkingController.users.length; i++) {
      User u = server.networkingController.users[i];
      u.sendString(message);
    }
  }
  void sendAllChats(User u) { //Send only last 20
    int start = 0;
    // if (chats.length > 20) {
    //   start = chats.length - 20;
    // }
    for (int i = start; i < chats.length; i++) {
      Map m = new Map();
      m['Controller'] = 'Chat';
      m['Title'] = 'Msg';
      m['Data'] = chats[i];
      u.sendMap(m);
    }
  }
  void onMessage(User u, Map message) {
    if (message['Title'] == 'Msg' && u.account != null) {
      addChat(u, message['Data']);
    }
  }
}