using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace DndBoard.SeleniumTests
{
    [CollectionDefinition(nameof(StartServerCollection))]
    public class StartServerCollection : ICollectionFixture<StartServerFixture> { }


    public class StartServerFixture : IDisposable
    {
        private const string DotnetCmdName = "dotnet";
        private const string ServerProjDir = "../../../../../DndBoard/Server";
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
            Thread.Sleep(12500);
        }

        public void StopServer()
        {
            _process.Kill();
        }
    }
}
