import {ThreadInfo} from "./ThreadInfo";
import {Section} from "./Section";
import {Interface} from "../../Utility/Interface";
import {CommentInfo} from "./CommentInfo";
import {Utility} from "../../Utility/Utility";

export class ThreadController {
  threads : ThreadInfo[];
  sectionController : Section;
  mainView : HTMLDivElement;
  headerView : HTMLDivElement;
  fullView : HTMLDivElement;
  newPostButton : HTMLButtonElement;
  backButton : HTMLButtonElement;
  viewingPosition = 0;
  viewingThread : ThreadInfo = null;
  constructor(c : Section) {
    this.sectionController = c;
    this.threads = [];
    this.mainView = Interface.Create({type: 'div'});
    this.headerView = Interface.Create({type: 'div'});
    let s : HTMLSpanElement = Interface.Create({type: 'span'});
    s.innerText = "The Programmer's Corner";
    s.style.position = "absolute";
    s.style.width = "100%";
    s.style.textAlign = "center";
    s.style.top = "14px";
    s.style.fontSize = "25px";
    s.style.color = "white";
    this.headerView.appendChild(s);
    this.fullView = Interface.Create({type: 'div'});
    this.makeNewPostButtonComponent();
    this.makeBackButtonComponent();
    this.mainView.appendChild(this.headerView);
  }
  loggedInEvent() {
    this.headerView.appendChild(this.newPostButton);
    for (let i = 0; i < this.threads.length; i++) {
      let t : ThreadInfo = this.threads[i];
      t.fullView.main.appendChild(t.fullView.commentButtonAndBox);
    }
  }
  loggedOutEvent() {
    this.newPostButton.remove();
    for (let i = 0; i < this.threads.length; i++) {
      let t : ThreadInfo = this.threads[i];
      t.fullView.commentButtonAndBox.remove();
    }
  }
  restoreToDefaultState() {
    this.fullView.remove();
    Utility.ClearElements(this.fullView);
    this.mainView.appendChild(this.headerView);
    this.backButton.remove();
    this.viewingThread = null;
  }
  threadClicked(thread : ThreadInfo) {
    this.headerView.remove();
    this.mainView.appendChild(this.fullView);
    this.fullView.appendChild(thread.fullView.getDiv());
    this.mainView.appendChild(this.backButton);
    this.viewingThread = thread;
  }
  backButtonClicked() {
    this.restoreToDefaultState();
  }
  updateThreadPositions() {
    for (let i = 0; i < this.threads.length; i++) {
      let t : ThreadInfo = this.threads[i];
      t.headerView.getDiv().style.top = `${i*85+60}px`;
      //animate(t.headerView.getDiv(), duration: 250, properties: {
      //  'top': "${i*85+60}px"
      //});
    }
  }
  getThread(id : number) : ThreadInfo {
    for (let t of this.threads) {
      if (t.getID() == id) return t;
    }
    return null;
  }
  showThread(threadID : number) {
    let thread : ThreadInfo = this.getThread(threadID);
    if (thread != null) {
      this.threadClicked(thread);
    }
  }
  //Actions
  clearAllThreads() {
    for (let i = 0; i < this.threads.length; i++) {
      let t : ThreadInfo = this.threads[i];
      t.headerView.getDiv().remove();
      t.fullView.getDiv().remove();
    }
    this.threads = [];
  }
  addThread(threadMap : any) {
    let id : number = threadMap["ID"];
    if (this.getThread(id) == null) {
      let thread : ThreadInfo = new ThreadInfo(this);
      thread.setID(id);
      this.threads.push(thread);

      thread.setOwner(threadMap["Owner"]);
      if (threadMap["AvatarURL"] != null) {
        thread.setAvatarURL(threadMap["AvatarURL"]);
      }
      thread.setTitle(threadMap["Title"]);
      thread.setDescription(threadMap["Description"]);

      let commentArray : any[] = threadMap["Comments"];
      for (let i = 0; i < commentArray.length; i++) {
        let c : any = commentArray[i];
        let cf : CommentInfo = new CommentInfo(thread, this);
        thread.addComment(cf);
        console.log(`Loaded comment with threadID: ${c['ThreadID']} and commentID: ${c['CommentID']}`);
        cf.setThreadID(c['ThreadID']);
        cf.setCommentID(c['CommentID']);
        cf.setComment(c['Comment']);
        cf.setOwner(c['Owner']);
        if (c["AvatarURL"] != null) {
        cf.setAvatarURL(c["AvatarURL"]);
        }
      }
      thread.headerView.getDiv().style.opacity = "1.0";
      thread.headerView.getDiv().style.top = `${(this.threads.length-1)*85+60}px`;
      this.headerView.appendChild(thread.headerView.getDiv());
      //animate(thread.headerView.getDiv(), duration: 250, properties: {
      //  'opacity': 1.0
      //});

      this.updateThreadPositions();
    }
  }
  removeThread(threadID : number) {
    let thread : ThreadInfo = this.getThread(threadID);
    if (thread != null) {
      this.threads.splice(this.threads.indexOf(thread), 1);
      thread.headerView.getDiv().style.opacity = "1.0";
      //animate(thread.headerView.getDiv(), duration: 250, properties: {
      //  'opacity': 0.0
      //}).onComplete.listen((_) {
        thread.headerView.getDiv().remove();
      //});
      thread.fullView.getDiv().style.opacity = "1.0";
      //animate(thread.fullView.getDiv(), duration: 250, properties: {
      //  'opacity': 0.0
      //}).onComplete.listen((_) {
        thread.fullView.getDiv().remove();
      //});
      this.updateThreadPositions();
      if (thread == this.viewingThread) {
        this.restoreToDefaultState();
      }
    }
  }
  updateThread(threadID : number, title : string, description : string) {
    let thread : ThreadInfo = this.getThread(threadID);
    if (thread != null) {
      thread.setTitle(title);
      thread.setDescription(description);
    }
  }
  moveThreadToTop(threadID : number) {
    let thread : ThreadInfo = this.getThread(threadID);
    if (thread != null) {
      this.threads.splice(this.threads.indexOf(thread), 1);
      this.threads.splice(0, 0, thread);
      this.updateThreadPositions();
    }
  }
  addComment(commentMap : any) {
    let threadID : number = commentMap["ThreadID"];
    let thread : ThreadInfo = this.getThread(threadID);
    if (thread != null) {
      let cf : CommentInfo = new CommentInfo(thread, this);
      thread.addComment(cf);
      cf.setThreadID(commentMap['ThreadID']);
      cf.setCommentID(commentMap['CommentID']);
      cf.setComment(commentMap['Comment']);
      cf.setOwner(commentMap['Owner']);
      cf.setAvatarURL(commentMap["AvatarURL"]);
    }
  }
  deleteComment(threadID : number, commentID : number) {
    let thread : ThreadInfo = this.getThread(threadID);
    if (thread != null) {
      thread.removeComment(commentID);
    }
  }
  updateComment(threadID : number, commentID : number, comment : string) {
    let thread : ThreadInfo = this.getThread(threadID);
    if (thread != null) {
      let ci : CommentInfo = thread.getComment(commentID);
      ci.setComment(comment);
    }
  }
  //GUI
  makeBackButtonComponent() {
    //Add the new post button to it
    this.backButton = Interface.Create({type: 'button'});
        this.backButton.style.width = "60px";
        this.backButton.style.height = "30px";
        this.backButton.style.backgroundColor = "white";
        this.backButton.style.borderRadius = "4px";
        this.backButton.style.borderStyle = "solid";
        this.backButton.style.borderWidth = "1px";
        this.backButton.style.borderColor = "#DDDDDD";
        this.backButton.style.fontSize = "15px";
        this.backButton.style.top = "5px";
        this.backButton.style.position = "absolute";
        this.backButton.style.outline = "none";
        this.backButton.style.left = "5px";
    this.backButton.dataset['active'] = "background: #BBBBBB;";
    this.backButton.innerText = 'Back';
    this.backButton.onclick = () => {
      this.backButtonClicked();
    };
  }
  makeNewPostButtonComponent() {
    //Add the new post button to it
    this.newPostButton = Interface.Create({type: 'button'});
        this.newPostButton.style.width = "140px";
        this.newPostButton.style.height = "40px";
        this.newPostButton.style.backgroundColor = "white";
        this.newPostButton.style.borderRadius = "4px";
        this.newPostButton.style.borderStyle = "solid";
        this.newPostButton.style.borderWidth = "1px";
        this.newPostButton.style.borderColor = "#DDDDDD";
        this.newPostButton.style.fontSize = "15px";
        this.newPostButton.style.top = "5px";
        this.newPostButton.style.position = "absolute";
        this.newPostButton.style.outline = "none";
        this.newPostButton.style.right = "5px";
    this.newPostButton.dataset['active'] = "background: #BBBBBB;";
    this.newPostButton.innerText = 'Create New Post';
    this.newPostButton.onclick = () => {
      this.sectionController.newPostButtonClicked();
    };
  }
}
