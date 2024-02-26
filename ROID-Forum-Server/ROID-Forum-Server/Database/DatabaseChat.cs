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
    
    public static (Guid creatorAccountId, Guid chatId, TimeUuid createdTime, string content) SubmitChat(ISession session, Guid accountId, string content)
    {
        var chatId = Guid.NewGuid();
        var createdTime = TimeUuid.NewId();
        var insertStatement = session.Prepare($"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableChat}\" (creator_account_id, chat_id, created_time, content) VALUES (?, ?, ?, ?)");
        session.Execute(insertStatement.Bind(accountId, chatId, createdTime, content));
        return (creatorAccountId: accountId, chatId, createdTime, content);
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