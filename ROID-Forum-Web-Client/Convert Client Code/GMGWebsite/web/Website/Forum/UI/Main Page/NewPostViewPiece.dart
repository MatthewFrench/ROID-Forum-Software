library NewPostViewPiece;

import 'dart:html';

class NewPostViewPiece {
  int id;
  String name, description;
  int sectionCount, threadCount, postCount;
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
  NewPostViewPiece(this.id, this.name, this.description, this.sectionCount,
      this.threadCount, this.postCount) {
    //Create the div elements
    container = new DivElement();
    container.id = "newPostViewPiece";
    container.className = "container";

    DivElement forumIndicatorDiv = new DivElement();
    forumIndicatorDiv.id = "newPostViewPiece";
    forumIndicatorDiv.className = "forumIndicatorDiv";
    container.append(forumIndicatorDiv);
    ImageElement imageIndicator =
    new ImageElement(src: "Resources/GMGPostIndicator.png");
    forumIndicatorDiv.append(imageIndicator);

    DivElement forumNameDescriptionDiv = new DivElement();
    forumNameDescriptionDiv.id = "newPostViewPiece";
    forumNameDescriptionDiv.className = "forumNameDescriptionDiv";
    container.append(forumNameDescriptionDiv);
    forumNameLabel = new LabelElement();
    forumNameLabel.text = name;
    forumNameLabel.id = "newPostViewPiece";
    forumNameLabel.className = "forumNameLabel";
    forumNameDescriptionDiv.append(forumNameLabel);
    forumDescriptionLabel = new LabelElement();
    forumDescriptionLabel.text = description;
    forumDescriptionLabel.id = "newPostViewPiece";
    forumDescriptionLabel.className = "forumDescriptionLabel";
    forumNameDescriptionDiv.append(forumDescriptionLabel);

    DivElement forumSectionsDiv = new DivElement();
    forumSectionsDiv.id = "newPostViewPiece";
    forumSectionsDiv.className = "forumSectionsDiv";
    container.append(forumSectionsDiv);

    forumSectionsLabel = new LabelElement();
    forumSectionsLabel.text = "Need";
    forumSectionsDiv.append(forumSectionsLabel);

    DivElement forumThreadsDiv = new DivElement();
    forumThreadsDiv.id = "newPostViewPiece";
    forumThreadsDiv.className = "forumThreadsDiv";
    container.append(forumThreadsDiv);
    forumThreadsLabel = new LabelElement();
    forumThreadsLabel.text = "Need";
    forumThreadsDiv.append(forumThreadsLabel);

    DivElement forumPostsDiv = new DivElement();
    forumPostsDiv.id = "newPostViewPiece";
    forumPostsDiv.className = "forumPostsDiv";
    container.append(forumPostsDiv);
    forumPostsLabel = new LabelElement();
    forumPostsLabel.text = "Need";
    forumPostsDiv.append(forumPostsLabel);

    DivElement forumLastPostDiv = new DivElement();
    forumLastPostDiv.id = "newPostViewPiece";
    forumLastPostDiv.className = "forumLastPostDiv";
    container.append(forumLastPostDiv);
    //forumLastPostDateLabel, forumLastPostUserLabel, forumLastPostThreadDateLabel
    DivElement forumLastPostDateDiv = new DivElement();
    forumLastPostDateDiv.id = "newPostViewPiece";
    forumLastPostDateDiv.className = "forumLastPostDateDiv";
    forumLastPostDiv.append(forumLastPostDateDiv);
    forumLastPostDateLabel1 = new LabelElement();
    forumLastPostDateLabel1.text = "Need";
    forumLastPostDateLabel1.id = "newPostViewPiece";
    forumLastPostDateLabel1.className = "forumLastPostDateLabel1";
    forumLastPostDateDiv.append(forumLastPostDateLabel1);
    forumLastPostDateLabel2 = new LabelElement();
    forumLastPostDateLabel2.text = " at Need";
    forumLastPostDateDiv.append(forumLastPostDateLabel2);

    DivElement forumLastPostUserDiv = new DivElement();
    forumLastPostUserDiv.id = "newPostViewPiece";
    forumLastPostUserDiv.className = "forumLastPostUserDiv";
    forumLastPostDiv.append(forumLastPostUserDiv);
    forumLastPostUserLabel1 = new LabelElement();
    forumLastPostUserLabel1.text = "by ";
    forumLastPostUserDiv.append(forumLastPostUserLabel1);
    forumLastPostUserLabel2 = new LabelElement();
    forumLastPostUserLabel2.text = "Need";
    forumLastPostUserLabel2.id = "newPostViewPiece";
    forumLastPostUserLabel2.className = "forumLastPostUserLabel2";
    forumLastPostUserDiv.append(forumLastPostUserLabel2);

    DivElement forumLastPostThreadDiv = new DivElement();
    forumLastPostThreadDiv.id = "newPostViewPiece";
    forumLastPostThreadDiv.className = "forumLastPostThreadDiv";
    forumLastPostDiv.append(forumLastPostThreadDiv);
    forumLastPostThreadDateLabel1 = new LabelElement();
    forumLastPostThreadDateLabel1.text = "in ";
    forumLastPostThreadDiv.append(forumLastPostThreadDateLabel1);
    forumLastPostThreadDateLabel2 = new LabelElement();
    forumLastPostThreadDateLabel2.text = "Need";
    forumLastPostThreadDateLabel2.id = "newPostViewPiece";
    forumLastPostThreadDateLabel2.className = "forumLastPostThreadDateLabel2";
    forumLastPostThreadDiv.append(forumLastPostThreadDateLabel2);
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

  DivElement getDiv() {
    return container;
  }
}
