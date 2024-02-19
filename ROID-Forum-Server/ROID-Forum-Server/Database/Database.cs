using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public class Database
{
    ServerController serverController;
    private string IP = "0.0.0.0";
    private int PORT = 9042;
    private string USERNAME = "cassandra";
    private string PASSWORD = "cassandra";
    private Cluster clusterConnection;
    private ISession session;
    private string DEFAULT_KEYSPACE = "forum";
    private string TABLE_ACCOUNT_IP_ACCESS = "AccountIPAccess";
    private string TABLE_ACCOUNT_AUTHENTICATION = "AccountAuthentication";
    private string TABLE_ACCOUNT = "Account";
    private string TABLE_SECTION = "Section";
    private string TABLE_THREAD = "Thread";
    private string TABLE_COMMENT = "Comment";
    private string TABLE_CHAT = "Chat";
    private string TABLE_ACTIVE_IN_SECTION = "ActiveInSection";
    private string TABLE_ACTIVE_IN_THREAD = "ActiveInThread";
    private string TABLE_CURRENT_ONLINE = "CurrentOnline";
    public Database(ServerController controller) {
        serverController = controller;
        ConnectToSession();
    }
    
    /*
     * Connect to the cluster and create a session.
     */
    private void ConnectToSession() {         
        Console.WriteLine($"Connecting to Cassandra cluster at {IP}:{PORT}!");
        clusterConnection = Cluster.Builder()
            .AddContactPoints(IP).WithPort(PORT)
            .WithCredentials(USERNAME, PASSWORD)
            .WithCompression(CompressionType.Snappy)
            .Build();
        session = clusterConnection.Connect();
        // Set up default keyspace
        session.Execute(@"CREATE KEYSPACE IF NOT EXISTS """ + DEFAULT_KEYSPACE + 
                        "\" WITH replication = {'class': 'SimpleStrategy',  'replication_factor': '1' }; ");
        // Set up tables if they don't exist
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT_IP_ACCESS}"" (
                   account_id uuid,
                   ip_address inet,
                   PRIMARY KEY (account_id, ip_address)
                )");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT_AUTHENTICATION}"" (
                   account_id uuid,
                   username TEXT,
                   password TEXT,
                   PRIMARY KEY (username)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT_AUTHENTICATION}"" (account_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_ACCOUNT}"" (
                   account_id uuid,
                   display_name TEXT,
                   email TEXT,
                   avatar_url TEXT,
                   created_time timeuuid,
                   PRIMARY KEY (account_id)
                )");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_SECTION}"" (
                   section_id uuid,
                   name TEXT,
                   PRIMARY KEY (section_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{DEFAULT_KEYSPACE}"".""{TABLE_SECTION}"" (name)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_THREAD}"" (
                   section_id uuid,
                   thread_id uuid,
                   creator_account_id uuid,
                   title TEXT,
                   created_time timeuuid,
                   updated_time timeuuid,
                   PRIMARY KEY (section_id, updated_time)
                ) WITH compaction = {{ 'class' : 'TimeWindowCompactionStrategy' }}");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{DEFAULT_KEYSPACE}"".""{TABLE_THREAD}"" (thread_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_COMMENT}"" (
                   thread_id uuid,
                   comment_id uuid,
                   content text,
                   creator_account_id uuid,
                   created_time timeuuid,
                   PRIMARY KEY (thread_id, created_time)
                ) WITH compaction = {{ 'class' : 'TimeWindowCompactionStrategy' }}");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{DEFAULT_KEYSPACE}"".""{TABLE_COMMENT}"" (comment_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_CHAT}"" (
                   chat_id uuid,
                   creator_account_id uuid,
                   created_time timeuuid,
                   content text,
                   PRIMARY KEY (chat_id, created_time)
                ) WITH CLUSTERING ORDER BY (created_time DESC)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_SECTION}"" (
                   account_id uuid,
                   section_id uuid,
                   PRIMARY KEY (section_id, account_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_SECTION}"" (account_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_THREAD}"" (
                   account_id uuid,
                   thread_id uuid,
                   PRIMARY KEY (thread_id, account_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_THREAD}"" (account_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{DEFAULT_KEYSPACE}"".""{TABLE_CURRENT_ONLINE}"" (
                   account_id uuid,
                   PRIMARY KEY (account_id)
                )");
    }

    public Guid LoadSectionID(String sectionName)
    {
	    // Create the section if it doesn't already exist
	    PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{DEFAULT_KEYSPACE}\".\"{TABLE_SECTION}\" (section_id, name) VALUES (uuid(), ?) IF NOT EXISTS");
	    session.Execute(insertStatement.Bind(sectionName));
	    PreparedStatement selectStatement = session.Prepare($"SELECT section_id FROM \"{DEFAULT_KEYSPACE}\".\"{TABLE_SECTION}\" where name=?");
	    var result = session.Execute(selectStatement.Bind(sectionName));
	    return result.First().GetValue<Guid>("section_id");
    }

    public Guid? GetAccountIDForCredentials(string username, string password)
    {
	    string lowercaseUsername = username.ToLower();
	    PreparedStatement selectStatement = session.Prepare($"SELECT account_id, password FROM \"{DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT_AUTHENTICATION}\" where username=?");
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
    public (CreateAccountStatus, Guid?) CreateAccount(string username, string password, string email)
    {
	    // First ensure that the username isn't already taken
	    string lowercaseUsername = username.ToLower();
	    PreparedStatement selectStatement = session.Prepare($"SELECT account_id, password FROM \"{DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT_AUTHENTICATION}\" where username=?");
	    var result = session.Execute(selectStatement.Bind(lowercaseUsername));
	    var firstItem = result.FirstOrDefault();
	    if (firstItem != null)
	    {
		    return (CreateAccountStatus.AlreadyExists, null);
	    }
	    // Create the authentication account
	    string hashedPassword = SecretHasher.Hash(password);
	    Guid accountID = Guid.NewGuid();
	    PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT_AUTHENTICATION}\" (account_id, username, password) VALUES (?, ?, ?)");
	    session.Execute(insertStatement.Bind(accountID, lowercaseUsername, hashedPassword));
	    // Create the account details
	    // Todo: Use a generic picture url for avatar_url
	    PreparedStatement insertAccount = session.Prepare($"INSERT INTO \"{DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" (account_id, display_name, email, created_time) VALUES (?, ?, ?, now())");
	    session.Execute(insertAccount.Bind(accountID, username, email));
	    return (CreateAccountStatus.Created, accountID);
    }
    public void SubmitChat(Guid accountID, string content)
    {
	    PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{DEFAULT_KEYSPACE}\".\"{TABLE_CHAT}\" (creator_account_id, chat_id, created_time, content) VALUES (?, uuid(), now(), ?)");
	    session.Execute(insertStatement.Bind(accountID, content));
    }
    public record class DatabaseChatData(Guid creator_account_id, Guid chat_id, string content, TimeUuid created_time);
    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in chats
    public List<DatabaseChatData> GetRecentChats()
    {
	    // Order by isn't needed since Cassandra is ordering the data in descending order
	    var result = session.Execute($"SELECT chat_id, creator_account_id, created_time, content FROM \"{DEFAULT_KEYSPACE}\".\"{TABLE_CHAT}\" LIMIT 100");
	    List<DatabaseChatData> results = new List<DatabaseChatData>();
	    foreach (Row item in result)
	    {
		    results.Add(new DatabaseChatData(item.GetValue<Guid>("creator_account_id"), item.GetValue<Guid>("chat_id"), item.GetValue<String>("content"), item.GetValue<TimeUuid>("created_time")));
	    }

	    return results;
    }
    public String GetAccountName(Guid accountId)
    {
	    PreparedStatement selectStatement = session.Prepare($"SELECT display_name FROM \"{DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" where account_id=?");
	    var result = session.Execute(selectStatement.Bind(accountId));
	    // Todo: Add safety checks
	    return result.FirstOrDefault().GetValue<String>("display_name");
    }
    public String GetAvatarUrl(Guid accountId)
    {
	    PreparedStatement selectStatement = session.Prepare($"SELECT avatar_url FROM \"{DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" where account_id=?");
	    var result = session.Execute(selectStatement.Bind(accountId));
	    // Todo: Add safety checks
	    string avatarUrl = result.FirstOrDefault().GetValue<String>("avatar_url");
	    return avatarUrl == null ? "" : avatarUrl;
    }
    public void SetAvatarUrl(Guid accountId, String text)
    {
	    PreparedStatement selectStatement = session.Prepare($"UPDATE \"{DEFAULT_KEYSPACE}\".\"{TABLE_ACCOUNT}\" set avatar_url=? where account_id=?");
	    session.Execute(selectStatement.Bind(text, accountId));
    }
    /*
     * Schemas
		AccountIPAccess
       		account_id UUID
       		ip_address inet
       		PRIMARY KEY (account_id, ip_address)
		AccountAuthentication
       		account_id UUID
       		username TEXT
       		password TEXT
       		PRIMARY KEY (username, password)
		Account
       		account_id UUID
       		display_name TEXT
       		email TEXT
       		avatar_url TEXT
       		created_time timeuuid
       		PRIMARY KEY (account_id)
		Section
       		section_id UUID
       		name TEXT
       		PRIMARY KEY (section_id)
		Thread
       		section_id UUID
			thread_id UUID
       		title TEXT
       		creator_account_id UUID
       		created_time timeuuid
       		updated_time timeuuid
       		PRIMARY KEY (section_id, updated_time) WITH compaction = { 'class' : 'TimeWindowCompactionStrategy' };
       		CREATE INDEX ON Thread(thread_id);
		Comment
       		thread_id UUID
       		comment_id UUID
       		content TEXT
       		creator_account_id UUID
       		created_time timeuuid
       		PRIMARY KEY (thread_id, created_time) WITH compaction = { 'class' : 'TimeWindowCompactionStrategy' };
       		CREATE INDEX ON Comment(comment_id);
		Chat
       		chat_id UUID
       		creator_account_id UUID
       		created_time timeuuid
       		content TEXT
       		PRIMARY KEY (created_time) WITH compaction = { 'class' : 'TimeWindowCompactionStrategy' };
       		CREATE INDEX ON Chat(chat_id);
		ActiveInSection (add TTL and make server regularly update)
       		account_id UUID
       		section_id UUID
       		PRIMARY KEY (account_id)
		ActiveInThread (add TTL and make server regularly update)
       		account_id UUID
       		thread_id UUID
       		PRIMARY KEY (account_id)
		CurrentOnline (add TTL and make server regularly update)
       		account_id UUID
       		PRIMARY KEY (account_id)
     */
}