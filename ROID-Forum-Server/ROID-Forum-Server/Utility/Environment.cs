namespace ROIDForumServer;

public class Environment
{
    public static bool IsRunningLocally()
    {
#nullable enable
        string? environment = System.Environment.GetEnvironmentVariable("environment");
        return environment != null && environment.Equals("local");
    }
}