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
    
    public static Guid CreateThread(ISession session, Guid accountId, Guid sectionId, string title)
    {
	    Guid threadId = Guid.NewGuid();
	    PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableThread}\" (section_id, thread_id, creator_account_id, title, created_time) VALUES (?, ?, ?, ?, now())");
	    session.Execute(insertStatement.Bind(sectionId, threadId, accountId, title));
	    DatabaseSection.UpdateSectionThreadOrdering(session, sectionId, threadId);
	    return threadId;
    }
    public static Guid? GetThreadOwner(ISession session, Guid threadId)
    {
	    PreparedStatement selectStatement = session.Prepare($"SELECT creator_account_id FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
	    var result = session.Execute(selectStatement.Bind(threadId));
	    var item = result.FirstOrDefault();
	    if (item == null)
	    {
		    return null;
	    }
	    return item.GetValue<Guid>("creator_account_id");
    }
    public static void UpdateThreadTitle(ISession session, Guid accountId, Guid sectionId, Guid threadId, string title)
    {
	    // Todo: Owner thread shouldn't be checked here unless the function name made that clear
	    if (accountId != GetThreadOwner(session, threadId))
	    {
		    return;
	    }
	    PreparedStatement updateStatement = session.Prepare($"UPDATE \"{Database.DefaultKeyspace}\".\"{TableThread}\" set title=? where thread_id=?");
	    session.Execute(updateStatement.Bind(title, threadId));
	    // Update also acts as an insert if the row doesn't exist.
	    DatabaseSection.UpdateSectionThreadOrdering(session, sectionId, threadId);
    }
    public static void DeleteThread(ISession session, Guid accountId, Guid sectionId, Guid threadId)
    {
	    // Todo: Owner thread shouldn't be checked here unless the function name made that clear
	    if (accountId != GetThreadOwner(session, threadId))
	    {
		    return;
	    }
	    // Delete ordering
	    DatabaseSection.DeleteSectionThreadOrdering(session, sectionId, threadId);
	    PreparedStatement deleteStatement = session.Prepare($"DELETE FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
	    session.Execute(deleteStatement.Bind(threadId));
	    DatabaseComment.DeleteCommentsForThread(session, threadId);
    }
    
    public record class DatabaseThreadData(Guid sectionId, Guid threadId, Guid creatorAccountId, string title, TimeUuid createdTime, TimeUuid updated_time);
    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in a section
    public static List<DatabaseThreadData> GetThreadsInSection(ISession session, Guid sectionId)
    {
	    var orderedThreadIds = DatabaseSection.GetOrderedThreadIdsInSection(session, sectionId);
	    List<DatabaseThreadData> results = new List<DatabaseThreadData>();
	    foreach (var (_, threadId, updatedTime) in orderedThreadIds)
	    {
		    var selectThreadStatement = session.Prepare(
			    $"SELECT creator_account_id, title, created_time FROM \"{Database.DefaultKeyspace}\".\"{TableThread}\" where thread_id=?");
		    var threadResult = session.Execute(selectThreadStatement.Bind(threadId));
		    var threadItem = threadResult.FirstOrDefault();
		    results.Add(new DatabaseThreadData(
			    sectionId,
			    threadId, 
			    threadItem.GetValue<Guid>("creator_account_id"),
			    threadItem.GetValue<String>("title"), 
			    threadItem.GetValue<TimeUuid>("created_time"),
			    updatedTime
		    ));
	    }

	    return results;
    }
}