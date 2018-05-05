library NetworkingController;
import 'Website.dart';
import 'dart:html';
import 'dart:typed_data';
import 'dart:async';
import 'dart:convert';

class NetworkingController {
  Website website;
  WebSocket websocket;
  NetworkingController(Website w) {
    website = w;
    //websocket = new WebSocket('ws://70.179.140.59:3066');
    //websocket = new WebSocket('ws://174.70.154.160:7777');
    websocket = new WebSocket('ws://127.0.0.1:7777');
    websocket.binaryType = 'arraybuffer';
    websocket.onOpen.listen(onopen);
    websocket.onClose.listen(onclose);
    websocket.onMessage.listen(onmessage);
  }
  void onopen(Event evt) {
    website.connectedToServer();
  }
  
  void onerror(Event evt) {
    // an error occurred when sending/receiving data
    print('Websocket error');
  }
  
  void onclose(Event evt) {
    //Try a reconnect in 2 seconds
    website.prepareReset();
    new Timer(new Duration(seconds:4), (){
      window.location.assign('http://gamemakersgarage.com/archive/forum/');
    });
  }
  
  void onmessage(MessageEvent e) {    
    if (e.data is String) {
      website.Message(JSON.decode(e.data));
    } else if (e.data is ByteBuffer) {
      ByteBuffer b = e.data;
      ByteData bdata = new ByteData.view(b);
      website.BinaryMessage(bdata);
    }
  }
  
  void Send(Map msg) {
    websocket.send(JSON.encode(msg));
  }
  void SendBinary(ByteBuffer msg) {
    websocket.send(msg);
  }
}