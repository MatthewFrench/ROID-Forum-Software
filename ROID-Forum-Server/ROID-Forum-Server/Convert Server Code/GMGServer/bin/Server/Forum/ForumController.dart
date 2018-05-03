library ForumController;

import '../Server.dart';
import '../User/User.dart';
import 'ForumMsgCreator.dart';
import '../Networking/MessageReader.dart';
import 'ForumMsgHandler.dart';

class ForumController {
    Server server;
    ForumMsgHandler forumMsgHandler;
    int peopleOnline = 0;
    int registeredPeople = 0;
    ForumController(this.server) {
        forumMsgHandler = new ForumMsgHandler(this);
    }
    void userConnected(User u) {
        peopleOnline += 1;
        server.networkingController.sendMessageToAllConnected(ForumMsgCreator.PeopleOnline(peopleOnline));
        u.sendBinary(ForumMsgCreator.PeopleRegistered(registeredPeople));
    }
    void userDisconnected(User u) {
        peopleOnline -= 1;
        server.networkingController.sendMessageToAllConnected(ForumMsgCreator.PeopleOnline(peopleOnline));
    }
    void userRegistered(User u) {
        registeredPeople += 1;
        server.networkingController.sendMessageToAllConnected(ForumMsgCreator.PeopleRegistered(registeredPeople));
    }
    void userLoggedIn(User u) {

    }
    void userLoggedOut(User u) {

    }
    void handleUserMessage(User u, MessageReader message) {
        forumMsgHandler.onBinaryMessage(u, message);
    }
}