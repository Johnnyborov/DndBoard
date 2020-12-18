using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Xunit;

namespace DndBoard.SeleniumTests
{
    public class UnitTest1 : IDisposable
    {
        private const string DotnetCmdName = "dotnet";
        private const string ServerProjDir = "../../../../../DndBoard/Server";
        private readonly Process _process;
        private readonly string _dataDir;

        public UnitTest1()
        {
            string currentDir = Directory.GetCurrentDirectory();
            _dataDir = Path.GetFullPath(Path.Combine(currentDir, "../../../../Data"));

            Process buildProcess = new Process();
            buildProcess.StartInfo.WorkingDirectory = ServerProjDir;
            buildProcess.StartInfo.FileName = DotnetCmdName;
            buildProcess.StartInfo.UseShellExecute = false;
            buildProcess.StartInfo.Arguments = "build .";
            buildProcess.Start();
            buildProcess.Kill();

            _process = new Process();
            _process.StartInfo.WorkingDirectory = ServerProjDir;
            _process.StartInfo.FileName = DotnetCmdName;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.Arguments = "run .";
            _process.Start();
        }

        public void Dispose()
        {
            _process.Kill();
        }

        [Fact]
        public void Test1()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            using ChromeDriver driver = new ChromeDriver(".", options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);


            driver.Navigate().GoToUrl("https://localhost:5001/board");

            IWebElement input = driver.FindElementById("boardId");
            input.SendKeys("11");
            IWebElement button = driver.FindElementByXPath("//*[contains(text(), 'Connect')]");
            button.Click();

            IWebElement files = driver.FindElementByXPath("//*[@type='file']");
            files.SendKeys($"{_dataDir}/blueSquare.png");
            Thread.Sleep(500);

            IWebElement imgCanvas = driver.FindElementByCssSelector("#ImagesDivCanvas > canvas");
            Actions actions = MoveToElemCorner(driver, imgCanvas);
            actions.MoveByOffset(50, 50).Perform();
            actions.Click().Perform();
            Thread.Sleep(500);

            IWebElement mapCanvas = driver.FindElementByCssSelector("#MapCanvasDiv > canvas");
            Dictionary<string, int> pixel1 = GetPixel(driver, 50, 50);
            Dictionary<string, int> pixel2 = GetPixel(driver, 250, 250);

            Assert.True(IsBluePixel(pixel1));
            Assert.False(IsBluePixel(pixel2));

            Draw(driver, mapCanvas);
            Thread.Sleep(500);

            Dictionary<string, int> pixel3 = GetPixel(driver, 50, 50);
            Dictionary<string, int> pixel4 = GetPixel(driver, 350, 350);

            Assert.False(IsBluePixel(pixel3));
            Assert.True(IsBluePixel(pixel4));
        }

        private static void Draw(ChromeDriver driver, IWebElement mapCanvas)
        {
            Actions actions = MoveToElemCorner(driver, mapCanvas);
            actions.MoveByOffset(50, 50).Perform();
            actions.ClickAndHold().Perform();
            Thread.Sleep(100);
            actions.MoveByOffset(250, 250).Perform();
            Thread.Sleep(100);
            actions.Release().Perform();
        }

        private static Actions MoveToElemCorner(ChromeDriver driver, IWebElement mapCanvas)
        {
            driver.ExecuteScript("window.scrollTo(0, arguments[0].getBoundingClientRect().top)", mapCanvas);
            int offsetY = (int)(dynamic)driver.ExecuteScript("return arguments[0].getBoundingClientRect().top;", mapCanvas);
            var actions = new Actions(driver);
            actions.MoveToElement(mapCanvas, 1, 1 + offsetY).Perform();
            return actions;
        }

        private static Dictionary<string, int> GetPixel(ChromeDriver driver, int x, int y)
        {
            string script = @"
var mapCanvas = document.getElementById('MapCanvasDiv').getElementsByTagName('canvas')[0];
var ctx = mapCanvas.getContext('2d');
var pixelData = ctx.getImageData(arguments[0], arguments[1], 1, 1).data;
return JSON.stringify(pixelData);
";
            string pixelData = (string)driver.ExecuteScript(script, x, y);
            var pixel = JsonSerializer.Deserialize<Dictionary<string, int>>(pixelData);
            return pixel;
        }

        private static bool IsBluePixel(Dictionary<string, int> pixel)
        {
            int r = pixel["0"];
            int g = pixel["1"];
            int b = pixel["2"];
            bool isBlue = true
                && r > 30 && r < 100
                && g > 30 && g < 100
                && b > 170 && b < 230
                ;
            return isBlue;
        }
    }
}
