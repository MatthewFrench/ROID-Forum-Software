import './FullView.scss';

import {ThreadInfo} from "../ThreadInfo";
import {ThreadController} from "../ThreadController";
import {ThreadDisplay} from "./ThreadDisplay";
import {CommentView} from "./CommentView";
import {CommentInfo} from "../CommentInfo";
import {Interface} from "../../../Utility/Interface";

export class FullView {
    thread: ThreadInfo;
    threadController: ThreadController;
    threadDisplay: ThreadDisplay;
    main: HTMLDivElement;
    commentBox: HTMLTextAreaElement;
    commentButton: HTMLButtonElement;
    commentButtonAndBox: HTMLDivElement;
    commentContainer: HTMLDivElement;
    commentViews: CommentView[] = [];

    constructor(t: ThreadInfo, tc: ThreadController) {
        this.threadController = tc;
        this.thread = t;

        this.main = Interface.Create({
            type: 'div', className: 'FullView', elements: [
                (this.threadDisplay = new ThreadDisplay(this)).getDiv(),
                this.commentContainer = Interface.Create({type: 'div', className: 'CommentContainer'})
            ]
        });

        this.commentButtonAndBox = Interface.Create({
            type: 'div', className: 'CommentBox', elements: [
                this.commentBox = Interface.Create({
                    type: 'textarea',
                    placeholder: 'Comment...',
                    className: 'CommentTextBox'
                }),
                this.commentButton = Interface.Create({
                    type: 'button', text: 'Comment',
                    className: 'CommentButton', onClick: this.sendComment
                })
            ]
        });

        if (this.threadController.sectionController.website.database.loggedIn) {
            this.main.appendChild(this.commentButtonAndBox);
        }
    }

    sendComment = () => {
        let text = this.commentBox.value;
        let m: any = {};
        m['Controller'] = this.threadController.sectionController.name;
        m['Title'] = 'Add Comment';
        m['ID'] = this.thread.getID();
        m['Text'] = text;
        this.threadController.sectionController.website.networkController.send(m);
        this.commentBox.value = "";
    };

    getDiv(): HTMLDivElement {
        return this.main;
    }

    addComment(ci: CommentInfo) {
        this.commentViews.push(ci.commentView);
        this.commentContainer.appendChild(ci.commentView.getDiv());
        //ci.commentView.getDiv().style.opacity = "0.01";
        //animate(ci.commentView.getDiv(), duration: 250, properties: {'opacity': 1.0});
    }

    removeComment(ci: CommentInfo) {
        this.commentViews.splice(this.commentViews.indexOf(ci.commentView), 1);
        ci.commentView.getDiv().remove();
    }
}