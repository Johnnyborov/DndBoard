using Xunit;

namespace DndBoard.SeleniumTests.Fixtures
{
    [CollectionDefinition(nameof(SetupClientsCollectionWasm))]
    public sealed class SetupClientsCollectionWasm : ICollectionFixture<SetupClientsFixtureWasm> { }


    public sealed class SetupClientsFixtureWasm : SetupClientsFixtureBase
    {
        public override string SiteBaseAddress => "https://localhost:5001";
    }
}
