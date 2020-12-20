using System;
using System.IO;
using DndBoard.SeleniumTests.Helpers;
using OpenQA.Selenium.Chrome;

namespace DndBoard.SeleniumTests
{
    public abstract class OverallTestsSetup : IDisposable
    {
        protected readonly string _dataDir;
        protected readonly ChromeDriver _driver;
        protected readonly CanvasHelper _canvasHelper;

        protected abstract string SiteBaseAddress { get; }


        public OverallTestsSetup()
        {
            string currentDir = Directory.GetCurrentDirectory();
            _dataDir = Path.GetFullPath(Path.Combine(currentDir, "../../../../../TestData"));

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("ignore-certificate-errors");
            _driver = new ChromeDriver(".", options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _canvasHelper = new CanvasHelper(_driver);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}
