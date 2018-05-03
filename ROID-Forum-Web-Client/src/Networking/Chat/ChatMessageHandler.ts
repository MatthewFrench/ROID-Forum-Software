import {MessageReader} from "../../Utility/Message/MessageReader";
import {Controllers} from "../MessageDefinitions/ClientMessageDefinitions";
import {AppController} from "../../AppController";

export class ChatMessageHandler {
    appController : AppController;
    constructor(appController : AppController) {
        this.appController = appController;
    }

    routeMessage(message : MessageReader) {
        let messageID = message.getUint8();
        switch(messageID) {
            case Controllers.Chat.Messages.AddChatMessage: {
                this.addChatMessage(message);
            } break;
        }
    }

    addChatMessage(message : MessageReader) {
        //Parse message with validation
        if (!message.hasUint32()) {
            console.error('Invalid Message');
            console.trace();
            return;
        }
        let boardID = message.getUint32();
        if (!message.hasUint32()) {
            console.error('Invalid Message');
            console.trace();
            return;
        }
        let playerID = message.getUint32();
        if (!message.hasString()) {
            console.error('Invalid Message');
            console.trace();
            return;
        }
        let chatPrefix = message.getString();
        if (!message.hasString()) {
            console.error('Invalid Message');
            console.trace();
            return;
        }
        let chatMessage = message.getString();
        if (!message.hasFloat64()) {
            console.error('Invalid Message');
            console.trace();
            return;
        }
        let time = message.getFloat64();
        if (!message.isAtEndOfData()) {
            console.error('Invalid Message');
            console.trace();
            return;
        }

        //Send to be processed in logic
        //this.appController.chatController.newChatMessageEvent(boardID, playerID, chatPrefix, chatMessage, time);
    }
}