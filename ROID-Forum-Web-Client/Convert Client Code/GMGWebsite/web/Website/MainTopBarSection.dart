library MainSection;
import 'Website.dart';
import 'dart:html';
import 'Sections/GenericSection.dart';
//import 'package:css_animation/css_animation.dart';

class MainSection {
  Website website;
  DivElement mainDiv;
  DivElement titleDiv;
  DivElement sectionsBarDiv;
  DivElement sectionBarHighlight;
  List<DivElement> divSections = new List<DivElement>();
  List<double> widthPercentages = new List();
  List<double> positionPercentages = new List();
  double currentHighlightWidth = 0.0;
  double currentHighlightPosition = 0.0;
  double newHighlightWidth = 0.0;
  double newHighlightPosition = 0.0;
  double hightlightMoveSpeed = 0.4;
  MainSection(Website w) {
    website = w;
    makeMainDivComponent();
    makeGMGTitleComponent();
    makeSectionBarComponent();
    makeSectionBarHighlightComponent();
    addSections();
    website.websiteDiv.append(mainDiv);
  }
  
  void darkTheme() {
    titleDiv.style.color = "white";
    sectionsBarDiv.style.color = "white";
  }
  void lightTheme() {
    titleDiv.style.color = "Black";
    sectionsBarDiv.style.color = "Black"; 
  }
  
  void logic() {
    if (currentHighlightWidth != newHighlightWidth) {
      double movementSpeed = ((newHighlightWidth-currentHighlightWidth)/5.0).abs();
      if (movementSpeed < hightlightMoveSpeed) {movementSpeed = hightlightMoveSpeed;}
      if (currentHighlightWidth < newHighlightWidth) {
        currentHighlightWidth += movementSpeed;
      }
      if (currentHighlightWidth > newHighlightWidth) {
        currentHighlightWidth -= movementSpeed;
      }
      if ((currentHighlightWidth - newHighlightWidth).abs() < movementSpeed) {
        currentHighlightWidth = newHighlightWidth;
      }
      sectionBarHighlight.style.width = "${currentHighlightWidth}%";
    }
    if (currentHighlightPosition != newHighlightPosition) {
      double movementSpeed = ((newHighlightPosition-currentHighlightPosition)/5.0).abs();
            if (movementSpeed < hightlightMoveSpeed) {movementSpeed = hightlightMoveSpeed;}
      if (currentHighlightPosition < newHighlightPosition) {
        currentHighlightPosition += movementSpeed;
      }
      if (currentHighlightPosition > newHighlightPosition) {
        currentHighlightPosition -= movementSpeed;
      }
      if ((currentHighlightPosition - newHighlightPosition).abs() < movementSpeed) {
        currentHighlightPosition = newHighlightPosition;
      }
      sectionBarHighlight.style.left = "${currentHighlightPosition}%";
    }
  }
  void sectionClick(int section) {
    for (int i = 0; i < divSections.length; i++) {
      DivElement d = divSections[i];
      d.style.textDecoration = '';
    }
    divSections[section].style.textDecoration = 'underline';
    website.showView(section);
    newHighlightWidth = widthPercentages[section];
    newHighlightPosition = positionPercentages[section];
  }

  /*********** Create GUI Components *************/

  void makeMainDivComponent() {
    mainDiv = new DivElement();
    mainDiv.style
        ..position = "absolute"
        ..left = "0px"
        ..top = "0px"
        ..height = "92px"
        ..width = "calc(100% - 300px)";
    
  }
  void makeGMGTitleComponent() {
    //Add the GMG name to the main div
    titleDiv = new DivElement();
    titleDiv.style
        ..position = "absolute"
        ..left = "0px"
        ..top = "20px"
        ..width = "100%"
        ..fontSize = "15px"
        ..textAlign = "center";
    titleDiv.text = 'Game Maker\'s Garage';
    mainDiv.append(titleDiv);
  }
  void makeSectionBarComponent() {
    //Add the section bar
    sectionsBarDiv = new DivElement();
    sectionsBarDiv.style
        ..position = "absolute"
        ..width = "80%"
        ..left = "10%"
        ..top = "50px"
        ..height = "40px"
        ..borderTopStyle = "solid"
        ..borderBottomStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..userSelect = "none";
    mainDiv.append(sectionsBarDiv);
  }
  void addSections() {
    //Add the sections to the bar
    int totalLetters = 0;
    for (int i = 0; i < website.sectionOrder.length; i++) {
      GenericSection section = website.sectionOrder[i];
      totalLetters += section.getDisplayName().length;
    }
    double p = 0.0;
    for (int i = 0; i < website.sectionOrder.length; i++) {
      GenericSection section = website.sectionOrder[i];
      String sectionName = section.getDisplayName();
      DivElement s = new DivElement();
      s.text = sectionName;
      double width = 100.0 / totalLetters * sectionName.length;
      widthPercentages.add(width);
      positionPercentages.add(p);
      s.style
          ..position = 'absolute'
          ..width = '${width}%'
          ..textAlign = 'center'
          ..height = '40px'
          ..top = '0px'
          ..left = '${p}%'
          ..lineHeight = '40px'
          ..textOverflow = 'ellipsis'
          ..whiteSpace = 'nowrap'
          ..overflow = 'hidden'
          ..cursor = 'pointer';
      s.onClick.listen((var d) {
        sectionClick(i);
      });
      p += width;
      if (i != 0) {
        s.style.borderLeft = 'solid';
        s.style.borderColor = '#DDDDDD';
        s.style.borderWidth = '1px';
      }
      sectionsBarDiv.append(s);
      divSections.add(s);
      
    }
    
  }
  void makeSectionBarHighlightComponent() {
    sectionBarHighlight = new DivElement();
    sectionBarHighlight.style
        ..position = "absolute"
        ..top = "0px"
        ..left = "0px"
        ..height = "100%"
        ..width = "0%"
        ..backgroundColor = "rgba(100,200,100,0.5)";
    sectionsBarDiv.append(sectionBarHighlight);
  }
}
