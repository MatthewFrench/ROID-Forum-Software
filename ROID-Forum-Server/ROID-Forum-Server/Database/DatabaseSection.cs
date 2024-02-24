using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public class DatabaseSection
{
    private static string TABLE_SECTION = "Section";
    private static string TABLE_SECTION_THREAD_ORDERING = "SectionThreadOrdering";
    private static string MATERIALIZED_VIEW_SECTION_THREAD_ORDERING = "SectionThreadOrderingView";
    private static string TABLE_ACTIVE_IN_SECTION = "ActiveInSection";
    
    public static void CreateTablesIfNotExist(ISession session)
    {
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_SECTION}"" (
                   section_id uuid,
                   name TEXT,
                   PRIMARY KEY (section_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_SECTION}"" (name)");
        // I may need to make multiple tables for threads.
        // One for ordering updated time with section, one for thread data with lookup by thread id
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_SECTION_THREAD_ORDERING}"" (
                   section_id uuid,
                   thread_id uuid,
                   updated_time timeuuid,
                   PRIMARY KEY (section_id, thread_id)
                )");
        session.Execute($@"
                CREATE MATERIALIZED VIEW IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{MATERIALIZED_VIEW_SECTION_THREAD_ORDERING}"" AS SELECT * FROM ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_SECTION_THREAD_ORDERING}"" WHERE updated_time IS NOT NULL and thread_id IS NOT NULL PRIMARY KEY (section_id, updated_time, thread_id) WITH CLUSTERING ORDER BY (updated_time ASC)");
        session.Execute($@"
                CREATE TABLE IF NOT EXISTS ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_SECTION}"" (
                   account_id uuid,
                   section_id uuid,
                   PRIMARY KEY (section_id, account_id)
                )");
        session.Execute($@"
                CREATE INDEX IF NOT EXISTS ON ""{Database.DEFAULT_KEYSPACE}"".""{TABLE_ACTIVE_IN_SECTION}"" (account_id)");
    }
    
    public static Guid LoadSectionID(ISession session, String sectionName)
    {
            // Create the section if it doesn't already exist
            PreparedStatement insertStatement = session.Prepare($"INSERT INTO \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_SECTION}\" (section_id, name) VALUES (uuid(), ?) IF NOT EXISTS");
            session.Execute(insertStatement.Bind(sectionName));
            PreparedStatement selectStatement = session.Prepare($"SELECT section_id FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_SECTION}\" where name=?");
            var result = session.Execute(selectStatement.Bind(sectionName));
            return result.First().GetValue<Guid>("section_id");
    }

    public static void UpdateSectionThreadOrdering(ISession session, Guid sectionID, Guid threadID)
    {
            // Update also acts as an insert if the row doesn't exist.
            PreparedStatement updateOrderStatement = session.Prepare($"UPDATE \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_SECTION_THREAD_ORDERING}\" SET updated_time=now() where section_id=? and thread_id=?");
            session.Execute(updateOrderStatement.Bind(sectionID, threadID));
    }

    public static void DeleteSectionThreadOrdering(ISession session, Guid sectionID, Guid threadID)
    {
            PreparedStatement deleteOrderStatement = session.Prepare($"DELETE FROM \"{Database.DEFAULT_KEYSPACE}\".\"{TABLE_SECTION_THREAD_ORDERING}\" where section_id=? and thread_id=?");
            session.Execute(deleteOrderStatement.Bind(sectionID, threadID));
    }

    public static List<(Guid sectionID, Guid threadID, TimeUuid updatedTime)> GetOrderedThreadIDsInSection(ISession session, Guid sectionID)
    {
            var selectStatement = session.Prepare(
                    $"SELECT section_id, thread_id, updated_time FROM \"{Database.DEFAULT_KEYSPACE}\".\"{MATERIALIZED_VIEW_SECTION_THREAD_ORDERING}\" where section_id=?");
            return session.Execute(selectStatement.Bind(sectionID)).GetRows().Select(row => (
                    row.GetValue<Guid>("section_id"), 
                    row.GetValue<Guid>("thread_id"), 
                    row.GetValue<TimeUuid>("update_time"))
            ).ToList();
    }
}