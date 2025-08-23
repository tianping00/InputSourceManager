using System;
using System.Threading.Tasks;
using Xunit;
using InputSourceManager;

namespace InputSourceManager.Tests
{
    public class InputSourceManagerTests
    {
        [Fact]
        public void WindowsInputSourceManager_ShouldInheritFromInputSourceManager()
        {
            // Arrange & Act
            var manager = new WindowsInputSourceManager();

            // Assert
            Assert.IsAssignableFrom<InputSourceManagerBase>(manager);
        }

        [Fact]
        public void LinuxInputSourceManager_ShouldInheritFromInputSourceManager()
        {
            // Arrange & Act
            var manager = new LinuxInputSourceManager();

            // Assert
            Assert.IsAssignableFrom<InputSourceManagerBase>(manager);
        }

        [Fact]
        public async Task WindowsInputSourceManager_GetCurrentApplicationAsync_ShouldReturnString()
        {
            // Arrange
            var manager = new WindowsInputSourceManager();

            // Act
            var result = await manager.GetCurrentApplicationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task WindowsInputSourceManager_GetCurrentInputSourceAsync_ShouldReturnString()
        {
            // Arrange
            var manager = new WindowsInputSourceManager();

            // Act
            var result = await manager.GetCurrentInputSourceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task WindowsInputSourceManager_GetAvailableInputSourcesAsync_ShouldReturnArray()
        {
            // Arrange
            var manager = new WindowsInputSourceManager();

            // Act
            var result = await manager.GetAvailableInputSourcesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string[]>(result);
        }

        [Fact]
        public async Task WindowsInputSourceManager_SwitchToInputSourceAsync_ShouldReturnBoolean()
        {
            // Arrange
            var manager = new WindowsInputSourceManager();

            // Act
            var result = await manager.SwitchToInputSourceAsync("English");

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task WindowsInputSourceManager_SwitchToInputSourceByHotkeyAsync_ShouldReturnBoolean()
        {
            // Arrange
            var manager = new WindowsInputSourceManager();

            // Act
            var result = await manager.SwitchToInputSourceByHotkeyAsync();

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_GetCurrentApplicationAsync_ShouldReturnString()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.GetCurrentApplicationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_GetCurrentInputSourceAsync_ShouldReturnString()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.GetCurrentInputSourceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_GetAvailableInputSourcesAsync_ShouldReturnArray()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.GetAvailableInputSourcesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string[]>(result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_SwitchToInputSourceAsync_ShouldReturnBoolean()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.SwitchToInputSourceAsync("English");

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_SwitchToInputSourceByHotkeyAsync_ShouldReturnBoolean()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.SwitchToInputSourceByHotkeyAsync();

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_GetAvailableInputSourcesAsync_ShouldContainExpectedLanguages()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.GetAvailableInputSourcesAsync();

            // Assert
            Assert.Contains("英语 (美国)", result);
            Assert.Contains("中文 (简体)", result);
            Assert.Contains("日语", result);
            Assert.Contains("韩语", result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_GetCurrentApplicationAsync_ShouldReturnExpectedValue()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.GetCurrentApplicationAsync();

            // Assert
            Assert.Equal("linux-app", result);
        }

        [Fact]
        public async Task LinuxInputSourceManager_GetCurrentInputSourceAsync_ShouldReturnExpectedValue()
        {
            // Arrange
            var manager = new LinuxInputSourceManager();

            // Act
            var result = await manager.GetCurrentInputSourceAsync();

            // Assert
            Assert.Equal("中文 (简体)", result);
        }
    }
}
