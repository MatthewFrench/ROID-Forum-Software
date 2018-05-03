library ForumViewPiece;

import 'dart:html';
import 'ForumView.dart';

class ForumViewPiece {
  ForumView forumView;
  int id;
  String name, description;
  int sectionCount, threadCount, postCount;
  String lastPostThreadName, lastPostAuthor;
  DateTime lastPostDate;
  DivElement container;
  LabelElement forumNameLabel, forumDescriptionLabel;
  LabelElement forumSectionsLabel;
  LabelElement forumThreadsLabel;
  LabelElement forumPostsLabel;
  LabelElement forumLastPostDateLabel1,
      forumLastPostDateLabel2,
      forumLastPostUserLabel1,
      forumLastPostUserLabel2,
      forumLastPostThreadDateLabel1,
      forumLastPostThreadDateLabel2;
  ForumViewPiece(this.forumView, this.id, this.name, this.description, this.sectionCount,
      this.threadCount, this.postCount, this.lastPostThreadName,
  this.lastPostAuthor, this.lastPostDate) {
    //Create the div elements
    container = new DivElement();
    container.id = "forumViewPiece";
    container.className = "container";

    DivElement forumIndicatorDiv = new DivElement();
    forumIndicatorDiv.id = "forumViewPiece";
    forumIndicatorDiv.className = "forumIndicatorDiv";
    container.append(forumIndicatorDiv);
    ImageElement imageIndicator =
        new ImageElement(src: "Resources/GMGPostIndicator.png");
    forumIndicatorDiv.append(imageIndicator);

    DivElement forumNameDescriptionDiv = new DivElement();
    forumNameDescriptionDiv.id = "forumViewPiece";
    forumNameDescriptionDiv.className = "forumNameDescriptionDiv";
    container.append(forumNameDescriptionDiv);
    forumNameLabel = new LabelElement();
    forumNameLabel.id = "forumViewPiece";
    forumNameLabel.className = "forumNameLabel";
    forumNameLabel.onClick.listen((_){forumPieceClicked();});
    forumNameDescriptionDiv.append(forumNameLabel);
    forumDescriptionLabel = new LabelElement();
    forumDescriptionLabel.id = "forumViewPiece";
    forumDescriptionLabel.className = "forumDescriptionLabel";
    forumNameDescriptionDiv.append(forumDescriptionLabel);

    DivElement forumSectionsDiv = new DivElement();
    forumSectionsDiv.id = "forumViewPiece";
    forumSectionsDiv.className = "forumSectionsDiv";
    container.append(forumSectionsDiv);

    forumSectionsLabel = new LabelElement();
    forumSectionsDiv.append(forumSectionsLabel);

    DivElement forumThreadsDiv = new DivElement();
    forumThreadsDiv.id = "forumViewPiece";
    forumThreadsDiv.className = "forumThreadsDiv";
    container.append(forumThreadsDiv);
    forumThreadsLabel = new LabelElement();
    forumThreadsDiv.append(forumThreadsLabel);

    DivElement forumPostsDiv = new DivElement();
    forumPostsDiv.id = "forumViewPiece";
    forumPostsDiv.className = "forumPostsDiv";
    container.append(forumPostsDiv);
    forumPostsLabel = new LabelElement();
    forumPostsDiv.append(forumPostsLabel);

    DivElement forumLastPostDiv = new DivElement();
    forumLastPostDiv.id = "forumViewPiece";
    forumLastPostDiv.className = "forumLastPostDiv";
    container.append(forumLastPostDiv);

    DivElement forumLastPostDateDiv = new DivElement();
    forumLastPostDateDiv.id = "forumViewPiece";
    forumLastPostDateDiv.className = "forumLastPostDateDiv";
    forumLastPostDiv.append(forumLastPostDateDiv);
    forumLastPostDateLabel1 = new LabelElement();
    forumLastPostDateLabel1.id = "forumViewPiece";
    forumLastPostDateLabel1.className = "forumLastPostDateLabel1";
    forumLastPostDateDiv.append(forumLastPostDateLabel1);
    forumLastPostDateLabel2 = new LabelElement();
    forumLastPostDateDiv.append(forumLastPostDateLabel2);

    DivElement forumLastPostUserDiv = new DivElement();
    forumLastPostUserDiv.id = "forumViewPiece";
    forumLastPostUserDiv.className = "forumLastPostUserDiv";
    forumLastPostDiv.append(forumLastPostUserDiv);
    forumLastPostUserLabel1 = new LabelElement();
    forumLastPostUserDiv.append(forumLastPostUserLabel1);
    forumLastPostUserLabel2 = new LabelElement();
    forumLastPostUserLabel2.id = "forumViewPiece";
    forumLastPostUserLabel2.className = "forumLastPostUserLabel2";
    forumLastPostUserDiv.append(forumLastPostUserLabel2);

    DivElement forumLastPostThreadDiv = new DivElement();
    forumLastPostThreadDiv.id = "forumViewPiece";
    forumLastPostThreadDiv.className = "forumLastPostThreadDiv";
    forumLastPostDiv.append(forumLastPostThreadDiv);
    forumLastPostThreadDateLabel1 = new LabelElement();
    forumLastPostThreadDateLabel1.id = "forumViewPiece";
    forumLastPostThreadDateLabel1.className = "forumLastPostThreadDateLabel1";
    forumLastPostThreadDiv.append(forumLastPostThreadDateLabel1);
    forumLastPostThreadDateLabel2 = new LabelElement();
    forumLastPostThreadDateLabel2.id = "forumViewPiece";
    forumLastPostThreadDateLabel2.className = "forumLastPostThreadDateLabel2";
    forumLastPostThreadDiv.append(forumLastPostThreadDateLabel2);

    updateDescription(description);
    updateName(name);
    updateSectionCount(sectionCount);
    updateLastPostAuthor(lastPostAuthor);
    updateLastPostDate(lastPostDate);
    updateLastPostThreadName(lastPostThreadName);
    updateThreadCount(threadCount);
    updatePostCount(postCount);
  }
  void forumPieceClicked() {
    forumView.forum.showSections(id);
  }
  void updateName(String newName) {
    name = newName;
    forumNameLabel.text = name;
  }

  void updateDescription(String newDescription) {
    description = newDescription;
    forumDescriptionLabel.text = description;
  }

  void updateSectionCount(int count) {
    sectionCount = count;
    forumSectionsLabel.text = "${sectionCount}";
  }

  void updateThreadCount(int count) {
    threadCount = count;
    forumThreadsLabel.text = "${threadCount}";
  }

  void updatePostCount(int count) {
    postCount = count;
    forumPostsLabel.text = "${postCount}";
  }

  void updateLastPostThreadName(String name) {
    lastPostThreadName = name;
    forumLastPostThreadDateLabel2.text = lastPostThreadName;
  }

  void updateLastPostAuthor(String name) {
    lastPostAuthor = name;
    forumLastPostUserLabel2.text = lastPostAuthor;
  }

  void updateLastPostDate(DateTime date) {
    lastPostDate = date.toLocal();
    DateTime now = new DateTime.now();
    if (lastPostDate.day == now.day &&
        lastPostDate.month == now.month &&
        lastPostDate.year == now.year) {
      forumLastPostDateLabel1.text = "Today";
    } else if ((lastPostDate.day - now.day).abs() < 7 &&
        lastPostDate.month == now.month &&
        lastPostDate.year == now.year) {
        switch(lastPostDate.weekday) {
          case DateTime.SUNDAY: {forumLastPostDateLabel1.text = "Sunday";} break;
          case DateTime.MONDAY: {forumLastPostDateLabel1.text = "Monday";} break;
          case DateTime.TUESDAY: {forumLastPostDateLabel1.text = "Tuesday";} break;
          case DateTime.WEDNESDAY: {forumLastPostDateLabel1.text = "Wednesday";} break;
          case DateTime.THURSDAY: {forumLastPostDateLabel1.text = "Thursday";} break;
          case DateTime.FRIDAY: {forumLastPostDateLabel1.text = "Friday";} break;
          case DateTime.SATURDAY: {forumLastPostDateLabel1.text = "Saturday";} break;
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
      forumLastPostDateLabel1.text = "${month} ${lastPostDate.day}, ${lastPostDate.year}";
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
    forumLastPostDateLabel2.text = " at ${hour}:${minutes}${timeOfDay}";
  }

  DivElement getDiv() {
    return container;
  }
}