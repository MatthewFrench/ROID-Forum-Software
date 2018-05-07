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
        this.makeMainComponent();
        this.makeImageComponent();
        this.makeOwnerComponent();
        this.makeDescriptionComponent();
        this.makeEditDescriptionComponent();
        this.makeEditCommentComponent();
        this.makeSaveEditComponent();
        this.makeDeleteCommentComponent();
    }

    editButtonClick() {
        this.editComment.remove();
        this.descriptionSection.appendChild(this.saveEdit);
        this.descriptionSection.appendChild(this.deleteComment);
        this.descriptionSection.insertBefore(this.editDescription, this.description);
        this.description.remove();
        this.editDescription.value = this.comment.getComment();
    }

    saveEditButtonClick() {
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
        this.threadController.sectionController.website.networkingController.Send(message);
        console.log("Pressed Edit Comment on thread ${this.thread.getID()} and on comment ${this.comment.getCommentID()}");
    }

    deleteCommentButtonClick() {
        let message: any = {};
        message['Controller'] = this.threadController.sectionController.name;
        message['Title'] = 'Delete Comment';
        message['Thread ID'] = this.thread.getID();
        message['Comment ID'] = this.comment.getCommentID();
        this.threadController.sectionController.website.networkController.send(message);
    }

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
        this.image.src = this.comment.getAvatarURL();
    }

    updateDescription() {
        Utility.ClearElements(this.description);
        this.description.appendChild(DescriptionParser.parseDescription(this.comment.getComment()));
    }

    makeMainComponent() {
        this.main = Interface.Create({type: 'div'});
        this.main.style.display = "inline-block";
        this.main.style.marginLeft = "75px";
        this.main.style.position = "relative";
        this.main.style.width = "calc(100% - 75px)";
        this.main.style.opacity = "1.0";

        this.profileSection = Interface.Create({type: 'div'});
        this.profileSection.style.display = "inline-block";
        this.profileSection.style.width = "60px";
        this.profileSection.style.height = "80px";
        this.profileSection.style.position = "absolute";
        this.profileSection.style.top = "5px";
        this.profileSection.style.textAlign = "center";
        this.profileSection.style.color = "white";
        this.main.appendChild(this.profileSection);
        this.descriptionSection = Interface.Create({type: 'div'});
        this.descriptionSection.style.display = "inline-block";
        this.descriptionSection.style.width = "calc(100% - 95px)";
        this.descriptionSection.style.marginLeft = "70px";
        this.descriptionSection.style.marginTop = "5px";
        this.descriptionSection.style.minHeight = "90px";
        this.descriptionSection.style.textAlign = "left";
        this.descriptionSection.style.color = "white";
        this.main.appendChild(this.descriptionSection);
        this.main.appendChild(Interface.Create({type: 'hr'}));
    }

    makeOwnerComponent() {
        this.owner = Interface.Create({type: 'span'});
        this.owner.style.fontSize = "15px";
        this.owner.style.color = "white";
        this.profileSection.appendChild(this.owner);
    }

    makeImageComponent() {
        this.image = Interface.Create({type: 'img'});
        this.image.style.height = "60px";
        this.image.style.width = "60px";
        this.image.style.backgroundColor = "rgb(200,200,200)";
        this.image.style.borderStyle = "solid";
        this.image.style.borderWidth = "1px";
        this.profileSection.appendChild(this.image);
    }

    makeDescriptionComponent() {
        this.description = Interface.Create({type: 'div'});
        this.description.style.width = "90%";
        this.description.style.wordWrap = "normal";
        this.description.style.color = "white";
        this.descriptionSection.appendChild(this.description);
    }

    makeEditDescriptionComponent() {
        this.editDescription = Interface.Create({type: 'textarea'});
        this.editDescription.style.width = "90%";
        this.editDescription.style.wordWrap = "normal";
        this.editDescription.style.color = "black";
    }

    makeEditCommentComponent() {
        this.editComment = Interface.Create({type: 'button'});
        this.editComment.style.cssFloat = "right";
        this.editComment.innerText = "Edit";
        this.editComment.onclick = () => {
            this.editButtonClick();
        };
    }

    makeSaveEditComponent() {
        this.saveEdit = Interface.Create({type: 'button'});
        this.saveEdit.style.cssFloat = "right";
        this.saveEdit.innerText = "Save";
        this.saveEdit.onclick = () => {
            this.saveEditButtonClick();
        };
    }

    makeDeleteCommentComponent() {
        this.deleteComment = Interface.Create({type: 'button'});
        this.deleteComment.style.cssFloat = "left";
        this.deleteComment.innerText = "Delete";
        this.deleteComment.onclick = () => {
            this.deleteCommentButtonClick();
        };
    }
}