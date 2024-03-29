using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public static class DatabaseThread
{
    private const string TableThread = "Thread";
    private const string TableActiveInThread = "ActiveInThread";

    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableThread}"" (
                   section_id uuid,
                   thread_id uuid,
                   creator_account_id uuid,
                   title TEXT,
                   description TEXT,
                   created_time timeuuid,
                   PRIMARY KEY (thread_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DefaultKeyspace}"".""{TableThread}"" (creator_account_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableActiveInThread}"" (
                   account_id uuid,
                   thread_id uuid,
                   PRIMARY KEY (thread_id, account_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DefaultKeyspace}"".""{TableActiveInThread}"" (account_id)");
    }

    public static Guid CreateThread(ISession session, Guid accountId, Guid sectionId, string title, string description)
    {
        Guid threadId = Guid.NewGuid();
        PreparedStatement insertStatement =
            session.Prepare(
                $"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableThread}\" (section_id, thread_id, creator_account_id, title, description, created_time) VALUES (?, ?, ?, ?, ?, now())");
        session.Execute(insertStatement.Bind(sectionId, threadId, accountId, title, description));
        DatabaseSection.UpdateSectionThreadOrdering(session, sectionId, threadId);
        return threadId;
    }

    public static Guid? GetThreadOwner(ISession session, Guid threadId)
    {
        PreparedStatement selectStatement =
            session.Prepare(
                $"SELECT creator_account_id FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
        var result = session.Execute(selectStatement.Bind(threadId));
        var item = result.FirstOrDefault();
        if (item == null)
        {
            return null;
        }

        return item.GetValue<Guid>("creator_account_id");
    }

    public static void UpdateThreadTitleAndDescription(ISession session, Guid threadId,
        string title, string description)
    {
        PreparedStatement updateStatement =
            session.Prepare(
                $"UPDATE \"{Database.DefaultKeyspace}\".\"{TableThread}\" set title=? and description=? where thread_id=?");
        session.Execute(updateStatement.Bind(title, description, threadId));
    }

    public static void DeleteThreadAndComments(ISession session, Guid accountId, Guid sectionId, Guid threadId)
    {
        // Delete ordering
        DatabaseSection.DeleteSectionThreadOrdering(session, sectionId, threadId);
        PreparedStatement deleteStatement =
            session.Prepare($"DELETE FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
        session.Execute(deleteStatement.Bind(threadId));
        DatabaseComment.DeleteCommentsForThread(session, threadId);
    }

    public record class DatabaseThreadHeaderData(
        Guid sectionId,
        Guid threadId,
        Guid creatorAccountId,
        string title,
        string description,
        TimeUuid createdTime,
        TimeUuid updatedTime,
        UInt32 commentCount,
        string creatorDisplayName,
        string creatorAvatarUrl
    );

    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in a section
    public static List<DatabaseThreadHeaderData> GetThreadHeadersInSection(ISession session, Guid sectionId)
    {
        var orderedThreadIds = DatabaseSection.GetOrderedThreadIdsInSection(session, sectionId);
        List<DatabaseThreadHeaderData> results = new List<DatabaseThreadHeaderData>();
        foreach (var (_, threadId, updatedTime) in orderedThreadIds)
        {
            var selectThreadStatement = session.Prepare(
                $"SELECT creator_account_id, title, description, created_time FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
            var threadResult = session.Execute(selectThreadStatement.Bind(threadId));
            var threadItem = threadResult.FirstOrDefault();
            var creatorAccountId = threadItem.GetValue<Guid>("creator_account_id");
            results.Add(new DatabaseThreadHeaderData(
                sectionId,
                threadId,
                creatorAccountId,
                threadItem.GetValue<String>("title"),
                threadItem.GetValue<String>("description"),
                threadItem.GetValue<TimeUuid>("created_time"),
                updatedTime,
                DatabaseComment.GetCommentCount(session, threadId),
                DatabaseAccount.GetAccountDisplayName(session, creatorAccountId),
                DatabaseAccount.GetAvatarUrl(session, creatorAccountId)
            ));
        }

        return results;
    }

    public static DatabaseThreadHeaderData GetThreadHeader(ISession session, Guid threadId)
    {
        var selectThreadStatement = session.Prepare(
            $"SELECT section_id, creator_account_id, title, description, created_time FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
        var threadResult = session.Execute(selectThreadStatement.Bind(threadId));
        var threadItem = threadResult.FirstOrDefault();
        var creatorAccountId = threadItem.GetValue<Guid>("creator_account_id");
        var sectionId = threadItem.GetValue<Guid>("section_id");
        return new DatabaseThreadHeaderData(
            sectionId,
            threadId,
            creatorAccountId,
            threadItem.GetValue<String>("title"),
            threadItem.GetValue<String>("description"),
            threadItem.GetValue<TimeUuid>("created_time"),
            DatabaseSection.GetThreadUpdatedTime(session, sectionId, threadId),
            DatabaseComment.GetCommentCount(session, threadId),
            DatabaseAccount.GetAccountDisplayName(session, creatorAccountId),
            DatabaseAccount.GetAvatarUrl(session, creatorAccountId)
        );
    }

    public static bool ThreadIdExists(ISession session, Guid threadId)
    {
        PreparedStatement selectStatement =
            session.Prepare($"SELECT thread_id FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
        var result = session.Execute(selectStatement.Bind(threadId));
        if (result.FirstOrDefault() == null)
        {
            return false;
        }

        return true;
    }
}