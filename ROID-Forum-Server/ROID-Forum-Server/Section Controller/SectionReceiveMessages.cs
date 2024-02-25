namespace ROIDForumServer;

public enum SectionReceiveMessages : byte
{ 
    NewPost = 0,
    EditPost = 1,
    DeletePost = 2,
    AddComment = 3,
    EditComment = 4,
    DeleteComment = 5,
    ViewingSection = 6,
    ViewingThread = 7
}