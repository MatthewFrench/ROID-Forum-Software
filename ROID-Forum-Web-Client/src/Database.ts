import {AppController} from "./AppController";

export class Database {
    appController : AppController;
    loggedIn = false;
    accountId = "";
    displayName = "";
    //password = "";
    constructor(appController : AppController) {
        this.appController = appController;
    }
    public processEvent(event : String, message : any) {
    switch (event) {
    case "Logged In":
        {
            this.displayName = message.getString();
            this.accountId = message.getString();
            this.loggedIn = true;
        }
        break;
    case "Logged Out":
        {
            //Clear local storage
            let localStorage : Storage = window.localStorage;
            localStorage.removeItem('Name');
            localStorage.removeItem('Password');
            this.displayName = "";
            //this.password = "";
            this.loggedIn = false;
        }
        break;
    }
}
}