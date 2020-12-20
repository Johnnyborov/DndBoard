using Xunit;

namespace DndBoard.SeleniumTests.Fixtures
{
    [CollectionDefinition(nameof(StartWasmServerCollection))]
    public sealed class StartWasmServerCollection : ICollectionFixture<StartWasmServerFixturecs> { }


    public sealed class StartWasmServerFixturecs : StartServerBaseFixture
    {
        protected override string ServerProjDir => "../../../../../../Src/DndBoard/WasmServer";
        protected override int StartTimeoutSec => 30;
        protected override string Host => "localhost";
        protected override int Port => 5001;
    }
}
