import './Chatbox.scss';

import {AppController} from "../AppController";
import {Interface} from "../Utility/Interface";
import {DescriptionParser} from "../Utility/DescriptionParser";
import {MessageReader} from "../Utility/Message/MessageReader";
import {MessageWriter} from "../Utility/Message/MessageWriter";
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

        this.chatboxDiv = Interface.Create({type: 'div', className: 'ChatboxDiv', elements: [
                this.chatOnlineBox = Interface.Create({type: 'div', className: 'OnlineBox'}),
                this.textField = Interface.Create({type: 'input', className: 'TextInput', placeholder: 'Login to chat.', onKeyPress: this.sendChat}),
                this.chatField = Interface.Create({type: 'div', className: 'ChatText'})
            ]});
        this.website.aheadWebsiteDiv.appendChild(this.chatboxDiv);

        setTimeout(()=>{
            this.doneLoading = true;
        }, 1000);
        this.ding = Interface.Create({type: 'audio', src: ding});
    }
    darkTheme() {
        this.chatOnlineBox.classList.add('DarkTheme');
        this.chatOnlineBox.classList.remove('LightTheme');
    }
    lightTheme() {
        this.chatOnlineBox.classList.add('LightTheme');
        this.chatOnlineBox.classList.remove('DarkTheme');
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
    sendChat = (e : KeyboardEvent) => {
        if (e.keyCode == 13 && this.textField.value.length != 0 && this.website.database.loggedIn) {
            let message = new MessageWriter();
            let m : any = {};
            m['Controller'] = 'Chat';
            m['Title'] = 'Msg';
            m['Data'] = this.textField.value;
            this.textField.value = '';
            this.website.networkController.send(m);
        }
    };
    addChat = (chat : string) => {
        let d : HTMLDivElement = Interface.Create({type: 'div', className: 'ChatMsg'});
        d.appendChild(DescriptionParser.parseDescription(chat));
        this.chatMsgs.push(new ChatMsg(chat, d));
        this.updateChatHighlights();
        this.chatField.appendChild(d);
        this.bottomNode = d;
        //Loop to loaded image if there is one
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
    };
    scrollToBottom() {
        if (this.bottomNode == null) {
            return;
        }
        this.bottomNode.scrollIntoView({behavior: "smooth", block: "end", inline: "end"});
    }
    updateChatHighlights() {
        let name : string = this.website.database.name;
        for (let chatMsg of this.chatMsgs) {
            let chatName : string = chatMsg.chat.substring(0, chatMsg.chat.indexOf(":"));
            if (chatName == name) {
                    chatMsg.div.classList.add('SelfMsg');
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
    gotMessageBinary(message : MessageReader) {
        let chatString = message.getString();
        this.addChat(chatString);
    }
    gotOnlineListBinary(message : MessageReader) {
        let onlineString = message.getString();
        this.chatOnlineBox.innerText = onlineString;
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