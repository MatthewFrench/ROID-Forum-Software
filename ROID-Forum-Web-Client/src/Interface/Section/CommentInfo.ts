import {CommentView} from "./ThreadViews/CommentView";
import {ThreadInfo} from "./ThreadInfo";
import {ThreadController} from "./ThreadController";
import {MessageReader} from "../../Utility/Message/MessageReader";

export class CommentInfo {
    _threadID: string;
    _commentID: string;
    _comment: string;
    _creatorAccountId: string;
    _creatorDisplayName: string;
    _creatorAvatarUrl: string;
    _createdTime: string;
    commentView: CommentView;
    thread: ThreadInfo;
    threadController: ThreadController;

    constructor(t: ThreadInfo, tc: ThreadController, darkTheme : boolean) {
        this.thread = t;
        this.threadController = tc;
        this.commentView = new CommentView(this, this.thread, this.threadController, darkTheme);
    }

    /*
    setBinary(message: MessageReader) {
        let threadID = message.getString();
        let commentID = message.getString();
        let comment = message.getString();
        let owner = message.getString();
        let avatarURL = message.getString();
        this._threadID = threadID;
        this._commentID = commentID;
        this._comment = comment;
        this._owner = owner;
        this._avatarURL = avatarURL;
        this.commentView.updateAvatarURL();
        this.commentView.updateOwner();
        this.commentView.updateDescription();
    }

     */

    setThreadID(id: string) {
        this._threadID = id;
    }

    getThreadID(): string {
        return this._threadID;
    }

    setCommentID(id: string) {
        this._commentID = id;
    }

    getCommentID(): string {
        return this._commentID;
    }

    setComment(comment: string) {
        this._comment = comment;
        this.commentView.updateDescription();
    }

    getComment(): string {
        return this._comment;
    }

    setCreatedTime(createdTime: string) {
        this._createdTime = createdTime;
    }

    getCreatedTime(): string {
        return this._createdTime;
    }

    setCreatorDisplayname(creatorDisplayName: string) {
        this._creatorDisplayName = creatorDisplayName;
        this.commentView.updateOwner();
    }

    getCreatorDisplayName(): string {
        return this._creatorDisplayName;
    }

    setCreatorAccountId(creatorAccountId: string) {
        this._creatorAccountId = creatorAccountId;
    }

    getCreatorAccountId(): string {
        return this._creatorAccountId;
    }

    setAvatarURL(avatarURL: string) {
        this._creatorAvatarUrl = avatarURL;
        this.commentView.updateAvatarURL();
    }

    getAvatarURL(): string {
        return this._creatorAvatarUrl;
    }
}