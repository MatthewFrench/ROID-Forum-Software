import {AppController} from "../AppController";
import {MessageReader} from "../Utility/Message/MessageReader";

export class NetworkController {
    port : string = '7779';
    // Todo: Switch this to a configuration to automatically run localhost for local dev
    connection : WebSocket = null;
    connected : boolean = false;
    pingTime : number = 0;
    pingTimeArray : number[] = [];
    appController : AppController;

    constructor(appController : AppController) {
        this.appController = appController;
    }

    initialize = () => {
        // Create WebSocket connection.
        if (window.location.hostname === 'localhost') {
            this.connection = new WebSocket(`ws://localhost:${this.port}`);
        } else {
            this.connection = new WebSocket(`wss://server.matthewfrench.io:${this.port}`);
        }
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
        this.appController.connectedToServer();
    };

    disconnectedEvent = (close : CloseEvent) => {
        if (this.connected == false) {
            //Try to reconnect at an interval
            setTimeout(()=>{this.appController.prepareReset();}, 5 * 1000);
        } else {
            //Was just connected and now isn't, try an immediate reconnect
            this.connected = false;
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

        if (!(messageData instanceof ArrayBuffer)) {
            console.error('Invalid Message Type Not Binary');
            console.trace();
            return;
        }
        this.appController.Message(new MessageReader(messageData));
    };

    send(message : ArrayBuffer) {
        this.connection.send(message);
    }
}