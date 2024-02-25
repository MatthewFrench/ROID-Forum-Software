using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public static class DatabaseChat
{
    private const string TableChat = "Chat";
    
    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableChat}"" (
                   chat_id uuid,
                   creator_account_id uuid,
                   created_time timeuuid,
                   content text,
                   PRIMARY KEY (chat_id, created_time)
                ) WITH CLUSTERING ORDER BY (created_time DESC)");
    }
    
    public static void SubmitChat(ISession session, Guid accountId, string content)
    {
        PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableChat}\" (creator_account_id, chat_id, created_time, content) VALUES (?, uuid(), now(), ?)");
        session.Execute(insertStatement.Bind(accountId, content));
    }
    
    public record class DatabaseChatData(Guid CreatorAccountId, Guid ChatId, string Content, TimeUuid CreatedTime);
    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in chats
    public static List<DatabaseChatData> GetRecentChats(ISession session)
    {
        // Order by isn't needed since Cassandra is ordering the data in descending order
        var result = session.Execute($"SELECT chat_id, creator_account_id, created_time, content FROM \"{Database.DefaultKeyspace}\".\"{TableChat}\" LIMIT 100");
        List<DatabaseChatData> results = new List<DatabaseChatData>();
        foreach (var item in result)
        {
            results.Add(new DatabaseChatData(item.GetValue<Guid>("creator_account_id"), item.GetValue<Guid>("chat_id"), item.GetValue<String>("content"), item.GetValue<TimeUuid>("created_time")));
        }

        return results;
    }
}