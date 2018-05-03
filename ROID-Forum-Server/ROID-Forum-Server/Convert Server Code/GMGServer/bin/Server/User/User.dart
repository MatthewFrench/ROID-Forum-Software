library User;
import 'dart:io';
import 'dart:typed_data';
import '../Database/Account.dart';
import '../Networking/MessageWriter.dart';
import '../Networking/MessageReader.dart';
import '../Networking/NetworkingController.dart';
import '../Server.dart';
import 'UserMsgHandler.dart';
import 'dart:async';
import 'PingTracker.dart';

const int Binary_Flush_Max = 1300; //Normal MTU is 1500 but we're giving some leeway for whatever extras it packs on

class User {
  WebSocket connection;
  Server server;
  NetworkingController networkingController;
  Account account;
  List<MessageWriter> binaryMessages;
  int binaryFlushLength = 0;
  UserMsgHandler userMsgHandler;
  Timer flushTimer;
  Timer autoFlushTimer;
  PingTracker pingTracker;
  User(WebSocket c, NetworkingController nc, Server s) {
    server = s;
    networkingController = nc;
    connection = c;
    binaryMessages = new List();
    account = null;
    userMsgHandler = new UserMsgHandler(this);
    pingTracker = new PingTracker(this);
  }
  //Let the user notify everyone about it's specific state instead of the server
  void eventAccountLoggedIn() {
    server.forumController.userLoggedIn(this);
    //gameController.accountLoggedIn(u);
    //server.chatController.processUserLoginEvent(this);
    //server.blockGameController.processUserLoginEvent(this);
  }
  void eventAccountLoggedOut() {
    server.forumController.userLoggedOut(this);
    //gameController.accountLoggedOut(u);
    //server.chatController.processUserLogoutEvent(this);
    //server.blockGameController.processUserLogoutEvent(this);
  }
  void eventOnOpen() {
    server.forumController.userConnected(this);
  }
  void eventOnClose(User u) {
    server.forumController.userDisconnected(this);
  }
  void onBinaryMessage(MessageReader message) {
    userMsgHandler.onBinaryMessage(message);
  }
  
  void sendBinary(MessageWriter data, [bool immediately = false]) {
    //We don't want to go over the MTU, so flush early if it will be too full
    if (binaryFlushLength + data.getLength() > Binary_Flush_Max) {
      autoFlush();
    }
    binaryMessages.add(data);
    binaryFlushLength += data.getLength();
    if (binaryFlushLength >= Binary_Flush_Max || immediately) {
      flushBinary();
      if (autoFlushTimer != null) {autoFlushTimer.cancel(); autoFlushTimer = null;}
    } else { //Flush 10ms after the main thread finishes execution
      if (autoFlushTimer == null) {
        autoFlushTimer = new Timer(new Duration(milliseconds: 10), autoFlush);
      }
    }

    flushBinary();
  }
  void autoFlush() {
    if (autoFlushTimer != null) {
      autoFlushTimer.cancel();
      autoFlushTimer = null;
    }
    flushBinary();
  }
  void flushBinary() {
    if (binaryFlushLength > 0) {
      ByteData byteData = new ByteData(binaryFlushLength);
      int loc = 0;
      for (int i = 0; i < binaryMessages.length; i++) {
        MessageWriter m = binaryMessages[i];
        m.addToByteData(byteData, loc);
        loc += m.getLength();
      }
      print("Sending data: ${byteData.buffer.asUint8List()}");
      connection.add(byteData.buffer.asUint8List());

      binaryMessages.clear();
      networkingController.bytesSent += binaryFlushLength;
      binaryFlushLength = 0;
    }
  }
}