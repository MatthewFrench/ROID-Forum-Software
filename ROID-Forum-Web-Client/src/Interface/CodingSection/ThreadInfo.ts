import {HeaderView} from "./ThreadViews/HeaderView";
import {CommentInfo} from "./CommentInfo";
import {FullView} from "./ThreadViews/FullView";
import {ThreadController} from "./ThreadController";

export class ThreadInfo {
    _id: number;
    _title: string;
    _description: string;
    _owner: string;
    _avatarURL = "";
    _comments: CommentInfo[];
    headerView: HeaderView;
    fullView: FullView;
    threadController: ThreadController;

    constructor(tc: ThreadController) {
        this.threadController = tc;
        this._comments = [];
        this.headerView = new HeaderView(this, tc);
        this.fullView = new FullView(this, tc);
        this.headerView.updateCommentCount();
    }

    setID(id: number) {
        this._id = id;
    }

    getID(): number {
        return this._id;
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

    setOwner(owner: string) {
        this._owner = owner;
        this.headerView.updateOwner();
        this.fullView.threadDisplay.updateOwner();
    }

    getOwner(): string {
        return this._owner;
    }

    setAvatarURL(avatarURL: string) {
        this._avatarURL = avatarURL;
        this.headerView.updateAvatarURL();
        this.fullView.threadDisplay.updateAvatarURL();
    }

    getAvatarURL(): string {
        return this._avatarURL;
    }

    getCommentCount(): number {
        return this._comments.length;
    }

    addComment(comment: CommentInfo) {
        this._comments.push(comment);
        this.fullView.addComment(comment);
        this.headerView.updateCommentCount();
    }

    getComment(commentID: number): CommentInfo {
        for (let i = 0; i < this._comments.length; i++) {
            let ci: CommentInfo = this._comments[i];
            if (ci.getCommentID() == commentID) {
                return ci;
            }
        }

        return null;
    }

    removeComment(commentID: number) {
        let c: CommentInfo = this.getComment(commentID);
        if (c != null) {
            this._comments.splice(this._comments.indexOf(c), 1);
            this.fullView.removeComment(c);
            this.headerView.updateCommentCount();
        }
    }
}
