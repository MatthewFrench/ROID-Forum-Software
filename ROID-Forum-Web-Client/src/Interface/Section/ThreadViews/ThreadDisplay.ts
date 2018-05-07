import './ThreadDisplay.scss';

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

        this.main = Interface.Create({type: 'div', className: 'ThreadDisplay', elements: [
            this.mainView = Interface.Create({type: 'div', className: 'View', elements: [
                this.profileSection = Interface.Create({type: 'div', className: 'ProfileSection', elements: [
                    this.image = Interface.Create({type: 'img', className: 'ProfileImage'}),
                    this.owner = Interface.Create({type: 'span', className: 'OwnerText'})
                ]}),
                this.titleSection = Interface.Create({type: 'div', className: 'TitleSection', elements: [
                    this.title = Interface.Create({type: 'span', className: 'Title'})
                ]}),
                this.descriptionSection = Interface.Create({type: 'div', className: 'DescriptionSection', elements: [
                    this.description = Interface.Create({type: 'div', className: 'Description'})
                ]}),
                {type: 'hr'}
            ]})
        ]});

        this.switchToEditViewButton = Interface.Create({type: 'button', text: 'Edit',
            className: 'EditViewButton', onClick: this.switchView});

        //Make the edit stuff
        this.editView = Interface.Create({type: 'div', className: 'EditView', elements: [
            this.editTitle = Interface.Create({type: 'input', className: 'Title', placeholder: 'Title here'}),
            this.editDescription = Interface.Create({type: 'textarea', placeholder: 'Description here', className: 'Description'}),
            this.editButton = Interface.Create({type: 'button', className: 'EditButton', text: 'Save Edit', onClick: this.saveEdit}),
            this.deleteButton = Interface.Create({type: 'button', className: 'DeleteButton', text: 'Delete Thread', onClick: this.deleteThread})
        ]});
    }

    updateTitle = () => {
        this.title.innerText = this.controller.thread.getTitle();
        this.editTitle.value = this.controller.thread.getTitle();
    };

    updateOwner = () => {
        this.owner.innerText = this.controller.thread.getOwner();
        //If this.owner is self add edit button
        if (this.controller.thread.getOwner() == this.controller.threadController.sectionController.website.database.name) {
            this.descriptionSection.appendChild(this.switchToEditViewButton);
        }
    };

    updateAvatarURL = () => {
        this.image.src = this.controller.thread.getAvatarURL();
    };

    updateDescription = () => {
        Utility.ClearElements(this.description);
        this.description.appendChild(DescriptionParser.parseDescription(this.controller.thread.getDescription()));
        this.editDescription.innerText = this.controller.thread.getDescription();
    };

    saveEdit = () => {
        //Send new post to server
        let message: any = {};
        message['Controller'] = this.controller.threadController.sectionController.name;
        message['Title'] = 'Edit Post';
        message['Thread ID'] = this.controller.thread.getID();
        message['Edit Title'] = this.editTitle.value;
        message['Text'] = this.editDescription.value;
        this.controller.threadController.sectionController.website.networkController.send(message);
        this.switchView();
    };

    deleteThread = () => {
        //Send new post to server
        let message: any = {};
        message['Controller'] = this.controller.threadController.sectionController.name;
        message['Title'] = 'Delete Post';
        message['Thread ID'] = this.controller.thread.getID();
        this.controller.threadController.sectionController.website.networkController.send(message);
        this.controller.threadController.restoreToDefaultState();
    };

    switchView = () => {
        this.onMainView = !this.onMainView;
        if (this.onMainView) {
            this.switchToMainView();
        } else {
            this.switchToEditView();
        }
    };

    switchToMainView = () => {
        this.editView.remove();
        this.main.appendChild(this.mainView);
    };

    switchToEditView = () => {
        this.mainView.remove();
        this.main.appendChild(this.editView);
    };

    getDiv(): HTMLDivElement {
        return this.main;
    }
}