using DndBoard.SeleniumTests.Fixtures;
using Xunit;

namespace DndBoard.SeleniumTests
{
    [Collection(nameof(StartServerCollectionWasm))]
    public sealed class OverallTestsWasm : OverallTestsBase, IClassFixture<SetupClientsFixtureWasm>
    {
        public OverallTestsWasm(SetupClientsFixtureWasm setup)
            : base(setup) { }
    }
}
