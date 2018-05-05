library Chatbox;
import 'Website.dart';
import 'dart:html';
import 'dart:async';
import 'Tools/DescriptionParser.dart';
//import 'package:css_animation/css_animation.dart';
import 'dart:math';
import 'Tools/KeyFramesHelper.dart';

class Chatbox {
  Website website;
  DivElement chatboxDiv;
  InputElement textField;
  DivElement chatField;
  DivElement chatOnlineBox;
  AudioElement ding;
  bool doneLoading = false;
  List<ChatMsg> chatMsgs = new List();
  DivElement bottomNode;
  Chatbox(Website w) {
    website = w;
    makeChatboxDivComponent();
    makeChatWhosOnlineComponent();
    makeChatInputComponent();
    makeChatFieldComponent();
    Timer t = new Timer(new Duration(seconds: 1), () {
      doneLoading = true;
    });
    ding = new AudioElement("Resources/ding8.wav");
  }
  void darkTheme() {
    chatOnlineBox.style.color = "white";
  }
  void lightTheme() {
    chatOnlineBox.style.color = "Black";
  }
  void processEvent(String event) {
    switch (event) {
      case "Logged In":
        {
          textField.placeholder = "Type a message...";
          updateChatHighlights();
          scrollToBottom();
        }
        break;
      case "Logged Out":
        {
          textField.placeholder = "Login to chat.";
          updateChatHighlights();
          scrollToBottom();
        }
        break;
    }
  }
  void sendChat(KeyboardEvent e) {
    if (e.keyCode == 13 && textField.value.length != 0 && website.database.loggedIn) {
      Map m = new Map();
      m['Controller'] = 'Chat';
      m['Title'] = 'Msg';
      m['Data'] = textField.value;
      textField.value = '';
      website.networkingController.Send(m);
    }
  }
  void addChat(String chat) {
    DivElement d = new DivElement();
    d.append(DescriptionParser.parseDescription(chat));

    d.style
        ..width = '100%'
        ..wordWrap = "break-word"
        ..display="inline-block"
        ..marginBottom = "10px";
    chatMsgs.add(new ChatMsg(chat, d));
    updateChatHighlights();
    chatField.append(d);
    bottomNode = d;
    var loopNodesFunction = (Node d2) {};
    loopNodesFunction = (Node d2) {
      for (Node node in d2.nodes) {
        if (node is ImageElement) {
          node.onLoad.listen((_) {
            scrollToBottom();
          });
        } else {
          loopNodesFunction(node);
        }
      }
    };
    loopNodesFunction(d);
    scrollToBottom();
    if (doneLoading) {
      ding.play();
    }
  }
  void scrollToBottom() {
    new Future(() {
      bottomNode.scrollIntoView(ScrollAlignment.BOTTOM);
    });
  }
  void updateChatHighlights() {
    String name = website.database.name;
    for (ChatMsg chatMsg in chatMsgs) {
      String chatName = chatMsg.chat.substring(0, chatMsg.chat.indexOf(":"));
      if (chatName != name) {
        chatMsg.div.style
            ..backgroundColor = "rgba(255,255,255,0.5)"
            ..borderWidth = "1px"
            ..borderColor = "white"
            ..borderStyle = "solid";
      } else {
        chatMsg.div.style
            ..backgroundColor = ""
            ..borderWidth = "0px"
            ..borderColor = ""
            ..borderStyle = "none";
      }
    }
  }
  void onMessage(Map message) {
    if (message['Title'] == 'Msg') {
      addChat(message['Data']);
    }
    if (message['Title'] == 'Online List') {
      chatOnlineBox.text = message['Data'];
    }
  }
  /**********Create GUI Components***********/
  void makeChatboxDivComponent() {
    chatboxDiv = new DivElement();
    chatboxDiv.style
        //..width = "300px"
        //..height = "calc(100% - 100px)"
        ..width = "300px"
        ..height = "300px"
        ..left = "0px"
        ..top = "0px"
        //..right = "0px"
        //..bottom = "2px"
        ..position = "absolute"
        ..borderStyle = "solid"
        ..borderWidth = "2px"
        ..display = "block"
        ..borderColor = "#DDDDDD"
        ..fontSmoothing = "antialiased";

    chatboxDiv.style.borderTopLeftRadius = "6px";
    //website.aheadWebsiteDiv.append(chatboxDiv);

    int cubeWidth = 300;
    int cubeHeight = 300;


    DivElement cubeContainer = new DivElement();
    cubeContainer.style
        ..perspective = "1000px"
        ..perspectiveOrigin = "50% 50%"
        ..transformStyle = "preserve-3d"
        ..backfaceVisibility="hidden"
        ..right = "50px"
        ..bottom = "100px"
        ..position = "absolute"
        ..display = "block";
    website.aheadWebsiteDiv.append(cubeContainer);

    DivElement cubeDiv = new DivElement();
    cubeDiv.style
        ..display = "block"
        ..transformStyle = "preserve-3d"
        ..position = "relative"
        ..margin = "0 auto"
        ..height = "${cubeHeight}px"
        ..width = "${cubeWidth}px";
        //..transformOriginZ = "-200px";
    cubeContainer.append(cubeDiv);
    
    
    
    
    // create a stylesheet element
     StyleElement styleElement = new StyleElement();
     document.head.append(styleElement);
     // use the styleSheet from that
     CssStyleSheet sheet = styleElement.sheet;

     //sheet.insertRule("@-moz-keyframes chatboxAnim {0% {-moz-transform: rotateX(0deg) rotateY(0deg) rotateZ(0deg);} 100% {-moz-transform: rotateX(360deg) rotateY(360deg) rotateZ(360deg);}}", 0);
    //sheet.insertRule("@-webkit-keyframes chatboxAnim {0% {-webkit-transform: rotateX(0deg) rotateY(0deg) rotateZ(0deg);} 100% {-webkit-transform: rotateX(360deg) rotateY(360deg) rotateZ(360deg);}}", 0);
    //sheet.insertRule("@keyframes chatboxAnim {0% {transform: rotateX(0deg) rotateY(0deg) rotateZ(0deg);} 100% {transform: rotateX(360deg) rotateY(360deg) rotateZ(360deg);}}", 0);
        //sheet.addRule(selector, style)
     try {
//sheet.addRule("@-webkit-keyframes chatboxAnim", "0% {-webkit-transform: rotateX(0deg) rotateY(0deg) rotateZ(0deg);} 100% {-webkit-transform: rotateX(360deg) rotateY(360deg) rotateZ(360deg);}");
     } catch (_) {
       //sheet.insertRule("", 0);
            
     }
    //KeyFramesHelper.changeRotation("chatboxAnim", 0, 360);
    cubeDiv.style..animationName = "chatboxAnim"
        ..animationDuration = "3s"
        ..animationDelay = "0s"
        ..animationIterationCount = "1" //1
        ..animationTimingFunction = "linear"
        ..animationPlayState = "running";
    
    cubeDiv.addEventListener('webkitAnimationEnd', (_){
      cubeDiv.style..animationName = ""
          ..animationPlayState = "paused";
      new Future((){cubeDiv.style..animationName = "chatboxAnim"..animationPlayState = "running";});
      }, true);
    
    //cubeDiv.style.
    /*
    var rng = new Random();
    var slightRotation = (var oX, var oY){};
    slightRotation = (var oX, var oY) {
      int rX = 0;
      if (rng.nextInt(3) == 1) {rX = 5;}
      if (rng.nextInt(3) == 1) {rX = -5;}
      int rY = 0;
            if (rng.nextInt(3) == 1) {rY = 5;}
            if (rng.nextInt(3) == 1) {rY = -5;}
      var keyframes = new Map<int, Map<String, Object>>();
      keyframes[0] = {
        'transform': 'rotateX(${oX}deg) rotateY(${oY}deg)'
      };
      keyframes[100] = {
        'transform': 'rotateX(${rX}deg) rotateY(${rY}deg)'
      };

      var animation = new CssAnimation.keyframes(keyframes);
      animation.apply(cubeDiv, onComplete: () {
        slightRotation(rX, rY);
      }, delay: 2000, iterations: 1, duration: 5000, timing: "ease");
    };

    var keyframes = new Map<int, Map<String, Object>>();
    keyframes[0] = {
      'transform': 'rotateY(0)'
    };
    keyframes[100] = {
      'transform': 'rotateY(360deg)'
    };

    var animation = new CssAnimation.keyframes(keyframes);
    animation.apply(cubeDiv, onComplete: () {
      var keyframes = new Map<int, Map<String, Object>>();
      keyframes[0] = {
        'transform': 'rotateX(0)'
      };
      keyframes[100] = {
        'transform': 'rotateX(360deg)'
      };

      var animation = new CssAnimation.keyframes(keyframes);
      animation.apply(cubeDiv, onComplete: () {

        var keyframes = new Map<int, Map<String, Object>>();
        keyframes[0] = {
          'transform': 'rotateX(0) rotateY(0)'
        };
        keyframes[100] = {
          'transform': 'rotateX(360deg) rotateY(360deg)'
        };

        var animation = new CssAnimation.keyframes(keyframes);
        animation.apply(cubeDiv, onComplete: () {
          slightRotation(0,0);
        }, delay: 2000, iterations: 1, duration: 5000, timing: "ease");
      }, delay: 2000, iterations: 1, duration: 5000, timing: "ease");
    }, delay: 2000, iterations: 1, duration: 5000, timing: "ease");
*/

    DivElement cubeFace = new DivElement();
    cubeFace.style
        ..display = "block"
        ..position = "absolute"
        ..height = "${cubeHeight-40}px"
        ..width = "${cubeWidth-40}px"
        ..padding = "20px"
        ..opacity = "0.9"
        ..backgroundPosition = "center center"
        ..transform = "translateZ(0px)"
        ..background = "rgba(255,255,255,0.1)"
        ..boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
    cubeDiv.append(cubeFace);
    cubeFace.append(chatboxDiv);

    cubeFace = new DivElement();
    cubeFace.style
        ..display = "block"
        ..position = "absolute"
        ..height = "${cubeHeight-40}px"
        ..width = "${cubeWidth-40}px"
        ..padding = "20px"
        ..opacity = "0.9"
        ..backgroundPosition = "center center"
        ..transform = "translateZ(-200px) rotateY(90deg) translateZ(200px)"
        ..background = "rgba(255,255,255,0.1)"
        ..boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
    cubeDiv.append(cubeFace);

    cubeFace = new DivElement();
    cubeFace.style
        ..display = "block"
        ..position = "absolute"
        ..height = "${cubeHeight-40}px"
        ..width = "${cubeWidth-40}px"
        ..padding = "20px"
        ..opacity = "0.9"
        ..backgroundPosition = "center center"
        ..transform = "translateZ(-200px) rotateY(180deg) translateZ(200px)"
        ..background = "rgba(255,255,255,0.1)"
        ..boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
    cubeDiv.append(cubeFace);

    cubeFace = new DivElement();
    cubeFace.style
        ..display = "block"
        ..position = "absolute"
        ..height = "${cubeHeight-40}px"
        ..width = "${cubeWidth-40}px"
        ..padding = "20px"
        ..opacity = "0.9"
        ..backgroundPosition = "center center"
        ..transform = "translateZ(-200px) rotateY(-90deg) translateZ(200px)"
        ..background = "rgba(255,255,255,0.1)"
        ..boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
    cubeDiv.append(cubeFace);

    cubeFace = new DivElement();
    cubeFace.style
        ..display = "block"
        ..position = "absolute"
        ..height = "${cubeHeight-40}px"
        ..width = "${cubeWidth-40}px"
        ..padding = "20px"
        ..opacity = "0.9"
        ..backgroundPosition = "center center"
        ..transform = "translateZ(-200px) rotateX(-90deg) translateZ(200px) rotate(180deg)"
        ..background = "rgba(255,255,255,0.1)"
        ..boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
    cubeDiv.append(cubeFace);

    cubeFace = new DivElement();
    cubeFace.style
        ..display = "block"
        ..position = "absolute"
        ..height = "${cubeHeight-40}px"
        ..width = "${cubeWidth-40}px"
        ..padding = "20px"
        ..opacity = "0.9"
        ..backgroundPosition = "center center"
        ..transform = "translateZ(-200px) rotateX(90deg) translateZ(200px) rotate(180deg)"
        ..background = "rgba(255,255,255,0.1)"
        ..boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
    cubeDiv.append(cubeFace);
  }
  void makeChatWhosOnlineComponent() {
    chatOnlineBox = new DivElement();
    chatOnlineBox.style
        ..position = "absolute"
        ..width = "100%"
        ..height = "44px"
        ..top = "0px"
        ..left = "0px"
        ..overflow = "scroll";
    chatboxDiv.append(chatOnlineBox);
  }
  void makeChatInputComponent() {
    textField = new InputElement();
    textField.style
        ..position = "absolute"
        ..width = "calc(100% - 9px)"
        ..height = "15px"
        ..bottom = "2px"
        ..left = "2px";
    textField.placeholder = 'Login to chat.';
    textField.onKeyPress.listen((KeyboardEvent e) {
      sendChat(e);
    });
    chatboxDiv.append(textField);
  }
  void makeChatFieldComponent() {
    chatField = new DivElement();
    chatField.style
        ..position = "absolute"
        ..width = "calc(100% - 4px - 10px)"
        ..height = "calc(100% - 44px - 23px)"
        ..top = "42px"
        ..left = "2px"
        ..borderTop = "solid"
        ..borderWidth = "px"
        ..borderColor = "#DDDDDD"
        ..backgroundColor = "rgba(20, 100, 180, 0.8)"
        ..overflow = "hidden"
        ..overflowY = "scroll"
        ..padding = "5px"
        ..paddingTop = "0px"
        ..paddingBottom = "0px";
    chatboxDiv.append(chatField);
  }
}
class ChatMsg {
  String chat;
  DivElement div;
  ChatMsg(String _chat, DivElement _div) {
    chat = _chat;
    div = _div;
  }
}
