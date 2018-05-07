import {MessageRouter} from "./MessageRouter";

import {AppController} from "../AppController";
//import {ChatMessageCreator} from "./Chat/ChatMessageCreator";
//import {Controllers} from "./MessageDefinitions/ServerMessages";
//import {MessageWriter} from "../Utility/Message/MessageWriter";
import {MessageReader} from "../Utility/Message/MessageReader";

export class NetworkController {
    port : string = '7779';
    ip : string = 'localhost';
    serverURL : string = `ws://${this.ip}:${this.port}`;
    connection : WebSocket = null;
    connected : boolean = false;
    pingTime : number = 0;
    pingTimeArray : number[] = [];
    appController : AppController;
    messageRouter : MessageRouter;

    constructor(appController : AppController) {
        this.appController = appController;
        this.messageRouter = new MessageRouter(this);
    }

    initialize = () => {
        // Create WebSocket connection.
        this.connection = new WebSocket(this.serverURL);
        this.connection.binaryType = "arraybuffer";

        // Connection opened
        this.connection.addEventListener('open', this.connectedEvent);

        // Listen for messages
        this.connection.addEventListener('message', this.gotMessageEvent);

        // Connection closed
        this.connection.addEventListener('close', this.disconnectedEvent);

        // Connection error
        this.connection.addEventListener('error', this.errorEvent);
    };

    connectedEvent = () => {
        this.connected = true;
        console.log('Client has connected to the server!');
        this.appController.connectedToServer();

        /*
        let message = new MessageWriter();
        message.addUint8(1);
        message.addInt8(-1);
        message.addUint16(2);
        message.addInt16(-2);
        message.addUint32(3);
        message.addInt32(-3);
        message.addFloat32(3.3);
        message.addFloat64(4.4);
        message.addString("This is a test string");
        let message2 = new MessageWriter();
        message2.addString("Inner Binary");
        message.addBinary(message2.toBuffer());
        this.send(message.toBuffer());
        */
    };

    disconnectedEvent = (close : CloseEvent) => {
        if (this.connected == false) {
            //Try to reconnect at an interval
            setTimeout(()=>{this.appController.prepareReset();}, 10 * 1000);
        } else {
            //Was just connected and now isn't, try an immediate reconnect
            this.connected = false;
            console.log('The client has disconnected!');
            this.appController.prepareReset();
        }
    };

    errorEvent = (error : ErrorEvent) => {
        console.log('Connection error: ' + JSON.stringify(error));
    };

    getIsConnected = () : boolean => {
        return this.connected;
    };

    //This will be the heartbeat to detect dropped connections
    //Give a heartbeat of 10 seconds after expected ping interval
    pongEvent = (milliseconds : number) => {
        this.pingTime = milliseconds;
        this.pingTimeArray.push(this.pingTime);
        if (this.pingTimeArray.length > 3) {
            this.pingTimeArray.shift();
        }
    };

    getPing() : number {
        if (this.pingTimeArray.length === 0) {
            return this.pingTime;
        }
        let average = 0;
        for (let ping of this.pingTimeArray) {
            average += ping;
        }
        average = average / this.pingTimeArray.length;
        return (this.pingTime + average) / 2.0;
    }

    getAppController() : AppController {
        return this.appController;
    }

    gotMessageEvent = (event : MessageEvent) => {
        let messageData = event.data;

        if (typeof messageData === 'string') {
            console.log('Got string message: ' + messageData);
            this.appController.Message(JSON.parse(messageData));
            return;
        } else if (messageData instanceof ArrayBuffer === false) {
            console.error('Invalid Message Type Not Binary');
            console.trace();
            return;
        }
        this.appController.BinaryMessage(new MessageReader(messageData));
        //this.messageRouter.handleMessageEvent(messageData);

        /*
        //Handle test message
        document.body.innerText += ("\n" + "Bytes: " + messageData.byteLength);
        var enc = new TextDecoder("utf-8");
        document.body.innerText +=("\n" + "Got Message! : new byte[] { " + messageData + " } = " + enc.decode(messageData));
        document.body.innerText +=("\n" + "Parsed message: ");
        var message = new MessageReader(messageData);
        document.body.innerText +=("\n" + "GetUint8: " + message.getUint8());
        document.body.innerText +=("\n" + "GetInt8: " + message.getInt8());
        document.body.innerText +=("\n" + "GetUint16: " + message.getUint16());
        document.body.innerText +=("\n" + "GetInt16: " + message.getInt16());
        document.body.innerText +=("\n" + "GetUint32: " + message.getUint32());
        document.body.innerText +=("\n" + "GetInt32: " + message.getInt32());
        document.body.innerText +=("\n" + "GetFloat32: " + message.getFloat32());
        document.body.innerText +=("\n" + "GetFloat64: " + message.getFloat64());
        document.body.innerText +=("\n" + "GetString: " + message.getString());
        var message2 = new MessageReader(message.getBinary());
        document.body.innerText +=("\n" + "GetBinary GetString: " + message2.getString());
        if (message.isAtEndOfData()) {
            document.body.innerText +=("\n" + "End of Message");
        }
        */
    };

    send(msg : {}) {
        this.connection.send(JSON.stringify(msg));
    }

    sendBinary(msg : ArrayBuffer) {
        this.connection.send(msg);
    }
}