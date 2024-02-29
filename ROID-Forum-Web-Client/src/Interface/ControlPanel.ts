import './ControlPanel.scss';

import {AppController} from "../AppController";
import {Interface} from "../Utility/Interface";
import {MessageWriter} from "../Utility/Message/MessageWriter";
import {SendMessages} from "../Networking/MessageDefinitions/SendMessages";

export class ControlPanel {
    website : AppController;
    showingLoginWindow = false;
    showingPreferencesWindow = false;
    loginButton : HTMLButtonElement;
    logoutButton : HTMLButtonElement;
    preferencesButton : HTMLButtonElement;
    topControlPanel : HTMLDivElement;
    loginDiv : HTMLDivElement;
    hideBackgroundDiv : HTMLDivElement;
    loginErrorDiv : HTMLDivElement;
    topNameDiv : HTMLDivElement;
    preferencesDiv : HTMLDivElement;
    preferencesAvatarInput : HTMLInputElement;
    preferencesDisplayNameInput : HTMLInputElement;

    loginWindowName : HTMLInputElement;
    loginWindowPassword : HTMLInputElement;
    loginWindowEmail : HTMLInputElement;
    constructor(w : AppController) {
        this.website = w;

        //Make the control panel that sits above the chatbox
        this.topControlPanel = Interface.Create({type: 'div', className: 'TopControlPanel', elements: [
                this.loginButton = Interface.Create({type: 'button', className: 'LoginButton', text: 'Login/Register', onClick: this.loginButtonClicked})
            ]});
        this.website.websiteDiv.appendChild(this.topControlPanel);

        //Add the preferences button to it
        this.preferencesButton = Interface.Create({type: 'button', className: 'PreferencesButton', text: 'Preferences', onClick: this.preferencesButtonClicked});

        this.logoutButton = Interface.Create({type: 'button', className: 'LogoutButton', text: 'Logout', onClick: this.logoutButtonClicked});

        //Make the login/register div that's invisible until the button is clicked
        this.hideBackgroundDiv = Interface.Create({type: 'div', className: 'HideBackgroundDiv', onClick: ()=>{
                if (this.showingLoginWindow) {
                    this.removeLoginDiv();
                } else {
                    this.removePreferencesDiv();
                }
            }});

        this.loginDiv = Interface.Create({type: 'div', className: 'LoginWindowDiv', elements: [
                {type: 'div', className: 'Title', text: 'Login or Register'},
                {type: 'hr', className: 'LoginHR'},
                {type: 'button', className: 'LoginButton', text: 'Login', onClick: this.loginWindowLoginButtonClicked},
                {type: 'button', className: 'RegisterButton', text: 'Register', onClick: this.loginWindowRegisterButtonClicked},
                this.loginWindowName = Interface.Create({type: 'input', className: 'Name', placeholder: 'Name'}),
                this.loginWindowPassword = Interface.Create({type: 'input', inputType: 'password', className: 'Password', placeholder: 'Password'}),
                this.loginWindowEmail = Interface.Create({type: 'input', className: 'Email', placeholder: 'Email for Registration'}),
                this.loginErrorDiv = Interface.Create({type: 'div', className: 'Error'})
            ]});

        this.preferencesDiv = Interface.Create({type: 'div', className: 'PreferencesDiv', elements: [
                {type: 'div', className: 'Title', text: 'Preferences'},
                {type: 'hr', className: 'PreferencesHR'},
                this.preferencesAvatarInput = Interface.Create({type: 'input', className: 'AvatarInput'}),
                {type: 'label', className: 'AvatarLabel', text: 'Avatar Image'},
                {type: 'button', className: 'AvatarSaveButton', text: 'Save Avatar', onClick: this.avatarSaveButtonClick},
                this.preferencesDisplayNameInput = Interface.Create({type: 'input', className: 'DisplayNameInput'}),
                {type: 'label', className: 'DisplayNameLabel', text: 'Display Name'},
                {type: 'button', className: 'DisplayNameSaveButton', text: 'Save DisplayName', onClick: this.displayNameSaveButtonClick}
            ]});

        this.topNameDiv = Interface.Create({type: 'div', className: 'TopNameDiv'});
    }
    darkTheme = () => {
        this.topNameDiv.classList.add('DarkTheme');
        this.topNameDiv.classList.remove('LightTheme');
    };
    lightTheme = () => {
        this.topNameDiv.classList.remove('DarkTheme');
        this.topNameDiv.classList.add('LightTheme');
    };
    preferencesButtonClicked = () => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Profile);
        message.addUint8(SendMessages.ProfileMessage.GetAvatar);
        this.website.networkController.send(message.toBuffer());
        message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Profile);
        message.addUint8(SendMessages.ProfileMessage.GetDisplayName);
        this.website.networkController.send(message.toBuffer());
        this.website.websiteDiv.appendChild(this.hideBackgroundDiv);
        this.website.websiteDiv.appendChild(this.preferencesDiv);
        this.showingPreferencesWindow = true;
    };
    loginButtonClicked = () => {
        this.website.websiteDiv.appendChild(this.hideBackgroundDiv);
        this.website.websiteDiv.appendChild(this.loginDiv);
        this.showingLoginWindow = true;
    };
    logoutButtonClicked = () => {
        //Notify server to log out
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Profile);
        message.addUint8(SendMessages.ProfileMessage.Logout);
        this.website.networkController.send(message.toBuffer());
        //Server will tell the client to log out and the client will do all the necessaries
    };
    avatarSaveButtonClick = () => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Profile);
        message.addUint8(SendMessages.ProfileMessage.SetAvatar);
        message.addString(this.preferencesAvatarInput.value);
        this.website.networkController.send(message.toBuffer());
    };
    displayNameSaveButtonClick = () => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Profile);
        message.addUint8(SendMessages.ProfileMessage.UpdateDisplayName);
        message.addString(this.preferencesDisplayNameInput.value);
        this.website.networkController.send(message.toBuffer());
    };
    loginWindowLoginButtonClicked = () => {
        this.loginErrorDiv.innerText = "";
        if (this.loginWindowName.value.length == 0) {
            this.loginErrorDiv.innerText = 'Need a name';
            return;
        }
        if (this.loginWindowPassword.value.length == 0) {
            this.loginErrorDiv.innerText = 'Need a password';
            return;
        }
        this.login(this.loginWindowName.value, this.loginWindowPassword.value);
    };
    login = (name : string, password : string) => {
        this.securelyHash(password).then((hashedPassword)=>{
            let message = new MessageWriter();
            message.addUint8(SendMessages.Controller.Profile);
            message.addUint8(SendMessages.ProfileMessage.Login);
            message.addString(name);
            message.addString(hashedPassword);
            this.website.networkController.send(message.toBuffer());
            // Todo: Don't do this, or do it better. This is janky for setting up auto-login.
            localStorage.setItem('Name', name);
            localStorage.setItem('Password', hashedPassword);
        });
    };
    loginAlreadyEncrypted = (name : string, password : string) => {
        let message = new MessageWriter();
        message.addUint8(SendMessages.Controller.Profile);
        message.addUint8(SendMessages.ProfileMessage.Login);
        message.addString(name);
        message.addString(password);
        this.website.networkController.send(message.toBuffer());
    };
    loginWindowRegisterButtonClicked = () => {
        this.loginErrorDiv.innerText = "";
        if (this.loginWindowName.value.length == 0) {
            this.loginErrorDiv.innerText = 'Need a name';
            return;
        }
        if (this.loginWindowPassword.value.length == 0) {
            this.loginErrorDiv.innerText = 'Need a password';
            return;
        }
        if (this.loginWindowEmail.value.length == 0) {
            this.loginErrorDiv.innerText = 'Need an email';
            return;
        }
        this.securelyHash(this.loginWindowPassword.value).then((hashedPassword)=>{
            let message = new MessageWriter();
            message.addUint8(SendMessages.Controller.Profile);
            message.addUint8(SendMessages.ProfileMessage.Login);
            message.addString(this.loginWindowName.value);
            message.addString(hashedPassword);
            message.addString(this.loginWindowEmail.value);
            this.website.networkController.send(message.toBuffer());
            // Todo: Don't do this, or do it better. This is janky for setting up auto-login.
            localStorage.setItem('Name', this.loginWindowName.value);
            localStorage.setItem('Password', hashedPassword);
        });
    };
    removeLoginDiv = () => {
        this.hideBackgroundDiv.remove();
        this.loginDiv.remove();
        this.showingLoginWindow = false;
    };
    removePreferencesDiv = () => {
        this.hideBackgroundDiv.remove();
        this.preferencesDiv.remove();
        this.showingPreferencesWindow = false;
    };
    processEvent = (event : string) => {
        switch (event) {
            case 'Logged In':
            {
                this.removeLoginDiv();
                //Now lets add a logout button at the top right
                this.topControlPanel.appendChild(this.logoutButton);
                this.topControlPanel.appendChild(this.preferencesButton);
                //Remove the login button
                this.loginButton.remove();
                //Show name at the top
                this.topNameDiv.innerText = this.website.database.displayName;
                this.topControlPanel.appendChild(this.topNameDiv);
            }
                break;
            case "Logged Out":
            {
                this.preferencesButton.remove();
                this.logoutButton.remove();
                this.topControlPanel.appendChild(this.loginButton);
                this.topNameDiv.remove();
            }
                break;
            case 'Register Failed':
            {
                this.loginErrorDiv.innerText = 'Username taken. Or email used. One of those. Not sure.';
            }
                break;
            case 'Login Failed':
            {
                this.loginErrorDiv.innerText = 'Username or password wrong.';
            }
                break;
        }
    };
    async securelyHash(value : string) : Promise<string> {
        // Todo: When I'm no longer handling manual passwords, remove this.
        const msgUint8 = new TextEncoder().encode(value);                           // encode as (utf-8) Uint8Array
        const hashBuffer = await crypto.subtle.digest('SHA-256', msgUint8);           // hash the message
        const hashArray = Array.from(new Uint8Array(hashBuffer));                     // convert buffer to byte array
        const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join(''); // convert bytes to hex string
        return hashHex;
    }
}