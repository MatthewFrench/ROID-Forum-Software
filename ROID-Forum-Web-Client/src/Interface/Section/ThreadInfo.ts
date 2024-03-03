import {HeaderView} from "./ThreadViews/HeaderView";
import {CommentInfo} from "./CommentInfo";
import {FullView} from "./ThreadViews/FullView";
import {ThreadController} from "./ThreadController";

export class ThreadInfo {
    _threadId: string;
    _title: string;
    _description: string;
    _creatorAccountId: string;
    _creatorDisplayName: string;
    _creatorAvatarUrl: string;
    _createdTime: string;
    _updatedTime: string;
    _commentCount: number;
    _comments: CommentInfo[];
    headerView: HeaderView;
    fullView: FullView;
    threadController: ThreadController;

    constructor(tc: ThreadController, darkTheme : boolean) {
        this.threadController = tc;
        this._comments = [];
        this.headerView = new HeaderView(this, tc, darkTheme);
        this.fullView = new FullView(this, tc, darkTheme);
        this.headerView.updateCommentCount();
    }

    setCommentCount(count: number) {
        this._commentCount = count;
        this.headerView.updateCommentCount();
    }

    getCommentCount(): number {
        return this._commentCount;
    }

    setCreatedTime(createdTime: string) {
        this._createdTime = createdTime;
    }

    getCreatedTime(): string {
        return this._createdTime;
    }

    setUpdatedTime(updatedTime: string) {
        this._updatedTime = updatedTime;
    }

    getUpdatedTime(): string {
        return this._updatedTime;
    }

    setThreadId(id: string) {
        this._threadId = id;
    }

    getThreadId(): string {
        return this._threadId;
    }

    setTitle(title: string) {
        this._title = title;
        this.headerView.updateTitle();
        this.fullView.threadDisplay.updateTitle();
    }

    getTitle(): string {
        return this._title;
    }

    setDescription(description: string) {
        this._description = description;
        this.fullView.threadDisplay.updateDescription();
    }

    getDescription(): string {
        return this._description;
    }

    setCreatorAccountId(owner: string) {
        this._creatorAccountId = owner;
        this.headerView.updateOwner();
        this.fullView.threadDisplay.updateOwner();
    }

    getCreatorAccountId(): string {
        return this._creatorAccountId;
    }

    setCreatorDisplayName(displayName: string) {
        this._creatorDisplayName = displayName;
    }

    getCreatorDisplayName(): string {
        return this._creatorDisplayName;
    }

    setAvatarURL(avatarURL: string) {
        this._creatorAvatarUrl = avatarURL;
        this.headerView.updateAvatarURL();
        this.fullView.threadDisplay.updateAvatarURL();
    }

    getAvatarURL(): string {
        return this._creatorAvatarUrl;
    }

    addComment(comment: CommentInfo) {
        this._comments.push(comment);
        this.fullView.addComment(comment);
        this.headerView.updateCommentCount();
    }

    getComment(commentID: string): CommentInfo {
        for (let i = 0; i < this._comments.length; i++) {
            let ci: CommentInfo = this._comments[i];
            if (ci.getCommentID() == commentID) {
                return ci;
            }
        }

        return null;
    }

    removeComment(commentID: string) {
        let c: CommentInfo = this.getComment(commentID);
        if (c != null) {
            this._comments.splice(this._comments.indexOf(c), 1);
            this.fullView.removeComment(c);
            this.headerView.updateCommentCount();
        }
    }
}
