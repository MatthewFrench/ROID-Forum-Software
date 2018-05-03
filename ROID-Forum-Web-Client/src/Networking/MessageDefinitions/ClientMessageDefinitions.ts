//These are the definitions that the client receives.
export const Controllers = {
    Chat: {
        ID: 2,
        Messages: {
            //Sends a chat message to the client
            AddChatMessage: 1
        }
    },
    Network: {
        ID: 4,
        Messages: {
            //Combined message
            CombinedMessage: 1
        }
    }
};