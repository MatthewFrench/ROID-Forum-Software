import {Interface} from "./Utility/Interface";
import {MainPage} from "./Interface/MainPage";
import {NetworkController} from "./Networking/NetworkController";

export class AppController {
    mainDiv : HTMLDivElement;
    mainPage : MainPage;
    networkController : NetworkController;

    constructor() {
        this.networkController = new NetworkController(this);
        this.mainPage = new MainPage();
        this.mainDiv = Interface.Create({type: 'div', className: 'ApplicationDiv', elements: [
            this.mainPage.getInterface()
        ]});
        this.networkController.initialize();
    }

    getInterface() {
        return this.mainDiv;
    }
}