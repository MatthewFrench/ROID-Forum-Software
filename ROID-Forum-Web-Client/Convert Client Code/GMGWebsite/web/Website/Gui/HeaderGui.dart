library HeaderGui;

import '../Website.dart';
import 'dart:html';

class HeaderGui {
  Website website;
  DivElement headerGuiDiv;
  int peopleOnline = 0;
  int registeredPeople = 0;
  SpanElement onlineTicker;
  HeaderGui(Website w) {
    website = w;
  }
  void updatePeopleOnline(int num) {
    peopleOnline = num;
    onlineTicker.text = "${peopleOnline} people online, ${registeredPeople} game makers registered";
  }
  void updateRegisteredPeople(int num) {
    registeredPeople = num;
    onlineTicker.text = "${peopleOnline} people online, ${registeredPeople} game makers registered";
  }
  void addHeader() {
    headerGuiDiv = new DivElement();
    website.websiteDiv.append(headerGuiDiv);
    //Make it black like the old GMG
    headerGuiDiv.id = "headerGuiDiv";
    //Add GMG words
    SpanElement titleSpan = new SpanElement();
    titleSpan.id = "titleSpan";
    titleSpan.text = "Game Maker's Garage";
    headerGuiDiv.append(titleSpan);

    onlineTicker = new SpanElement();
    onlineTicker.id = "onlineTicker";
    onlineTicker.text = "0 people online, 0 game makers registered";
    headerGuiDiv.append(onlineTicker);
  }
}