import './ThreadController.scss';

import {ThreadInfo} from "./ThreadInfo";
import {Section} from "./Section";
import {Interface} from "../../Utility/Interface";
import {CommentInfo} from "./CommentInfo";
import {Utility} from "../../Utility/Utility";
import {MessageWriter} from "../../Utility/Message/MessageWriter";
import {SendMessages} from "../../Networking/MessageDefinitions/SendMessages";

type User = { connectionId: string, accountId?: string, displayName?: string };

export class ThreadController {
    threads: ThreadInfo[];
    sectionController: Section;
    mainView: HTMLDivElement;
    headerView: HTMLDivElement;
    fullView: HTMLDivElement;
    newPostButton: HTMLButtonElement;
    backButton: HTMLButtonElement;
    viewingPosition = 0;
    viewingThread: ThreadInfo = null;
    sectionViewingUsers: User[] = [];
    sectionViewersElement: HTMLDivElement;
    threadHeaderTitle: HTMLSpanElement;
    hasDarkTheme: boolean;

    constructor(c: Section, title: string, darkTheme: boolean) {
        this.sectionController = c;
        this.threads = [];
        this.mainView = Interface.Create({
            type: 'div', className: 'ThreadControllerView', elements: [
                this.headerView = Interface.Create({
                    type: 'div', className: 'ThreadHeaderView', elements: [
                        this.threadHeaderTitle = Interface.Create({
                            type: 'span',
                            className: 'ThreadHeaderTitle',
                            text: title
                        }),
                        this.sectionViewersElement = Interface.Create({
                            type: 'span',
                            className: 'SectionViewers',
                            text: ""
                        })
                    ]
                })
            ]
        });
        this.hasDarkTheme = darkTheme;
        if (darkTheme) {
            this.threadHeaderTitle.classList.add('DarkTheme');
            this.sectionViewersElement.classList.add('DarkTheme');
        } else {
            this.threadHeaderTitle.classList.add('LightTheme');
            this.sectionViewersElement.classList.add('LightTheme');
        }

        this.fullView = Interface.Create({type: 'div', className: 'FullView'});
        this.newPostButton = Interface.Create({
            type: 'button',
            className: 'NewPostButton',
            text: 'Create New Post',
            onClick: this.sectionController.newPostButtonClicked
        });
        this.backButton = Interface.Create({
            type: 'button',
            className: 'BackButton',
            onClick: this.backButtonClicked,
            text: 'Back'
        });
    }


    loggedInEvent = () => {
        this.headerView.appendChild(this.newPostButton);
        for (let i = 0; i < this.threads.length; i++) {
            let t: ThreadInfo = this.threads[i];
            t.fullView.main.appendChild(t.fullView.commentButtonAndBox);
        }
    };

    loggedOutEvent = () => {
        this.newPostButton.remove();
        for (let i = 0; i < this.threads.length; i++) {
            let t: ThreadInfo = this.threads[i];
            t.fullView.commentButtonAndBox.remove();
        }
    };

    restoreToDefaultState = () => {
        this.fullView.remove();
        Utility.ClearElements(this.fullView);
        this.mainView.appendChild(this.headerView);
        this.backButton.remove();
        this.setViewingThread(null);
    };

    threadClicked = (thread: ThreadInfo) => {
        this.headerView.remove();
        this.mainView.appendChild(this.fullView);
        this.fullView.appendChild(thread.fullView.getDiv());
        this.mainView.appendChild(this.backButton);
        this.setViewingThread(thread);
    };

    setViewingThread = (thread: ThreadInfo) => {
        if (this.viewingThread != null) {
            let message = new MessageWriter();
            message.addUint8(SendMessages.Controller.Thread);
            message.addString(this.sectionController.sectionId);
            message.addString(this.viewingThread.getThreadId());
            message.addUint8(SendMessages.ThreadMessage.ExitViewingThread);
            this.sectionController.website.networkController.send(message.toBuffer());
        }
        this.viewingThread = thread;
        if (this.viewingThread != null) {
            let message = new MessageWriter();
            message.addUint8(SendMessages.Controller.Thread);
            message.addString(this.sectionController.sectionId);
            message.addString(this.viewingThread.getThreadId());
            message.addUint8(SendMessages.ThreadMessage.BeginViewingThread);
            this.sectionController.website.networkController.send(message.toBuffer());
        }
    };

    backButtonClicked = () => {
        this.restoreToDefaultState();
    };

    updateThreadPositions = () => {
        // Sort the threads by last update time
        this.threads.sort((a, b) => Utility.TimeuuidToMilliseconds(b.getUpdatedTime()) - Utility.TimeuuidToMilliseconds(a.getUpdatedTime()));
        for (let i = 0; i < this.threads.length; i++) {
            let t: ThreadInfo = this.threads[i];
            t.headerView.getDiv().style.top = `${i * 85 + 60}px`;
        }
    };

    getThread(id: string): ThreadInfo {
        for (let t of this.threads) {
            if (t.getThreadId() == id) return t;
        }
        return null;
    }

    showThread = (threadID: string) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            this.threadClicked(thread);
        }
    };

    //Actions
    clearAllThreads = () => {
        for (let i = 0; i < this.threads.length; i++) {
            let t: ThreadInfo = this.threads[i];
            t.headerView.getDiv().remove();
            t.fullView.getDiv().remove();
        }
        this.threads = [];
        this.setViewingThread(null);
    };

    addThread = (threadId: string,
                 creatorAccountId: string,
                 title: string,
                 description: string,
                 createdTime: string,
                 updatedTime: string,
                 commentCount: number,
                 creatorDisplayName: string,
                 creatorAvatarUrl: string) => {
        if (this.getThread(threadId) == null) {
            let thread: ThreadInfo = new ThreadInfo(this, this.hasDarkTheme);
            thread.setThreadId(threadId);
            this.threads.push(thread);

            thread.setCreatorAccountId(creatorAccountId);
            thread.setAvatarURL(creatorAvatarUrl);
            thread.setTitle(title);
            thread.setDescription(description)
            thread.setCreatedTime(createdTime);
            thread.setUpdatedTime(updatedTime);
            thread.setCommentCount(commentCount);
            thread.setCreatorDisplayName(creatorDisplayName);
            thread.headerView.getDiv().style.opacity = "1.0";
            thread.headerView.getDiv().style.top = `${(this.threads.length - 1) * 85 + 60}px`;
            this.headerView.appendChild(thread.headerView.getDiv());

            this.updateThreadPositions();
        }
    };

    removeThread = (threadID: string) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            this.threads.splice(this.threads.indexOf(thread), 1);
            thread.headerView.getDiv().style.opacity = "1.0";
            thread.headerView.getDiv().remove();
            thread.fullView.getDiv().style.opacity = "1.0";
            thread.fullView.getDiv().remove();
            this.updateThreadPositions();
            if (thread == this.viewingThread) {
                this.restoreToDefaultState();
            }
        }
    };

    updateThread = (threadID: string, title: string, description: string) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            thread.setTitle(title);
            thread.setDescription(description);
        }
    };

    updateCommentCount = (threadId: string, commentCount: number) => {
        let thread: ThreadInfo = this.getThread(threadId);
        if (thread != null) {
            thread.setCommentCount(commentCount);
        }
    };

    updateUpdatedTime = (threadId: string, updatedTime: string) => {
        let thread: ThreadInfo = this.getThread(threadId);
        if (thread != null) {
            thread.setUpdatedTime(updatedTime);
            this.updateThreadPositions();
        }
    }

    deleteComment = (threadID: string, commentID: string) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            thread.removeComment(commentID);
        }
    };

    updateComment = (threadID: string, commentID: string, comment: string) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            let ci: CommentInfo = thread.getComment(commentID);
            ci.setComment(comment);
        }
    };

    updateSectionViewersDisplay = () => {
        let currentGuestsOnline = 0;
        let namesOnline = [];
        for (let user of this.sectionViewingUsers) {
            if (user.displayName == undefined) {
                currentGuestsOnline += 1;
            } else {
                namesOnline.push(user.displayName);
            }
        }
        let onlineText = "Currently viewing: ";
        if (namesOnline.length > 0) {
            onlineText += namesOnline.join(", ");
            if (currentGuestsOnline > 0) {
                onlineText += ` and ${currentGuestsOnline} Guest`;
            }
        } else if (currentGuestsOnline > 0) {
            onlineText += `${currentGuestsOnline} Guest`;
        }
        if (currentGuestsOnline > 1) {
            onlineText += "s"
        }
        this.sectionViewersElement.innerText = onlineText;
    }
}