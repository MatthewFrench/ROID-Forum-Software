library SectionViewPiece;

import 'dart:html';
import 'SectionView.dart';

class SectionViewPiece {
  SectionView sectionView;
  int id, forumID;
  String name, description;
  int threadCount, postCount;
  String lastPostThreadName, lastPostAuthor;
  DateTime lastPostDate;
  DivElement container;
  LabelElement sectionNameLabel, sectionDescriptionLabel;
  LabelElement sectionThreadsLabel;
  LabelElement sectionPostsLabel;
  LabelElement sectionLastPostDateLabel1,
      sectionLastPostDateLabel2,
      sectionLastPostUserLabel1,
      sectionLastPostUserLabel2,
      sectionLastPostThreadDateLabel1,
      sectionLastPostThreadDateLabel2;
  SectionViewPiece(this.sectionView, this.id, this.forumID, this.name, this.description,
      this.threadCount, this.postCount, this.lastPostThreadName,
      this.lastPostAuthor, this.lastPostDate) {
    //Create the div elements
    container = new DivElement();
    container.id = "sectionViewPiece";
    container.className = "container";

    DivElement sectionIndicatorDiv = new DivElement();
    sectionIndicatorDiv.id = "sectionViewPiece";
    sectionIndicatorDiv.className = "sectionIndicatorDiv";
    container.append(sectionIndicatorDiv);
    ImageElement imageIndicator =
    new ImageElement(src: "Resources/GMGPostIndicator.png");
    sectionIndicatorDiv.append(imageIndicator);

    DivElement sectionNameDescriptionDiv = new DivElement();
    sectionNameDescriptionDiv.id = "sectionViewPiece";
    sectionNameDescriptionDiv.className = "sectionNameDescriptionDiv";
    container.append(sectionNameDescriptionDiv);
    sectionNameLabel = new LabelElement();
    sectionNameLabel.id = "sectionViewPiece";
    sectionNameLabel.className = "sectionNameLabel";
    sectionNameLabel.onClick.listen((_){sectionPieceClicked();});
    sectionNameDescriptionDiv.append(sectionNameLabel);
    sectionDescriptionLabel = new LabelElement();
    sectionDescriptionLabel.id = "sectionViewPiece";
    sectionDescriptionLabel.className = "sectionDescriptionLabel";
    sectionNameDescriptionDiv.append(sectionDescriptionLabel);

    DivElement sectionThreadsDiv = new DivElement();
    sectionThreadsDiv.id = "sectionViewPiece";
    sectionThreadsDiv.className = "sectionThreadsDiv";
    container.append(sectionThreadsDiv);
    sectionThreadsLabel = new LabelElement();
    sectionThreadsDiv.append(sectionThreadsLabel);

    DivElement sectionPostsDiv = new DivElement();
    sectionPostsDiv.id = "sectionViewPiece";
    sectionPostsDiv.className = "sectionPostsDiv";
    container.append(sectionPostsDiv);
    sectionPostsLabel = new LabelElement();
    sectionPostsDiv.append(sectionPostsLabel);

    DivElement sectionLastPostDiv = new DivElement();
    sectionLastPostDiv.id = "sectionViewPiece";
    sectionLastPostDiv.className = "sectionLastPostDiv";
    container.append(sectionLastPostDiv);
    DivElement sectionLastPostDateDiv = new DivElement();
    sectionLastPostDateDiv.id = "sectionViewPiece";
    sectionLastPostDateDiv.className = "sectionLastPostDateDiv";
    sectionLastPostDiv.append(sectionLastPostDateDiv);
    sectionLastPostDateLabel1 = new LabelElement();
    sectionLastPostDateLabel1.id = "sectionViewPiece";
    sectionLastPostDateLabel1.className = "sectionLastPostDateLabel1";
    sectionLastPostDateDiv.append(sectionLastPostDateLabel1);
    sectionLastPostDateLabel2 = new LabelElement();
    sectionLastPostDateDiv.append(sectionLastPostDateLabel2);

    DivElement sectionLastPostUserDiv = new DivElement();
    sectionLastPostUserDiv.id = "sectionViewPiece";
    sectionLastPostUserDiv.className = "sectionLastPostUserDiv";
    sectionLastPostDiv.append(sectionLastPostUserDiv);
    sectionLastPostUserLabel1 = new LabelElement();
    sectionLastPostUserDiv.append(sectionLastPostUserLabel1);
    sectionLastPostUserLabel2 = new LabelElement();
    sectionLastPostUserLabel2.id = "sectionViewPiece";
    sectionLastPostUserLabel2.className = "sectionLastPostUserLabel2";
    sectionLastPostUserDiv.append(sectionLastPostUserLabel2);

    DivElement sectionLastPostThreadDiv = new DivElement();
    sectionLastPostThreadDiv.id = "sectionViewPiece";
    sectionLastPostThreadDiv.className = "sectionLastPostThreadDiv";
    sectionLastPostDiv.append(sectionLastPostThreadDiv);
    sectionLastPostThreadDateLabel1 = new LabelElement();
    sectionLastPostThreadDiv.append(sectionLastPostThreadDateLabel1);
    sectionLastPostThreadDateLabel2 = new LabelElement();
    sectionLastPostThreadDateLabel2.id = "sectionViewPiece";
    sectionLastPostThreadDateLabel2.className = "sectionLastPostThreadDateLabel2";
    sectionLastPostThreadDiv.append(sectionLastPostThreadDateLabel2);

    updateDescription(description);
    updateName(name);
    updateLastPostAuthor(lastPostAuthor);
    updateLastPostDate(lastPostDate);
    updateLastPostThreadName(lastPostThreadName);
    updateThreadCount(threadCount);
    updatePostCount(postCount);
  }
  void sectionPieceClicked() {
    sectionView.forum.showThreadView(forumID, id);
  }
  void updateName(String newName) {
    name = newName;
    sectionNameLabel.text = name;
  }

  void updateDescription(String newDescription) {
    description = newDescription;
    sectionDescriptionLabel.text = description;
  }

  void updateThreadCount(int count) {
    threadCount = count;
    sectionThreadsLabel.text = "${threadCount}";
  }

  void updatePostCount(int count) {
    postCount = count;
    sectionPostsLabel.text = "${postCount}";
  }

  void updateLastPostThreadName(String name) {
    lastPostThreadName = name;
    sectionLastPostThreadDateLabel2.text = lastPostThreadName;
  }

  void updateLastPostAuthor(String name) {
    lastPostAuthor = name;
    sectionLastPostUserLabel2.text = lastPostAuthor;
  }

  void updateLastPostDate(DateTime date) {
    lastPostDate = date.toLocal();
    DateTime now = new DateTime.now();
    if (lastPostDate.day == now.day &&
        lastPostDate.month == now.month &&
        lastPostDate.year == now.year) {
      sectionLastPostDateLabel1.text = "Today";
    } else if ((lastPostDate.day - now.day).abs() < 7 &&
        lastPostDate.month == now.month &&
        lastPostDate.year == now.year) {
      switch(lastPostDate.weekday) {
        case DateTime.SUNDAY: {sectionLastPostDateLabel1.text = "Sunday";} break;
        case DateTime.MONDAY: {sectionLastPostDateLabel1.text = "Monday";} break;
        case DateTime.TUESDAY: {sectionLastPostDateLabel1.text = "Tuesday";} break;
        case DateTime.WEDNESDAY: {sectionLastPostDateLabel1.text = "Wednesday";} break;
        case DateTime.THURSDAY: {sectionLastPostDateLabel1.text = "Thursday";} break;
        case DateTime.FRIDAY: {sectionLastPostDateLabel1.text = "Friday";} break;
        case DateTime.SATURDAY: {sectionLastPostDateLabel1.text = "Saturday";} break;
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
      sectionLastPostDateLabel1.text = "${month} ${lastPostDate.day}, ${lastPostDate.year}";
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
    sectionLastPostDateLabel2.text = " at ${hour}:${minutes}${timeOfDay}";
  }

  DivElement getDiv() {
    return container;
  }
}