library TagController;
import '../../User.dart';
import '../GameController.dart';
import 'MessageSender.dart';
import 'dart:typed_data';
import 'TagUser.dart';
import 'dart:math';

class TagController {
  GameController controller;
  String name = 'Tag';
  List<TagUser> usersViewing = new List();
  MessageSender messageSender;
  int idCount = 0;
  double speed = 3.0;
  int currentTagged = -1,
      lastTagged = -1;
  TagController(GameController c) {
    controller = c;
    messageSender = new MessageSender(this);
  }
  void logic() {
    for (int i = 0; i < usersViewing.length; i++) {
      TagUser tu = usersViewing[i];
      double moveX = 0.0,
          moveY = 0.0;
      if (tu.downPress) moveY += speed;
      if (tu.upPress) moveY -= speed;
      if (tu.leftPress) moveX -= speed;
      if (tu.rightPress) moveX += speed;
      if (moveX != 0.0 || moveY != 0.0) {
        if (moveX != 0.0 && moveY != 0.0) {
          moveX *= 0.7;
          moveY *= 0.7;
        }
        tu.x += moveX;
        tu.y += moveY;
        messageSender.sendPositionUpdateToAll(tu);
      }
    }
    TagUser tagged = getTagUserForID(currentTagged);
    for (int i = 0; i < usersViewing.length; i++) {
      TagUser tu = usersViewing[i];
      if (tu.id != currentTagged && tu.id != lastTagged) {
        if ((tagged.x - tu.x) * (tagged.x - tu.x) + (tagged.y - tu.y) * (tagged.y - tu.y) <= 20 * 20) {
          setTagged(tu.id);
          i = usersViewing.length;
        }
      }
    }
  }
  void setTagged(int newTag) {
    lastTagged = currentTagged;
    if (usersViewing.length <=2) {lastTagged = -1;}
    currentTagged = newTag;
    messageSender.sendTaggedToAll(currentTagged);
  }
  void addUser(User u) {
    TagUser tu = new TagUser(u, idCount++);
    messageSender.sendAllUsersTo(tu);
    messageSender.sendTaggedToUser(tu, currentTagged);
    usersViewing.add(tu);
    messageSender.sendAddUserToAll(tu);
    if (currentTagged == -1) {
      setTagged(tu.id);
    }
  }
  void removeUser(User u) {
    TagUser tu = getTagUser(u);
    if (tu != null) {
      usersViewing.remove(tu);
      messageSender.sendRemoveUserToAll(tu);
      if (currentTagged == tu.id) {
        if (usersViewing.length != 0) {
          Random r = new Random();
          setTagged(usersViewing[r.nextInt(usersViewing.length)].id);
        } else {
          setTagged(-1);
        }
      }
    }
  }
  void accountLoggedIn(User u) {
    TagUser tu = getTagUser(u);
    if (tu != null) {
      tu.name = tu.user.account.name;
      messageSender.sendNameChangeToAll(tu);
    }
  }
  void accountLoggedOut(User u) {
    TagUser tu = getTagUser(u);
    if (tu != null) {
      tu.name = "Guest ${tu.id}";
      messageSender.sendNameChangeToAll(tu);
    }
  }
  void onMessage(User p, Map message) {
    TagUser tu = getTagUser(p);
    switch (message['Title']) {
      case 'Key Down':
        {
          if (message['Data'] == 'Left') tu.leftPress = true;
          if (message['Data'] == 'Right') tu.rightPress = true;
          if (message['Data'] == 'Down') tu.downPress = true;
          if (message['Data'] == 'Up') tu.upPress = true;
        }
        break;
      case 'Key Up':
        {
          if (message['Data'] == 'Left') tu.leftPress = false;
          if (message['Data'] == 'Right') tu.rightPress = false;
          if (message['Data'] == 'Down') tu.downPress = false;
          if (message['Data'] == 'Up') tu.upPress = false;
        }
        break;
    }
  }
  void onBinaryMessage(User p, Uint8List message) {
    TagUser tu = getTagUser(p);
    int title = message[0];
    switch (title) {
      case 0:
        {
          tu.leftPress = true;
        }
        break;
      case 1:
        {
          tu.rightPress = true;
        }
        break;
      case 2:
        {
          tu.upPress = true;
        }
        break;
      case 3:
        {
          tu.downPress = true;
        }
        break;
      case 4:
        {
          tu.leftPress = false;
        }
        break;
      case 5:
        {
          tu.rightPress = false;
        }
        break;
      case 6:
        {
          tu.downPress = false;
        }
        break;
      case 7:
        {
          tu.upPress = false;
        }
        break;
    }
  }
  TagUser getTagUser(User u) {
    for (int i = 0; i < usersViewing.length; i++) {
      TagUser tagUser = usersViewing[i];
      if (tagUser.user == u) {
        return tagUser;
      }
    }
    return null;
  }
  TagUser getTagUserForID(int id) {
    for (int i = 0; i < usersViewing.length; i++) {
      TagUser tagUser = usersViewing[i];
      if (tagUser.id == id) {
        return tagUser;
      }
    }
    return null;
  }
}
