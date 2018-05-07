import {FullView} from "./FullView";
import {DescriptionParser} from "../../../Utility/DescriptionParser";
import {Utility} from "../../../Utility/Utility";
import {Interface} from "../../../Utility/Interface";

export class ThreadDisplay {
    main: HTMLDivElement;
    mainView: HTMLDivElement;
    profileSection: HTMLDivElement;
    titleSection: HTMLDivElement;
    descriptionSection: HTMLDivElement;
    title: HTMLSpanElement;
    owner: HTMLSpanElement;
    image: HTMLImageElement;
    description: HTMLDivElement;
    //Edit stuff
    editView: HTMLDivElement;
    editTitle: HTMLInputElement;
    editDescription: HTMLTextAreaElement;
    editButton: HTMLButtonElement;
    deleteButton: HTMLButtonElement;
    switchToEditViewButton: HTMLButtonElement;
    controller: FullView;
    onMainView = true;

    constructor(c: FullView) {
        this.controller = c;
        this.makeMain();
        this.makeMainView();
        this.makeTitleComponent();
        this.makeImageComponent();
        this.makeDescriptionComponent();
        this.makeOwnerComponent();
        this.makeSwitchToEditViewButton();
        //Make the edit stuff
        this.makeEditView();
        this.makeEditTitleComponent();
        this.makeEditDescriptionComponent();
        this.makeEditButtonComponent();
        this.makeDeleteButtonComponent();
        this.main.appendChild(this.mainView);
    }

    updateTitle() {
        this.title.innerText = this.controller.thread.getTitle();
        this.editTitle.value = this.controller.thread.getTitle();
    }

    updateOwner() {
        this.owner.innerText = this.controller.thread.getOwner();
        //If this.owner is self add edit button
        if (this.controller.thread.getOwner() == this.controller.threadController.sectionController.website.database.name) {
            this.descriptionSection.appendChild(this.switchToEditViewButton);
        }
    }

    updateAvatarURL() {
        this.image.src = this.controller.thread.getAvatarURL();
    }

    updateDescription() {
        Utility.ClearElements(this.description);
        this.description.appendChild(DescriptionParser.parseDescription(this.controller.thread.getDescription()));
        this.editDescription.innerText = this.controller.thread.getDescription();
    }

    getDiv(): HTMLDivElement {
        return this.main;
    }

    saveEdit() {
        //Send new post to server
        let message: any = {};
        message['Controller'] = this.controller.threadController.sectionController.name;
        message['Title'] = 'Edit Post';
        message['Thread ID'] = this.controller.thread.getID();
        message['Edit Title'] = this.editTitle.value;
        message['Text'] = this.editDescription.value;
        this.controller.threadController.sectionController.website.networkController.send(message);
        this.switchView();
    }

    deleteThread() {
        //Send new post to server
        let message: any = {};
        message['Controller'] = this.controller.threadController.sectionController.name;
        message['Title'] = 'Delete Post';
        message['Thread ID'] = this.controller.thread.getID();
        this.controller.threadController.sectionController.website.networkController.send(message);
        this.controller.threadController.restoreToDefaultState();
    }

    switchView() {
        this.onMainView = !this.onMainView;
        if (this.onMainView) {
            this.switchToMainView();
        } else {
            this.switchToEditView();
        }
    }

    switchToMainView() {
        this.editView.remove();
        this.main.appendChild(this.mainView);
    }

    switchToEditView() {
        this.mainView.remove();
        this.main.appendChild(this.editView);
    }

    //Make GUI Components
    makeMain() {
        this.main = Interface.Create({type: 'div'});
    }

    makeMainView() {
        this.mainView = Interface.Create({type: 'div'});
        this.mainView.style.display = "inline-block";
        this.mainView.style.marginLeft = "75px";
        this.profileSection = Interface.Create({type: 'div'});
        this.profileSection.style.display = "inline-block";
        this.profileSection.style.width = "60px";
        this.profileSection.style.position = "absolute";
        this.profileSection.style.top = "5px";
        this.profileSection.style.textAlign = "center";
        this.profileSection.style.color = "white";
        this.mainView.appendChild(this.profileSection);
        this.titleSection = Interface.Create({type: 'div'});
        this.titleSection.style.display = "inline-block";
        this.titleSection.style.width = "calc(100% - 80px)";
        this.titleSection.style.minHeight = "80px";
        this.titleSection.style.marginLeft = "70px";
        this.titleSection.style.textAlign = "left";
        this.titleSection.style.color = "white";
        this.mainView.appendChild(this.titleSection);
        this.descriptionSection = Interface.Create({type: 'div'});
        this.descriptionSection.style.display = "inline-block";
        this.descriptionSection.style.width = "calc(100% - 90px)";
        this.descriptionSection.style.marginLeft = "70px";
        this.descriptionSection.style.textAlign = "left";
        this.descriptionSection.style.color = "white";
        this.mainView.appendChild(this.descriptionSection);

        this.mainView.appendChild(Interface.Create({type: 'hr'}));
    }

    makeTitleComponent() {
        this.title = Interface.Create({type: 'span'});
        this.title.style.fontSize = "50px";
        this.title.style.textDecoration = "underline";
        this.title.style.paddingTop = "10px";
        this.title.style.color = "white";
        this.titleSection.appendChild(this.title);
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
        this.description.style.wordWrap = "normal";
        this.description.style.width = "90%";
        this.description.style.color = "white";
        this.descriptionSection.appendChild(this.description);
    }

    makeSwitchToEditViewButton() {
        this.switchToEditViewButton = Interface.Create({type: 'button'});
        this.switchToEditViewButton.style.color = "black";
        this.switchToEditViewButton.style.cssFloat = "right";
        //this.switchToEditViewButton.style.width = "100px";
        this.switchToEditViewButton.innerText = "Edit";
        this.switchToEditViewButton.onclick = () => {
            this.switchView();
        };
    }

    //Edit stuff
    makeEditView() {
        this.editView = Interface.Create({type: 'div'});
        this.editView.style.margin = "30px";
        this.editView.style.marginLeft = "70px";
    }

    makeEditTitleComponent() {
        this.editTitle = Interface.Create({type: 'input'});
        this.editTitle.style.padding = "10px";
        this.editTitle.style.color = "black";
        this.editTitle.style.width = "calc(100% - 20px)";
        this.editTitle.placeholder = "Title here";
        this.editView.appendChild(this.editTitle);
    }

    makeEditDescriptionComponent() {
        this.editDescription = Interface.Create({type: 'textarea'});
        this.editDescription.style.color = "black";
        this.editDescription.style.width = "100%";
        this.editDescription.style.height = "100px";
        this.editDescription.placeholder = "Description here";
        this.editView.appendChild(this.editDescription);
    }

    makeEditButtonComponent() {
        this.editButton = Interface.Create({type: 'button'});
        this.editButton.style.color = "black";
        this.editButton.style.cssFloat = "right";
        this.editButton.innerText = "Save Edit";
        this.editView.appendChild(this.editButton);
        this.editButton.onclick = () => {
            this.saveEdit();
        };
    }

    makeDeleteButtonComponent() {
        this.deleteButton = Interface.Create({type: 'button'});
        this.deleteButton.style.color = "black";
        this.deleteButton.style.cssFloat = "left";
        this.deleteButton.innerText = "Delete Thread";
        this.editView.appendChild(this.deleteButton);
        this.deleteButton.onclick = () => {
            this.deleteThread();
        };
    }
}