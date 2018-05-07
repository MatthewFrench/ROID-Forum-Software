import './CommentView.scss';

import {ThreadInfo} from "../ThreadInfo";
import {CommentInfo} from "../CommentInfo";
import {ThreadController} from "../ThreadController";
import {DescriptionParser} from "../../../Utility/DescriptionParser";
import {Utility} from "../../../Utility/Utility";
import {Interface} from "../../../Utility/Interface";

export class CommentView {
    thread: ThreadInfo;
    comment: CommentInfo;
    threadController: ThreadController;
    main: HTMLDivElement;
    profileSection: HTMLDivElement;
    descriptionSection: HTMLDivElement;
    description: HTMLDivElement;
    image: HTMLImageElement;
    editDescription: HTMLTextAreaElement;
    editComment: HTMLButtonElement;
    saveEdit: HTMLButtonElement;
    deleteComment: HTMLButtonElement;
    owner: HTMLSpanElement;

    constructor(ci: CommentInfo, t: ThreadInfo, tc: ThreadController) {
        this.threadController = tc;
        this.thread = t;
        this.comment = ci;
        this.main = Interface.Create({type: 'div', className: 'CommentView', elements: [
            this.profileSection = Interface.Create({type: 'div', className: 'ProfileSection', elements: [
                this.image = Interface.Create({type: 'img', className: 'ProfileImage'}),
                this.owner = Interface.Create({type: 'span', className: 'ProfileOwnerName'})
            ]}),
            this.descriptionSection = Interface.Create({type: 'div', className: 'DescriptionSection', elements: [
                this.description = Interface.Create({type: 'div', className: 'Description'})
            ]}),
            {type: 'hr'}
        ]});

        this.editDescription = Interface.Create({type: 'textarea', className: 'EditDescription'});


        this.editComment = Interface.Create({type: 'button', className: 'EditComment', text: 'Edit'});
        this.editComment.onclick = this.editButtonClick;

        this.saveEdit = Interface.Create({type: 'button', className: 'SaveComment', text: 'Save'});
        this.saveEdit.onclick = this.saveEditButtonClick;

        this.deleteComment = Interface.Create({type: 'button', className: 'DeleteComment', text: 'Delete'});
        this.deleteComment.onclick = this.deleteCommentButtonClick;
    }

    editButtonClick = () => {
        this.editComment.remove();
        this.descriptionSection.appendChild(this.saveEdit);
        this.descriptionSection.appendChild(this.deleteComment);
        this.descriptionSection.insertBefore(this.editDescription, this.description);
        this.description.remove();
        this.editDescription.value = this.comment.getComment();
    };

    saveEditButtonClick = () => {
        this.descriptionSection.appendChild(this.editComment);
        this.saveEdit.remove();
        this.deleteComment.remove();
        this.descriptionSection.insertBefore(this.description, this.editDescription);
        this.editDescription.remove();

        let message: any = {};
        message['Controller'] = this.threadController.sectionController.name;
        message['Title'] = 'Edit Comment';
        message['Thread ID'] = this.thread.getID();
        message['Comment ID'] = this.comment.getCommentID();
        message['Text'] = this.editDescription.value;
        this.threadController.sectionController.website.networkController.send(message);
        console.log(`Pressed Edit Comment on thread ${this.thread.getID()} and on comment ${this.comment.getCommentID()}`);
    };

    deleteCommentButtonClick = () => {
        let message: any = {};
        message['Controller'] = this.threadController.sectionController.name;
        message['Title'] = 'Delete Comment';
        message['Thread ID'] = this.thread.getID();
        message['Comment ID'] = this.comment.getCommentID();
        this.threadController.sectionController.website.networkController.send(message);
    };

    getDiv(): HTMLDivElement {
        return this.main;
    }

    updateOwner() {
        this.owner.innerText = this.comment.getPosterName();
        //If owner is self add edit button
        if (this.comment.getPosterName() == this.threadController.sectionController.website.database.name) {
            this.descriptionSection.appendChild(this.editComment);
        }
    }

    updateAvatarURL() {
        if (this.comment.getAvatarURL() == null || this.comment.getAvatarURL() == undefined) {
            return;
        }
        this.image.src = this.comment.getAvatarURL();
    }

    updateDescription() {
        Utility.ClearElements(this.description);
        this.description.appendChild(DescriptionParser.parseDescription(this.comment.getComment()));
    }
}