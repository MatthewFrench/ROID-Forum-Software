using System;
using Cassandra;

namespace Cassandra_CSharp_Example
{
    class Program
    {
		//Connection port
		static int PORT = 32769;
        //Connection ip
		static string IP = "127.0.0.1";
        //Keyspace / Database name
		static string KEYSPACE = "TestKeyspace2"; //Database
        //Table name
		static string TABLE = "TestTable";
        //Username credential
		static string USERNAME = "cassandra";
        //Password credential
		static string PASSWORD = "cassandra";
        //Cluster connection
		static Cluster ClusterConnection;
        //Session connection, only use a single session connection
		static ISession Session;
        static void Main(string[] args)
        {
			ConnectToSession();
			CreateKeyspaceIfDoesntExist();
			CreateTableIfDoesntExist();
			InsertTestData();
			SelectAllTestRows();
        }
        /*
         * Connect to the cluster and create a session.
         */
		public static void ConnectToSession() {         
			Console.WriteLine($"Connecting to Cassandra cluster at {IP}:{PORT}!");
			ClusterConnection = Cluster.Builder()
                     .AddContactPoints(IP).WithPort(PORT)
                     .WithCredentials(USERNAME, PASSWORD)
			         .WithCompression(CompressionType.Snappy)
                     .Build();
			// Connect to a keyspace(database) on the cluster
			Session = ClusterConnection.Connect(); //Optional pass in keyspace
		}
        /*
         * Insert data into the table.
         */
		public static void InsertTestData() {
			Console.WriteLine($"Inserting row into {KEYSPACE}.{TABLE}");
			PreparedStatement insertStatement = Session.Prepare($"INSERT INTO \"{KEYSPACE}\".\"{TABLE}\" (id, text) VALUES (uuid(), ?)");
            Session.Execute(insertStatement.Bind("Blah blah"));
		}
        /*
         * Grab data from the table.
         */
		public static void SelectAllTestRows() {         
			Console.WriteLine($"Returning {KEYSPACE}.{TABLE} rows:");
			// Execute a query on a connection synchronously(async also supported)
			var result = Session.Execute($"SELECT * FROM \"{KEYSPACE}\".\"{TABLE}\"");
            foreach (var row in result)
            {
                Guid id = row.GetValue<Guid>("id");
                Console.WriteLine("    Row ID: " + id);
                var text = row.GetValue<string>("text"); 
                Console.WriteLine("        Text: " + text);
            }
		}
        /*
         * Create keyspace / database.
         */
		public static void CreateKeyspaceIfDoesntExist() {
			Session.Execute(@"CREATE KEYSPACE IF NOT EXISTS """ + KEYSPACE + 
			                "\" WITH replication = {'class': 'SimpleStrategy',  'replication_factor': '1' }; ");
			//Session.ChangeKeyspace
		}
        /*
         * Create table if it doesn't exist.
         */
		public static void CreateTableIfDoesntExist() {
			var createCql = $@"
                CREATE TABLE IF NOT EXISTS ""{KEYSPACE}"".""{TABLE}"" (
                   id uuid PRIMARY KEY,
                   text text
                )";
			Session.Execute(createCql);
		}
    }
}