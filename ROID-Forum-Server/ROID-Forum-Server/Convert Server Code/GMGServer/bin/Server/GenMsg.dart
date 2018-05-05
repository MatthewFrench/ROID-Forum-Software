library Server.GenMsg;
import 'dart:convert';

class GenMsg {
  static String Login(bool success) {
    Map m = new Map();
    m["Title"] = "Login";
    m["Status"] = success;
    return JSON.encode(m);
  }
  static String Register(bool success) {
    Map m = new Map();
    m["Title"] = "Register";
    m["Status"] = success;
    return JSON.encode(m);
  }
  static String Name(String status) {
    Map m = new Map();
    m["Title"] = "Name";
    m["Status"] = status;
    return JSON.encode(m);
  }
  static String Enter(String game) {
    Map m = new Map();
    m["Title"] = "Enter";
    m["Game"] = game;
    return JSON.encode(m);
  }
}