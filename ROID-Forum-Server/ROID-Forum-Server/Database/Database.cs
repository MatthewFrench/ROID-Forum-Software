using System;
using System.Collections.Generic;
using System.Linq;
using Cassandra;

namespace ROIDForumServer;

public class Database
{
	public static string DEFAULT_KEYSPACE = "forum";
	
    private Cluster clusterConnection;
    private ISession session;
    public Database() {
        ConnectToSession();
    }
    
    /*
     * Connect to the cluster and create a session.
     */
    private void ConnectToSession() {
        // Todo: These should be moved to a configuration or environment variables
        string IP = "0.0.0.0";
        int PORT = 9042;
        string USERNAME = "cassandra";
        string PASSWORD = "cassandra";
        Console.WriteLine($"Connecting to Cassandra cluster at {IP}:{PORT}!");
        clusterConnection = Cluster.Builder()
            .AddContactPoints(IP).WithPort(PORT)
            .WithCredentials(USERNAME, PASSWORD)
            .WithCompression(CompressionType.Snappy)
            .Build();
        session = clusterConnection.Connect();
        // Set up default keyspace
        session.Execute(@"CREATE KEYSPACE IF NOT EXISTS """ + DEFAULT_KEYSPACE + 
                        "\" WITH replication = {'class': 'SimpleStrategy',  'replication_factor': '1' }; ");
        // Set up tables if they don't exist
        DatabaseAccount.CreateTablesIfNotExist(session);
        DatabaseChat.CreateTablesIfNotExist(session);
        DatabaseSection.CreateTablesIfNotExist(session);
        DatabaseThread.CreateTablesIfNotExist(session);
        DatabaseComment.CreateTablesIfNotExist(session);
    }

    public ISession GetSession()
    {
        return session;
    }
}