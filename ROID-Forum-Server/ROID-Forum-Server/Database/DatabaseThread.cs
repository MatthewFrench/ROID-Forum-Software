using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public class DatabaseThread
{
    private static string TABLE_THREAD = "Thread";
    private static string TABLE_ACTIVE_IN_THREAD = "ActiveInThread";
    
    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_THREAD}"" (
                   section_id uuid,
                   thread_id uuid,
                   creator_account_id uuid,
                   title TEXT,
                   created_time timeuuid,
                   PRIMARY KEY (thread_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_THREAD}"" (creator_account_id)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_THREAD}"" (
                   account_id uuid,
                   thread_id uuid,
                   PRIMARY KEY (thread_id, account_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_THREAD}"" (account_id)");
    }
    
    public Guid CreateThread(ISession session, Guid accountID, Guid sectionID, string title)
    {
	    Guid threadID = Guid.NewGuid();
	    PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_THREAD}\" (section_id, thread_id, creator_account_id, title, created_time) VALUES (?, ?, ?, ?, now())");
	    session.Execute(insertStatement.Bind(sectionID, threadID, accountID, title));
	    DatabaseSection.UpdateSectionThreadOrdering(session, sectionID, threadID);
	    return threadID;
    }
    public Guid? GetThreadOwner(ISession session, Guid threadID)
    {
	    PreparedStatement selectStatement = session.Prepare($"SELECT creator_account_id FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_THREAD}\" where thread_id=?");
	    var result = session.Execute(selectStatement.Bind(threadID));
	    var item = result.FirstOrDefault();
	    if (item == null)
	    {
		    return null;
	    }
	    return item.GetValue<Guid>("creator_account_id");
    }
    public void UpdateThreadTitle(ISession session, Guid accountID, Guid sectionID, Guid threadID, string title)
    {
	    // Todo: Owner thread shouldn't be checked here unless the function name made that clear
	    if (accountID != GetThreadOwner(session, threadID))
	    {
		    return;
	    }
	    PreparedStatement updateStatement = session.Prepare($"UPDATE \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_THREAD}\" set title=? where thread_id=?");
	    session.Execute(updateStatement.Bind(title, threadID));
	    // Update also acts as an insert if the row doesn't exist.
	    DatabaseSection.UpdateSectionThreadOrdering(session, sectionID, threadID);
    }
    public void DeleteThread(ISession session, Guid accountID, Guid sectionID, Guid threadID)
    {
	    // Todo: Owner thread shouldn't be checked here unless the function name made that clear
	    if (accountID != GetThreadOwner(session, threadID))
	    {
		    return;
	    }
	    // Delete ordering
	    DatabaseSection.DeleteSectionThreadOrdering(session, sectionID, threadID);
	    PreparedStatement deleteStatement = session.Prepare($"DELETE FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_THREAD}\" where thread_id=?");
	    session.Execute(deleteStatement.Bind(threadID));
	    DatabaseComment.DeleteCommentsForThread(session, threadID);
    }
    
    public record class DatabaseThreadData(Guid sectionID, Guid threadID, Guid creatorAccountID, string title, TimeUuid createdTime, TimeUuid updated_time);
    // Todo: Add pagination and dynamic loading/lookback as the user scrolls down in a section
    public List<DatabaseThreadData> GetThreadsInSection(ISession session, Guid sectionID)
    {
	    var orderedThreadIDs = DatabaseSection.GetOrderedThreadIDsInSection(session, sectionID);
	    List<DatabaseThreadData> results = new List<DatabaseThreadData>();
	    foreach (var (_, threadID, updatedTime) in orderedThreadIDs)
	    {
		    var selectThreadStatement = session.Prepare(
			    $"SELECT creator_account_id, title, created_time FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_THREAD}\" where thread_id=?");
		    var threadResult = session.Execute(selectThreadStatement.Bind(threadID));
		    var threadItem = threadResult.FirstOrDefault();
		    results.Add(new DatabaseThreadData(
			    sectionID,
			    threadID, 
			    threadItem.GetValue<Guid>("creator_account_id"),
			    threadItem.GetValue<String>("title"), 
			    threadItem.GetValue<TimeUuid>("created_time"),
			    updatedTime
		    ));
	    }

	    return results;
    }
}