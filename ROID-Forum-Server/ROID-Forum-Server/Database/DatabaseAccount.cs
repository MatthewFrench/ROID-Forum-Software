using System;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public class DatabaseAccount
{
	private static string TABLE_CURRENT_ONLINE = "CurrentOnline";
    private static string TABLE_ACCOUNT_IP_ACCESS = "AccountIPAccess";
    private static string TABLE_ACCOUNT_AUTHENTICATION = "AccountAuthentication";
    private static string TABLE_ACCOUNT = "Account";

    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT_IP_ACCESS}"" (
                   account_id uuid,
                   ip_address inet,
                   PRIMARY KEY (account_id, ip_address)
                )");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT_AUTHENTICATION}"" (
                   account_id uuid,
                   username TEXT,
                   password TEXT,
                   PRIMARY KEY (username)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT_AUTHENTICATION}"" (account_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT}"" (
                   account_id uuid,
                   display_name TEXT,
                   email TEXT,
                   avatar_url TEXT,
                   created_time timeuuid,
                   PRIMARY KEY (account_id)
                )");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_CURRENT_ONLINE}"" (
                   account_id uuid,
                   PRIMARY KEY (account_id)
                )");
    }
    
    public Guid? GetAccountIDForCredentials(ISession session, string username, string password)
    {
	    string lowercaseUsername = username.ToLower();
	    PreparedStatement selectStatement = session.Prepare($"SELECT account_id, password FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT_AUTHENTICATION}\" where username=?");
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
    public (CreateAccountStatus, Guid?) CreateAccount(ISession session, string username, string password, string email)
    {
	    // First ensure that the username isn't already taken
	    string lowercaseUsername = username.ToLower();
	    PreparedStatement selectStatement = session.Prepare($"SELECT account_id, password FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT_AUTHENTICATION}\" where username=?");
	    var result = session.Execute(selectStatement.Bind(lowercaseUsername));
	    var firstItem = result.FirstOrDefault();
	    if (firstItem != null)
	    {
		    return (CreateAccountStatus.AlreadyExists, null);
	    }
	    // Create the authentication account
	    string hashedPassword = SecretHasher.Hash(password);
	    Guid accountID = Guid.NewGuid();
	    PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT_AUTHENTICATION}\" (account_id, username, password) VALUES (?, ?, ?)");
	    session.Execute(insertStatement.Bind(accountID, lowercaseUsername, hashedPassword));
	    // Create the account details
	    // Todo: Use a generic picture url for avatar_url
	    PreparedStatement insertAccount = session.Prepare($"INSERT INTO \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" (account_id, display_name, email, created_time) VALUES (?, ?, ?, now())");
	    session.Execute(insertAccount.Bind(accountID, username, email));
	    return (CreateAccountStatus.Created, accountID);
    }
    
    public String GetAccountName(ISession session, Guid accountId)
    {
	    PreparedStatement selectStatement = session.Prepare($"SELECT display_name FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" where account_id=?");
	    var result = session.Execute(selectStatement.Bind(accountId));
	    // Todo: Add safety checks
	    return result.FirstOrDefault().GetValue<String>("display_name");
    }
    public String GetAvatarUrl(ISession session, Guid accountId)
    {
	    PreparedStatement selectStatement = session.Prepare($"SELECT avatar_url FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" where account_id=?");
	    var result = session.Execute(selectStatement.Bind(accountId));
	    // Todo: Add safety checks
	    string avatarUrl = result.FirstOrDefault().GetValue<String>("avatar_url");
	    return avatarUrl == null ? "" : avatarUrl;
    }
    public void SetAvatarUrl(ISession session, Guid accountId, String text)
    {
	    PreparedStatement selectStatement = session.Prepare($"UPDATE \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" set avatar_url=? where account_id=?");
	    session.Execute(selectStatement.Bind(text, accountId));
    }
}