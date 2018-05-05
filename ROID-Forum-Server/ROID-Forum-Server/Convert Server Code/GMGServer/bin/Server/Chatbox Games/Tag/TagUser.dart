library TagUser;
import '../../User.dart';

class TagUser {
  User user;
  double x = 0.0;
  double y = 0.0;
  bool leftPress = false, rightPress = false, downPress = false, upPress = false;
  int id;
  String name;
  TagUser(User u, _id) {
    user = u;
    id = _id;
    if (user.account == null) {
      name = "Guest ${id}";
    } else {name = user.account.name;}
  }
}