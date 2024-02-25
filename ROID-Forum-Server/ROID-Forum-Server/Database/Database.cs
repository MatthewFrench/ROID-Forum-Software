using System;
using Cassandra;

namespace ROIDForumServer;

public class Database
{
	public const string DefaultKeyspace = "forum";
	
    private Cluster _clusterConnection;
    private ISession _session;
    public Database() {
        ConnectToSession();
    }
    
    /*
     * Connect to the cluster and create a session.
     */
    private void ConnectToSession() {
        // Todo: These should be moved to a configuration or environment variables
        string ip = "0.0.0.0";
        int port = 9042;
        string username = "cassandra";
        string password = "cassandra";
        Console.WriteLine($"Connecting to Cassandra cluster at {ip}:{port}!");
        _clusterConnection = Cluster.Builder()
            .AddContactPoints(ip).WithPort(port)
            .WithCredentials(username, password)
            .WithCompression(CompressionType.Snappy)
            .Build();
        _session = _clusterConnection.Connect();
        // Set up default keyspace
        _session.Execute(@"CREATE KEYSPACE IF NOT EXISTS """ + DefaultKeyspace + 
                        "\" WITH replication = {'class': 'SimpleStrategy',  'replication_factor': '1' }; ");
        DatabaseAccount.CreateTablesIfNotExist(_session);
        DatabaseChat.CreateTablesIfNotExist(_session);
        DatabaseSection.CreateTablesIfNotExist(_session);
        DatabaseThread.CreateTablesIfNotExist(_session);
        DatabaseComment.CreateTablesIfNotExist(_session);
    }

    public ISession GetSession()
    {
        return _session;
    }
}