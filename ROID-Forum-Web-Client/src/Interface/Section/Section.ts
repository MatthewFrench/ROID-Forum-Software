import './Section.scss';

import {AppController} from "../../AppController";
import {NewPostWindow} from "./NewPostWindow";
import {ThreadController} from "./ThreadController";
import {MatrixBackground} from "./MatrixBackground";
import {Interface} from "../../Utility/Interface";
import {MessageWriter} from "../../Utility/Message/MessageWriter";
import {SendMessages} from "../../Networking/MessageDefinitions/SendMessages";

export class Section {
    website: AppController;
    content: HTMLDivElement;
    sectionId = '';
    displayName = '';
    newPostWindow: NewPostWindow = null;
    threadController: ThreadController;
    showThreadWhenLoaded: string = null;
    background: MatrixBackground;
    hasDarkTheme = false;
    createdTime = '';

    constructor({appController, darkTheme = false, hasMatrixBackground = false, sectionId, displayName, title, createdTime} :
                    {appController : AppController, darkTheme? : boolean,
                        hasMatrixBackground? : boolean, sectionId: string, displayName: string, title: string, createdTime: string}) {
        this.sectionId = sectionId;
        this.displayName = displayName;
        this.hasDarkTheme = darkTheme;
        this.createdTime = createdTime;
        this.website = appController;

        this.content = Interface.Create({type: 'div', className: 'Section', elements: [
            (this.threadController = new ThreadController(this, title, this.hasDarkTheme)).mainView
        ]});

        this.newPostWindow = new NewPostWindow(this);
        if (hasMatrixBackground) {
            this.background = new MatrixBackground(this);
        }
    }

    show = () => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Section);
        message.addString(this.sectionId);
        message.addUint8(SendMessages.SectionMessage.BeginViewingSection);
        this.website.networkController.send(message.toBuffer());

        this.threadController.restoreToDefaultState();

        if (this.background != null) {
            this.background.show();
        }

        if (this.hasDarkTheme) {
            this.website.darkTheme();
            this.website.chatbox.darkTheme();
            this.website.mainSection.darkTheme();
            this.website.controlPanel.darkTheme();
        } else {
            this.website.lightTheme();
            this.website.chatbox.lightTheme();
            this.website.mainSection.lightTheme();
            this.website.controlPanel.lightTheme();
        }
    };

    hide = () => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Section);
        message.addString(this.sectionId);
        message.addUint8(SendMessages.SectionMessage.ExitViewingSection);
        this.website.networkController.send(message.toBuffer());
        //Destroy all posts
        this.threadController.clearAllThreads();

        if (this.background != null) {
            this.background.hide();
        }

        this.website.chatbox.lightTheme();
        this.website.mainSection.lightTheme();
        this.website.controlPanel.lightTheme();
    };

    logic = () => {
        if (this.background != null) {
            this.background.logic();
        }
    };

    showThread = (threadID: string) => {
        this.showThreadWhenLoaded = threadID;
    };

    processEvent = (event: string) => {
        switch (event) {
            case "Logged In": {
                this.threadController.loggedInEvent();
            }
                break;
            case "Logged Out": {
                this.threadController.loggedOutEvent();
            }
                break;
        }
    };

    newPostButtonClicked = () => {
        this.newPostWindow.show();
    };

    addThread = (
        threadId: string,
        creatorAccountId: string,
        title: string,
        description: string,
        createdTime: string,
        updatedTime: string,
        commentCount: number,
        creatorDisplayName: string,
        creatorAvatarUrl: string) => {
        this.threadController.addThread(
            threadId,
            creatorAccountId,
            title,
            description,
            createdTime,
            updatedTime,
            commentCount,
            creatorDisplayName,
            creatorAvatarUrl
        );
    }

    getDiv(): HTMLDivElement {
        return this.content;
    }

    getId(): string {
        return this.sectionId;
    }

    getDisplayName(): string {
        return this.displayName;
    }
}
