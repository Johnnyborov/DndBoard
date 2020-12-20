using DndBoard.SeleniumTests.Fixtures;
using Xunit;

namespace DndBoard.SeleniumTests
{
    [Collection(nameof(StartBlazorServerCollection))]
    public sealed class OverallTestsBlazorServer : OverallTestsBase
    {
        protected override string SiteBaseAddress => "https://localhost:5003";
    }
}
