using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public class DatabaseComment
{
    private static string TABLE_COMMENT = "Comment";
    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_COMMENT}"" (
                   thread_id uuid,
                   comment_id uuid,
                   content text,
                   creator_account_id uuid,
                   created_time timeuuid,
                   PRIMARY KEY (thread_id, created_time)
                ) WITH CLUSTERING ORDER BY (created_time DESC)");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_COMMENT}"" (comment_id)");
    }
    
    public static void CreateComment(ISession session, Guid accountID, Guid threadID, Guid sectionID, string content)
    {
	    Guid commentID = Guid.NewGuid();
	    PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_COMMENT}\" (thread_id, comment_id, content, creator_account_id, created_time) VALUES (?, ?, ?, ?, now())");
	    session.Execute(insertStatement.Bind(threadID, commentID, content, accountID));
	    // Update also acts as an insert if the row doesn't exist.
	    DatabaseSection.UpdateSectionThreadOrdering(session, sectionID, threadID);
    }
    public static Guid? GetCommentOwner(ISession session, Guid commentID)
    {
	    PreparedStatement selectStatement = session.Prepare($"SELECT creator_account_id FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_COMMENT}\" where comment_id=?");
	    var result = session.Execute(selectStatement.Bind(commentID));
	    var item = result.FirstOrDefault();
	    if (item == null)
	    {
		    return null;
	    }
	    return item.GetValue<Guid>("creator_account_id");
    }
    public static void UpdateComment(ISession session, Guid accountID, Guid commentID, string content)
    {
	    // Todo: Owner shouldn't be checked here unless the function name made that clear
	    if (accountID != GetCommentOwner(session, commentID))
	    {
		    return;
	    }
	    PreparedStatement updateStatement = session.Prepare($"UPDATE \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_COMMENT}\" set content=? where comment_id=?");
	    session.Execute(updateStatement.Bind(content, commentID));
    }
    public static void DeleteComment(ISession session, Guid accountID, Guid commentID)
    {
	    // Todo: Owner shouldn't be checked here unless the function name made that clear
	    if (accountID != GetCommentOwner(session, commentID))
	    {
		    return;
	    }
	    PreparedStatement deleteStatement = session.Prepare($"DELETE FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_COMMENT}\" where comment_id=?");
	    session.Execute(deleteStatement.Bind(commentID));
    }
    public record class DatabaseCommentData(Guid commentID, Guid creatorAccountID, String text, TimeUuid createdTime);
    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in a section
    public static List<DatabaseCommentData> GetThreadComments(ISession session, Guid threadID)
    {
	    List<DatabaseCommentData> commentDatas = new List<DatabaseCommentData>();
	    var commentSelectStatement = session.Prepare(
		    $"SELECT comment_id, content, creator_account_id, created_time FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_COMMENT}\" where thread_id=?");
	    var commentResult = session.Execute(commentSelectStatement.Bind(threadID));
	    foreach (Row item in commentResult)
	    {
		    commentDatas.Add(new DatabaseCommentData(
			    item.GetValue<Guid>("comment_id"),
			    item.GetValue<Guid>("creator_account_id"), 
			    item.GetValue<String>("content"),
			    item.GetValue<TimeUuid>("created_time")
		    ));
	    }
	    return commentDatas;
    }
    public static (Guid commentID, Guid creatorAccoundID) GetThreadFirstComment(ISession session, Guid threadID)
    {
	    var commentSelectStatement = session.Prepare(
		    $"SELECT comment_id, creator_account_id FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_COMMENT}\" where thread_id=? LIMIT 1");
	    var commentResult = session.Execute(commentSelectStatement.Bind(threadID));
	    var item = commentResult.FirstOrDefault();
	    return (item.GetValue<Guid>("comment_id"), item.GetValue<Guid>("creator_account_id"));
    }

    public static void DeleteCommentsForThread(ISession session, Guid threadID)
    {
	    PreparedStatement deleteCommentsStatement = session.Prepare($"DELETE FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_COMMENT}\" where thread_id=?");
	    session.Execute(deleteCommentsStatement.Bind(threadID));
    }
}