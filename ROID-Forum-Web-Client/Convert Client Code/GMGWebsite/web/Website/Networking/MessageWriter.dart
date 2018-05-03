library MessageWriter;
import 'dart:typed_data';

class MessageWriter {
  int byteLength;
  List<MessageData> data;
  MessageWriter() {
    data = new List();
    byteLength = 4;
  }
  /*
  ByteData generateByteData() {
    ByteData byteData = new ByteData(byteLength);
    int loc = 0;
    byteData.setUint32(loc, byteLength);
    loc += 4;
    for (int i = 0; i < data.length; i++) {
      MessageData m = data[i];
      m.addToByteData(byteData, loc);
      loc += m.getLength();
    }
    return byteData;
  }*/
  void addToByteData(ByteData byteData, int loc) {
    byteData.setUint32(loc, byteLength);
    loc += 4;
    for (int i = 0; i < data.length; i++) {
      MessageData m = data[i];
      m.addToByteData(byteData, loc);
      loc += m.getLength();
    }
  }
  void addUint8(int value) {
    MessageDataUint8 m = new MessageDataUint8(value);
    data.add(m);
    byteLength += m.getLength();
  }
  void addUint16(int value) {
    MessageDataUint16 m = new MessageDataUint16(value);
    data.add(m);
    byteLength += m.getLength();
  }
  void addInt16(int value) {
      MessageDataInt16 m = new MessageDataInt16(value);
      data.add(m);
      byteLength += m.getLength();
    }
  void addUint32(int value) {
    MessageDataUint32 m = new MessageDataUint32(value);
    data.add(m);
    byteLength += m.getLength();
  }
  void addInt32(int value) {
    MessageDataInt32 m = new MessageDataInt32(value);
    data.add(m);
    byteLength += m.getLength();
  }
  void addFloat64(double value) {
    MessageDataFloat64 m = new MessageDataFloat64(value);
    data.add(m);
    byteLength += m.getLength();
  }
  void addFloat32(double value) {
    MessageDataFloat32 m = new MessageDataFloat32(value);
    data.add(m);
    byteLength += m.getLength();
  }
  void addString(String value) {
    MessageDataString m = new MessageDataString(value);
    data.add(m);
    byteLength += m.getLength();
  }
  /*
  void addBinary(MessageWriter value) {
    MessageDataBinary m = new MessageDataBinary(value);
    data.add(m);
    byteLength += m.getLength();
  }*/
  int getLength() {
    return byteLength;
  }
}






abstract class MessageData {
  void addToByteData(ByteData byteData, int loc);
  int getLength();
}
class MessageDataUint8 implements MessageData {
  int value;
  MessageDataUint8(this.value);
  void addToByteData(ByteData byteData, int loc) {
    byteData.setUint8(loc, value);
  }
  int getLength() {
    return 1;
  }
}
class MessageDataUint16 implements MessageData {
  int value;
  MessageDataUint16(this.value);
  void addToByteData(ByteData byteData, int loc) {
    byteData.setUint16(loc, value);
  }
  int getLength() {
    return 2;
  }
}
class MessageDataInt16 implements MessageData {
  int value;
  MessageDataInt16(this.value);
  void addToByteData(ByteData byteData, int loc) {
    byteData.setInt16(loc, value);
  }
  int getLength() {
    return 2;
  }
}
class MessageDataUint32 implements MessageData {
  int value;
  MessageDataUint32(this.value);
  void addToByteData(ByteData byteData, int loc) {
    byteData.setUint32(loc, value);
  }
  int getLength() {
    return 4;
  }
}
class MessageDataFloat64 implements MessageData {
  double value;
  MessageDataFloat64(this.value);
  void addToByteData(ByteData byteData, int loc) {
    byteData.setFloat64(loc, value);
  }
  int getLength() {
    return 8;
  }
}
class MessageDataFloat32 implements MessageData {
  double value;
  MessageDataFloat32(this.value);
  void addToByteData(ByteData byteData, int loc) {
    byteData.setFloat32(loc, value);
  }
  int getLength() {
    return 4;
  }
}
class MessageDataInt32 implements MessageData {
  int value;
  MessageDataInt32(this.value);
  void addToByteData(ByteData byteData, int loc) {
    byteData.setInt32(loc, value);
  }
  int getLength() {
    return 4;
  }
}
class MessageDataString implements MessageData {
  String value;
  List<int> stringCode;
  MessageDataString(this.value) {
    stringCode = value.codeUnits;
  }
  void addToByteData(ByteData byteData, int loc) {
    byteData.setUint16(loc, value.length);
    for (int i = 0; i < stringCode.length; i++) {
      byteData.setUint16(loc + 2 + i * 2, stringCode[i]);
    }
  }
  int getLength() {
    return 2 + stringCode.length * 2;
  }
}
/*
class MessageDataBinary implements MessageData {
  MessageWriter value;
  MessageDataBinary(this.value) {}
  void addToByteData(ByteData byteData, int loc) {
    value.addToByteData(byteData, loc);
  }
  int getLength() {
    return value.getLength();
  }
}*/