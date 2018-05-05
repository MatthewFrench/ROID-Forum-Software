library GenericSection;
import 'dart:html';

abstract class GenericSection {
  DivElement getDiv();
  String getName();
  String getDisplayName();
  void processEvent(String event);
  void show();
  void hide();
}