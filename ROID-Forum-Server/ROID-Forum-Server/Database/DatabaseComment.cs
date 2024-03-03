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
                   comment_id uuid,
                   content text,
                   creator_account_id uuid,
                   created_time timeuuid,
                   PRIMARY KEY (thread_id, created_time)
                ) WITH CLUSTERING ORDER BY (created_time ASC)");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DefaultKeyspace}"".""{TableComment}"" (comment_id)");
    }

    public static Guid CreateComment(ISession session, Guid accountId, Guid sectionId, Guid threadId, string content)
    {
        Guid commentId = Guid.NewGuid();
        PreparedStatement insertStatement =
            session.Prepare(
                $"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableComment}\" (section_id, thread_id, comment_id, content, creator_account_id, created_time) VALUES (?, ?, ?, ?, ?, now())");
        session.Execute(insertStatement.Bind(sectionId, threadId, commentId, content, accountId));
        return commentId;
    }

    public static Guid? GetCommentOwner(ISession session, Guid commentId)
    {
        PreparedStatement selectStatement =
            session.Prepare(
                $"SELECT creator_account_id FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where comment_id=?");
        var result = session.Execute(selectStatement.Bind(commentId));
        var item = result.FirstOrDefault();
        if (item == null)
        {
            return null;
        }

        return item.GetValue<Guid>("creator_account_id");
    }

    public static void UpdateComment(ISession session, Guid commentId, string content)
    {
        PreparedStatement updateStatement =
            session.Prepare(
                $"UPDATE \"{Database.DefaultKeyspace}\".\"{TableComment}\" set content=? where comment_id=?");
        session.Execute(updateStatement.Bind(content, commentId));
    }

    public static void DeleteComment(ISession session, Guid commentId)
    {
        PreparedStatement deleteStatement =
            session.Prepare($"DELETE FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where comment_id=?");
        session.Execute(deleteStatement.Bind(commentId));
    }

    public record class DatabaseCommentData(
        Guid sectionId,
        Guid threadId,
        Guid commentId,
        Guid creatorAccountId,
        String text,
        TimeUuid createdTime);

    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in a section
    public static List<DatabaseCommentData> GetThreadComments(ISession session, Guid threadId)
    {
        List<DatabaseCommentData> commentDatas = new List<DatabaseCommentData>();
        var commentSelectStatement = session.Prepare(
            $"SELECT section_id, thread_id, comment_id, content, creator_account_id, created_time FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where thread_id=?");
        var commentResult = session.Execute(commentSelectStatement.Bind(threadId));
        foreach (Row item in commentResult)
        {
            commentDatas.Add(new DatabaseCommentData(
                item.GetValue<Guid>("section_id"),
                item.GetValue<Guid>("thread_id"),
                item.GetValue<Guid>("comment_id"),
                item.GetValue<Guid>("creator_account_id"),
                item.GetValue<String>("content"),
                item.GetValue<TimeUuid>("created_time")
            ));
        }

        return commentDatas;
    }

    public static DatabaseCommentData GetComment(ISession session, Guid commentId)
    {
        List<DatabaseCommentData> commentDatas = new List<DatabaseCommentData>();
        var commentSelectStatement = session.Prepare(
            $"SELECT section_id, thread_id, comment_id, content, creator_account_id, created_time FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where commentId=?");
        var commentResult = session.Execute(commentSelectStatement.Bind(commentId));
        var commentItem = commentResult.FirstOrDefault();
        return new DatabaseCommentData(
            commentItem.GetValue<Guid>("section_id"),
            commentItem.GetValue<Guid>("thread_id"),
            commentItem.GetValue<Guid>("comment_id"),
            commentItem.GetValue<Guid>("creator_account_id"),
            commentItem.GetValue<String>("content"),
            commentItem.GetValue<TimeUuid>("created_time")
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
    
    public static bool CommentIdExists(ISession session, Guid commentId)
    {
        PreparedStatement selectStatement =
            session.Prepare($"SELECT comment_id FROM \"{Database.DefaultKeyspace}\".\"{TableComment}\" where comment_id=?");
        var result = session.Execute(selectStatement.Bind(commentId));
        if (result.FirstOrDefault() == null)
        {
            return false;
        }

        return true;
    }
}