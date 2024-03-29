import './CommentView.scss';

import {ThreadInfo} from "../ThreadInfo";
import {CommentInfo} from "../CommentInfo";
import {ThreadController} from "../ThreadController";
import {DescriptionParser} from "../../../Utility/DescriptionParser";
import {Utility} from "../../../Utility/Utility";
import {Interface} from "../../../Utility/Interface";
import {MessageWriter} from "../../../Utility/Message/MessageWriter";
import {SendMessages} from "../../../Networking/MessageDefinitions/SendMessages";

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

    constructor(ci: CommentInfo, t: ThreadInfo, tc: ThreadController, darkTheme : boolean) {
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

        if (darkTheme) {
            this.main.classList.add('DarkTheme');
        } else {
            this.main.classList.add('LightTheme');
        }

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

        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Thread);
        message.addString(this.threadController.sectionController.sectionId);
        message.addString(this.thread.getThreadId());
        message.addUint8(SendMessages.ThreadMessage.EditComment);
        message.addString(this.comment.getCommentIDAndCreatedTime());
        message.addString(this.editDescription.value);
        this.threadController.sectionController.website.networkController.send(message.toBuffer());
    };

    deleteCommentButtonClick = () => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Thread);
        message.addString(this.threadController.sectionController.sectionId);
        message.addString(this.thread.getThreadId());
        message.addUint8(SendMessages.ThreadMessage.DeleteComment);
        message.addString(this.comment.getCommentIDAndCreatedTime());
        this.threadController.sectionController.website.networkController.send(message.toBuffer());
    };

    getDiv(): HTMLDivElement {
        return this.main;
    }

    updateOwner() {
        this.owner.innerText = this.comment.getCreatorDisplayName();
        //If owner is self add edit button
        if (this.comment.getCreatorDisplayName() == this.threadController.sectionController.website.database.displayName) {
            this.descriptionSection.appendChild(this.editComment);
        }
    }

    updateAvatarURL() {
        if (this.comment.getCreatorAvatarUrl() == null || this.comment.getCreatorAvatarUrl() == undefined || this.comment.getCreatorAvatarUrl() == '') {
            this.image.src = require('../../../../static/assets/generic-icon.png');
            return;
        }
        this.image.src = this.comment.getCreatorAvatarUrl();
    }

    updateDescription() {
        Utility.ClearElements(this.description);
        this.description.appendChild(DescriptionParser.parseDescription(this.comment.getComment()));
    }
}