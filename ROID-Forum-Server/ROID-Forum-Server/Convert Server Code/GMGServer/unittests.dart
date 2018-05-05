import 'package:unittest/unittest.dart';
import 'dart:io';
import 'dart:async';
import 'dart:convert';

void main() {
  //I'm not entirely sure why the unit test does that
  testChatLoading();
  testAccountLoading();
}
void testAccountLoading() {
  test('Read the accounts', () {
    File file = new File('bin/accounts.json');
    file.exists().then((bool e) {
      if (e) {
        Future<String> finishedReading = file.readAsString(encoding: ASCII);
        finishedReading.then(expectAsync((String text) {
          expect(text.length > 0, true);
        }));
      }
    });
  });
}

void testChatLoading() {
  test('Read the chat logs', () {
    File file = new File('bin/chat.json');
    file.exists().then(expectAsync((bool e) {
      if (e) {
        Future<String> finishedReading = file.readAsString(encoding: ASCII);
        finishedReading.then((String text) {
          expect(text.length > 0, true);
        });
      }
    }));
  });
}
