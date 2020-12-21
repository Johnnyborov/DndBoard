using DndBoard.SeleniumTests.Fixtures;
using Xunit;

namespace DndBoard.SeleniumTests
{
    [Collection(nameof(StartServerCollectionBlazorServer))]
    public sealed class OverallTestsBlazorServer : OverallTestsBase, IClassFixture<SetupClientsFixtureBlazorServer>
    {
        public OverallTestsBlazorServer(SetupClientsFixtureBlazorServer setup)
            : base(setup) { }
    }
}
