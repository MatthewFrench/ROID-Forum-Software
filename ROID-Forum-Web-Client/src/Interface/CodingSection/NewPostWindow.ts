import {Section} from "./Section";
import {Interface} from "../../Utility/Interface";

export class NewPostWindow {
    sectionController: Section;
    backgroundBlockDiv: HTMLDivElement;
    mainWindowDiv: HTMLDivElement;
    titleInput: HTMLInputElement;
    mainText: HTMLTextAreaElement;

    constructor(cs: Section) {
        this.sectionController = cs;

        this.makeWindowComponent();
        this.makeTitleComponent();
        this.makeTitleInputComponent();
        this.makeDescriptionComponent();
        this.makePostButtonComponent();
        this.makeBackgroundBlockComponent();
    }

    show() {
        this.sectionController.website.aheadWebsiteDiv.appendChild(this.backgroundBlockDiv);
        this.sectionController.website.aheadWebsiteDiv.appendChild(this.mainWindowDiv);
    }

    hide() {
        this.backgroundBlockDiv.remove();
        this.mainWindowDiv.remove();
    }

    reset() {
        this.titleInput.value = '';
        this.mainText.value = '';
    }

    postButtonClick() {
        //Send new post to server
        let message: any = {};
        message['Controller'] = this.sectionController.name;
        message['Title'] = 'New Post';
        message['Post Title'] = this.titleInput.value;
        message['Post Description'] = this.mainText.value;
        this.sectionController.website.networkController.send(message);
        //Hide the window
        this.hide();
        //Reset window
        this.reset();
    }

    /*****GUI COMPONENT CREATION********/
    makeWindowComponent() {
        this.mainWindowDiv = Interface.Create({type: 'div'});
        this.mainWindowDiv.style.position = "absolute";
        this.mainWindowDiv.style.width = "400px";
        this.mainWindowDiv.style.height = "300px";
        this.mainWindowDiv.style.borderRadius = "4px";
        this.mainWindowDiv.style.borderStyle = "solid";
        this.mainWindowDiv.style.borderWidth = "1px";
        this.mainWindowDiv.style.borderColor = "#DDDDDD";
        this.mainWindowDiv.style.backgroundColor = "white";
        this.mainWindowDiv.style.top = "calc(50% - 150px)";
        this.mainWindowDiv.style.left = "calc(50% - 200px)";
    }

    makeBackgroundBlockComponent() {
        this.backgroundBlockDiv = Interface.Create({type: 'div'});
        this.backgroundBlockDiv.style.position = "absolute";
        this.backgroundBlockDiv.style.width = "100%";
        this.backgroundBlockDiv.style.height = "100%";
        this.backgroundBlockDiv.style.backgroundColor = "rgba(0, 0, 0, 0.5)";
        this.backgroundBlockDiv.style.top = "0px";
        this.backgroundBlockDiv.style.left = "0px";
        this.backgroundBlockDiv.onclick = () => {
            this.hide();
        };
    }

    makeTitleComponent() {
        let title: HTMLDivElement = Interface.Create({type: 'div'});
        title.innerText = "Create New Post";
        title.style.position = 'absolute';
        title.style.top = '10px';
        title.style.left = '0px';
        title.style.width = '100%';
        title.style.textAlign = 'center';
        title.style.fontSize = '25px';
        this.mainWindowDiv.appendChild(title);
    }

    makeTitleInputComponent() {
        this.titleInput = Interface.Create({type: 'input'});
        this.titleInput.placeholder = "Post Title";
        this.titleInput.style.position = 'absolute';
        this.titleInput.style.top = '40px';
        this.titleInput.style.left = '10px';
        this.titleInput.style.width = 'calc(100% - 20px)';
        this.mainWindowDiv.appendChild(this.titleInput);
    }

    makeDescriptionComponent() {
        this.mainText = Interface.Create({type: 'textarea'});
        this.mainText.placeholder = "Write description here";
        this.mainText.style.position = 'absolute';
        this.mainText.style.top = '65px';
        this.mainText.style.left = '10px';
        this.mainText.style.width = 'calc(100% - 20px)';
        this.mainText.style.height = 'calc(100% - 125px)';
        this.mainText.style.resize = 'none';
        this.mainWindowDiv.appendChild(this.mainText);
    }

    makePostButtonComponent() {
        let postButton: HTMLButtonElement = Interface.Create({type: 'button'});
        postButton.innerText = "Post";
        postButton.style.width = "140px";
        postButton.style.height = "40px";
        postButton.style.backgroundColor = "white";
        postButton.style.borderRadius = "4px";
        postButton.style.borderStyle = "solid";
        postButton.style.borderWidth = "1px";
        postButton.style.borderColor = "#DDDDDD";
        postButton.style.fontSize = "15px";
        postButton.style.bottom = "5px";
        postButton.style.position = "absolute";
        postButton.style.outline = "none";
        postButton.style.right = "5px";
        postButton.dataset['active'] = "background: #BBBBBB;";
        postButton.onclick = () => {
            this.postButtonClick();
        };
        this.mainWindowDiv.appendChild(postButton);
    }
}
