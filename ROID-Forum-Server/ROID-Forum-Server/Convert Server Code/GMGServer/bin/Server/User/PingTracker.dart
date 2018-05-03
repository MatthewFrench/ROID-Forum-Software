library PingTracker;
import 'User.dart';
import 'UserMsgCreator.dart';

class PingTracker {
  User user;
  Stopwatch stopwatch;
  int pingMilliseconds=100;
  PingTracker(this.user) {
    stopwatch = new Stopwatch();
  }
  void gotFirstPing() {
    user.sendBinary(UserMsgCreator.Ping(), true);
    stopwatch.start();
  }
  void gotFinalPing() {
    stopwatch.stop();
    pingMilliseconds = stopwatch.elapsedMilliseconds;
    stopwatch.reset();
  }
  int getPingMilliseconds() {
    return pingMilliseconds;
  }
}