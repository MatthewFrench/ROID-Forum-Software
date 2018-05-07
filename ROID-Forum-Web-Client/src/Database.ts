import {AppController} from "./AppController";

export class Database {
    appController : AppController;
    loggedIn = false;
    name = "";
    password = "";
    constructor(appController : AppController) {
        this.appController = appController;
    }
    public processEvent(event : String, message : any) {
    switch (event) {
    case "Logged In":
        {
            let n : string = message['Name'];
            let p : string = message['Password'];

            //Save name and password in local storage
            let localStorage : Storage = window.localStorage;
            localStorage.setItem('Name', n);
            localStorage.setItem('Password', p);

            this.loggedIn = true;
            this.name = n;
            this.password = p;
        }
        break;
        case "Logged In Binary":
        {
            let n : string = message.getString();
            let p : string = message.getString();

            //Save name and password in local storage
            let localStorage : Storage = window.localStorage;
            localStorage.setItem('Name', n);
            localStorage.setItem('Password', p);

            this.loggedIn = true;
            this.name = n;
            this.password = p;
        }
            break;
    case "Logged Out":
        {
            //Clear local storage
            let localStorage : Storage = window.localStorage;
            localStorage.removeItem('Name');
            localStorage.removeItem('Password');
            this.name = "";
            this.password = "";
            this.loggedIn = false;
        }
        break;
    }
}
}