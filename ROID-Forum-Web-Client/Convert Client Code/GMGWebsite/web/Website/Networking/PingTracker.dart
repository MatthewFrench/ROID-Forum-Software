library PingTracker;
import 'NetworkingController.dart';
import 'dart:async';
import '../WebsiteMsgCreator.dart';

class PingTracker {
  NetworkingController networkingController;
  Stopwatch stopwatch;
  int pingMilliseconds=100;
  Timer pingTimer = null;
  bool waitingForPing = false;
  PingTracker(this.networkingController) {
    stopwatch = new Stopwatch();
  }
  void start() {
    if (pingTimer != null) pingTimer.cancel();
    pingTimer = null;
    networkingController.SendBinary(WebsiteMsgCreator.Ping(), true);
    stopwatch.start();
    waitingForPing = true;
  }
  void gotPing() {
    if (!waitingForPing) return;
    stopwatch.stop();
    pingMilliseconds = stopwatch.elapsedMilliseconds;
    stopwatch.reset();
    networkingController.SendBinary(WebsiteMsgCreator.PingFinal(), true);
    //Update display
    //networkingController.website.mainSideBarGui.pingDisplay.text = "Ping: ${pingMilliseconds}ms";
    if (pingTimer != null) {pingTimer.cancel();pingTimer = null;}
    pingTimer = new Timer(new Duration(seconds:1), start);
    waitingForPing = false;
  }
  int getPingMilliseconds() {
    return pingMilliseconds;
  }
}