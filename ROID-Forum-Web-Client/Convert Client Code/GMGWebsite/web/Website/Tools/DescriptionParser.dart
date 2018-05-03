library CodingDescriptionParser;
import 'dart:html';

//description.appendHtml(thread.getDescription().replaceAll('\n', "</br>"));
// bold, underline, italics
class DescriptionParser {
  static DivElement parseDescription(String description) {
    DivElement div = new DivElement();

    CommandToken firstToken = new CommandToken(description);
    div.append(firstToken.getContent());

    return div;
  }
}


/****Change Command Token to only put inner tags inside each other. It needs of keep a list
 *   of all tags that are on the same level. Right now it's making all tags go inside the first.
 *****/


class CommandToken {
  String originalText = "";
  CommandStartTag startTag;
  CommandEndTag endTag;
  String innerText = "",
      firstTagText = "",
      lastTagText = "",
      beforeTagText = "",
      afterTagText = "";
  CommandToken innerCommandToken, afterCommandToken;
  CommandToken(String text) {
    originalText = text;
    findStartAndEndTagLocations(0);
    if (startTag != null && endTag != null) {
      beforeTagText = originalText.substring(0, startTag.startTagStart);
      firstTagText = originalText.substring(startTag.startTagStart, startTag.startTagEnd);
      innerText = originalText.substring(startTag.startTagEnd, endTag.endTagStart);
      lastTagText = originalText.substring(endTag.endTagStart, endTag.endTagEnd);
      afterTagText = originalText.substring(endTag.endTagEnd);
      innerCommandToken = new CommandToken(innerText);
      afterCommandToken = new CommandToken(afterTagText);
    }
  }
  String getOriginalText() {
    return originalText;
  }
  SpanElement getContent() {

    //Based on the command, return the proper element
    if (startTag != null && endTag != null) {
      SpanElement tokenSpan = new SpanElement();
      tokenSpan.style..maxWidth="100%"..display="inline-block"; 
      tokenSpan.append(convertPlainText(beforeTagText));

      if (startTag.command == "b") {
        SpanElement commandSpan = new SpanElement();
        commandSpan.style.fontWeight = "bold";
        commandSpan.append(innerCommandToken.getContent());
        tokenSpan.append(commandSpan);
      }
      if (startTag.command == "u") {
        SpanElement commandSpan = new SpanElement();
        commandSpan.style.textDecoration = "underline";
        commandSpan.append(innerCommandToken.getContent());
        tokenSpan.append(commandSpan);
      }
      if (startTag.command == "i") {
        SpanElement commandSpan = new SpanElement();
        commandSpan.style.fontStyle = "italic";
        commandSpan.append(innerCommandToken.getContent());
        tokenSpan.append(commandSpan);
      }
      if (startTag.command == "s") {
        SpanElement commandSpan = new SpanElement();
        commandSpan.style.textDecoration = "line-through";
        commandSpan.append(innerCommandToken.getContent());
        tokenSpan.append(commandSpan);
      }
      if (startTag.command == "url") {
        AnchorElement link = new AnchorElement(href: innerCommandToken.getOriginalText());
        link.target = "Blank";
        link.append(innerCommandToken.getContent());
        tokenSpan.append(link);
      }
      if (startTag.command == "img") {
        ImageElement link = new ImageElement(src: innerCommandToken.getOriginalText());
        link.style.maxWidth = "100%";
        link.style.display="inline-block";
        /*
        link.onClick.listen((_) {
          if (link.style.width == "") {
            link.style.maxWidth = "100%";
          } else {
            link.style.width = "";
          }
        });*/
        tokenSpan.append(link);
      }
      if (startTag.command == "youtube") {
        IFrameElement link = new IFrameElement();
        link.style
            ..width = "100%"
            ;//..height = "400px";
        link.style.display="block";
        link.setAttribute("frameBorder", "0");
        link.setAttribute("allowFullscreen", "true");
        link.allowFullscreen = true;
        //Find the youtube thing
        String loc = innerCommandToken.getOriginalText();
        int startLoc = loc.indexOf("v=") + 2;
        if (startLoc != -1) {
          loc = loc.substring(startLoc);
          int endLoc = loc.indexOf("&");
          if (endLoc != -1) {
            loc = loc.substring(0, endLoc);
          }
          link.src = "//www.youtube.com/embed/${loc}";
          tokenSpan.append(link);
        }
      }
      tokenSpan.append(afterCommandToken.getContent());

      return tokenSpan;
    }
    return convertPlainText(originalText);
  }
  SpanElement convertPlainText(String text) {
    SpanElement span = new SpanElement();
    List<String> strings = text.split('\n');
    for (int i = 0; i < strings.length; i++) {
      String s = strings[i];
      span.appendText(s);
      if (i != strings.length - 1) {
        span.append(new BRElement());
      }
    }
    return span;
  }
  void findStartAndEndTagLocations(int offset) {
    for (int i = offset; i < originalText.length; i++) {
      CommandStartTag c = commandStartTagAtLoc(i);
      if (c.command != "") {
        startTag = c;
        break;
      }
    }
    if (startTag != null) {
      int commandCount = 1;
      for (int i = startTag.startTagEnd + 1; i < originalText.length; i++) {
        CommandStartTag start = commandStartTagAtLoc(i);
        CommandEndTag end = commandEndTagAtLoc(i);
        if (start.command == startTag.command) {
          commandCount += 1;
        } else if (startTag.command == end.command) {
          commandCount -= 1;
        }
        if (commandCount == 0) {
          endTag = end;
          break;
        }
      }
    }
    //If we have a start tag but no end tag, lets restart this function and act
    //like the start tag is just normal text
    if (startTag != null && endTag == null) {
      int loc = startTag.startTagEnd;
      startTag = null;
      findStartAndEndTagLocations(loc);
    }
  }
  CommandStartTag commandStartTagAtLoc(int loc) {
    CommandStartTag c = new CommandStartTag();
    if (loc + 2 < originalText.length && originalText.substring(loc, loc + 3) == "[b]") {
      c.command = "b";
      c.startTagStart = loc;
      c.startTagEnd = loc + 3;
    } else if (loc + 2 < originalText.length && originalText.substring(loc, loc + 3) == "[u]") {
      c.command = "u";
      c.startTagStart = loc;
      c.startTagEnd = loc + 3;
    } else if (loc + 2 < originalText.length && originalText.substring(loc, loc + 3) == "[i]") {
      c.command = "i";
      c.startTagStart = loc;
      c.startTagEnd = loc + 3;
    } else if (loc + 2 < originalText.length && originalText.substring(loc, loc + 3) == "[s]") {
      c.command = "s";
      c.startTagStart = loc;
      c.startTagEnd = loc + 3;
    } else if (loc + 4 < originalText.length && originalText.substring(loc, loc + 5) == "[url]") {
      c.command = "url";
      c.startTagStart = loc;
      c.startTagEnd = loc + 5;
    } else if (loc + 4 < originalText.length && originalText.substring(loc, loc + 5) == "[img]") {
      c.command = "img";
      c.startTagStart = loc;
      c.startTagEnd = loc + 5;
    } else if (loc + 8 < originalText.length && originalText.substring(loc, loc + 9) == "[youtube]") {
      c.command = "youtube";
      c.startTagStart = loc;
      c.startTagEnd = loc + 9;
    }
    return c;
  }
  CommandEndTag commandEndTagAtLoc(int loc) {
    CommandEndTag c = new CommandEndTag();
    if (loc + 3 < originalText.length && originalText.substring(loc, loc + 4) == "[/b]") {
      c.command = "b";
      c.endTagStart = loc;
      c.endTagEnd = loc + 4;
    } else if (loc + 3 < originalText.length && originalText.substring(loc, loc + 4) == "[/u]") {
      c.command = "u";
      c.endTagStart = loc;
      c.endTagEnd = loc + 4;
    } else if (loc + 3 < originalText.length && originalText.substring(loc, loc + 4) == "[/i]") {
      c.command = "i";
      c.endTagStart = loc;
      c.endTagEnd = loc + 4;
    } else if (loc + 3 < originalText.length && originalText.substring(loc, loc + 4) == "[/s]") {
      c.command = "s";
      c.endTagStart = loc;
      c.endTagEnd = loc + 4;
    } else if (loc + 5 < originalText.length && originalText.substring(loc, loc + 6) == "[/url]") {
      c.command = "url";
      c.endTagStart = loc;
      c.endTagEnd = loc + 6;
    } else if (loc + 5 < originalText.length && originalText.substring(loc, loc + 6) == "[/img]") {
      c.command = "img";
      c.endTagStart = loc;
      c.endTagEnd = loc + 6;
    } else if (loc + 9 < originalText.length && originalText.substring(loc, loc + 10) == "[/youtube]") {
      c.command = "youtube";
      c.endTagStart = loc;
      c.endTagEnd = loc + 10;
    }
    return c;
  }
}
class CommandStartTag {
  String command = "";
  int startTagStart = -1;
  int startTagEnd = -1;
}
class CommandEndTag {
  String command = "";
  int endTagStart = -1;
  int endTagEnd = -1;
}
