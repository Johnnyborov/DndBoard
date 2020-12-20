using DndBoard.SeleniumTests.Fixtures;
using Xunit;

namespace DndBoard.SeleniumTests
{
    [Collection(nameof(StartBlazorServerCollection))]
    public sealed class BlazorServerTests : BaseTests
    {
        protected override string SiteBaseAddress => "https://localhost:5003";
    }
}
