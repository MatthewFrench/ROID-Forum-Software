library ThreadViewPiece;

import 'ThreadView.dart';
import 'dart:html';

class ThreadViewPiece {
  int id, sectionID, forumID;
  String name, creatorName;
  int replies, views;
  String lastPostAuthor;
  DateTime lastPostDate;
  DivElement container;
  LabelElement threadNameLabel, threadDescriptionLabel;
  LabelElement startedByLabel;
  LabelElement threadRepliesLabel, threadViewsLabel;
  LabelElement threadLastPostDateLabel1,
      threadLastPostDateLabel2,
      threadLastPostUserLabel1,
      threadLastPostUserLabel2,
      threadLastPostThreadDateLabel1,
      threadLastPostThreadDateLabel2;
  ThreadView threadView;

  ThreadViewPiece(this.threadView, this.id, this.sectionID, this.forumID, this.name, this.creatorName, this.replies, this.views,
      this.lastPostAuthor, this.lastPostDate) {
    //Create the div elements
    container = new DivElement();
    container.id = "threadViewPiece";
    container.className = "container";

    DivElement threadIndicatorDiv = new DivElement();
    threadIndicatorDiv.id = "threadViewPiece";
    threadIndicatorDiv.className = "threadIndicatorDiv";
    container.append(threadIndicatorDiv);
    ImageElement imageIndicator =
    new ImageElement(src: "Resources/GMGPostIndicator.png");
    threadIndicatorDiv.append(imageIndicator);

    DivElement threadNameDescriptionDiv = new DivElement();
    threadNameDescriptionDiv.id = "threadViewPiece";
    threadNameDescriptionDiv.className = "threadNameDescriptionDiv";
    container.append(threadNameDescriptionDiv);
    threadNameLabel = new LabelElement();
    threadNameLabel.id = "threadViewPiece";
    threadNameLabel.className = "threadNameLabel";
    threadNameLabel.onClick.listen((_){
      threadPieceClicked();
    });
    threadNameDescriptionDiv.append(threadNameLabel);
    threadDescriptionLabel = new LabelElement();
    threadDescriptionLabel.id = "threadViewPiece";
    threadDescriptionLabel.className = "threadDescriptionLabel";
    threadDescriptionLabel.text = "« Pages 1 »";
    threadNameDescriptionDiv.append(threadDescriptionLabel);

    DivElement startedByDiv = new DivElement();
    startedByDiv.id = "threadViewPiece";
    startedByDiv.className = "startedByDiv";
    container.append(startedByDiv);
    startedByLabel = new LabelElement();
    startedByDiv.append(startedByLabel);

    DivElement threadRepliesDiv = new DivElement();
    threadRepliesDiv.id = "threadViewPiece";
    threadRepliesDiv.className = "threadRepliesDiv";
    container.append(threadRepliesDiv);
    threadRepliesLabel = new LabelElement();
    threadRepliesDiv.append(threadRepliesLabel);

    DivElement threadViewsDiv = new DivElement();
    threadViewsDiv.id = "threadViewPiece";
    threadViewsDiv.className = "threadViewsDiv";
    container.append(threadViewsDiv);
    threadViewsLabel = new LabelElement();
    threadViewsDiv.append(threadViewsLabel);

    DivElement threadLastPostDiv = new DivElement();
    threadLastPostDiv.id = "threadViewPiece";
    threadLastPostDiv.className = "threadLastPostDiv";
    container.append(threadLastPostDiv);
    DivElement threadLastPostDateDiv = new DivElement();
    threadLastPostDateDiv.id = "threadViewPiece";
    threadLastPostDateDiv.className = "threadLastPostDateDiv";
    threadLastPostDiv.append(threadLastPostDateDiv);
    threadLastPostDateLabel1 = new LabelElement();
    threadLastPostDateLabel1.id = "threadViewPiece";
    threadLastPostDateLabel1.className = "threadLastPostDateLabel1";
    threadLastPostDateDiv.append(threadLastPostDateLabel1);
    threadLastPostDateLabel2 = new LabelElement();
    threadLastPostDateDiv.append(threadLastPostDateLabel2);

    DivElement threadLastPostUserDiv = new DivElement();
    threadLastPostUserDiv.id = "threadViewPiece";
    threadLastPostUserDiv.className = "threadLastPostUserDiv";
    threadLastPostDiv.append(threadLastPostUserDiv);
    threadLastPostUserLabel1 = new LabelElement();
    threadLastPostUserDiv.append(threadLastPostUserLabel1);
    threadLastPostUserLabel2 = new LabelElement();
    threadLastPostUserLabel2.id = "threadViewPiece";
    threadLastPostUserLabel2.className = "threadLastPostUserLabel2";
    threadLastPostUserDiv.append(threadLastPostUserLabel2);

    updateName(name);
    updateLastPostAuthor(lastPostAuthor);
    updateLastPostDate(lastPostDate);
    updateRepliesCount(replies);
    updateViewsCount(views);
    updateStartedByName(creatorName);
  }

  void threadPieceClicked() {
    threadView.forum.showPostView(forumID, sectionID, id);
  }
  void updateName(String newName) {
    name = newName;
    threadNameLabel.text = name;
  }

  void updateRepliesCount(int count) {
    replies = count;
    threadRepliesLabel.text = "${count}";
  }

  void updateViewsCount(int count) {
    views = count;
    threadViewsLabel.text = "${count}";
  }

  void updateStartedByName(String name) {
    creatorName = name;
    startedByLabel.text = name;
  }

  void updateLastPostAuthor(String name) {
    lastPostAuthor = name;
    threadLastPostUserLabel2.text = lastPostAuthor;
  }

  void updateLastPostDate(DateTime date) {
    lastPostDate = date.toLocal();
    DateTime now = new DateTime.now();
    if (lastPostDate.day == now.day &&
        lastPostDate.month == now.month &&
        lastPostDate.year == now.year) {
      threadLastPostDateLabel1.text = "Today";
    } else if ((lastPostDate.day - now.day).abs() < 7 &&
        lastPostDate.month == now.month &&
        lastPostDate.year == now.year) {
      switch(lastPostDate.weekday) {
        case DateTime.SUNDAY: {threadLastPostDateLabel1.text = "Sunday";} break;
        case DateTime.MONDAY: {threadLastPostDateLabel1.text = "Monday";} break;
        case DateTime.TUESDAY: {threadLastPostDateLabel1.text = "Tuesday";} break;
        case DateTime.WEDNESDAY: {threadLastPostDateLabel1.text = "Wednesday";} break;
        case DateTime.THURSDAY: {threadLastPostDateLabel1.text = "Thursday";} break;
        case DateTime.FRIDAY: {threadLastPostDateLabel1.text = "Friday";} break;
        case DateTime.SATURDAY: {threadLastPostDateLabel1.text = "Saturday";} break;
      }
    } else {
      String month = "";
      switch(lastPostDate.month) {
        case DateTime.JANUARY: {month = "January";} break;
        case DateTime.FEBRUARY: {month = "February";} break;
        case DateTime.MARCH: {month = "March";} break;
        case DateTime.APRIL: {month = "April";} break;
        case DateTime.MAY: {month = "May";} break;
        case DateTime.JUNE: {month = "June";} break;
        case DateTime.JULY: {month = "July";} break;
        case DateTime.AUGUST: {month = "August";} break;
        case DateTime.SEPTEMBER: {month = "September";} break;
        case DateTime.OCTOBER: {month = "October";} break;
        case DateTime.NOVEMBER: {month = "November";} break;
        case DateTime.DECEMBER: {month = "December";} break;
      }
      threadLastPostDateLabel1.text = "${month} ${lastPostDate.day}, ${lastPostDate.year}";
    }
    int hour = lastPostDate.hour;
    String timeOfDay = "am";
    if (hour > 12) {
      hour -= 12;
      timeOfDay = "pm";
    }
    String minutes = "${lastPostDate.minute}";
    if (lastPostDate.minute < 10) {
      minutes = "0${lastPostDate.minute}";
    }
    threadLastPostDateLabel2.text = " at ${hour}:${minutes}${timeOfDay}";
  }

  DivElement getDiv() {
    return container;
  }
}