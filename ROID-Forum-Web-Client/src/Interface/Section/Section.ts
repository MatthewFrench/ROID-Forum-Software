import './Section.scss';

import {AppController} from "../../AppController";
import {NewPostWindow} from "./NewPostWindow";
import {ThreadController} from "./ThreadController";
import {MatrixBackground} from "./MatrixBackground";
import {Interface} from "../../Utility/Interface";

export class Section {
    website: AppController;
    content: HTMLDivElement;
    name = '';
    displayName = '';
    newPostWindow: NewPostWindow = null;
    threadController: ThreadController;
    showThreadWhenLoaded = -1;
    background: MatrixBackground;
    hasDarkTheme = false;

    constructor({appController, darkTheme = false, hasMatrixBackground = false, name, displayName, title} :
                    {appController : AppController, darkTheme? : boolean,
                        hasMatrixBackground? : boolean, name: string, displayName: string, title: string}) {
        this.name = name;
        this.displayName = displayName;
        this.hasDarkTheme = darkTheme;
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
        let m: any = {};
        m['Controller'] = "Server";
        m['Title'] = 'Viewing';
        m['Section'] = this.name;
        this.website.networkController.send(m);

        this.threadController.restoreToDefaultState();

        if (this.background != null) {
            this.background.show();
        }

        if (this.hasDarkTheme) {
            this.website.chatbox.darkTheme();
            this.website.mainSection.darkTheme();
            this.website.controlPanel.darkTheme();
        } else {
            this.website.chatbox.lightTheme();
            this.website.mainSection.lightTheme();
            this.website.controlPanel.lightTheme();
        }
    };

    hide = () => {
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

    showThread = (threadID: number) => {
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

    onMessage(message: any) {
        switch (message['Title']) {
            case "All Threads": {
                this.threadController.clearAllThreads();
                let threads: any[] = message['Threads'];
                for (let i = 0; i < threads.length; i++) {
                    this.threadController.addThread(threads[i]);
                }
                if (this.showThreadWhenLoaded != -1) {
                    this.threadController.showThread(this.showThreadWhenLoaded);
                    //console.log("Told to show thread ${this.showThreadWhenLoaded}");
                    this.showThreadWhenLoaded = -1;
                }
            }
                break;
            case "Thread Add": {
                this.threadController.addThread(message["Thread Map"]);
            }
                break;
            case "Thread Remove": {
                this.threadController.removeThread(message["ThreadID"]);
            }
                break;
            case "Thread Update": {
                this.threadController.updateThread(message["ThreadID"], message["Thread Title"], message["Thread Description"]);
            }
                break;
            case "Thread Move To Top": {
                this.threadController.moveThreadToTop(message["ThreadID"]);
            }
                break;
            case "Comment Add": {
                this.threadController.addComment(message["Comment"]);
            }
                break;
            case "Comment Delete": {
                this.threadController.deleteComment(message["ThreadID"], message["CommentID"]);
            }
                break;
            case "Comment Update": {
                this.threadController.updateComment(message["ThreadID"], message["CommentID"], message["Comment"]);
            }
                break;
        }
    }

    getDiv(): HTMLDivElement {
        return this.content;
    }

    getName(): string {
        return this.name;
    }

    getDisplayName(): string {
        return this.displayName;
    }
}
