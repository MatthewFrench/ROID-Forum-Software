library MessageReader;
import 'dart:typed_data';

class MessageReader {
  int byteLength;
  ByteData byteData;
  int currentLoc;
  MessageReader(ByteData messageData, int startingLocation) {
    byteData = messageData;
    currentLoc = startingLocation;
    byteLength = getUint32();
  }
  int getUint8() {
    int b = byteData.getUint8(currentLoc);
    currentLoc += 1;
    return b;
  }
  int getUint16() {
      int b = byteData.getUint16(currentLoc);
      currentLoc += 2;
      return b;
    }
  int getInt16() {
        int b = byteData.getInt16(currentLoc);
        currentLoc += 2;
        return b;
      }
  int getUint32() {
    int b = byteData.getUint32(currentLoc);
    currentLoc += 4;
    return b;
  }
  int getInt32() {
    int b = byteData.getInt32(currentLoc);
    currentLoc += 4;
    return b;
  }
  double getFloat64() {
    double b = byteData.getFloat64(currentLoc);
    currentLoc += 8;
    return b;
  }
  double getFloat32() {
    double b = byteData.getFloat32(currentLoc);
    currentLoc += 4;
    return b;
  }
  String getString() {
    List<int> codeUnits = new List();
    int nameLength = getUint16();
    for (int i = 0; i < nameLength; i++) {
      codeUnits.add(getUint16());
    }
    return new String.fromCharCodes(codeUnits);
  }
  /*
  MessageReader getBinary() {
    MessageReader m = new MessageReader(byteData, currentLoc);
    currentLoc += m.getLength();
    return m;
  }*/
  int getLength() {
    return byteLength;
  }
}