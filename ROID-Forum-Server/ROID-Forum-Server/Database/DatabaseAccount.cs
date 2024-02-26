using System;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public static class DatabaseAccount
{
	private const string TableCurrentOnline = "CurrentOnline";
    private const string TableAccountIpAccess = "AccountIPAccess";
    private const string TableAccountAuthentication = "AccountAuthentication";
    private const string TableAccount = "Account";

    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableAccountIpAccess}"" (
                   account_id uuid,
                   ip_address inet,
                   PRIMARY KEY (account_id, ip_address)
                )");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableAccountAuthentication}"" (
                   account_id uuid,
                   username TEXT,
                   password TEXT,
                   PRIMARY KEY (username)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DefaultKeyspace}"".""{TableAccountAuthentication}"" (account_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableAccount}"" (
                   account_id uuid,
                   display_name TEXT,
                   email TEXT,
                   avatar_url TEXT,
                   created_time timeuuid,
                   PRIMARY KEY (account_id)
                )");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableCurrentOnline}"" (
                   account_id uuid,
                   PRIMARY KEY (account_id)
                )");
    }
    
    public static Guid? GetAccountIdForCredentials(ISession session, string username, string password)
    {
	    var lowercaseUsername = username.ToLower();
	    var selectStatement = session.Prepare($"SELECT account_id, password FROM \"{Database.DefaultKeyspace}\".\"{TableAccountAuthentication}\" where username=?");
	    var result = session.Execute(selectStatement.Bind(lowercaseUsername));
	    var firstItem = result.FirstOrDefault();
	    if (firstItem == null)
	    {
		    return null;
	    }
	    if (SecretHasher.Verify(password, firstItem.GetValue<string>("password")))
	    {
		    return firstItem.GetValue<Guid>("account_id");
	    }
	    return null;
    }
    public enum CreateAccountStatus
    {
	    Created,
	    AlreadyExists
    }
    public static (CreateAccountStatus, Guid?) CreateAccount(ISession session, string username, string password, string email)
    {
	    // First ensure that the username isn't already taken
	    var lowercaseUsername = username.ToLower();
	    PreparedStatement selectStatement = session.Prepare($"SELECT account_id, password FROM \"{Database.DefaultKeyspace}\".\"{TableAccountAuthentication}\" where username=?");
	    var result = session.Execute(selectStatement.Bind(lowercaseUsername));
	    var firstItem = result.FirstOrDefault();
	    if (firstItem != null)
	    {
		    return (CreateAccountStatus.AlreadyExists, null);
	    }
	    // Create the authentication account
	    var hashedPassword = SecretHasher.Hash(password);
	    var accountId = Guid.NewGuid();
	    var insertStatement = session.Prepare($"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableAccountAuthentication}\" (account_id, username, password) VALUES (?, ?, ?)");
	    session.Execute(insertStatement.Bind(accountId, lowercaseUsername, hashedPassword));
	    // Create the account details
	    // Todo: Use a generic picture url for avatar_url
	    var insertAccount = session.Prepare($"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableAccount}\" (account_id, display_name, email, created_time) VALUES (?, ?, ?, now())");
	    session.Execute(insertAccount.Bind(accountId, username, email));
	    return (CreateAccountStatus.Created, accountId);
    }
    
    public static string GetAccountDisplayName(ISession session, Guid accountId)
    {
	    var selectStatement = session.Prepare($"SELECT display_name FROM \"{Database.DefaultKeyspace}\".\"{TableAccount}\" where account_id=?");
	    var result = session.Execute(selectStatement.Bind(accountId));
	    // Todo: Add safety checks
	    return result.FirstOrDefault().GetValue<String>("display_name");
    }
    public static string GetAvatarUrl(ISession session, Guid accountId)
    {
	    var selectStatement = session.Prepare($"SELECT avatar_url FROM \"{Database.DefaultKeyspace}\".\"{TableAccount}\" where account_id=?");
	    var result = session.Execute(selectStatement.Bind(accountId));
	    // Todo: Add safety checks
	    string avatarUrl = result.FirstOrDefault().GetValue<String>("avatar_url");
	    return avatarUrl == null ? "" : avatarUrl;
    }
    public static void SetAvatarUrl(ISession session, Guid accountId, String text)
    {
	    var updateStatement = session.Prepare($"UPDATE \"{Database.DefaultKeyspace}\".\"{TableAccount}\" set avatar_url=? where account_id=?");
	    session.Execute(updateStatement.Bind(text, accountId));
    }
    public static void SetDisplayName(ISession session, Guid accountId, String text)
    {
	    var updateStatement = session.Prepare($"UPDATE \"{Database.DefaultKeyspace}\".\"{TableAccount}\" set display_name=? where account_id=?");
	    session.Execute(updateStatement.Bind(text, accountId));
    }
}