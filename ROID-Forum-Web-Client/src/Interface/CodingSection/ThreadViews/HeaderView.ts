import {ThreadInfo} from "../ThreadInfo";
import {ThreadController} from "../ThreadController";
import {Interface} from "../../../Utility/Interface";

export class HeaderView {
    headerDiv: HTMLDivElement;
    threadHeaderTitle: HTMLSpanElement;
    threadHeaderOwner: HTMLSpanElement;
    threadHeaderComment: HTMLSpanElement;
    threadHeaderImage: HTMLImageElement;
    thread: ThreadInfo;
    threadController: ThreadController;

    /**********Make GUI Components***********/
    constructor(t: ThreadInfo, tc: ThreadController) {
        this.thread = t;
        this.threadController = tc;
        this.makeThreadHeaderDivComponent();
        this.makeThreadHeaderTitleComponent();
        this.makeThreadHeaderOwnerComponent();
        this.makeThreadHeaderCommentComponent();
        this.makeThreadHeaderImageComponent();
    }

    getDiv(): HTMLDivElement {
        return this.headerDiv;
    }

    onClick() {
        this.threadController.threadClicked(this.thread);
    }

    updateTitle() {
        this.threadHeaderTitle.innerText = this.thread.getTitle();
    }

    updateOwner() {
        this.threadHeaderOwner.innerText = this.thread.getOwner();
    }

    updateAvatarURL() {
        this.threadHeaderImage.src = this.thread.getAvatarURL();
    }

    updateCommentCount() {
        this.threadHeaderComment.innerText = `Comments: ${this.thread.getCommentCount()}`;
    }

    makeThreadHeaderDivComponent() {
        this.headerDiv = Interface.Create({type: 'div'});
        this.headerDiv.style.position = "absolute";
        this.headerDiv.style.width = "calc(100% - 20px)";
        this.headerDiv.style.top = "0px";
        this.headerDiv.style.left = "10px";
        this.headerDiv.style.height = "80px";
        this.headerDiv.style.borderTopStyle = "solid";
        this.headerDiv.style.borderWidth = "1px";
        this.headerDiv.style.borderColor = "#AAAAAA";
        this.headerDiv.style.color = "white";
    }

    makeThreadHeaderTitleComponent() {
        this.threadHeaderTitle = Interface.Create({type: 'span'});
        this.threadHeaderTitle.style.position = "absolute";
        this.threadHeaderTitle.style.fontSize = "30px";
        this.threadHeaderTitle.style.left = "70px";
        this.threadHeaderTitle.style.top = "10px";
        this.threadHeaderTitle.style.textDecoration = "underline";
        this.threadHeaderTitle.style.textOverflow = "ellipsis";
        this.threadHeaderTitle.style.maxWidth = "calc(100% - 70px)";
        this.threadHeaderTitle.style.height = "30px";
        this.threadHeaderTitle.style.cursor = "pointer";
        this.threadHeaderTitle.style.whiteSpace = "nowrap";
        this.threadHeaderTitle.style.overflow = "hidden";
        this.threadHeaderTitle.style.paddingTop = "10px";
        this.threadHeaderTitle.style.color = "white";
        this.headerDiv.appendChild(this.threadHeaderTitle);
        this.threadHeaderTitle.onclick = () => {
            this.onClick();
        };
    }

    makeThreadHeaderOwnerComponent() {
        this.threadHeaderOwner = Interface.Create({type: 'span'});
        this.threadHeaderOwner.style.position = "absolute";
        this.threadHeaderOwner.style.fontSize = "15px";
        this.threadHeaderOwner.style.right = "10px";
        this.threadHeaderOwner.style.bottom = "10px";
        this.threadHeaderOwner.style.cursor = "pointer";
        this.threadHeaderOwner.style.color = "white";
        this.headerDiv.appendChild(this.threadHeaderOwner);
    }

    makeThreadHeaderCommentComponent() {
        this.threadHeaderComment = Interface.Create({type: 'span'});
        this.threadHeaderComment.style.position = "absolute";
        this.threadHeaderComment.style.fontSize = "15px";
        this.threadHeaderComment.style.left = "70px";
        this.threadHeaderComment.style.bottom = "10px";
        this.threadHeaderComment.style.cursor = "pointer";
        this.threadHeaderComment.style.color = "white";
        this.headerDiv.appendChild(this.threadHeaderComment);
        this.threadHeaderComment.onclick = () => {
            this.onClick();
        };
    }

    makeThreadHeaderImageComponent() {
        this.threadHeaderImage = Interface.Create({type: 'img'});
        this.threadHeaderImage.style.position = "absolute";
        this.threadHeaderImage.style.left = "0px";
        this.threadHeaderImage.style.top = "10px";
        this.threadHeaderImage.style.height = "60px";
        this.threadHeaderImage.style.width = "60px";
        this.threadHeaderImage.style.backgroundColor = "rgb(200,200,200)";
        this.threadHeaderImage.style.borderStyle = "solid";
        this.threadHeaderImage.style.borderWidth = "1px";
        this.headerDiv.appendChild(this.threadHeaderImage);
    }
}
