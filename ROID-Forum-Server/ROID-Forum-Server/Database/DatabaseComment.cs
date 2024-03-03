using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public static class DatabaseComment
{
    private const string TableComment = "Comment";

    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableComment}"" (
                   section_id uuid,
                   thread_id uuid,
                   comment_id_and_created_time timeuuid,
                   content text,
                   creator_account_id uuid,
                   PRIMARY KEY (thread_id, comment_id_and_created_time)
                ) WITH CLUSTERING ORDER BY (comment_id_and_created_time ASC)");
    }

    public static TimeUuid CreateComment(ISession session, Guid accountId, Guid sectionId, Guid threadId, string content)
    {
        TimeUuid commentIdAndCreatedTime = TimeUuid.NewId();
        PreparedStatement insertStatement =
            session.Prepare(
                $"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableComment}\" (section_id, thread_id, comment_id_and_created_time, content, creator_account_id) VALUES (?, ?, ?, ?, ?)");
        session.Execute(insertStatement.Bind(sectionId, threadId, commentIdAndCreatedTime, content, accountId));
        return commentIdAndCreatedTime;
    }

    public static Guid? GetCommentOwner(ISession session, Guid threadId, TimeUuid commentIdAndCreatedTime)
    {
        PreparedStatement selectStatement =
            session.Prepare(
                $"SELECT creator_account_id FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=? and comment_id_and_created_time=?");
        var result = session.Execute(selectStatement.Bind(threadId, commentIdAndCreatedTime));
        var item = result.FirstOrDefault();
        if (item == null)
        {
            return null;
        }

        return item.GetValue<Guid>("creator_account_id");
    }

    public static void UpdateComment(ISession session, Guid threadId, TimeUuid commentIdAndCreatedTime, string content)
    {
        PreparedStatement updateStatement =
            session.Prepare(
                $"UPDATE \"{Database.DefaultKeyspace}\".\"{TableComment}\" set content=? where thread_id=? and comment_id_and_created_time=?");
        session.Execute(updateStatement.Bind(content, threadId, commentIdAndCreatedTime));
    }

    public static void DeleteComment(ISession session, Guid threadId, TimeUuid commentIdAndCreatedTime)
    {
        PreparedStatement deleteStatement =
            session.Prepare($"DELETE FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=? and comment_id_and_created_time=?");
        session.Execute(deleteStatement.Bind(threadId, commentIdAndCreatedTime));
    }

    public record class DatabaseCommentData(
        Guid sectionId,
        Guid threadId,
        TimeUuid commentIdAndCreatedTime,
        Guid creatorAccountId,
        String text,
        string creatorDisplayName,
        string creatorAvatarUrl);

    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in a section
    public static List<DatabaseCommentData> GetThreadComments(ISession session, Guid threadId)
    {
        List<DatabaseCommentData> commentDatas = new List<DatabaseCommentData>();
        var commentSelectStatement = session.Prepare(
            $"SELECT section_id, thread_id, comment_id_and_created_time, content, creator_account_id FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=?");
        var commentResult = session.Execute(commentSelectStatement.Bind(threadId));
        foreach (Row item in commentResult)
        {
            var creatorAccountId = item.GetValue<Guid>("creator_account_id");
            commentDatas.Add(new DatabaseCommentData(
                item.GetValue<Guid>("section_id"),
                item.GetValue<Guid>("thread_id"),
                item.GetValue<TimeUuid>("comment_id_and_created_time"),
                creatorAccountId,
                item.GetValue<String>("content"),
                DatabaseAccount.GetAccountDisplayName(session, creatorAccountId),
                DatabaseAccount.GetAvatarUrl(session, creatorAccountId)
            ));
        }

        return commentDatas;
    }

    public static DatabaseCommentData GetComment(ISession session, Guid threadId, TimeUuid commentIdAndCreatedTime)
    {
        var commentSelectStatement = session.Prepare(
            $"SELECT section_id, thread_id, comment_id_and_created_time, content, creator_account_id FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=? and comment_id_and_created_time=?");
        var commentResult = session.Execute(commentSelectStatement.Bind(threadId, commentIdAndCreatedTime));
        var commentItem = commentResult.FirstOrDefault();
        var creatorAccountId = commentItem.GetValue<Guid>("creator_account_id");
        return new DatabaseCommentData(
            commentItem.GetValue<Guid>("section_id"),
            commentItem.GetValue<Guid>("thread_id"),
            commentItem.GetValue<TimeUuid>("comment_id_and_created_time"),
            creatorAccountId,
            commentItem.GetValue<String>("content"),
            DatabaseAccount.GetAccountDisplayName(session, creatorAccountId),
            DatabaseAccount.GetAvatarUrl(session, creatorAccountId)
        );
    }

    public static UInt32 GetCommentCount(ISession session, Guid threadId)
    {
        var commentSelectStatement = session.Prepare(
            $"SELECT count(*) FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=?");
        var commentResult = session.Execute(commentSelectStatement.Bind(threadId));
        var item = commentResult.FirstOrDefault();
        return (UInt32)item.GetValue<Int64>("count");
    }

    public static void DeleteCommentsForThread(ISession session, Guid threadId)
    {
        PreparedStatement deleteCommentsStatement =
            session.Prepare($"DELETE FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=?");
        session.Execute(deleteCommentsStatement.Bind(threadId));
    }
    
    public static bool CommentIdExists(ISession session, Guid threadId, TimeUuid commentIdAndCreatedTime)
    {
        PreparedStatement selectStatement =
            session.Prepare($"SELECT comment_id_and_created_time FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=? and comment_id_and_created_time=?");
        var result = session.Execute(selectStatement.Bind(threadId, commentIdAndCreatedTime));
        if (result.FirstOrDefault() == null)
        {
            return false;
        }

        return true;
    }
}