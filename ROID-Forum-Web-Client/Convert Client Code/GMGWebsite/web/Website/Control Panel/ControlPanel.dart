library ControlPanel;
import '../Website.dart';
//import 'package:crypto/crypto.dart';
import 'ControlPanelGui.dart';
import '../WebsiteMsgCreator.dart';

class ControlPanel {
  Website website;
  bool showingLoginWindow = false,
      showingPreferencesWindow = false;
  ControlPanelGui controlPanelGui;
  ControlPanel(Website w) {
    website = w;
    controlPanelGui = new ControlPanelGui(this);
  }
  void preferencesButtonClicked() {
    website.networkingController.SendBinary(WebsiteMsgCreator.GetAvatar(), true);
    website.websiteDiv.append(controlPanelGui.hideBackgroundDiv);
    website.websiteDiv.append(controlPanelGui.preferencesDiv);
    showingPreferencesWindow = true;
  }
  void loginButtonClicked() {
    website.websiteDiv.append(controlPanelGui.hideBackgroundDiv);
    website.websiteDiv.append(controlPanelGui.loginRegisterDiv);
    showingLoginWindow = true;
  }
  void logoutButtonClicked() {
    //Notify server to log out
    website.networkingController.SendBinary(WebsiteMsgCreator.LogOut(), true);
    //Server will tell the client to log out and the client will do all the necessaries
  }
  void avatarSaveButtonClick() {
    website.networkingController.SendBinary(WebsiteMsgCreator.SetAvatar(controlPanelGui.preferencesAvatarInput.value), true);
  }
  void loginWindowLoginButtonClicked() {
    controlPanelGui.loginErrorDiv.text = "";
    if (controlPanelGui.loginWindowName.value.length == 0) {
      controlPanelGui.loginErrorDiv.text = 'Need a name';
      return;
    }
    if (controlPanelGui.loginWindowPassword.value.length == 0) {
      controlPanelGui.loginErrorDiv.text = 'Need a password';
      return;
    }
    login(controlPanelGui.loginWindowName.value, controlPanelGui.loginWindowPassword.value);
  }
  void login(String name, String password) {
    website.networkingController.SendBinary(WebsiteMsgCreator.LogIn(name, encrypt(password)), true);
  }
  void loginAlreadyEncrypted(String name, String password) {
    website.networkingController.SendBinary(WebsiteMsgCreator.LogIn(name, password), true);
  }
  void loginWindowRegisterButtonClicked() {
    controlPanelGui.loginErrorDiv.text = "";
    if (controlPanelGui.registerWindowUsername.value.length == 0) {
      controlPanelGui.loginErrorDiv.text = 'Need a name';
      return;
    }
    if (controlPanelGui.registerWindowPassword.value.length == 0) {
      controlPanelGui.loginErrorDiv.text = 'Need a password';
      return;
    }
    if (controlPanelGui.registerWindowEmail.value.length == 0) {
      controlPanelGui.loginErrorDiv.text = 'Need an email';
      return;
    }
    if (controlPanelGui.registerWindowDisplayName.value.length == 0) {
      controlPanelGui.loginErrorDiv.text = 'Need a display name';
      return;
    }
    String name = controlPanelGui.registerWindowUsername.value;
    String password = encrypt(controlPanelGui.registerWindowPassword.value);
    String email = controlPanelGui.registerWindowEmail.value;
    String displayName = controlPanelGui.registerWindowDisplayName.value;
    String avatarURL = controlPanelGui.registerWindowAvatarURL.value;
    website.networkingController.SendBinary(WebsiteMsgCreator.Register(name, password, email, displayName, avatarURL), true);
  }
  void removeLoginRegisterDiv() {
    controlPanelGui.hideBackgroundDiv.remove();
    controlPanelGui.loginRegisterDiv.remove();
    showingLoginWindow = false;
  }
  void removePreferencesDiv() {
    controlPanelGui.hideBackgroundDiv.remove();
    controlPanelGui.preferencesDiv.remove();
    showingPreferencesWindow = false;
  }
  void resetLoginRegisterWindow() {
    controlPanelGui.loginWindowName.value = "";
    controlPanelGui.loginWindowPassword.value = "";
    controlPanelGui.registerWindowUsername.value = "";
    controlPanelGui.registerWindowPassword.value = "";
    controlPanelGui.registerWindowEmail.value = "";
    controlPanelGui.registerWindowAvatarURL.value = "";
    website.controlPanel.controlPanelGui.loginWindowPassword.value = "";
    controlPanelGui.loginSegmentClicked();
    controlPanelGui.loginRadio.checked = true;
  }
  void processEvent(String event) {
    switch (event) {
      case 'Logged In':
        {
          removeLoginRegisterDiv();
          //Now lets add a logout button at the top right
          controlPanelGui.topControlPanel.append(controlPanelGui.logoutButton);
          controlPanelGui.topControlPanel.append(controlPanelGui.preferencesButton);
          //Remove the login button
          controlPanelGui.loginButton.remove();
          //Show name at the top
          controlPanelGui.topNameDiv.text = "Welcome, ${website.database.name}!";
          controlPanelGui.topControlPanel.append(controlPanelGui.topNameDiv);
        }
        break;
      case "Logged Out":
        {
          controlPanelGui.preferencesButton.remove();
          controlPanelGui.logoutButton.remove();
          controlPanelGui.topControlPanel.append(controlPanelGui.loginButton);
          controlPanelGui.topNameDiv.remove();
        }
        break;
      case 'Register Failed':
        {
          controlPanelGui.loginErrorDiv.text = 'Username taken. Or email used. One of those. Not sure.';
        }
        break;
      case 'Login Failed':
        {
          controlPanelGui.loginErrorDiv.text = 'Username or password wrong.';
        }
        break;
    }
  }
  String encrypt(String s) {
    //Need to salt the password, generate salt and return it too
    //Make the salt the username that way it's really easy
    //Also use the pbkdf2 or any password strengthening function that will
    //hash the password multiple times. Increases workload for people
    //that are trying to guess the password through a dictionary
    /*
    var sha256 = new SHA256();
    sha256.add(s.codeUnits);
    var digest = sha256.close();
    String hexString = CryptoUtils.bytesToHex(digest);
    return hexString;
    */
    return s;
  }
}