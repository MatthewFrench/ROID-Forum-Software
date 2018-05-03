library Account;

//    Accounts
//        ID
//        DisplayName
//        Username
//        Password
//        Email
//        CreatedDate
//        AvatarURL

class Account {
  String displayName, username, password, email, avatarURL;
  int id;
  DateTime createdDate;

  Account(Map m) {
    id = m['ID'];
    displayName = m['DisplayName'];
    username = m['Username'];
    password = m['Password'];
    email = m['Email'];
    createdDate = m['CreatedDate'];
    avatarURL = m['AvatarURL'];
  }
}
