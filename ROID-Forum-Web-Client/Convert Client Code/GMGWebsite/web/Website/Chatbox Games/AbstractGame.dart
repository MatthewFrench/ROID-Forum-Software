library AbstractGame;
import 'dart:html';
import 'dart:typed_data';

abstract class AbstractGame {
  CanvasElement getCanvas();
  void show();
  void hide();
  String getName();
  void logic();
  void onMessage(Map message);
  void onBinaryMessage(ByteData buffer);
}
