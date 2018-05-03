library ControlPanelGui;

import 'dart:html';
import 'ControlPanel.dart';

class ControlPanelGui {
  ButtonElement loginButton, logoutButton, preferencesButton;
  DivElement topControlPanel;
  DivElement loginRegisterDiv, loginDiv, registerDiv;
  DivElement hideBackgroundDiv;
  DivElement loginErrorDiv;
  DivElement topNameDiv;
  DivElement preferencesDiv;
  InputElement preferencesAvatarInput;
  InputElement loginWindowName,
      loginWindowPassword,
      registerWindowEmail,
      registerWindowUsername,
      registerWindowPassword,
      registerWindowAvatarURL,
      registerWindowDisplayName;
  InputElement registerRadio, loginRadio;
  ControlPanel controlPanel;
  ControlPanelGui(this.controlPanel) {
    makeControlPanelComponent();
    makePreferencesButtonComponent();
    makeLoginButtonComponent();
    makeLogoutButtonComponent();
    makeLoginWindowComponents();
    makeTopNameDivComponent();
    makePreferencesWindowComponents();
  }

  void loginSegmentClicked() {
    registerDiv.remove();
    loginRegisterDiv.insertBefore(loginDiv, loginRegisterDiv.firstChild);
  }

  void registerSegmentClicked() {
    loginDiv.remove();
    loginRegisterDiv.insertBefore(registerDiv, loginRegisterDiv.firstChild);
  }

  void makeControlPanelComponent() {
    //Make the control panel that sits above the chatbox
    topControlPanel = new DivElement();
    topControlPanel.id = "topControlPanel";
    controlPanel.website.websiteDiv.append(topControlPanel);
  }

  void makePreferencesButtonComponent() {
    //Add the preferences button to it
    preferencesButton = new ButtonElement();
    preferencesButton.id = "preferencesButton";
    preferencesButton.text = 'Account';
    preferencesButton.onClick.listen((var d) {
      controlPanel.preferencesButtonClicked();
    });
    //topControlPanel.append(loginButton);
  }

  void makeLoginButtonComponent() {
    //Add the login button to it
    loginButton = new ButtonElement();
    loginButton.id = "loginButton";
    loginButton.text = 'Login';
    loginButton.onClick.listen((var d) {
      controlPanel.loginButtonClicked();
    });
    topControlPanel.append(loginButton);
  }

  void makeLogoutButtonComponent() {
    logoutButton = new ButtonElement();
    logoutButton.id = "logoutButton";
    logoutButton.text = 'Logout';
    logoutButton.onClick.listen((var d) {
      controlPanel.logoutButtonClicked();
    });
  }

  void makePreferencesWindowComponents() {
    preferencesDiv = new DivElement();
    preferencesDiv.id = "preferencesDiv";
    //Add the visuals to the login div
    DivElement title = new DivElement();
    title.id = "preferencesTitleDiv";
    title.text = "Account";
    preferencesDiv.append(title);

    preferencesAvatarInput = new InputElement();
    preferencesAvatarInput.id = "preferencesAvatarInput";
    preferencesDiv.append(preferencesAvatarInput);
    LabelElement avLabel = new LabelElement();
    avLabel.id = "preferencesAvatarLabel";
    avLabel.text = "Avatar";
    preferencesDiv.append(avLabel);
    ButtonElement avSaveBtn = new ButtonElement();
    avSaveBtn.text = "Save Avatar";
    avSaveBtn.id = "avSaveBtn";
    preferencesDiv.append(avSaveBtn);
    avSaveBtn.onClick.listen((_) {
      controlPanel.avatarSaveButtonClick();
    });
  }

  void makeLoginWindowComponents() {
    //Make the login/register div that's invisible until the button is clicked
    hideBackgroundDiv = new DivElement();
    hideBackgroundDiv.id = "hideBackgroundDiv";
    hideBackgroundDiv.onClick.listen((var d) {
      if (controlPanel.showingLoginWindow) {
        controlPanel.removeLoginRegisterDiv();
      } else {
        controlPanel.removePreferencesDiv();
      }
    });
    loginRegisterDiv = new DivElement();
    loginRegisterDiv.id = "loginRegisterDiv";

    loginDiv = new DivElement();
    loginDiv.id = "loginDiv";
    loginRegisterDiv.append(loginDiv);

    registerDiv = new DivElement();
    registerDiv.id = "registerDiv";

    //Login/Register segmented control
    SpanElement loginRegisterSwitcher = new SpanElement();
    loginRegisterSwitcher.className = "segmented";
    loginRegisterDiv.append(loginRegisterSwitcher);
    LabelElement registerRadioLabel = new LabelElement();
    registerRadioLabel.id = "registerRadioLabel";
    loginRegisterSwitcher.append(registerRadioLabel);
    registerRadio = new InputElement();
    registerRadio.type = "radio";
    registerRadio.name = "loginRegisterRadio";
    registerRadio.onClick.listen((_) {
      registerSegmentClicked();
    });
    registerRadioLabel.append(registerRadio);
    SpanElement registerRadioSpan = new SpanElement();
    registerRadioSpan.text = "Register";
    registerRadioSpan.className = "label";
    registerRadioLabel.append(registerRadioSpan);
    LabelElement loginRadioLabel = new LabelElement();
    loginRadioLabel.id = "loginRadioLabel";
    loginRegisterSwitcher.append(loginRadioLabel);
    loginRadio = new InputElement();
    loginRadio.onClick.listen((_) {
      loginSegmentClicked();
    });
    loginRadio.type = "radio";
    loginRadio.name = "loginRegisterRadio";
    loginRadio.checked = true;
    loginRadioLabel.append(loginRadio);
    SpanElement loginRadioSpan = new SpanElement();
    loginRadioSpan.text = "Login";
    loginRadioSpan.className = "label";
    loginRadioLabel.append(loginRadioSpan);

    ButtonElement loginWindowLoginButton = new ButtonElement();
    loginWindowLoginButton.id = "loginWindowLoginButton";
    loginWindowLoginButton.text = 'Login';
    loginWindowLoginButton.onClick.listen((var d) {
      controlPanel.loginWindowLoginButtonClicked();
    });
    loginDiv.append(loginWindowLoginButton);

    ButtonElement registerWindowRegisterButton = new ButtonElement();
    registerWindowRegisterButton.id = "registerWindowRegisterButton";
    registerWindowRegisterButton.text = 'Register';
    registerWindowRegisterButton.onClick.listen((var d) {
      controlPanel.loginWindowRegisterButtonClicked();
    });
    registerDiv.append(registerWindowRegisterButton);

    loginWindowName = new InputElement();
    loginWindowName.id = "loginWindowName";
    loginWindowName.placeholder = 'Username';
    loginWindowName.autocomplete = "off";
    loginDiv.append(loginWindowName);

    registerWindowUsername = new InputElement();
    registerWindowUsername.id = "registerWindowUsername";
    registerWindowUsername.placeholder = 'Username';
    registerWindowUsername.autocomplete = "off";
    registerDiv.append(registerWindowUsername);

    loginWindowPassword = new InputElement();
    loginWindowPassword.type = 'Password';
    loginWindowPassword.id = "loginWindowPassword";
    loginWindowPassword.placeholder = 'Password';
    loginWindowPassword.autocomplete = "off";
    loginWindowPassword.onKeyPress.listen((KeyboardEvent ke) {
      if (ke.keyCode == 13) {
        controlPanel.loginWindowLoginButtonClicked();
      }
    });
    loginDiv.append(loginWindowPassword);

    registerWindowPassword = new InputElement();
    registerWindowPassword.type = 'Password';
    registerWindowPassword.id = "registerWindowPassword";
    registerWindowPassword.placeholder = 'Password';
    registerWindowPassword.autocomplete = "off";
    registerDiv.append(registerWindowPassword);

    registerWindowAvatarURL = new InputElement();
    registerWindowAvatarURL.id = "registerWindowAvatarURL";
    registerWindowAvatarURL.placeholder = 'Avatar URL (Make it cool)';
    registerWindowAvatarURL.autocomplete = "off";
    registerDiv.append(registerWindowAvatarURL);

    registerWindowDisplayName = new InputElement();
    registerWindowDisplayName.id = "registerWindowDisplayName";
    registerWindowDisplayName.placeholder = 'Display Name';
    registerWindowDisplayName.autocomplete = "off";
    registerDiv.append(registerWindowDisplayName);

    registerWindowEmail = new InputElement();
    registerWindowEmail.id = "registerWindowEmail";
    registerWindowEmail.placeholder = 'Email for registration';
    registerWindowEmail.autocomplete = "off";
    registerWindowEmail.onKeyPress.listen((KeyboardEvent ke) {
      if (ke.keyCode == 13) {
        controlPanel.loginWindowRegisterButtonClicked();
      }
    });
    registerDiv.append(registerWindowEmail);

    loginErrorDiv = new DivElement();
    loginErrorDiv.id = "loginErrorDiv";
    loginRegisterDiv.append(loginErrorDiv);
  }

  void makeTopNameDivComponent() {
    //Make a top name div for after logging in
    topNameDiv = new DivElement();
    topNameDiv.id = "topNameDiv";
  }
}