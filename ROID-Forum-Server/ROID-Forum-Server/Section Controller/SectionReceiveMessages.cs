namespace ROIDForumServer;

public enum SectionReceiveMessages : byte
{ 
    BeginViewingSection = 0,
    ExitViewingSection = 1,
    NewThread = 2,
    EditThreadTitleAndDescription = 3,
    DeleteThread = 4,
}