//These are the definitions that the server receives.

export class SendMessages {
    public static Controller =
        {
            Chat: 0,
            Profile: 1,
            Section: 2,
            Thread: 3
        };

    public static ChatMessage =
        {
            Message: 0
        };

    public static ProfileMessage =
        {
            SetAvatar: 0,
            GetAvatar: 1,
            Login: 2,
            Logout: 3,
            Register: 4,
            UpdateDisplayName: 5,
            GetDisplayName: 6
        };

    public static SectionMessage = {
        BeginViewingSection: 0,
        ExitViewingSection: 1,
        NewThread: 2,
        EditThreadTitleAndDescription: 3,
        DeleteThread: 4,
    };

    public static ThreadMessage = {
        BeginViewingThread: 0,
        ExitViewingThread: 1,
        AddComment: 2,
        EditComment: 3,
        DeleteComment: 4
    }
}