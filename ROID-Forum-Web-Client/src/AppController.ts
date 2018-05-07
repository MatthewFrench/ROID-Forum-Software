import {Interface} from "./Utility/Interface";
import {NetworkController} from "./Networking/NetworkController";
import {Database} from "./Database";
import {ControlPanel} from "./Interface/ControlPanel";
import {Chatbox} from "./Interface/Chatbox";
import {MainTopBarSection} from "./Interface/MainTopBarSection";
import {Section} from "./Interface/Section/Section";
import {Utility} from "./Utility/Utility";

export class AppController {
    mainDiv: HTMLDivElement;
    networkController: NetworkController;
    websiteDiv: HTMLDivElement;
    aheadWebsiteDiv: HTMLDivElement;
    behindWebsiteDiv: HTMLDivElement;
    database: Database;
    controlPanel: ControlPanel;
    logicTimer: any;
    chatbox: Chatbox;
    mainSection: MainTopBarSection;
    allSection: Section;
    codingSection: Section;
    gameSection: Section;
    graphicsSection: Section;
    otherSection: Section;
    sectionOrder: Section[];
    showingSection: Section = null;
    body: HTMLBodyElement;
    reconnectingDiv: HTMLDivElement;

    constructor() {
        this.mainDiv = Interface.Create({
            type: 'div', className: 'ApplicationDiv', elements: [
                this.behindWebsiteDiv = Interface.Create({type: 'div', className: 'behindWebsite'}),
                this.websiteDiv = Interface.Create({type: 'div', className: 'Website'}),
                this.aheadWebsiteDiv = Interface.Create({type: 'div', className: 'aheadWebsite'})
            ]
        });
        this.body = document.body as HTMLBodyElement;

        this.websiteDiv.style.width = "100%";
        this.websiteDiv.style.height = "100%";
        this.websiteDiv.style.overflow = "scroll";

        this.logicTimer = setInterval(this.logic, 16);

        this.setUp();
        this.networkController.initialize();
    }

    setUp() {
        this.networkController = new NetworkController(this);
        this.database = new Database(this);
        this.controlPanel = new ControlPanel(this);
        //Put the chat on the right side
        this.chatbox = new Chatbox(this);
        //Lets make the visual stuff
        this.allSection = new Section(this);
        this.codingSection = new Section(this);
        this.gameSection = new Section(this);
        this.graphicsSection = new Section(this);
        this.otherSection = new Section(this);
        this.sectionOrder = [this.allSection, this.codingSection, this.gameSection, this.graphicsSection, this.otherSection];
        //Now add the main parts of the site
        this.mainSection = new MainTopBarSection(this);
    }

    logic = () => {
        this.mainSection.logic();
        this.codingSection.logic();
        this.allSection.logic();
        this.gameSection.logic();
        this.graphicsSection.logic();
        this.otherSection.logic();
    };

    prepareReset() {
        Utility.ClearElements(this.behindWebsiteDiv);
        Utility.ClearElements(this.websiteDiv);
        Utility.ClearElements(this.aheadWebsiteDiv);
        this.behindWebsiteDiv.hidden = true;
        this.websiteDiv.hidden = true;
        this.aheadWebsiteDiv.hidden = true;
        if (this.reconnectingDiv != null) {
            this.reconnectingDiv.remove();
            this.reconnectingDiv = null;
        }
        this.reconnectingDiv = Interface.Create({type: 'div'});
        this.reconnectingDiv.style.width = "100%";
        this.reconnectingDiv.style.height = "100%";
        this.reconnectingDiv.style.textAlign = "Center";
        this.reconnectingDiv.style.fontSize = "40px";
        this.reconnectingDiv.style.background = "white";
        this.reconnectingDiv.innerText = "Unable to connect...";
        this.body.appendChild(this.reconnectingDiv);
    }

    reset() {
        this.setUp();
        //console.log("Trying to reconnect");
        //networkingController = new NetworkingController(this);
    }


    connectedToServer() {
        this.behindWebsiteDiv.hidden = false;
        this.websiteDiv.hidden = false;
        this.aheadWebsiteDiv.hidden = false;
        if (this.reconnectingDiv != null) {
            this.reconnectingDiv.remove();
            this.reconnectingDiv = null;
        }
        //Try auto-login
        let localStorage: Storage = window.localStorage;
        if (localStorage.getItem('Name') != null) {
            this.controlPanel.loginAlreadyEncrypted(localStorage.getItem('Name'), localStorage.getItem('Password'));
        }
        //Load the All page
        this.mainSection.sectionClick(0);
    }

    showView(section: number) {
        if (this.showingSection != null) {
            this.showingSection.getDiv().style.opacity = "0.0";
            //animate(this.showingSection.getDiv(), duration: 250, properties: {
            //    'opacity': 0.0
            //}).onComplete.listen((_) {
            this.showingSection.getDiv().remove();
            this.showingSection.hide();
            this.showingSection = this.sectionOrder[section];
            this.showingSection.getDiv().style.opacity = "1.0";
            this.websiteDiv.appendChild(this.showingSection.getDiv());
            this.showingSection.show();
            //animate(this.showingSection.getDiv(), duration: 250, properties: {
            //    'opacity': 1.0
            //});
//});
        } else {
            this.showingSection = this.sectionOrder[section];
            this.showingSection.getDiv().style.opacity = "1.0";
            this.websiteDiv.appendChild(this.showingSection.getDiv());
            this.showingSection.show();
            //animate(this.showingSection.getDiv(), duration: 250, properties: {
            //    'opacity': 1.0
            //});
        }
    }

    goTo(section: string, threadID: number) {
        switch (section) {
            case "Coding Section": {
                this.mainSection.sectionClick(this.sectionOrder.indexOf(this.codingSection));
                this.codingSection.showThread(threadID);
            }
                break;
            case "Game Section": {
                this.mainSection.sectionClick(this.sectionOrder.indexOf(this.gameSection));
                this.gameSection.showThread(threadID);
            }
                break;
            case "Graphics Section": {
                this.mainSection.sectionClick(this.sectionOrder.indexOf(this.graphicsSection));
                this.graphicsSection.showThread(threadID);
            }
                break;
            case "Other Section": {
                this.mainSection.sectionClick(this.sectionOrder.indexOf(this.otherSection));
                this.otherSection.showThread(threadID);
            }
                break;
        }
    }

    Message(message: any) {
        if (message['Controller'] == 'Chat') {
            this.chatbox.onMessage(message);
        }
        if (message['Controller'] == 'Login') {
            switch (message['Title']) {
                case 'Logged In': {
                    this.database.processEvent('Logged In', message);
                    this.controlPanel.processEvent('Logged In');
                    this.chatbox.processEvent('Logged In');
                    for (let section of this.sectionOrder) section.processEvent('Logged In');
                }
                    break;
                case 'Logged Out': {
                    this.database.processEvent('Logged Out', message);
                    this.controlPanel.processEvent('Logged Out');
                    this.chatbox.processEvent('Logged Out');
                    for (let section of this.sectionOrder) section.processEvent('Logged Out');
                }
                    break;
                case 'Register Failed': {
                    this.controlPanel.processEvent('Register Failed');
                }
                    break;
                case 'Login Failed': {
                    this.controlPanel.processEvent('Login Failed');
                }
                    break;
                case 'Get Avatar': {
                    this.controlPanel.preferencesAvatarInput.value = message['AvatarURL'];
                }
                    break;
                default: {
                    console.log(`Got other message: ${message['Title']}`);
                }
            }
        }
        if (message['Controller'] == this.codingSection.name) {
            this.codingSection.onMessage(message);
        }
        if (message['Controller'] == this.allSection.name) {
            this.allSection.onMessage(message);
        }
        if (message['Controller'] == this.otherSection.name) {
            this.otherSection.onMessage(message);
        }
        if (message['Controller'] == this.graphicsSection.name) {
            this.graphicsSection.onMessage(message);
        }
        if (message['Controller'] == this.gameSection.name) {
            this.gameSection.onMessage(message);
        }
    }

    BinaryMessage(buffer: ArrayBuffer) {
    }

    getInterface() {
        return this.mainDiv;
    }

    getWebsiteDiv(): HTMLDivElement {
        return this.websiteDiv;
    }
}