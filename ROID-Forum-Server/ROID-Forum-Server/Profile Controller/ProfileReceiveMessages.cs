namespace ROIDForumServer;

public enum ProfileReceiveMessages : byte
{ 
    SetAvatar = 0,
    GetAvatar = 1,
    Login = 2,
    Logout = 3,
    Register = 4,
    UpdateDisplayName = 5,
    GetDisplayName = 6
}