import {AppController} from "../AppController";
import {Interface} from "../Utility/Interface";

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

    loginWindowName : HTMLInputElement;
    loginWindowPassword : HTMLInputElement;
    loginWindowEmail : HTMLInputElement;
    constructor(w : AppController) {
        this.website = w;
        this.makeControlPanelComponent();
        this.makePreferencesButtonComponent();
        this.makeLoginButtonComponent();
        this.makeLogoutButtonComponent();
        this.makeLoginWindowComponents();
        this.makeTopNameDivComponent();
        this.makePreferencesWindowComponents();
    }
    darkTheme() {
        this.topNameDiv.style.color = "white";
    }
    lightTheme() {
        this.topNameDiv.style.color = "Black";
    }
    preferencesButtonClicked() {
        let m : any = {};
        m['Controller'] = 'Login';
        m['Title'] = 'Get Avatar';
        this.website.networkController.send(m);
        this.website.websiteDiv.appendChild(this.hideBackgroundDiv);
        this.website.websiteDiv.appendChild(this.preferencesDiv);
        this.showingPreferencesWindow = true;
    }
    loginButtonClicked() {
        this.website.websiteDiv.appendChild(this.hideBackgroundDiv);
        this.website.websiteDiv.appendChild(this.loginDiv);
        this.showingLoginWindow = true;
    }
    logoutButtonClicked() {
        //Notify server to log out
        let m : any = {};
        m['Controller'] = 'Login';
        m['Title'] = 'Logout';
        this.website.networkController.send(m);
        //Server will tell the client to log out and the client will do all the necessaries
    }
    avatarSaveButtonClick() {
        let m : any = {};
        m['Controller'] = 'Login';
        m['Title'] = 'Set Avatar';
        m['AvatarURL'] = this.preferencesAvatarInput.value;
        this.website.networkController.send(m);
    }
    loginWindowLoginButtonClicked() {
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
    }
    login(name : string, password : string) {
        let m : any = {};
        m['Controller'] = 'Login';
        m['Title'] = 'Login';
        m['Name'] = name;
        m['Password'] = this.encrypt(password);
        this.website.networkController.send(m);
    }
    loginAlreadyEncrypted(name : string, password : string) {
        let m: any = {};
        m['Controller'] = 'Login';
        m['Title'] = 'Login';
        m['Name'] = name;
        m['Password'] = password;
        this.website.networkController.send(m);
    }
    loginWindowRegisterButtonClicked() {
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
        let m: any = {};
        m['Controller'] = 'Login';
        m['Title'] = 'Register';
        m['Name'] = this.loginWindowName.value;
        m['Password'] = this.encrypt(this.loginWindowPassword.value);
        m['Email'] = this.loginWindowEmail.value;
        this.website.networkController.send(m);
    }
    removeLoginDiv() {
        this.hideBackgroundDiv.remove();
        this.loginDiv.remove();
        this.showingLoginWindow = false;
    }
    removePreferencesDiv() {
        this.hideBackgroundDiv.remove();
        this.preferencesDiv.remove();
        this.showingPreferencesWindow = false;
    }
    processEvent(event : string) {
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
                this.topNameDiv.innerText = this.website.database.name;
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
    }
    encrypt(s : string) : string {
        /*
        var sha256 = new SHA256();
        sha256.add(s.codeUnits);
        var digest = sha256.close();
        String hexString = CryptoUtils.bytesToHex(digest);
        return hexString;
        */
        return s;
    }
    /********************Make GUI Components*************/
    makeControlPanelComponent() {
        //Make the control panel that sits above the chatbox
        this.topControlPanel = Interface.Create({type: 'div'});
        this.topControlPanel.style.position = "absolute";
        this.topControlPanel.style.width = "300px";
        this.topControlPanel.style.height = "80px";
        this.topControlPanel.style.top = "10px";
        this.topControlPanel.style.right = "2px";
        this.website.websiteDiv.appendChild(this.topControlPanel);
    }
    makePreferencesButtonComponent() {
        //Add the preferences button to it
        this.preferencesButton = Interface.Create({type: 'button'});
        this.preferencesButton.style.width = "140px";
        this.preferencesButton.style.height = "40px";
        this.preferencesButton.style.backgroundColor = "white";
        this.preferencesButton.style.borderRadius = "4px";
        this.preferencesButton.style.borderStyle = "solid";
        this.preferencesButton.style.borderWidth = "1px";
        this.preferencesButton.style.borderColor = "#DDDDDD";
        this.preferencesButton.style.fontSize = "15px";
        this.preferencesButton.style.top = "5px";
        this.preferencesButton.style.left = "5px";
        this.preferencesButton.style.position = "absolute";
        this.preferencesButton.style.outline = 'none';
        this.preferencesButton.dataset['active'] = "{ background: #BBBBBB; }";
        this.preferencesButton.innerText = 'Preferences';
        this.preferencesButton.onclick = () => {
            this.preferencesButtonClicked();
        };
        //topControlPanel.appendChild(loginButton);
    }
    makeLoginButtonComponent() {
        //Add the login button to it
        this.loginButton = Interface.Create({type: 'button'});
        this.loginButton.style.width = "140px";
        this.loginButton.style.height = "40px";
        this.loginButton.style.backgroundColor = "white";
        this.loginButton.style.borderRadius = "4px";
        this.loginButton.style.borderStyle = "solid";
        this.loginButton.style.borderWidth = "1px";
        this.loginButton.style.borderColor = "#DDDDDD";
        this.loginButton.style.fontSize = "15px";
        this.loginButton.style.bottom = "5px";
        this.loginButton.style.position = "absolute";
        this.loginButton.style.outline = 'none';
        this.loginButton.dataset['active'] = "{ background: #BBBBBB; }";
        this.loginButton.innerText = 'Login/Register';
        this.loginButton.onclick = () => {
            this.loginButtonClicked();
        };
        this.topControlPanel.appendChild(this.loginButton);
    }
    makeLogoutButtonComponent() {
        this.logoutButton = Interface.Create({type: 'button'});
        this.logoutButton.style.width = "140px";
        this.logoutButton.style.height = "40px";
        this.logoutButton.style.backgroundColor = "white";
        this.logoutButton.style.borderRadius = "4px";
        this.logoutButton.style.borderStyle = "solid";
        this.logoutButton.style.borderWidth = "1px";
        this.logoutButton.style.borderColor = "#DDDDDD";
        this.logoutButton.style.fontSize = "15px";
        this.logoutButton.style.top = "5px";
        this.logoutButton.style.position = "absolute";
        this.logoutButton.style.outline = "none";
        this.logoutButton.style.right = "5px";
        this.logoutButton.dataset["active"] = "{ background: #BBBBBB; }";
        this.logoutButton.innerText = 'Logout';
        this.logoutButton.onclick = () => {
            this.logoutButtonClicked();
        };
    }
    makePreferencesWindowComponents() {
        this.preferencesDiv = Interface.Create({type: 'div'});
        this.preferencesDiv.style.position = "absolute";
        this.preferencesDiv.style.width = "400px";
        this.preferencesDiv.style.height = "300px";
        this.preferencesDiv.style.borderRadius = "4px";
            this.preferencesDiv.style.borderStyle = "solid";
            this.preferencesDiv.style.borderWidth = "1px";
            this.preferencesDiv.style.borderColor = "#DDDDDD";
            this.preferencesDiv.style.backgroundColor = "white";
            this.preferencesDiv.style.top = "calc(50% - 150px)";
            this.preferencesDiv.style.left = "calc(50% - 200px)";
        //Add the visuals to the login div
        let title : HTMLDivElement = Interface.Create({type: 'div'});
        title.innerText = "Preferences";
            title.style.position = 'absolute';
            title.style.top = '15px';
            title.style.left = '0px';
            title.style.width = '100%';
            title.style.textAlign = 'center';
            title.style.fontSize = '25px';
        this.preferencesDiv.appendChild(title);
        let rule : HTMLHRElement = Interface.Create({type: 'hr'});
            rule.style.width = '70%';
            rule.style.top = '35px';
            rule.style.position = 'absolute';
            rule.style.left = '15%';
        this.preferencesDiv.appendChild(rule);
        this.preferencesAvatarInput = Interface.Create({type: 'input'});
        this.preferencesAvatarInput.style.width = '70%';
            this.preferencesAvatarInput.style.position = 'absolute';
            this.preferencesAvatarInput.style.top = '45px';
            this.preferencesAvatarInput.style.right = '5px';
        this.preferencesDiv.appendChild(this.preferencesAvatarInput);
        let avLabel : HTMLLabelElement = Interface.Create({type: 'label'});
        avLabel.innerText = "Avatar";
        avLabel.style.width = '25%';
            avLabel.style.position = 'absolute';
            avLabel.style.top = '45px';
            avLabel.style.left = '5px';
        this.preferencesDiv.appendChild(avLabel);
        let avSaveBtn : HTMLButtonElement = Interface.Create({type: 'button'});
        avSaveBtn.innerText = "Save Avatar";
        avSaveBtn.style.width = '120px';
            avSaveBtn.style.position = 'absolute';
            avSaveBtn.style.top = '65px';
            avSaveBtn.style.right = '5px';
        this.preferencesDiv.appendChild(avSaveBtn);
        avSaveBtn.onclick = () => {this.avatarSaveButtonClick();};
    }
    makeLoginWindowComponents() {
        //Make the login/register div that's invisible until the button is clicked
        this.hideBackgroundDiv = Interface.Create({type: 'div'});
        this.hideBackgroundDiv.style;
            this.hideBackgroundDiv.style.position = "absolute";
            this.hideBackgroundDiv.style.width = "100%";
            this.hideBackgroundDiv.style.height = "100%";
            this.hideBackgroundDiv.style.backgroundColor = "rgba(0, 0, 0, 0.5)";
            this.hideBackgroundDiv.style.top = "0px";
            this.hideBackgroundDiv.style.left = "0px";
        this.hideBackgroundDiv.onclick = () => {
            if (this.showingLoginWindow) {
                this.removeLoginDiv();
            } else {
                this.removePreferencesDiv();
            }
        };
        this.loginDiv = Interface.Create({type: 'div'});
            this.loginDiv.style.position = "absolute";
            this.loginDiv.style.width = "400px";
            this.loginDiv.style.height = "300px";
            this.loginDiv.style.borderRadius = "4px";
            this.loginDiv.style.borderStyle = "solid";
            this.loginDiv.style.borderWidth = "1px";
            this.loginDiv.style.borderColor = "#DDDDDD";
            this.loginDiv.style.backgroundColor = "white";
            this.loginDiv.style.top = "calc(50% - 150px)";
            this.loginDiv.style.left = "calc(50% - 200px)";
        //Add the visuals to the login div
        let title : HTMLDivElement = Interface.Create({type: 'div'});
        title.innerText = "Login or Register";
            title.style.position = 'absolute';
            title.style.top = '15px';
            title.style.left = '0px';
            title.style.width = '100%';
            title.style.textAlign = 'center';
            title.style.fontSize = '25px';
        this.loginDiv.appendChild(title);
        let rule : HTMLHRElement = Interface.Create({type: 'hr'});
            rule.style.width = '70%';
            rule.style.top = '35px';
            rule.style.position = 'absolute';
            rule.style.left = '15%';
        this.loginDiv.appendChild(rule);

        //control panel login window login button
        let loginWindowLoginButton : HTMLButtonElement = Interface.Create({type: 'button'});
            loginWindowLoginButton.style.width = "140px";
            loginWindowLoginButton.style.height = "40px";
            loginWindowLoginButton.style.backgroundColor = "white";
            loginWindowLoginButton.style.borderRadius = "4px";
            loginWindowLoginButton.style.borderStyle = "solid";
            loginWindowLoginButton.style.borderWidth = "1px";
            loginWindowLoginButton.style.borderColor = "#DDDDDD";
            loginWindowLoginButton.style.fontSize = "15px";
            loginWindowLoginButton.style.bottom = "5px";
            loginWindowLoginButton.style.position = "absolute";
            loginWindowLoginButton.style.outline = "none";
            loginWindowLoginButton.style.right = "5px";
        loginWindowLoginButton.dataset['active'] = "{ background: #BBBBBB; }";
        loginWindowLoginButton.innerText = 'Login';
        loginWindowLoginButton.onclick = () => {
            this.loginWindowLoginButtonClicked();
        };
        this.loginDiv.appendChild(loginWindowLoginButton);

        let registerWindowRegisterButton : HTMLButtonElement = Interface.Create({type: 'button'});
            registerWindowRegisterButton.style.width = "140px";
            registerWindowRegisterButton.style.height = "40px";
            registerWindowRegisterButton.style.backgroundColor = "white";
            registerWindowRegisterButton.style.borderRadius = "4px";
            registerWindowRegisterButton.style.borderStyle = "solid";
            registerWindowRegisterButton.style.borderWidth = "1px";
            registerWindowRegisterButton.style.borderColor = '#DDDDDD';
            registerWindowRegisterButton.style.fontSize = "15px";
            registerWindowRegisterButton.style.bottom = "5px";
            registerWindowRegisterButton.style.position = "absolute";
            registerWindowRegisterButton.style.outline = "none";
            registerWindowRegisterButton.style.left = "5px";
        registerWindowRegisterButton.dataset['active'] = "{ background: #BBBBBB; }";
        registerWindowRegisterButton.innerText = 'Register';
        registerWindowRegisterButton.onclick = () => {
            this.loginWindowRegisterButtonClicked();
        };
        this.loginDiv.appendChild(registerWindowRegisterButton);

        this.loginWindowName = Interface.Create({type: 'input'});
            this.loginWindowName.style.width = "90%";
            this.loginWindowName.style.height = "30px";
            this.loginWindowName.style.backgroundColor = "white";
            this.loginWindowName.style.borderRadius = "2px";
            this.loginWindowName.style.borderStyle = "solid";
            this.loginWindowName.style.borderWidth = "1px";
            this.loginWindowName.style.borderColor = "#DDDDDD";
            this.loginWindowName.style.fontSize = "25px";
            this.loginWindowName.style.top = "70px";
            this.loginWindowName.style.position = "absolute";
            this.loginWindowName.style.outline = "none";
            this.loginWindowName.style.left = "5%";
        this.loginWindowName.placeholder = 'Name';
        this.loginDiv.appendChild(this.loginWindowName);

        this.loginWindowPassword = Interface.Create({type: 'input'});
        this.loginWindowPassword.type = 'Password';
            this.loginWindowPassword.style.width = "90%";
            this.loginWindowPassword.style.height = "30px";
            this.loginWindowPassword.style.backgroundColor = "white";
            this.loginWindowPassword.style.borderRadius = "2px";
            this.loginWindowPassword.style.borderStyle = "solid";
            this.loginWindowPassword.style.borderWidth = "1px";
            this.loginWindowPassword.style.borderColor = "#DDDDDD";
            this.loginWindowPassword.style.fontSize = "25px";
            this.loginWindowPassword.style.top = "110px";
            this.loginWindowPassword.style.position = "absolute";
            this.loginWindowPassword.style.outline = "none";
            this.loginWindowPassword.style.left = "5%";
        this.loginWindowPassword.placeholder = 'Password';
        this.loginDiv.appendChild(this.loginWindowPassword);

        this.loginWindowEmail = Interface.Create({type: 'input'});
            this.loginWindowEmail.style.width = "90%";
            this.loginWindowEmail.style.height = "30px";
            this.loginWindowEmail.style.backgroundColor = "white";
            this.loginWindowEmail.style.borderRadius = "2px";
            this.loginWindowEmail.style.borderStyle = "solid";
            this.loginWindowEmail.style.borderWidth = "1px";
            this.loginWindowEmail.style.borderColor = "#DDDDDD";
            this.loginWindowEmail.style.fontSize = "25px";
            this.loginWindowEmail.style.top = "190px";
            this.loginWindowEmail.style.position = "absolute";
            this.loginWindowEmail.style.outline = "none";
            this.loginWindowEmail.style.left = "5%";
        this.loginWindowEmail.placeholder = 'Email for registration';
        this.loginDiv.appendChild(this.loginWindowEmail);

        this.loginErrorDiv = Interface.Create({type: 'div'});
            this.loginErrorDiv.style.width = "100%";
            this.loginErrorDiv.style.position = "absolute";
            this.loginErrorDiv.style.textAlign = "center";
            this.loginErrorDiv.style.color = "red";
            this.loginErrorDiv.style.fontSize = "15px";
            this.loginErrorDiv.style.left = "0px";
            this.loginErrorDiv.style.top = "230px";
        this.loginDiv.appendChild(this.loginErrorDiv);
    }
    makeTopNameDivComponent() {
        //Make a top name div for after logging in
        this.topNameDiv = Interface.Create({type: 'div'});
            this.topNameDiv.style.width = "100%";
            this.topNameDiv.style.position = "absolute";
            this.topNameDiv.style.textAlign = "center";
            this.topNameDiv.style.color = "black";
            this.topNameDiv.style.fontSize = "20px";
            this.topNameDiv.style.left = "0px";
            this.topNameDiv.style.bottom = "0px";
    }
}