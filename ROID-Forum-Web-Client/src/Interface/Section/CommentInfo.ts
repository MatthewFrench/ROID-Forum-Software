import {CommentView} from "./ThreadViews/CommentView";
import {ThreadInfo} from "./ThreadInfo";
import {ThreadController} from "./ThreadController";

export class CommentInfo {
    _threadID: string;
    _commentIDAndCreatedTime: string;
    _comment: string;
    _creatorAccountId: string;
    _creatorDisplayName: string;
    _creatorAvatarUrl: string;
    commentView: CommentView;
    thread: ThreadInfo;
    threadController: ThreadController;

    constructor(t: ThreadInfo, tc: ThreadController, darkTheme : boolean) {
        this.thread = t;
        this.threadController = tc;
        this.commentView = new CommentView(this, this.thread, this.threadController, darkTheme);
    }

    setThreadID(id: string) {
        this._threadID = id;
    }

    getThreadID(): string {
        return this._threadID;
    }

    setCommentIDAndCreatedTime(id: string) {
        this._commentIDAndCreatedTime = id;
    }

    getCommentIDAndCreatedTime(): string {
        return this._commentIDAndCreatedTime;
    }

    setComment(comment: string) {
        this._comment = comment;
        this.commentView.updateDescription();
    }

    getComment(): string {
        return this._comment;
    }

    setCreatorDisplayName(creatorDisplayName: string) {
        this._creatorDisplayName = creatorDisplayName;
        this.commentView.updateOwner();
    }

    getCreatorDisplayName(): string {
        return this._creatorDisplayName;
    }

    setCreatorAvatarUrl(creatorAvatarUrl: string) {
        this._creatorAvatarUrl = creatorAvatarUrl;
        this.commentView.updateAvatarURL();
    }

    getCreatorAvatarUrl(): string {
        return this._creatorAvatarUrl;
    }

    setCreatorAccountId(creatorAccountId: string) {
        this._creatorAccountId = creatorAccountId;
    }

    getCreatorAccountId(): string {
        return this._creatorAccountId;
    }
}