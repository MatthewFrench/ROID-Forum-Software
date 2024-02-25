using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public static class DatabaseSection
{
    private const string TableSection = "Section";
    private const string TableSectionThreadOrdering = "SectionThreadOrdering";
    private const string MaterializedViewSectionThreadOrdering = "SectionThreadOrderingView";
    private const string TableActiveInSection = "ActiveInSection";
    
    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableSection}"" (
                   section_id uuid,
                   name TEXT,
                   PRIMARY KEY (section_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DefaultKeyspace}"".""{TableSection}"" (name)");
        // I may need to make multiple tables for threads.
        // One for ordering updated time with section, one for thread data with lookup by thread id
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableSectionThreadOrdering}"" (
                   section_id uuid,
                   thread_id uuid,
                   updated_time timeuuid,
                   PRIMARY KEY (section_id, thread_id)
                )");
        session.Execute($@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{MaterializedViewSectionThreadOrdering}"" AS SELECT * FROM ""{Database.DefaultKeyspace}"".""{TableSectionThreadOrdering}"" WHERE updated_time IS NOT NULL and thread_id IS NOT NULL PRIMARY KEY (section_id, updated_time, thread_id) WITH CLUSTERING ORDER BY (updated_time ASC)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DefaultKeyspace}"".""{TableActiveInSection}"" (
                   account_id uuid,
                   section_id uuid,
                   PRIMARY KEY (section_id, account_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DefaultKeyspace}"".""{TableActiveInSection}"" (account_id)");
    }
    
    public static Guid LoadSectionId(ISession session, String sectionName)
    {
            // Create the section if it doesn't already exist
            PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableSection}\" (section_id, name) VALUES (uuid(), ?) IF NOT EXISTS");
            session.Execute(insertStatement.Bind(sectionName));
            PreparedStatement selectStatement = session.Prepare($"SELECT section_id FROM \"{Database.DefaultKeyspace}\".\"{TableSection}\" where name=?");
            var result = session.Execute(selectStatement.Bind(sectionName));
            return result.First().GetValue<Guid>("section_id");
    }

    public static void UpdateSectionThreadOrdering(ISession session, Guid sectionId, Guid threadId)
    {
            // Update also acts as an insert if the row doesn't exist.
            PreparedStatement updateOrderStatement = session.Prepare($"UPDATE \"{Database.DefaultKeyspace}\".\"{TableSectionThreadOrdering}\" SET updated_time=now() where section_id=? and thread_id=?");
            session.Execute(updateOrderStatement.Bind(sectionId, threadId));
    }

    public static void DeleteSectionThreadOrdering(ISession session, Guid sectionId, Guid threadId)
    {
            PreparedStatement deleteOrderStatement = session.Prepare($"DELETE FROM \"{Database.DefaultKeyspace}\".\"{TableSectionThreadOrdering}\" where section_id=? and thread_id=?");
            session.Execute(deleteOrderStatement.Bind(sectionId, threadId));
    }

    public static List<(Guid sectionId, Guid threadId, TimeUuid updatedTime)> GetOrderedThreadIdsInSection(ISession session, Guid sectionId)
    {
            var selectStatement = session.Prepare(
                    $"SELECT section_id, thread_id, updated_time FROM \"{Database.DefaultKeyspace}\".\"{MaterializedViewSectionThreadOrdering}\" where section_id=?");
            return session.Execute(selectStatement.Bind(sectionId)).GetRows().Select(row => (
                    row.GetValue<Guid>("section_id"), 
                    row.GetValue<Guid>("thread_id"), 
                    row.GetValue<TimeUuid>("update_time"))
            ).ToList();
    }
}