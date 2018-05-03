import {Controllers} from '../MessageDefinitions/ServerMessageDefinitions';
import {MessageWriter} from '../../Utility/Message/MessageWriter';

export class ChatMessageCreator {
    static NewChatMessage(chatMessage : any) : ArrayBuffer {
        let message = new MessageWriter();
        message.addUint8(Controllers.Chat.ID);
        message.addUint8(Controllers.Chat.Messages.NewChatMessage);
        message.addString(chatMessage);
        return message.toBuffer();
    }
}