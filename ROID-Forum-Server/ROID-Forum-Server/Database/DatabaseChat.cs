using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public class DatabaseChat
{
    private static string TABLE_CHAT = "Chat";
    
    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_CHAT}"" (
                   chat_id uuid,
                   creator_account_id uuid,
                   created_time timeuuid,
                   content text,
                   PRIMARY KEY (chat_id, created_time)
                ) WITH CLUSTERING ORDER BY (created_time DESC)");
    }
    
    public static void SubmitChat(ISession session, Guid accountID, string content)
    {
        PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_CHAT}\" (creator_account_id, chat_id, created_time, content) VALUES (?, uuid(), now(), ?)");
        session.Execute(insertStatement.Bind(accountID, content));
    }
    
    public record class DatabaseChatData(Guid creator_account_id, Guid chat_id, string content, TimeUuid created_time);
    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in chats
    public static List<DatabaseChatData> GetRecentChats(ISession session)
    {
        // Order by isn't needed since Cassandra is ordering the data in descending order
        var result = session.Execute($"SELECT chat_id, creator_account_id, created_time, content FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_CHAT}\" LIMIT 100");
        List<DatabaseChatData> results = new List<DatabaseChatData>();
        foreach (Row item in result)
        {
            results.Add(new DatabaseChatData(item.GetValue<Guid>("creator_account_id"), item.GetValue<Guid>("chat_id"), item.GetValue<String>("content"), item.GetValue<TimeUuid>("created_time")));
        }

        return results;
    }
}