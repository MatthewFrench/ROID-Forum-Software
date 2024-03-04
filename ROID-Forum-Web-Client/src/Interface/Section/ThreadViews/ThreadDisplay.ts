import './ThreadDisplay.scss';

import {FullView} from "./FullView";
import {DescriptionParser} from "../../../Utility/DescriptionParser";
import {Utility} from "../../../Utility/Utility";
import {Interface} from "../../../Utility/Interface";

type User = { connectionId: string, accountId?: string, displayName?: string };

export class ThreadDisplay {
    main: HTMLDivElement;
    mainView: HTMLDivElement;
    threadViewingUsers: User[] = [];
    threadViewersElement: HTMLDivElement;
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

    constructor(c: FullView, darkTheme: boolean) {
        this.controller = c;

        this.main = Interface.Create({
            type: 'div', className: 'ThreadDisplay', elements: [
                this.mainView = Interface.Create({
                    type: 'div', className: 'View', elements: [
                        this.threadViewersElement = Interface.Create({
                            type: 'span',
                            className: 'ThreadViewers',
                            text: ""
                        }),
                        this.profileSection = Interface.Create({
                            type: 'div', className: 'ProfileSection', elements: [
                                this.image = Interface.Create({type: 'img', className: 'ProfileImage'}),
                                this.owner = Interface.Create({type: 'span', className: 'OwnerText'})
                            ]
                        }),
                        this.titleSection = Interface.Create({
                            type: 'div', className: 'TitleSection', elements: [
                                this.title = Interface.Create({type: 'span', className: 'Title'})
                            ]
                        }),
                        this.descriptionSection = Interface.Create({
                            type: 'div', className: 'DescriptionSection', elements: [
                                this.description = Interface.Create({type: 'div', className: 'Description'})
                            ]
                        }),
                        {type: 'hr'}
                    ]
                })
            ]
        });

        if (darkTheme) {
            this.main.classList.add('DarkTheme');
            this.threadViewersElement.classList.add('DarkTheme');
        } else {
            this.main.classList.add('LightTheme');
            this.threadViewersElement.classList.add('LightTheme');
        }

        this.switchToEditViewButton = Interface.Create({
            type: 'button', text: 'Edit',
            className: 'EditViewButton', onClick: this.switchView
        });

        //Make the edit stuff
        this.editView = Interface.Create({
            type: 'div', className: 'EditView', elements: [
                this.editTitle = Interface.Create({type: 'input', className: 'Title', placeholder: 'Title here'}),
                this.editDescription = Interface.Create({
                    type: 'textarea',
                    placeholder: 'Description here',
                    className: 'Description'
                }),
                this.editButton = Interface.Create({
                    type: 'button',
                    className: 'EditButton',
                    text: 'Save Edit',
                    onClick: this.saveEdit
                }),
                this.deleteButton = Interface.Create({
                    type: 'button',
                    className: 'DeleteButton',
                    text: 'Delete Thread',
                    onClick: this.deleteThread
                })
            ]
        });
    }

    updateTitle = () => {
        this.title.innerText = this.controller.thread.getTitle();
        this.editTitle.value = this.controller.thread.getTitle();
    };

    updateOwner = () => {
        this.owner.innerText = this.controller.thread.getCreatorDisplayName();
        //If this.owner is self add edit button
        if (this.controller.thread.getCreatorDisplayName() == this.controller.threadController.sectionController.website.database.displayName) {
            this.descriptionSection.appendChild(this.switchToEditViewButton);
        }
    };

    updateAvatarURL = () => {
        if (this.controller.thread.getAvatarURL() == null || this.controller.thread.getAvatarURL() == '' || this.controller.thread.getAvatarURL() == undefined) {
            this.image.src =  require('../../../../static/assets/generic-icon.png');
            return;
        }
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
        //message['Controller'] = this.controller.threadController.sectionController.sectionName;
        message['Title'] = 'Edit Post';
        message['Thread ID'] = this.controller.thread.getThreadId();
        message['Edit Title'] = this.editTitle.value;
        message['Text'] = this.editDescription.value;
        this.controller.threadController.sectionController.website.networkController.send(message);
        this.switchView();
    };

    deleteThread = () => {
        //Send new post to server
        let message: any = {};
        //message['Controller'] = this.controller.threadController.sectionController.sectionName;
        message['Title'] = 'Delete Post';
        message['Thread ID'] = this.controller.thread.getThreadId();
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

    updateThreadViewersDisplay = () => {
        let currentGuestsOnline = 0;
        let namesOnline = [];
        for (let user of this.threadViewingUsers) {
            if (user.displayName == undefined) {
                currentGuestsOnline += 1;
            } else {
                namesOnline.push(user.displayName);
            }
        }
        let onlineText = "Currently viewing: ";
        if (namesOnline.length > 0) {
            onlineText += namesOnline.join(", ");
            if (currentGuestsOnline > 0) {
                onlineText += ` and ${currentGuestsOnline} Guest`;
            }
        } else if (currentGuestsOnline > 0) {
            onlineText += `${currentGuestsOnline} Guest`;
        }
        if (currentGuestsOnline > 1) {
            onlineText += "s"
        }
        this.threadViewersElement.innerText = onlineText;
    }
}