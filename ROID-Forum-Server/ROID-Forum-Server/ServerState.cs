namespace ROIDForumServer;

public class ServerState
{
    public Database Database { get; }
    public Networking Networking { get; }

    public ServerState()
    {
        Database = new Database();
        Networking = new Networking(this);
    }
}