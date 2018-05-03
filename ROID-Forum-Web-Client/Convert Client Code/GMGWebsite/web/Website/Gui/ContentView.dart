library ContentView;
import '../Website.dart';
import 'dart:html';

class ContentView {
  Website website;
  DivElement contentGuiDiv;
  ContentView(Website w) {
    website = w;

    contentGuiDiv = new DivElement();
    contentGuiDiv.id = "contentGuiDiv";
  }
  DivElement getDivElement() {
    return contentGuiDiv;
  }
  void appendView(DivElement div) {
    contentGuiDiv.append(div);
  }
  void clearView() {
    for (Element e in contentGuiDiv.children) {
      e.remove();
    }
  }
}