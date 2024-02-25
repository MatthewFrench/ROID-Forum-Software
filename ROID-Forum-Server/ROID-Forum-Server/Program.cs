using System;
using System.Threading;

namespace ROIDForumServer
{
    static class Server
    {
        private static void Main()
        {
			var server = new ServerController();
            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (_, eventArgs) => {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };
            exitEvent.WaitOne();
            server.Stop();
            Thread.Sleep(1000);
        }
    }
}