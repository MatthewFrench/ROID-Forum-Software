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
    commentViews: CommentView[];

    constructor(t: ThreadInfo, tc: ThreadController) {
        this.threadController = tc;
        this.thread = t;
        this.threadDisplay = new ThreadDisplay(this);

        this.makeMainComponent();
        this.makeComments();
        this.makeCommentBoxComponent();
        this.makeCommentButtonComponent();

        this.commentViews = [];
    }

    sendComment() {
        let text = this.commentBox.value;
        let m: any = {};
        m['Controller'] = this.threadController.sectionController.name;
        m['Title'] = 'Add Comment';
        m['ID'] = this.thread.getID();
        m['Text'] = text;
        this.threadController.sectionController.website.networkingController.Send(m);
        this.commentBox.value = "";
    }

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

    //GUI Components
    makeMainComponent() {
        this.main = Interface.Create({type: 'div'});
        this.main.appendChild(this.threadDisplay.getDiv());
    }

    makeComments() {
        this.commentContainer = Interface.Create({type: 'div'});
        this.commentContainer.style.color = "white";
        this.main.appendChild(this.commentContainer);
    }

    makeCommentBoxComponent() {
        this.commentButtonAndBox = Interface.Create({type: 'div'});
        if (this.threadController.sectionController.website.database.loggedIn) {
            this.main.appendChild(this.commentButtonAndBox);
        }

        this.commentBox = Interface.Create({type: 'textarea'});
        this.commentBox.placeholder = "Comment...";
        this.commentBox.style.cssFloat = 'left';
        this.commentBox.style.width = 'calc(100% - 100px)';
        this.commentBox.style.height = '200px';
        this.commentBox.style.marginLeft = '70px';
        this.commentBox.style.marginTop = '10px';
        this.commentBox.style.resize = 'none';
        this.commentButtonAndBox.appendChild(this.commentBox);
    }

    makeCommentButtonComponent() {
        this.commentButton = Interface.Create({type: 'button'});
        this.commentButton.innerText = "Comment";
        this.commentButton.style.cssFloat = "left";
        this.commentButton.style.marginLeft = '70px';
        this.commentButton.style.width = "100px";
        this.commentButtonAndBox.appendChild(this.commentButton);
        this.commentButton.onclick = () => {
            this.sendComment();
        };
    }
}