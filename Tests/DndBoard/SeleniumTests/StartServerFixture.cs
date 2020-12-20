using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace DndBoard.SeleniumTests
{
    [CollectionDefinition(nameof(StartServerCollection))]
    public class StartServerCollection : ICollectionFixture<StartServerFixture> { }


    public class StartServerFixture : IDisposable
    {
        private const string DotnetCmdName = "dotnet";
        private const string ServerProjDir = "../../../../../../Src/DndBoard/WasmServer";
        private Process _process;


        public StartServerFixture()
        {
            BuildServer();
            StartServer();
        }

        public void Dispose()
        {
            StopServer();
        }


        private static void BuildServer()
        {
            Process buildProcess = new Process();
            buildProcess.StartInfo.WorkingDirectory = ServerProjDir;
            buildProcess.StartInfo.FileName = DotnetCmdName;
            buildProcess.StartInfo.UseShellExecute = false;
            buildProcess.StartInfo.Arguments = "build . -c Release";
            buildProcess.Start();
            buildProcess.Kill();
        }

        private void StartServer()
        {
            _process = new Process();
            _process.StartInfo.WorkingDirectory = ServerProjDir;
            _process.StartInfo.FileName = DotnetCmdName;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.Arguments = "run . -c Release";
            _process.Start();
            WaitUntilServerStarted();
        }

        private void WaitUntilServerStarted()
        {
            int timeoutSec = 30000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!ServerStarted())
            {
                if (sw.ElapsedMilliseconds > timeoutSec * 1000)
                    throw new TimeoutException($"Server failed to start in {timeoutSec} seconds.");
                Thread.Sleep(1000);
            }
        }

        private bool ServerStarted() =>
            IsPortOpen("localhost", 5001, TimeSpan.FromMilliseconds(500));

        private bool IsPortOpen(string host, int port, TimeSpan timeout)
        {
            try
            {
                using TcpClient client = new TcpClient();
                IAsyncResult result = client.BeginConnect(host, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(timeout);
                client.EndConnect(result);
                return success;
            }
            catch
            {
                return false;
            }
        }

        private void StopServer()
        {
            _process.Kill();
        }
    }
}
