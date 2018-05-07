import {AppController} from "../AppController";
import {Interface} from "../Utility/Interface";
import {DescriptionParser} from "../Utility/DescriptionParser";
const ding = require('./../../static/assets/ding8.wav');

export class Chatbox {
    website : AppController;
    chatboxDiv : HTMLDivElement;
    textField : HTMLInputElement;
    chatField : HTMLDivElement;
    chatOnlineBox : HTMLDivElement;
    ding : HTMLAudioElement;
    doneLoading = false;
    chatMsgs : ChatMsg[] = [];
    bottomNode : HTMLDivElement = null;
    constructor(w : AppController) {
        this.website = w;
        this.makeChatboxDivComponent();
        this.makeChatWhosOnlineComponent();
        this.makeChatInputComponent();
        this.makeChatFieldComponent();
        setTimeout(()=>{
            this.doneLoading = true;
        }, 1000);
        this.ding = Interface.Create({type: 'audio', src: ding});
    }
    darkTheme() {
        this.chatOnlineBox.style.color = "white";
    }
    lightTheme() {
        this.chatOnlineBox.style.color = "Black";
    }
    processEvent(event : string) {
        switch (event) {
            case "Logged In":
            {
                this.textField.placeholder = "Type a message...";
                this.updateChatHighlights();
                this.scrollToBottom();
            }
                break;
            case "Logged Out":
            {
                this.textField.placeholder = "Login to chat.";
                this.updateChatHighlights();
                this.scrollToBottom();
            }
                break;
        }
    }
    sendChat(e : KeyboardEvent) {
        if (e.keyCode == 13 && this.textField.value.length != 0 && this.website.database.loggedIn) {
            let m : any = {};
            m['Controller'] = 'Chat';
            m['Title'] = 'Msg';
            m['Data'] = this.textField.value;
            this.textField.value = '';
            this.website.networkController.send(m);
        }
    }
    addChat(chat : string) {
        let d : HTMLDivElement = Interface.Create({type: 'div'});
        d.appendChild(DescriptionParser.parseDescription(chat));
            d.style.width = '100%';
            d.style.wordWrap = "break-word";
            d.style.display="inline-block";
            d.style.marginBottom = "10px";
        this.chatMsgs.push(new ChatMsg(chat, d));
        this.updateChatHighlights();
        this.chatField.appendChild(d);
        this.bottomNode = d;
        let loopNodesFunction = (d2 : Node) => {};
        loopNodesFunction = (d2 : Node) => {
            for (let node of Array.from(d2.childNodes)) {
                if (node instanceof HTMLImageElement) {
                    node.onload = () => {
                        this.scrollToBottom();
                    };
                } else {
                    loopNodesFunction(node);
                }
            }
        };
        loopNodesFunction(d);
        this.scrollToBottom();
        if (this.doneLoading) {
            this.ding.play();
        }
    }
    scrollToBottom() {
        if (this.bottomNode == null) {
            return;
        }
        this.bottomNode.scrollIntoView({behavior: "smooth", block: "end", inline: "nearest"});
    }
    updateChatHighlights() {
        let name : string = this.website.database.name;
        for (let chatMsg of this.chatMsgs) {
            let chatName : string = chatMsg.chat.substring(0, chatMsg.chat.indexOf(":"));
            if (chatName != name) {
                    chatMsg.div.style.backgroundColor = "rgba(255,255,255,0.5)";
                    chatMsg.div.style.borderWidth = "1px";
                    chatMsg.div.style.borderColor = "white";
                    chatMsg.div.style.borderStyle = "solid";
            } else {
                    chatMsg.div.style.backgroundColor = "";
                    chatMsg.div.style.borderWidth = "0px";
                    chatMsg.div.style.borderColor = "";
                    chatMsg.div.style.borderStyle = "none";
            }
        }
    }
    onMessage(message : any) {
        if (message['Title'] == 'Msg') {
            this.addChat(message['Data']);
        }
        if (message['Title'] == 'Online List') {
            this.chatOnlineBox.innerText = message['Data'];
        }
    }
    /**********Create GUI Components***********/
    makeChatboxDivComponent() {
        this.chatboxDiv = Interface.Create({type: 'div'});
            //this.chatboxDiv.style.width = "300px"
            //this.chatboxDiv.style.height = "calc(100% - 100px)"
            this.chatboxDiv.style.width = "300px";
            this.chatboxDiv.style.height = "300px";
            this.chatboxDiv.style.left = "0px";
            this.chatboxDiv.style.top = "0px";
            //this.chatboxDiv.style.right = "0px"
            //this.chatboxDiv.style.bottom = "2px"
            this.chatboxDiv.style.position = "absolute";
            this.chatboxDiv.style.borderStyle = "solid";
            this.chatboxDiv.style.borderWidth = "2px";
            this.chatboxDiv.style.display = "block";
            this.chatboxDiv.style.borderColor = "#DDDDDD";

        this.chatboxDiv.style.borderTopLeftRadius = "6px";
        //website.aheadWebsiteDiv.appendChild(chatboxDiv);

        let cubeWidth = 300;
        let cubeHeight = 300;


        let cubeContainer : HTMLDivElement = Interface.Create({type: 'div'});
            cubeContainer.style.perspective = "1000px";
            cubeContainer.style.perspectiveOrigin = "50% 50%";
            cubeContainer.style.transformStyle = "preserve-3d";
            cubeContainer.style.backfaceVisibility="hidden";
            cubeContainer.style.right = "50px";
            cubeContainer.style.bottom = "100px";
            cubeContainer.style.position = "absolute";
            cubeContainer.style.display = "block";
        this.website.aheadWebsiteDiv.appendChild(cubeContainer);

        let cubeDiv : HTMLDivElement = Interface.Create({type: 'div'});
            cubeDiv.style.display = "block";
            cubeDiv.style.transformStyle = "preserve-3d";
            cubeDiv.style.position = "relative";
            cubeDiv.style.margin = "0 auto";
            cubeDiv.style.height = `${cubeHeight}px`;
            cubeDiv.style.width = `${cubeWidth}px`;
        //cubeDiv.style.transformOriginZ = "-200px";
        cubeContainer.appendChild(cubeDiv);

        let cubeFace : HTMLDivElement = Interface.Create({type: 'div'});
            cubeFace.style.display = "block";
            cubeFace.style.position = "absolute";
            cubeFace.style.height = `${cubeHeight-40}px`;
            cubeFace.style.width = `${cubeWidth-40}px`;
            cubeFace.style.padding = "20px";
            cubeFace.style.opacity = "0.9";
            cubeFace.style.backgroundPosition = "center center";
            cubeFace.style.transform = "translateZ(0px)";
            cubeFace.style.background = "rgba(255,255,255,0.1)";
            cubeFace.style.boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
        cubeDiv.appendChild(cubeFace);
        cubeFace.appendChild(this.chatboxDiv);

        cubeFace = Interface.Create({type: 'div'});
            cubeFace.style.display = "block";
            cubeFace.style.position = "absolute";
            cubeFace.style.height = `${cubeHeight-40}px`;
            cubeFace.style.width = `${cubeWidth-40}px`;
            cubeFace.style.padding = "20px";
            cubeFace.style.opacity = "0.9";
            cubeFace.style.backgroundPosition = "center center";
            cubeFace.style.transform = "translateZ(-200px) rotateY(90deg) translateZ(200px)";
            cubeFace.style.background = "rgba(255,255,255,0.1)";
            cubeFace.style.boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
        cubeDiv.appendChild(cubeFace);

        cubeFace = Interface.Create({type: 'div'});
            cubeFace.style.display = "block";
            cubeFace.style.position = "absolute";
            cubeFace.style.height = `${cubeHeight-40}px`;
            cubeFace.style.width = `${cubeWidth-40}px`;
            cubeFace.style.padding = "20px";
            cubeFace.style.opacity = "0.9";
            cubeFace.style.backgroundPosition = "center center";
            cubeFace.style.transform = "translateZ(-200px) rotateY(180deg) translateZ(200px)";
            cubeFace.style.background = "rgba(255,255,255,0.1)";
            cubeFace.style.boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
        cubeDiv.appendChild(cubeFace);

        cubeFace = Interface.Create({type: 'div'});
            cubeFace.style.display = "block";
            cubeFace.style.position = "absolute";
            cubeFace.style.height = `${cubeHeight-40}px`;
            cubeFace.style.width = `${cubeWidth-40}px`;
            cubeFace.style.padding = "20px";
            cubeFace.style.opacity = "0.9";
            cubeFace.style.backgroundPosition = "center center";
            cubeFace.style.transform = "translateZ(-200px) rotateY(-90deg) translateZ(200px)";
            cubeFace.style.background = "rgba(255,255,255,0.1)";
            cubeFace.style.boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
        cubeDiv.appendChild(cubeFace);

        cubeFace = Interface.Create({type: 'div'});
            cubeFace.style.display = "block";
            cubeFace.style.position = "absolute";
            cubeFace.style.height = `${cubeHeight-40}px`;
            cubeFace.style.width = `${cubeWidth-40}px`;
            cubeFace.style.padding = "20px";
            cubeFace.style.opacity = "0.9";
            cubeFace.style.backgroundPosition = "center center";
            cubeFace.style.transform = "translateZ(-200px) rotateX(-90deg) translateZ(200px) rotate(180deg)";
            cubeFace.style.background = "rgba(255,255,255,0.1)";
            cubeFace.style.boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
        cubeDiv.appendChild(cubeFace);

        cubeFace = Interface.Create({type: 'div'});
            cubeFace.style.display = "block";
            cubeFace.style.position = "absolute";
            cubeFace.style.height = `${cubeHeight-40}px`;
            cubeFace.style.width = `${cubeWidth-40}px`;
            cubeFace.style.padding = "20px";
            cubeFace.style.opacity = "0.9";
            cubeFace.style.backgroundPosition = "center center";
            cubeFace.style.transform = "translateZ(-200px) rotateX(90deg) translateZ(200px) rotate(180deg)";
            cubeFace.style.background = "rgba(255,255,255,0.1)";
            cubeFace.style.boxShadow = "inset 0 0 30px rgba(125,125,125,0.8)";
        cubeDiv.appendChild(cubeFace);
    }
    makeChatWhosOnlineComponent() {
        this.chatOnlineBox = Interface.Create({type: 'div'});
            this.chatOnlineBox.style.position = "absolute";
            this.chatOnlineBox.style.width = "100%";
            this.chatOnlineBox.style.height = "44px";
            this.chatOnlineBox.style.top = "0px";
            this.chatOnlineBox.style.left = "0px";
            this.chatOnlineBox.style.overflow = "scroll";
        this.chatboxDiv.appendChild(this.chatOnlineBox);
    }
    makeChatInputComponent() {
        this.textField = Interface.Create({type: 'input'});
            this.textField.style.position = "absolute";
            this.textField.style.width = "calc(100% - 9px)";
            this.textField.style.height = "15px";
            this.textField.style.bottom = "2px";
            this.textField.style.left = "2px";
        this.textField.placeholder = 'Login to chat.';
        this.textField.onkeypress = (e : KeyboardEvent) => {
            this.sendChat(e);
        };
        this.chatboxDiv.appendChild(this.textField);
    }
    makeChatFieldComponent() {
        this.chatField = Interface.Create({type: 'div'});
            this.chatField.style.position = "absolute";
            this.chatField.style.width = "calc(100% - 4px - 10px)";
            this.chatField.style.height = "calc(100% - 44px - 23px)";
            this.chatField.style.top = "42px";
            this.chatField.style.left = "2px";
            this.chatField.style.borderTop = "solid";
            this.chatField.style.borderWidth = "px";
            this.chatField.style.borderColor = "#DDDDDD";
            this.chatField.style.backgroundColor = "rgba(20, 100, 180, 0.8)";
            this.chatField.style.overflow = "hidden";
            this.chatField.style.overflowY = "scroll";
            this.chatField.style.padding = "5px";
            this.chatField.style.paddingTop = "0px";
            this.chatField.style.paddingBottom = "0px";
        this.chatboxDiv.appendChild(this.chatField);
    }
}

class ChatMsg {
    chat: string;
    div: HTMLDivElement;

    constructor(_chat: string, _div: HTMLDivElement) {
        this.chat = _chat;
        this.div = _div;
    }
}