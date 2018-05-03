library Server;
import 'Networking/NetworkingController.dart';
import 'dart:core';
import 'Database/DatabaseController.dart';
import 'Forum/ForumController.dart';

class Server {
  DatabaseController databaseController;
  NetworkingController networkingController;
  ForumController forumController;
  Server() {
    databaseController = new DatabaseController(this);
    networkingController = new NetworkingController(this);
    forumController = new ForumController(this);
  }
}
