namespace ROIDForumServer;

public static class Environment
{
    public static bool IsRunningLocally()
    {
#nullable enable
        var environment = System.Environment.GetEnvironmentVariable("environment");
        return "local".Equals(environment);
    }
}