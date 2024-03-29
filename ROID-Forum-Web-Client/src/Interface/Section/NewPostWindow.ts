import './NewPostWindow.scss';

import {Section} from "./Section";
import {Interface} from "../../Utility/Interface";
import {MessageWriter} from "../../Utility/Message/MessageWriter";
import {SendMessages} from "../../Networking/MessageDefinitions/SendMessages";

export class NewPostWindow {
    sectionController: Section;
    backgroundBlockDiv: HTMLDivElement;
    mainWindowDiv: HTMLDivElement;
    titleInput: HTMLInputElement;
    mainText: HTMLTextAreaElement;

    constructor(cs: Section) {
        this.sectionController = cs;

        this.mainWindowDiv = Interface.Create({type: 'div', className: 'NewPostWindow', elements: [
                Interface.Create({type: 'div', className: 'Title', text: 'Create New Post'}),
                this.titleInput = Interface.Create({type: 'input', className: 'TitleInput', placeholder: 'Post Title'}),
                this.mainText = Interface.Create({type: 'textarea', className: 'DescriptionInput', placeholder: 'Write description here'}),
                {type: 'button', className: 'PostButton', text: 'Post', onClick: this.postButtonClick}
            ]});
        this.backgroundBlockDiv = Interface.Create({type: 'div', className: 'BackgroundBlock', onClick: this.hide});
    }

    show = () => {
        this.sectionController.website.aheadWebsiteDiv.appendChild(this.backgroundBlockDiv);
        this.sectionController.website.aheadWebsiteDiv.appendChild(this.mainWindowDiv);
    };

    hide = () => {
        this.backgroundBlockDiv.remove();
        this.mainWindowDiv.remove();
    };

    reset = () => {
        this.titleInput.value = '';
        this.mainText.value = '';
    };

    postButtonClick = () => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Section);
        message.addString(this.sectionController.sectionId);
        message.addUint8(SendMessages.SectionMessage.NewThread);
        message.addString(this.titleInput.value);
        message.addString(this.mainText.value);
        this.sectionController.website.networkController.send(message.toBuffer());
        //Hide the window
        this.hide();
        //Reset window
        this.reset();
    };
}