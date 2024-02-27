namespace ROIDForumServer;

public enum ThreadReceiveMessages : byte
{
    BeginViewingThread = 0,
    ExitViewingThread = 1,
    AddComment = 2,
    EditComment = 3,
    DeleteComment = 4
}