library Account;

class Account {
  String name = "";
  String password = "";
  String email = "";
  String avatarURL = "";
  Account(Map m) {
    name = m['Name'];
    password = m['Password'];
    email = m['Email'];
    if (m.containsKey('AvatarURL')) {
      avatarURL = m['AvatarURL'];
    }
  }
  Map toJSON() {
    Map m = new Map();
    m['Name'] = name;
    m['Password'] = password;
    m['Email'] = email;
    m['AvatarURL'] = avatarURL;
    return m;
  }
}
