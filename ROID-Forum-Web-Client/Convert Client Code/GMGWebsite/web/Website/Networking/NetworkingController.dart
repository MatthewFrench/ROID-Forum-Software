library NetworkingController;
import '../Website.dart';
import 'dart:html';
import 'dart:typed_data';
import 'PingTracker.dart';
import 'MessageReader.dart';
import 'MessageWriter.dart';
import 'dart:async';

const int Binary_Flush_Max = 1300;

class NetworkingController {
  Website website;
  WebSocket websocket;
  PingTracker pingTracker;
  List<MessageWriter> binaryMessages;
  int binaryFlushLength = 0;
  int lastMessage = -1;
  Timer autoFlushTimer;
  NetworkingController(Website w) {
    website = w;
    //websocket = new WebSocket('ws://45.55.81.104:7777');
    websocket = new WebSocket('ws://127.0.0.1:7777');
    websocket.binaryType = 'arraybuffer';
    websocket.onOpen.listen(onopen);
    websocket.onClose.listen(onclose);
    websocket.onMessage.listen(onmessage);
    pingTracker = new PingTracker(this);
    binaryMessages = new List();
  }
  void onopen(Event evt) {
    //pingTracker.start();
    website.connectedToServer();
  }

  void onerror(Event evt) {
    // an error occurred when sending/receiving data
    print('Websocket error');
  }

  void onclose(Event evt) {
    //Try a reconnect in 2 seconds
    //new Timer(new Duration(seconds: 2), game.reset);
  }

  void onmessage(MessageEvent e) {
    if (e.data is ByteBuffer) {
      ByteBuffer b = e.data;
      ByteData byteData = b.asByteData();
      //Lets separate the byte data into individual messages
      int i = 0;
      while (i < byteData.lengthInBytes) {
        MessageReader m = new MessageReader(byteData, i);
        print("Got message from server ${m.byteLength}");
        website.websiteMsgHandler.onBinaryMessage(m);
        if (m.getLength() == 0) {
          i = byteData.lengthInBytes;
          print("Corrupted Data in ByteData, total length: ${byteData.lengthInBytes}");
          m.currentLoc = 0;
          int l = m.getUint32();
          print("Read length is ${l}");
          int c = m.getUint8();
          print("Controller is ${c}");
          int msg = m.getUint8();
          print("Message is ${msg}");
        }
        i += m.getLength();
      }
    }
  }
  void SendBinary(MessageWriter data, [bool immediately = false]) {
    //We don't want to go over the MTU, so flush early if it will be too full
    if (binaryFlushLength + data.getLength() > Binary_Flush_Max) {
      autoFlush();
    }
    binaryMessages.add(data);
    binaryFlushLength += data.getLength();
    if (binaryFlushLength >= Binary_Flush_Max || immediately) {
      flushBinary();
      if (autoFlushTimer != null) {
        autoFlushTimer.cancel();
        autoFlushTimer = null;
      }
    } else { //Flush immediately after the main thread finishes execution
      autoFlushTimer = new Timer(new Duration(milliseconds: 10), autoFlush);
    }
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
      print("Flushed binary ${binaryFlushLength}");
      ByteData byteData = new ByteData(binaryFlushLength);
      int loc = 0;
      for (int i = 0; i < binaryMessages.length; i++) {
        MessageWriter m = binaryMessages[i];
        m.addToByteData(byteData, loc);
        loc += m.getLength();
      }
      websocket.sendByteBuffer(byteData.buffer);
      binaryMessages.clear();
      binaryFlushLength = 0;
    }
  }
}