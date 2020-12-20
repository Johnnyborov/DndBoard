using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DndBoard.SeleniumTests.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Xunit;
using static DndBoard.SeleniumTests.Helpers.PixelHelper;

namespace DndBoard.SeleniumTests
{
    [Collection(nameof(StartServerCollection))]
    public class UnitTest1 : IDisposable
    {
        private readonly string _dataDir;
        private readonly ChromeDriver _driver;
        private readonly CanvasHelper _canvasHelper;


        public UnitTest1()
        {
            string currentDir = Directory.GetCurrentDirectory();
            _dataDir = Path.GetFullPath(Path.Combine(currentDir, "../../../../Data"));

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            _driver = new ChromeDriver(".", options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _canvasHelper = new CanvasHelper(_driver);
        }

        public void Dispose()
        {
            _driver.Dispose();
        }


        [Fact]
        public void Test1()
        {
            _driver.Navigate().GoToUrl("https://localhost:5001/board");
            ConnectToBoard();
            UploadIcon();
            AddIconToMap();

            IWebElement mapCanvas = _driver.FindElementByCssSelector("#MapCanvasDiv > canvas");
            CheckIconAddedToMap();
            MoveIcon(mapCanvas);
            CheckItemWasMoved();
        }
     
        private void AddIconToMap()
        {
            IWebElement imgCanvas = _driver.FindElementByCssSelector("#ImagesDivCanvas > canvas");
            Actions actions = _canvasHelper.MoveToElemLeftTopCorner(imgCanvas);
            actions.MoveByOffset(50, 50).Perform();
            actions.Click().Perform();
            Thread.Sleep(500);
        }

        private void ConnectToBoard()
        {
            IWebElement input = _driver.FindElementById("boardId");
            input.SendKeys("11");
            IWebElement button = _driver.FindElementByXPath("//*[contains(text(), 'Connect')]");
            button.Click();
        }

        private void UploadIcon()
        {
            IWebElement files = _driver.FindElementByXPath("//*[@type='file']");
            files.SendKeys($"{_dataDir}/blueSquare.png");
            Thread.Sleep(500);
        }

        private void MoveIcon(IWebElement mapCanvas)
        {
            Actions actions = _canvasHelper.MoveToElemLeftTopCorner(mapCanvas);

            actions.MoveByOffset(50, 50).Perform();
            actions.ClickAndHold().Perform();

            Thread.Sleep(100);
            actions.MoveByOffset(250, 250).Perform();
            Thread.Sleep(100);

            actions.Release().Perform();
            Thread.Sleep(500);
        }

        private void CheckIconAddedToMap()
        {
            Dictionary<string, int> pixel1 = _canvasHelper.GetPixel(50, 50);
            Dictionary<string, int> pixel2 = _canvasHelper.GetPixel(250, 250);

            Assert.True(IsBluePixel(pixel1));
            Assert.False(IsBluePixel(pixel2));
        }

        private void CheckItemWasMoved()
        {
            Dictionary<string, int> pixel1 = _canvasHelper.GetPixel(50, 50);
            Dictionary<string, int> pixel2 = _canvasHelper.GetPixel(350, 350);

            Assert.False(IsBluePixel(pixel1));
            Assert.True(IsBluePixel(pixel2));
        }
    }
}
