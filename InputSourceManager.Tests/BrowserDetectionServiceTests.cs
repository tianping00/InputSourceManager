using System;
using System.Threading.Tasks;
using Xunit;
using InputSourceManager.Services;

namespace InputSourceManager.Tests
{
    public class BrowserDetectionServiceTests
    {
        private readonly BrowserDetectionService _browserService;

        public BrowserDetectionServiceTests()
        {
            _browserService = new BrowserDetectionService();
        }

        [Fact]
        public async Task GetActiveBrowsersAsync_ShouldReturnArray()
        {
            // Act
            var browsers = await _browserService.GetActiveBrowsersAsync();

            // Assert
            Assert.NotNull(browsers);
            Assert.IsType<string[]>(browsers);
        }

        [Fact]
        public async Task IsBrowserRunningAsync_ValidBrowserName_ShouldReturnBoolean()
        {
            // Act
            var isRunning = await _browserService.IsBrowserRunningAsync("chrome");

            // Assert
            Assert.IsType<bool>(isRunning);
        }

        [Fact]
        public async Task IsBrowserRunningAsync_EmptyBrowserName_ShouldReturnFalse()
        {
            // Act
            var isRunning = await _browserService.IsBrowserRunningAsync("");

            // Assert
            Assert.False(isRunning);
        }

        [Fact]
        public async Task IsBrowserRunningAsync_NullBrowserName_ShouldReturnFalse()
        {
            // Act
            var isRunning = await _browserService.IsBrowserRunningAsync(null);

            // Assert
            Assert.False(isRunning);
        }

        [Fact]
        public async Task GetBrowserStatusAsync_ShouldReturnDictionary()
        {
            // Act
            var status = await _browserService.GetBrowserStatusAsync();

            // Assert
            Assert.NotNull(status);
            Assert.IsType<System.Collections.Generic.Dictionary<string, bool>>(status);
        }

        [Fact]
        public async Task GetBrowserStatusAsync_ShouldContainExpectedBrowsers()
        {
            // Act
            var status = await _browserService.GetBrowserStatusAsync();

            // Assert
            Assert.Contains("chrome", status.Keys);
            Assert.Contains("firefox", status.Keys);
            Assert.Contains("msedge", status.Keys);
        }

        [Fact]
        public async Task GetBrowserStatusAsync_AllValuesShouldBeBoolean()
        {
            // Act
            var status = await _browserService.GetBrowserStatusAsync();

            // Assert
            foreach (var value in status.Values)
            {
                Assert.IsType<bool>(value);
            }
        }

        [Fact]
        public void ClearCache_ShouldNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => _browserService.ClearCache());
            Assert.Null(exception);
        }

        [Fact]
        public async Task IsWebsiteActiveAsync_ShouldReturnBoolean()
        {
            // Act
            var isActive = await _browserService.IsWebsiteActiveAsync();

            // Assert
            Assert.IsType<bool>(isActive);
        }

        [Fact]
        public async Task GetCurrentWebsiteAsync_ShouldReturnStringOrNull()
        {
            // Act
            var website = await _browserService.GetCurrentWebsiteAsync();

            // Assert
            // 可能返回null或字符串，取决于是否有浏览器运行
            if (website != null)
            {
                Assert.IsType<string>(website);
            }
        }
    }
}
