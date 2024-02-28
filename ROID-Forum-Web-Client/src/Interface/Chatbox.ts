import './Chatbox.scss';

import {AppController} from "../AppController";
import {Interface} from "../Utility/Interface";
import {DescriptionParser} from "../Utility/DescriptionParser";
import {MessageReader} from "../Utility/Message/MessageReader";
import {MessageWriter} from "../Utility/Message/MessageWriter";
import {SendMessages} from "../Networking/MessageDefinitions/SendMessages";
const ding = require('./../../static/assets/ding8.wav');

type User = { connectionId: string, accountId?: string, displayName?: string};

export class Chatbox {
    website : AppController;
    chatboxDiv : HTMLDivElement;
    textField : HTMLInputElement;
    chatField : HTMLDivElement;
    chatOnlineBox : HTMLDivElement;
    ding : HTMLAudioElement;
    doneLoading = false;
    chatMsgs : ChatMsg[] = [];
    onlineUsers: User[] = [];
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
            message.addUint8(SendMessages.Controller.Chat);
            message.addUint8(SendMessages.ChatMessage.Message);
            message.addString(this.textField.value);
            this.website.networkController.send(message.toBuffer());
            this.textField.value = '';
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
        let name : string = this.website.database.displayName;
        for (let chatMsg of this.chatMsgs) {
            let chatName : string = chatMsg.chat.substring(0, chatMsg.chat.indexOf(":"));
            if (chatName == name) {
                    chatMsg.div.classList.add('SelfMsg');
            }
        }
    }
    updateChatOnlineDisplay() {
        let currentGuestsOnline = 0;
        let namesOnline = [];
        for (let user of this.onlineUsers) {
            if (user.displayName == undefined) {
                currentGuestsOnline += 1;
            } else {
                namesOnline.push(user.displayName);
            }
        }
        let onlineText = "Online: ";
        if (namesOnline.length > 0) {
            onlineText += namesOnline.join(", ");
            if (currentGuestsOnline > 0) {
                onlineText += ` and ${currentGuestsOnline} Guest`;
            }
        } else if (currentGuestsOnline > 0) {
            onlineText += `${currentGuestsOnline} Guest`;
        }
        if (currentGuestsOnline > 1) {
            onlineText += "s"
        }
        this.chatOnlineBox.innerText = onlineText;
    }
    gotAllMessages(message : MessageReader) {
        /*
        uint32 chat count
        [
            string chatId
            string creatorAccountId
            string creatorDisplayname
            string createdTime
            string content
        ]
         */
        let count = message.getUint32()
    }
    gotNewMessage(message : MessageReader) {
        /*
        string chatId
        string creatorAccountId
        string creatorDisplayName
        string createdTime
        string content
         */
        let chatString = message.getString();
        this.addChat(chatString);
    }
    gotAllOnlineList(message : MessageReader) {
        /*
        uint32 count
        [
            string connectionId
            uint8 hasAccount
            optional string accountId
            optional string displayName
        ]
         */
        this.onlineUsers = [];
        let count = message.getUint32();
        for (let index = 0; index < count; index++) {
            let connectionId = message.getString();
            let hasAccount = message.getUint8() == 1;
            let accountId : string | undefined = hasAccount ? message.getString() : undefined;
            let displayName : string | undefined = hasAccount ? message.getString() : undefined;
            this.onlineUsers.push({
                connectionId: connectionId,
                accountId: accountId,
                displayName: displayName
            });
        }
        this.updateChatOnlineDisplay();
    }
    gotOnlineListAddUser(message : MessageReader) {
        /*
        string connectionId
         */
        let connectionId = message.getString();
        this.onlineUsers.push({
            connectionId: connectionId
        });
        this.updateChatOnlineDisplay();
    }
    gotOnlineListRemoveUser(message : MessageReader) {
        /*
        string connectionId
         */
        let connectionId = message.getString();
        for (let index = 0; index < this.onlineUsers.length; index++) {
            let user = this.onlineUsers[index];
            if (user.connectionId == connectionId) {
                this.onlineUsers.splice(index, 1);
                index -= 1;
            }
        }
        this.updateChatOnlineDisplay();
    }
    gotOnlineListLoggedInUser(message : MessageReader) {
        /*
        string connectionId
        string accountId
        string displayName
         */
        let connectionId = message.getString();
        let accountId = message.getString();
        let displayName = message.getString();
        for (let index = 0; index < this.onlineUsers.length; index++) {
            let user = this.onlineUsers[index];
            if (user.connectionId == connectionId) {
                user.accountId = accountId;
                user.displayName = displayName;
            }
        }
        this.updateChatOnlineDisplay();
    }
    gotOnlineListLoggedOutUser(message : MessageReader) {
        /*
        string connectionId
         */
        let connectionId = message.getString();
        for (let index = 0; index < this.onlineUsers.length; index++) {
            let user = this.onlineUsers[index];
            if (user.connectionId == connectionId) {
                user.accountId = undefined;
                user.displayName = undefined;
            }
        }
        this.updateChatOnlineDisplay();
    }
    gotDisplayNameUpdate(message : MessageReader) {
        /*
        string accountId
        string displayName
         */
        let accountId = message.getString();
        let displayName = message.getString();
        for (let index = 0; index < this.onlineUsers.length; index++) {
            let user = this.onlineUsers[index];
            if (user.accountId == accountId) {
                user.displayName = undefined;
            }
        }
        this.updateChatOnlineDisplay();
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