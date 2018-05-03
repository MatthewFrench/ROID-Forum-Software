import {Controllers} from "./MessageDefinitions/ClientMessageDefinitions";
import {ChatMessageHandler} from './Chat/ChatMessageHandler';
import {MessageReader} from "../Utility/Message/MessageReader";
import {MessageWriter} from "../Utility/Message/MessageWriter";
import {NetworkController} from "./NetworkController";

/**
 * Routes the messages to controllers.
 */
export class MessageRouter {
    networkController : NetworkController;
    chatMessageHandler : ChatMessageHandler;
    constructor(networkController : NetworkController) {
        this.networkController = networkController;
        this.chatMessageHandler = new ChatMessageHandler(this.networkController.getAppController());
    }

    handleMessageEvent(binary : ArrayBuffer) {
        let message = new MessageReader(binary);
        //Get the controller
        let controllerID = message.getUint8();
        //Send it to the correct callbacks
        switch(controllerID) {
            case Controllers.Chat.ID: {
                this.chatMessageHandler.routeMessage(message);
            } break;
            case Controllers.Network.ID: {
                this.handleNetworkingMessageEvent(message);
            } break;
            default: {
                console.error('Unknown Message');
                console.trace();
            }
        }
    }

    /**
     * Handle messages sent specifically for the networking controller.
     * @param {MessageReader} message
     */
    handleNetworkingMessageEvent(message : MessageReader) {
        let messageID = message.getUint8();
        switch(messageID) {
            case Controllers.Network.Messages.CombinedMessage: {
                while (message.hasBinary()) {
                    try {
                        this.handleMessageEvent(message.getBinary());
                    } catch (e) {
                        console.log(e);
                        console.error('Badly written combined message.');
                        console.trace();
                    }
                }
                if (!message.isAtEndOfData()) {
                    console.error('Invalid Message');
                    console.trace();
                }
            } break;
        }
    }
}