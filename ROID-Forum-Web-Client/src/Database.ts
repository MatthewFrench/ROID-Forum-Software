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
            localStorage['Name'] = n;
            localStorage['Password'] = p;

            this.loggedIn = true;
            this.name = n;
            this.password = p;
        }
        break;
    case "Logged Out":
        {
            //Clear local storage
            let localStorage : Storage = window.localStorage;
            localStorage.remove('Name');
            localStorage.remove('Password');
            this.name = "";
            this.password = "";
            this.loggedIn = false;
        }
        break;
    }
}
}