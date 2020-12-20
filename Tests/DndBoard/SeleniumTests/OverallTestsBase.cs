using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Xunit;
using static DndBoard.SeleniumTests.Helpers.PixelHelper;

namespace DndBoard.SeleniumTests
{
    public abstract class OverallTestsBase : OverallTestsSetup
    {
        private const int AddPosLeftTopX = 10;
        private const int AddPosLeftTopY = 10;
        private const int IconSize = 100;

        private const int StartPosMiddleX = AddPosLeftTopX + IconSize / 2;
        private const int StartPosMiddleY = AddPosLeftTopY + IconSize / 2;
        private const int MoveByX = 250;
        private const int MoveByY = 250;


        [Fact]
        public void UploadIcon_MoveIcon_Works()
        {
            _driver.Navigate().GoToUrl($"{SiteBaseAddress}/board");
            ConnectToBoard("11");
            UploadIcon($"{_dataDir}/blueSquare.png");
            AddIconToMap(StartPosMiddleX, StartPosMiddleY);

            EnsureIconAddedToMap(StartPosMiddleX, StartPosMiddleY);
            MoveIcon(StartPosMiddleX, StartPosMiddleY, MoveByX, MoveByY);
            EnsureItemWasMoved(StartPosMiddleX, StartPosMiddleY, MoveByX, MoveByY);
        }


        private void ConnectToBoard(string boardId)
        {
            IWebElement input = _driver.FindElementById("boardId");
            input.SendKeys(boardId);
            IWebElement button = _driver.FindElementByXPath("//*[contains(text(), 'Connect')]");
            button.Click();
        }

        private void UploadIcon(string filePath)
        {
            IWebElement files = _driver.FindElementByXPath("//*[@type='file']");
            files.SendKeys(filePath);
            Thread.Sleep(500);
        }

        private void AddIconToMap(int clickToX, int clickToY)
        {
            IWebElement imgCanvas = _driver.FindElementByCssSelector("#ImagesDivCanvas > canvas");
            Actions actions = _canvasHelper.MoveToElemLeftTopCorner(imgCanvas);
            actions.MoveByOffset(clickToX, clickToY).Perform();
            actions.Click().Perform();
            Thread.Sleep(500);
        }

        private void MoveIcon(int nowX, int nowY, int moveX, int moveY)
        {
            IWebElement mapCanvas = _driver.FindElementByCssSelector("#MapCanvasDiv > canvas");

            Actions actions = _canvasHelper.MoveToElemLeftTopCorner(mapCanvas);

            actions.MoveByOffset(nowX, nowY).Perform();
            actions.ClickAndHold().Perform();
            Thread.Sleep(100);

            // Moving top left corner => clicking icon middle already moves it by size/2.
            int realMoveX = moveX - IconSize / 2;
            int realMoveY = moveY - IconSize / 2;
            actions.MoveByOffset(realMoveX, realMoveY).Perform();
            Thread.Sleep(100);

            actions.Release().Perform();
            Thread.Sleep(500);
        }

        private void EnsureIconAddedToMap(int nowX, int nowY)
        {
            EnsureItemUnderMouse(nowX, nowY);
            EnsureItemNotUnderMouse(nowX + IconSize, nowY + IconSize);
        }

        private void EnsureItemWasMoved(int wasX, int wasY, int movedByX, int movedByY)
        {
            EnsureItemNotUnderMouse(wasX, wasY);
            EnsureItemUnderMouse(wasX + movedByX, wasY + movedByY);
        }

        private void EnsureItemUnderMouse(int x, int y)
        {
            Dictionary<string, int> pixel = _canvasHelper.GetPixel(x, y);
            Assert.True(IsBluePixel(pixel));
        }

        private void EnsureItemNotUnderMouse(int x, int y)
        {
            Dictionary<string, int> pixel = _canvasHelper.GetPixel(x, y);
            Assert.False(IsBluePixel(pixel));
        }
    }
}
