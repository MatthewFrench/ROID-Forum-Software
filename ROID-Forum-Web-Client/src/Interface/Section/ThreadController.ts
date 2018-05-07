import './ThreadController.scss';

import {ThreadInfo} from "./ThreadInfo";
import {Section} from "./Section";
import {Interface} from "../../Utility/Interface";
import {CommentInfo} from "./CommentInfo";
import {Utility} from "../../Utility/Utility";

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
    threadHeaderTitle : HTMLSpanElement;
    hasDarkTheme : boolean;

    constructor(c: Section, title : string, darkTheme : boolean) {
        this.sectionController = c;
        this.threads = [];
        this.mainView = Interface.Create({type: 'div', className: 'ThreadControllerView', elements: [
            this.headerView = Interface.Create({type: 'div', className: 'ThreadHeaderView', elements: [
                this.threadHeaderTitle = Interface.Create({type: 'span', className: 'ThreadHeaderTitle', text: title})
            ]})
        ]});
        this.hasDarkTheme = darkTheme;
        if (darkTheme) {
            this.threadHeaderTitle.classList.add('DarkTheme');
        } else {
            this.threadHeaderTitle.classList.add('LightTheme');
        }

        this.fullView = Interface.Create({type: 'div', className: 'FullView'});
        this.newPostButton = Interface.Create({type: 'button', className: 'NewPostButton', text: 'Create New Post', onClick: this.sectionController.newPostButtonClicked});
        this.backButton = Interface.Create({type: 'button', className: 'BackButton', onClick: this.backButtonClicked, text: 'Back'});
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
        this.viewingThread = null;
    };

    threadClicked = (thread: ThreadInfo) => {
        this.headerView.remove();
        this.mainView.appendChild(this.fullView);
        this.fullView.appendChild(thread.fullView.getDiv());
        this.mainView.appendChild(this.backButton);
        this.viewingThread = thread;
    };

    backButtonClicked = () => {
        this.restoreToDefaultState();
    };

    updateThreadPositions = () => {
        for (let i = 0; i < this.threads.length; i++) {
            let t: ThreadInfo = this.threads[i];
            t.headerView.getDiv().style.top = `${i * 85 + 60}px`;
        }
    };

    getThread(id: number): ThreadInfo {
        for (let t of this.threads) {
            if (t.getID() == id) return t;
        }
        return null;
    }

    showThread = (threadID: number) => {
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
    };

    addThread = (threadMap: any) => {
        let id: number = threadMap["ID"];
        if (this.getThread(id) == null) {
            let thread: ThreadInfo = new ThreadInfo(this, this.hasDarkTheme);
            thread.setID(id);
            this.threads.push(thread);

            thread.setOwner(threadMap["Owner"]);
            if (threadMap["AvatarURL"] != null) {
                thread.setAvatarURL(threadMap["AvatarURL"]);
            }
            thread.setTitle(threadMap["Title"]);
            thread.setDescription(threadMap["Description"]);

            let commentArray: any[] = threadMap["Comments"];
            for (let i = 0; i < commentArray.length; i++) {
                let c: any = commentArray[i];
                let cf: CommentInfo = new CommentInfo(thread, this, this.hasDarkTheme);
                thread.addComment(cf);
                console.log(`Loaded comment with threadID: ${c['ThreadID']} and commentID: ${c['CommentID']}`);
                cf.setThreadID(c['ThreadID']);
                cf.setCommentID(c['CommentID']);
                cf.setComment(c['Comment']);
                cf.setOwner(c['Owner']);
                if (c["AvatarURL"] != null) {
                    cf.setAvatarURL(c["AvatarURL"]);
                }
            }
            thread.headerView.getDiv().style.opacity = "1.0";
            thread.headerView.getDiv().style.top = `${(this.threads.length - 1) * 85 + 60}px`;
            this.headerView.appendChild(thread.headerView.getDiv());

            this.updateThreadPositions();
        }
    };

    removeThread = (threadID: number) => {
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

    updateThread = (threadID: number, title: string, description: string) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            thread.setTitle(title);
            thread.setDescription(description);
        }
    };

    moveThreadToTop = (threadID: number) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            this.threads.splice(this.threads.indexOf(thread), 1);
            this.threads.splice(0, 0, thread);
            this.updateThreadPositions();
        }
    };

    addComment = (commentMap: any) => {
        let threadID: number = commentMap["ThreadID"];
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            let cf: CommentInfo = new CommentInfo(thread, this, this.hasDarkTheme);
            thread.addComment(cf);
            cf.setThreadID(commentMap['ThreadID']);
            cf.setCommentID(commentMap['CommentID']);
            cf.setComment(commentMap['Comment']);
            cf.setOwner(commentMap['Owner']);
            cf.setAvatarURL(commentMap["AvatarURL"]);
        }
    };

    deleteComment = (threadID: number, commentID: number) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            thread.removeComment(commentID);
        }
    };

    updateComment = (threadID: number, commentID: number, comment: string) => {
        let thread: ThreadInfo = this.getThread(threadID);
        if (thread != null) {
            let ci: CommentInfo = thread.getComment(commentID);
            ci.setComment(comment);
        }
    };
}