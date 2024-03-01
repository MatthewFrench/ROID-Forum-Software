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
                   title Text,
                   theme Text,
                   background Text,
                   created_time timeuuid,
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

    public static bool SectionIdExists(ISession session, Guid sectionId)
    {
        PreparedStatement selectStatement =
            session.Prepare(
                $"SELECT section_id FROM \"{Database.DefaultKeyspace}\".\"{TableSection}\" where section_id=?");
        var result = session.Execute(selectStatement.Bind(sectionId));
        if (result.FirstOrDefault() == null)
        {
            return false;
        }

        return true;
    }

    public static List<(Guid sectionId, string name, string title, string theme, string background, TimeUuid createdTime)> GetSections(
        ISession session)
    {
        var result =
            session.Execute(
                $"SELECT section_id, name, title, theme, background, created_time FROM \"{Database.DefaultKeyspace}\".\"{TableSection}\"");
        List<(Guid sectionId, string name, string title, string theme, string background, TimeUuid createdTime)> sections =
            new List<(Guid sectionId, string name, string title, string theme, string background, TimeUuid createdTime)>();
        foreach (var item in result)
        {
            sections.Add(
                (
                    sectionId: item.GetValue<Guid>("section_id"),
                    name: item.GetValue<string>("name"),
                    title: item.GetValue<string>("title"),
                    theme: item.GetValue<string>("theme"),
                    background: item.GetValue<string>("background"),
                    createdTime: item.GetValue<TimeUuid>("created_time")
                )
            );
        }

        return sections;
    }

    public static void CreateSectionIfNotExists(ISession session, String sectionName, String sectionTitle, String sectionTheme, String sectionBackground)
    {
        PreparedStatement selectStatement =
            session.Prepare($"SELECT section_id FROM \"{Database.DefaultKeyspace}\".\"{TableSection}\" where name=?");
        var result = session.Execute(selectStatement.Bind(sectionName));
        if (result.FirstOrDefault() == null)
        {
            PreparedStatement insertStatement =
                session.Prepare(
                    $"INSERT INTO \"{Database.DefaultKeyspace}\".\"{TableSection}\" (section_id, name, title, theme, background, created_time) VALUES (uuid(), ?, ?, ?, ?, ?)");
            session.Execute(insertStatement.Bind(sectionName, sectionTitle, sectionTheme, sectionBackground, TimeUuid.NewId()));
        }
    }

    public static TimeUuid UpdateSectionThreadOrdering(ISession session, Guid sectionId, Guid threadId)
    {
        var updatedTime = TimeUuid.NewId();
        // Update also acts as an insert if the row doesn't exist.
        PreparedStatement updateOrderStatement =
            session.Prepare(
                $"UPDATE \"{Database.DefaultKeyspace}\".\"{TableSectionThreadOrdering}\" SET updated_time=? where section_id=? and thread_id=?");
        session.Execute(updateOrderStatement.Bind(updatedTime, sectionId, threadId));
        return updatedTime;
    }

    public static void DeleteSectionThreadOrdering(ISession session, Guid sectionId, Guid threadId)
    {
        PreparedStatement deleteOrderStatement =
            session.Prepare(
                $"DELETE FROM \"{Database.DefaultKeyspace}\".\"{TableSectionThreadOrdering}\" where section_id=? and thread_id=?");
        session.Execute(deleteOrderStatement.Bind(sectionId, threadId));
    }

    public static List<(Guid sectionId, Guid threadId, TimeUuid updatedTime)> GetOrderedThreadIdsInSection(
        ISession session, Guid sectionId)
    {
        var selectStatement = session.Prepare(
            $"SELECT section_id, thread_id, updated_time FROM \"{Database.DefaultKeyspace}\".\"{MaterializedViewSectionThreadOrdering}\" where section_id=?");
        return session.Execute(selectStatement.Bind(sectionId)).GetRows().Select(row => (
            row.GetValue<Guid>("section_id"),
            row.GetValue<Guid>("thread_id"),
            row.GetValue<TimeUuid>("update_time"))
        ).ToList();
    }

    public static TimeUuid GetThreadUpdatedTime(ISession session, Guid sectionId, Guid threadId)
    {
        var selectStatement = session.Prepare(
            $"SELECT updated_time FROM \"{Database.DefaultKeyspace}\".\"{MaterializedViewSectionThreadOrdering}\" where section_id=? and thread_id=?");
        return session.Execute(selectStatement.Bind(sectionId, threadId)).Select(row =>
                row.GetValue<TimeUuid>("update_time"))
            .FirstOrDefault();
    }
}