using DndBoard.SeleniumTests.Fixtures;
using Xunit;

namespace DndBoard.SeleniumTests
{
    [Collection(nameof(StartWasmServerCollection))]
    public sealed class WasmTests : BaseTests
    {
        protected override string SiteBaseAddress => "https://localhost:5001";
    }
}
