library ControlPanel;
import 'Website.dart';
import 'dart:html';
import 'package:crypto/crypto.dart';

class ControlPanel {
  Website website;
  bool showingLoginWindow = false,
      showingPreferencesWindow = false;
  ButtonElement loginButton, logoutButton, preferencesButton;
  DivElement topControlPanel;
  DivElement loginDiv;
  DivElement hideBackgroundDiv;
  DivElement loginErrorDiv;
  DivElement topNameDiv;
  DivElement preferencesDiv;
  InputElement preferencesAvatarInput;

  InputElement loginWindowName, loginWindowPassword, loginWindowEmail;
  ControlPanel(Website w) {
    website = w;
    makeControlPanelComponent();
    makePreferencesButtonComponent();
    makeLoginButtonComponent();
    makeLogoutButtonComponent();
    makeLoginWindowComponents();
    makeTopNameDivComponent();
    makePreferencesWindowComponents();
  }
  void darkTheme() {
    topNameDiv.style.color = "white";
  }
  void lightTheme() {
    topNameDiv.style.color = "Black";
  }
  void preferencesButtonClicked() {
    Map m = new Map();
        m['Controller'] = 'Login';
        m['Title'] = 'Get Avatar';
        website.networkingController.Send(m);
    website.websiteDiv.append(hideBackgroundDiv);
    website.websiteDiv.append(preferencesDiv);
    showingPreferencesWindow = true;
  }
  void loginButtonClicked() {
    website.websiteDiv.append(hideBackgroundDiv);
    website.websiteDiv.append(loginDiv);
    showingLoginWindow = true;
  }
  void logoutButtonClicked() {
    //Notify server to log out
    Map m = new Map();
    m['Controller'] = 'Login';
    m['Title'] = 'Logout';
    website.networkingController.Send(m);
    //Server will tell the client to log out and the client will do all the necessaries
  }
  void avatarSaveButtonClick() {
    Map m = new Map();
            m['Controller'] = 'Login';
            m['Title'] = 'Set Avatar';
            m['AvatarURL'] = preferencesAvatarInput.value;
            website.networkingController.Send(m);
  }
  void loginWindowLoginButtonClicked() {
    loginErrorDiv.text = "";
    if (loginWindowName.value.length == 0) {
      loginErrorDiv.text = 'Need a name';
      return;
    }
    if (loginWindowPassword.value.length == 0) {
      loginErrorDiv.text = 'Need a password';
      return;
    }
    login(loginWindowName.value, loginWindowPassword.value);
  }
  void login(String name, String password) {
    Map m = new Map();
    m['Controller'] = 'Login';
    m['Title'] = 'Login';
    m['Name'] = name;
    m['Password'] = encrypt(password);
    website.networkingController.Send(m);
  }
  void loginAlreadyEncrypted(String name, String password) {
    Map m = new Map();
    m['Controller'] = 'Login';
    m['Title'] = 'Login';
    m['Name'] = name;
    m['Password'] = password;
    website.networkingController.Send(m);
  }
  void loginWindowRegisterButtonClicked() {
    loginErrorDiv.text = "";
    if (loginWindowName.value.length == 0) {
      loginErrorDiv.text = 'Need a name';
      return;
    }
    if (loginWindowPassword.value.length == 0) {
      loginErrorDiv.text = 'Need a password';
      return;
    }
    if (loginWindowEmail.value.length == 0) {
      loginErrorDiv.text = 'Need an email';
      return;
    }
    Map m = new Map();
    m['Controller'] = 'Login';
    m['Title'] = 'Register';
    m['Name'] = loginWindowName.value;
    m['Password'] = encrypt(loginWindowPassword.value);
    m['Email'] = loginWindowEmail.value;
    website.networkingController.Send(m);
  }
  void removeLoginDiv() {
    hideBackgroundDiv.remove();
    loginDiv.remove();
    showingLoginWindow = false;
  }
  void removePreferencesDiv() {
    hideBackgroundDiv.remove();
    preferencesDiv.remove();
    showingPreferencesWindow = false;
  }
  void processEvent(String event) {
    switch (event) {
      case 'Logged In':
        {
          removeLoginDiv();
          //Now lets add a logout button at the top right
          topControlPanel.append(logoutButton);
          topControlPanel.append(preferencesButton);
          //Remove the login button
          loginButton.remove();
          //Show name at the top
          topNameDiv.text = website.database.name;
          topControlPanel.append(topNameDiv);
        }
        break;
      case "Logged Out":
        {
          preferencesButton.remove();
          logoutButton.remove();
          topControlPanel.append(loginButton);
          topNameDiv.remove();
        }
        break;
      case 'Register Failed':
        {
          loginErrorDiv.text = 'Username taken. Or email used. One of those. Not sure.';
        }
        break;
      case 'Login Failed':
        {
          loginErrorDiv.text = 'Username or password wrong.';
        }
        break;
    }
  }
  String encrypt(String s) {
    var sha256 = new SHA256();
    sha256.add(s.codeUnits);
    var digest = sha256.close();
    String hexString = CryptoUtils.bytesToHex(digest);
    return hexString;
  }
  /********************Make GUI Components*************/
  void makeControlPanelComponent() {
    //Make the control panel that sits above the chatbox
    topControlPanel = new DivElement();
    topControlPanel.style
        ..position = "absolute"
        ..width = "300px"
        ..height = "80px"
        ..top = "10px"
        ..right = "2px";
    website.websiteDiv.append(topControlPanel);
  }
  void makePreferencesButtonComponent() {
    //Add the preferences button to it
    preferencesButton = new ButtonElement();
    preferencesButton.style
        ..width = "140px"
        ..height = "40px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "15px"
        ..top = "5px"
        ..left = "5px"
        ..position = "absolute"
        ..outline = 'none';
    preferencesButton.dataset['active'] = "{ background: #BBBBBB; }";
    preferencesButton.text = 'Preferences';
    preferencesButton.onClick.listen((var d) {
      preferencesButtonClicked();
    });
    //topControlPanel.append(loginButton);
  }
  void makeLoginButtonComponent() {
    //Add the login button to it
    loginButton = new ButtonElement();
    loginButton.style
        ..width = "140px"
        ..height = "40px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "15px"
        ..bottom = "5px"
        ..position = "absolute"
        ..outline = 'none';
    loginButton.dataset['active'] = "{ background: #BBBBBB; }";
    loginButton.text = 'Login/Register';
    loginButton.onClick.listen((var d) {
      loginButtonClicked();
    });
    topControlPanel.append(loginButton);
  }
  void makeLogoutButtonComponent() {
    logoutButton = new ButtonElement();
    logoutButton.style
        ..width = "140px"
        ..height = "40px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "15px"
        ..top = "5px"
        ..position = "absolute"
        ..outline = "none"
        ..right = "5px";
    logoutButton.dataset["active"] = "{ background: #BBBBBB; }";
    logoutButton.text = 'Logout';
    logoutButton.onClick.listen((var d) {
      logoutButtonClicked();
    });
  }
  void makePreferencesWindowComponents() {
    preferencesDiv = new DivElement();
    preferencesDiv.style
        ..position = "absolute"
        ..width = "400px"
        ..height = "300px"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..backgroundColor = "white"
        ..top = "calc(50% - 150px)"
        ..left = "calc(50% - 200px)";
    //Add the visuals to the login div
    DivElement title = new DivElement();
    title.text = "Preferences";
    title.style
        ..position = 'absolute'
        ..top = '15px'
        ..left = '0px'
        ..width = '100%'
        ..textAlign = 'center'
        ..fontSize = '25px';
    preferencesDiv.append(title);
    HRElement rule = new HRElement();
    rule.style
        ..width = '70%'
        ..top = '35px'
        ..position = 'absolute'
        ..left = '15%';
    preferencesDiv.append(rule);
    preferencesAvatarInput = new InputElement();
    preferencesAvatarInput.style .. width = '70%'
        ..position = 'absolute'
        ..top = '45px'
        ..right = '5px';
    preferencesDiv.append(preferencesAvatarInput);
    LabelElement avLabel = new LabelElement();
    avLabel.text = "Avatar";
    avLabel.style .. width = '25%'
        ..position = 'absolute'
        ..top = '45px'
        ..left = '5px';
    preferencesDiv.append(avLabel);
    ButtonElement avSaveBtn = new ButtonElement();
    avSaveBtn.text = "Save Avatar";
    avSaveBtn.style .. width = '120px'
        ..position = 'absolute'
        ..top = '65px'
        ..right = '5px';
    preferencesDiv.append(avSaveBtn);
    avSaveBtn.onClick.listen((_){avatarSaveButtonClick();});
  }
  void makeLoginWindowComponents() {
    //Make the login/register div that's invisible until the button is clicked
    hideBackgroundDiv = new DivElement();
    hideBackgroundDiv.style
        ..position = "absolute"
        ..width = "100%"
        ..height = "100%"
        ..backgroundColor = "rgba(0, 0, 0, 0.5)"
        ..top = "0px"
        ..left = "0px";
    hideBackgroundDiv.onClick.listen((var d) {
      if (showingLoginWindow) {
        removeLoginDiv();
      } else {
        removePreferencesDiv();
      }
    });
    loginDiv = new DivElement();
    loginDiv.style
        ..position = "absolute"
        ..width = "400px"
        ..height = "300px"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..backgroundColor = "white"
        ..top = "calc(50% - 150px)"
        ..left = "calc(50% - 200px)";
    //Add the visuals to the login div
    DivElement title = new DivElement();
    title.text = "Login or Register";
    title.style
        ..position = 'absolute'
        ..top = '15px'
        ..left = '0px'
        ..width = '100%'
        ..textAlign = 'center'
        ..fontSize = '25px';
    loginDiv.append(title);
    HRElement rule = new HRElement();
    rule.style
        ..width = '70%'
        ..top = '35px'
        ..position = 'absolute'
        ..left = '15%';
    loginDiv.append(rule);

    //control panel login window login button
    ButtonElement loginWindowLoginButton = new ButtonElement();
    loginWindowLoginButton.style
        ..width = "140px"
        ..height = "40px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "15px"
        ..bottom = "5px"
        ..position = "absolute"
        ..outline = "none"
        ..right = "5px";
    loginWindowLoginButton.dataset['active'] = "{ background: #BBBBBB; }";
    loginWindowLoginButton.text = 'Login';
    loginWindowLoginButton.onClick.listen((var d) {
      loginWindowLoginButtonClicked();
    });
    loginDiv.append(loginWindowLoginButton);

    ButtonElement registerWindowRegisterButton = new ButtonElement();
    registerWindowRegisterButton.style
        ..width = "140px"
        ..height = "40px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = '#DDDDDD'
        ..fontSize = "15px"
        ..bottom = "5px"
        ..position = "absolute"
        ..outline = "none"
        ..left = "5px";
    registerWindowRegisterButton.dataset['active'] = "{ background: #BBBBBB; }";
    registerWindowRegisterButton.text = 'Register';
    registerWindowRegisterButton.onClick.listen((var d) {
      loginWindowRegisterButtonClicked();
    });
    loginDiv.append(registerWindowRegisterButton);

    loginWindowName = new InputElement();
    loginWindowName.style
        ..width = "90%"
        ..height = "30px"
        ..backgroundColor = "white"
        ..borderRadius = "2px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "25px"
        ..top = "70px"
        ..position = "absolute"
        ..outline = "none"
        ..left = "5%";
    loginWindowName.placeholder = 'Name';
    loginDiv.append(loginWindowName);

    loginWindowPassword = new InputElement();
    loginWindowPassword.type = 'Password';
    loginWindowPassword.style
        ..width = "90%"
        ..height = "30px"
        ..backgroundColor = "white"
        ..borderRadius = "2px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "25px"
        ..top = "110px"
        ..position = "absolute"
        ..outline = "none"
        ..left = "5%";
    loginWindowPassword.placeholder = 'Password';
    loginDiv.append(loginWindowPassword);

    loginWindowEmail = new InputElement();
    loginWindowEmail.style
        ..width = "90%"
        ..height = "30px"
        ..backgroundColor = "white"
        ..borderRadius = "2px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "25px"
        ..top = "190px"
        ..position = "absolute"
        ..outline = "none"
        ..left = "5%";
    loginWindowEmail.placeholder = 'Email for registration';
    loginDiv.append(loginWindowEmail);

    loginErrorDiv = new DivElement();
    loginErrorDiv.style
        ..width = "100%"
        ..position = "absolute"
        ..textAlign = "center"
        ..color = "red"
        ..fontSize = "15px"
        ..left = "0px"
        ..top = "230px";
    loginDiv.append(loginErrorDiv);
  }
  void makeTopNameDivComponent() {
    //Make a top name div for after logging in
    topNameDiv = new DivElement();
    topNameDiv.style
        ..width = "100%"
        ..position = "absolute"
        ..textAlign = "center"
        ..color = "black"
        ..fontSize = "20px"
        ..left = "0px"
        ..bottom = "0px";
  }
}