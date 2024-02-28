//These are the definitions that the client receives.
export const Controllers = {
    Chat: {
        ID: 0,
        Messages: {
            AllMessages: 0,
            NewMessage: 1,
            AllOnlineList: 2,
            OnlineListAddUser: 3,
            OnlineListRemoveUser: 4,
            OnlineListLoggedInUser: 5,
            OnlineListLoggedOutUser: 6,
            DisplayNameUpdate: 7
        }
    },
    Profile: {
        ID: 1,
        Messages: {
            ReturnAvatar: 0,
            LoginFailed: 1,
            LoggedOut: 2,
            RegisterFailed: 3,
            LoggedIn: 4,
            ReturnDisplayName: 5
        }
    },
    Section: {
        ID: 2,
        Messages: {
            AllSectionViewers: 0,
            SectionAddViewer: 1,
            SectionRemoveViewer: 2,
            SectionLoggedInViewer: 3,
            SectionLoggedOutViewer: 4,
            AllSectionHeaders: 5,
            AllThreadHeaders: 6,
            ThreadSuccessfullyCreated: 7,
            AddThreadHeader: 8,
            RemoveThreadHeader: 9,
            UpdateThreadTitleAndDescription: 10,
            UpdateThreadCommentCountAndUpdatedTime: 11,
            UpdateThreadCommentCount: 12,
            AvatarUpdate: 13,
            DisplayNameUpdate: 14
        }
    },
    Thread: {
        ID: 3,
        Messages: {
            AllThreadViewers: 0,
            ThreadAddViewer: 1,
            ThreadRemoveViewer: 2,
            ThreadLoggedInViewer: 3,
            ThreadLoggedOutViewer: 4,
            AllComments: 5,
            CommentSuccessfullyCreated: 6,
            AddComment: 7,
            RemoveComment: 8,
            UpdateComment: 9,
            AvatarUpdate: 10,
            DisplayNameUpdate: 11
        }
    },
};