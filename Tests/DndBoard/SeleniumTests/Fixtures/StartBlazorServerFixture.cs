using Xunit;

namespace DndBoard.SeleniumTests.Fixtures
{
    [CollectionDefinition(nameof(StartBlazorServerCollection))]
    public sealed class StartBlazorServerCollection : ICollectionFixture<StartBlazorServerFixture> { }


    public sealed class StartBlazorServerFixture : StartServerBaseFixture
    {
        protected override string ServerProjDir => "../../../../../../Src/DndBoard/BlazorServer";
        protected override int StartTimeoutSec => 30;
        protected override string Host => "localhost";
        protected override int Port => 5003;
    }
}
