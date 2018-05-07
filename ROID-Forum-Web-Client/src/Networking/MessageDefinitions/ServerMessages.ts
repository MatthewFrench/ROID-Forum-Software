//These are the definitions that the serverURL receives.

export class ServerMessages {
    public static Controller =
    {
        Chat : 0,
        Login : 1,
        Section : 2
    };

    public static ChatMsg =
    {
        Msg : 0,
        OnlineList : 1
    };

    public static SectionMsg = {
        AllThreads : 0,
        AddThread : 1,
        RemoveThread : 2,
        UpdateThread : 3,
        MoveThreadToTop : 4,
        AddComment : 5,
        RemoveComment : 6,
        UpdateComment : 7
    };

    public static LoginMsg = {
        GetAvatar : 0,
        LoginFailed : 1,
        LoggedOut : 2,
        RegisterFailed : 3,
        LoggedIn : 4,
    }
}