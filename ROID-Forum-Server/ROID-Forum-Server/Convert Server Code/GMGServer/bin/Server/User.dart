library User;
import 'dart:io';
import 'dart:typed_data';
import 'dart:convert';
import 'Account Controller/Account.dart';

class User {
  WebSocket connection;
  Account account;
  String viewingSection = "";
  String inGame = "";
  User(WebSocket c) {
    connection = c;
    account = null;
  }
  void sendBinary(ByteData data) {
    connection.add(data.buffer.asUint8List());
  }
  void sendMap(Map m) {
    connection.add(JSON.encode(m));
  }
  void sendString(String o) {
      connection.add(o);
  }
}