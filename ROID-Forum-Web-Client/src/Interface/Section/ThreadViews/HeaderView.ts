import './HeaderView.scss';

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

    constructor(t: ThreadInfo, tc: ThreadController) {
        this.thread = t;
        this.threadController = tc;

        this.headerDiv = Interface.Create({type: 'div', className: 'HeaderView', onClick: this.onClick, elements: [
            this.threadHeaderTitle = Interface.Create({type: 'span', className: 'HeaderTitle'}),
            this.threadHeaderOwner = Interface.Create({type: 'span', className: 'HeaderOwner'}),
            this.threadHeaderComment = Interface.Create({type: 'span', className: 'HeaderCommentCount'}),
            this.threadHeaderImage = Interface.Create({type: 'img', className: 'HeaderOwnerAvatarImage'})
        ]});
    }

    getDiv(): HTMLDivElement {
        return this.headerDiv;
    }

    onClick = () => {
        this.threadController.threadClicked(this.thread);
    };

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
}