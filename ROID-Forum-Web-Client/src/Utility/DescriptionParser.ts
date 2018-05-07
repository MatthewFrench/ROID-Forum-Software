
//description.appendHtml(thread.getDescription().replaceAll('\n', "</br>"));
// bold, underline, italics
import {Interface} from "./Interface";

export class DescriptionParser {
    static parseDescription(description : string) : HTMLDivElement {
        let div : HTMLDivElement = Interface.Create({type: 'div'});

        let firstToken = new CommandToken(description);
        div.appendChild(firstToken.getContent());

        return div;
    }
}


/****Change Command Token to only put inner tags inside each other. It needs of keep a list
 *   of all tags that are on the same level. Right now it's making all tags go inside the first.
 *****/


class CommandToken {
    originalText = "";
    startTag : CommandStartTag;
    endTag : CommandEndTag;
    innerText = "";
    firstTagText = "";
    lastTagText = "";
    beforeTagText = "";
    afterTagText = "";
    innerCommandToken : CommandToken;
    afterCommandToken : CommandToken ;
    constructor(text : string) {
        this.originalText = text;
        this.findStartAndEndTagLocations(0);
        if (this.startTag != null && this.endTag != null) {
            this.beforeTagText = this.originalText.substring(0, this.startTag.startTagStart);
            this.firstTagText = this.originalText.substring(this.startTag.startTagStart, this.startTag.startTagEnd);
            this.innerText = this.originalText.substring(this.startTag.startTagEnd, this.endTag.endTagStart);
            this.lastTagText = this.originalText.substring(this.endTag.endTagStart, this.endTag.endTagEnd);
            this.afterTagText = this.originalText.substring(this.endTag.endTagEnd);
            this.innerCommandToken = new CommandToken(this.innerText);
            this.afterCommandToken = new CommandToken(this.afterTagText);
        }
    }
    getOriginalText() : string {
        return this.originalText;
    }
    getContent() : HTMLSpanElement {
        //Based on the command, return the proper element
        if (this.startTag != null && this.endTag != null) {
            let tokenSpan : HTMLSpanElement = Interface.Create({type: 'span'});
            tokenSpan.style.maxWidth="100%";
            tokenSpan.style.display="inline-block";
            tokenSpan.appendChild(CommandToken.convertPlainText(this.beforeTagText));
    
            if (this.startTag.command == "b") {
                let commandSpan : HTMLSpanElement = Interface.Create({type: 'span'});
                commandSpan.style.fontWeight = "bold";
                commandSpan.appendChild(this.innerCommandToken.getContent());
                tokenSpan.appendChild(commandSpan);
            }
            if (this.startTag.command == "u") {
                let commandSpan : HTMLSpanElement = Interface.Create({type: 'span'});
                commandSpan.style.textDecoration = "underline";
                commandSpan.appendChild(this.innerCommandToken.getContent());
                tokenSpan.appendChild(commandSpan);
            }
            if (this.startTag.command == "i") {
                let commandSpan : HTMLSpanElement = Interface.Create({type: 'span'});
                commandSpan.style.fontStyle = "italic";
                commandSpan.appendChild(this.innerCommandToken.getContent());
                tokenSpan.appendChild(commandSpan);
            }
            if (this.startTag.command == "s") {
                let commandSpan : HTMLSpanElement = Interface.Create({type: 'span'});
                commandSpan.style.textDecoration = "line-through";
                commandSpan.appendChild(this.innerCommandToken.getContent());
                tokenSpan.appendChild(commandSpan);
            }
            if (this.startTag.command == "url") {
                let link : HTMLAnchorElement = Interface.Create({type: 'a', href: this.innerCommandToken.getOriginalText()});
                link.target = "Blank";
                link.appendChild(this.innerCommandToken.getContent());
                tokenSpan.appendChild(link);
            }
            if (this.startTag.command == "img") {
                let link : HTMLImageElement = Interface.Create({type: 'img', src: this.innerCommandToken.getOriginalText()});
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
                tokenSpan.appendChild(link);
            }
            if (this.startTag.command == "youtube") {
                let link : HTMLIFrameElement = Interface.Create({type: 'iframe'});
                link.style.width = "100%";
                //..height = "400px";
                link.style.display="block";
                link.setAttribute("frameBorder", "0");
                link.setAttribute("allowFullscreen", "true");
                link.allowFullscreen = true;
                //Find the youtube thing
                let loc : string = this.innerCommandToken.getOriginalText();
                let startLoc : number = loc.indexOf("v=") + 2;
                if (startLoc != -1) {
                    loc = loc.substring(startLoc);
                    let endLoc : number = loc.indexOf("&");
                    if (endLoc != -1) {
                        loc = loc.substring(0, endLoc);
                    }
                    link.src = `//www.youtube.com/embed/${loc}`;
                    tokenSpan.appendChild(link);
                }
            }
            tokenSpan.appendChild(this.afterCommandToken.getContent());
    
            return tokenSpan;
        }
        return CommandToken.convertPlainText(this.originalText);
    }
    static convertPlainText(text : string) : HTMLSpanElement {
        let span : HTMLSpanElement = Interface.Create({type: 'span'});
        let strings : string[] = text.split('\n');
        for (let i = 0; i < strings.length; i++) {
            span.innerText += strings[i];
            if (i != strings.length - 1) {
                span.appendChild(Interface.Create({type: 'br'}));
            }
        }
        return span;
    }
    findStartAndEndTagLocations(offset : number) {
        for (let i = offset; i < this.originalText.length; i++) {
            let c : CommandStartTag = this.commandStartTagAtLoc(i);
            if (c.command != "") {
                this.startTag = c;
                break;
            }
        }
        if (this.startTag != null) {
            let commandCount = 1;
            for (let i = this.startTag.startTagEnd + 1; i < this.originalText.length; i++) {
                let start : CommandStartTag = this.commandStartTagAtLoc(i);
                let end : CommandEndTag = this.commandEndTagAtLoc(i);
                if (start.command == this.startTag.command) {
                    commandCount += 1;
                } else if (this.startTag.command == end.command) {
                    commandCount -= 1;
                }
                if (commandCount == 0) {
                    this.endTag = end;
                    break;
                }
            }
        }
        //If we have a start tag but no end tag, lets restart this function and act
        //like the start tag is just normal text
        if (this.startTag != null && this.endTag == null) {
            let loc = this.startTag.startTagEnd;
            this.startTag = null;
            this.findStartAndEndTagLocations(loc);
        }
    }
    commandStartTagAtLoc(loc : number) : CommandStartTag {
        let c : CommandStartTag = new CommandStartTag();
        if (loc + 2 < this.originalText.length && this.originalText.substring(loc, loc + 3) == "[b]") {
            c.command = "b";
            c.startTagStart = loc;
            c.startTagEnd = loc + 3;
        } else if (loc + 2 < this.originalText.length && this.originalText.substring(loc, loc + 3) == "[u]") {
            c.command = "u";
            c.startTagStart = loc;
            c.startTagEnd = loc + 3;
        } else if (loc + 2 < this.originalText.length && this.originalText.substring(loc, loc + 3) == "[i]") {
            c.command = "i";
            c.startTagStart = loc;
            c.startTagEnd = loc + 3;
        } else if (loc + 2 < this.originalText.length && this.originalText.substring(loc, loc + 3) == "[s]") {
            c.command = "s";
            c.startTagStart = loc;
            c.startTagEnd = loc + 3;
        } else if (loc + 4 < this.originalText.length && this.originalText.substring(loc, loc + 5) == "[url]") {
            c.command = "url";
            c.startTagStart = loc;
            c.startTagEnd = loc + 5;
        } else if (loc + 4 < this.originalText.length && this.originalText.substring(loc, loc + 5) == "[img]") {
            c.command = "img";
            c.startTagStart = loc;
            c.startTagEnd = loc + 5;
        } else if (loc + 8 < this.originalText.length && this.originalText.substring(loc, loc + 9) == "[youtube]") {
            c.command = "youtube";
            c.startTagStart = loc;
            c.startTagEnd = loc + 9;
        }
        return c;
    }
    commandEndTagAtLoc(loc : number) : CommandEndTag {
        let c : CommandEndTag = new CommandEndTag();
        if (loc + 3 < this.originalText.length && this.originalText.substring(loc, loc + 4) == "[/b]") {
            c.command = "b";
            c.endTagStart = loc;
            c.endTagEnd = loc + 4;
        } else if (loc + 3 < this.originalText.length && this.originalText.substring(loc, loc + 4) == "[/u]") {
            c.command = "u";
            c.endTagStart = loc;
            c.endTagEnd = loc + 4;
        } else if (loc + 3 < this.originalText.length && this.originalText.substring(loc, loc + 4) == "[/i]") {
            c.command = "i";
            c.endTagStart = loc;
            c.endTagEnd = loc + 4;
        } else if (loc + 3 < this.originalText.length && this.originalText.substring(loc, loc + 4) == "[/s]") {
            c.command = "s";
            c.endTagStart = loc;
            c.endTagEnd = loc + 4;
        } else if (loc + 5 < this.originalText.length && this.originalText.substring(loc, loc + 6) == "[/url]") {
            c.command = "url";
            c.endTagStart = loc;
            c.endTagEnd = loc + 6;
        } else if (loc + 5 < this.originalText.length && this.originalText.substring(loc, loc + 6) == "[/img]") {
            c.command = "img";
            c.endTagStart = loc;
            c.endTagEnd = loc + 6;
        } else if (loc + 9 < this.originalText.length && this.originalText.substring(loc, loc + 10) == "[/youtube]") {
            c.command = "youtube";
            c.endTagStart = loc;
            c.endTagEnd = loc + 10;
        }
        return c;
    }
}
class CommandStartTag {
    command = "";
    startTagStart = -1;
    startTagEnd = -1;
}
class CommandEndTag {
    command = "";
    endTagStart = -1;
    endTagEnd = -1;
}
