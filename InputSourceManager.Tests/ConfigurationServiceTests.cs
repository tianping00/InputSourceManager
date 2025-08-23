using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using InputSourceManager.Models;
using InputSourceManager.Services;
using System.Text.Json;

namespace InputSourceManager.Tests
{
    public class ConfigurationServiceTests : IDisposable
    {
        private readonly string _testConfigDir;
        private readonly string _testConfigPath;
        private readonly ConfigurationService _configService;

        public ConfigurationServiceTests()
        {
            _testConfigDir = Path.Combine(Path.GetTempPath(), "InputSourceManagerTests");
            _testConfigPath = Path.Combine(_testConfigDir, "config.json");
            
            if (Directory.Exists(_testConfigDir))
                Directory.Delete(_testConfigDir, true);
            
            Directory.CreateDirectory(_testConfigDir);
            
            // 使用测试目录创建配置服务
            _configService = new TestConfigurationService(_testConfigPath);
        }

        [Fact]
        public async Task SaveRulesAsync_ValidRules_ShouldSaveSuccessfully()
        {
            // Arrange
            var rules = new List<InputSourceRule>
            {
                new InputSourceRule
                {
                    Name = "Test Rule 1",
                    Type = RuleType.Application,
                    Target = "testapp1",
                    TargetInputSource = "English",
                    Priority = 1
                },
                new InputSourceRule
                {
                    Name = "Test Rule 2",
                    Type = RuleType.Website,
                    Target = "example.com",
                    TargetInputSource = "Chinese",
                    Priority = 2
                }
            };

            // Act
            var result = await _configService.SaveRulesAsync(rules);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(_testConfigPath));
        }

        [Fact]
        public async Task LoadRulesAsync_ExistingConfig_ShouldLoadRules()
        {
            // Arrange
            var originalRules = new List<InputSourceRule>
            {
                new InputSourceRule
                {
                    Name = "Test Rule",
                    Type = RuleType.Application,
                    Target = "testapp",
                    TargetInputSource = "English",
                    Priority = 1
                }
            };
            await _configService.SaveRulesAsync(originalRules);

            // Act
            var loadedRules = await _configService.LoadRulesAsync();

            // Assert
            Assert.NotNull(loadedRules);
            Assert.Single(loadedRules);
            Assert.Equal("Test Rule", loadedRules[0].Name);
            Assert.Equal("testapp", loadedRules[0].Target);
        }

        [Fact]
        public async Task LoadRulesAsync_NonExistentConfig_ShouldReturnEmptyList()
        {
            // Act
            var rules = await _configService.LoadRulesAsync();

            // Assert
            Assert.NotNull(rules);
            Assert.Empty(rules);
        }

        [Fact]
        public async Task ExportRulesAsync_ValidRules_ShouldExportSuccessfully()
        {
            // Arrange
            var rules = new List<InputSourceRule>
            {
                new InputSourceRule
                {
                    Name = "Export Rule",
                    Type = RuleType.Process,
                    Target = "testproc",
                    TargetInputSource = "Japanese",
                    Priority = 3
                }
            };
            var exportPath = Path.Combine(_testConfigDir, "export.json");

            // Act
            var result = await _configService.ExportRulesAsync(exportPath, rules);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(exportPath));
        }

        [Fact]
        public async Task ImportRulesAsync_ValidFile_ShouldImportRules()
        {
            // Arrange
            var originalRules = new List<InputSourceRule>
            {
                new InputSourceRule
                {
                    Name = "Import Rule",
                    Type = RuleType.Website,
                    Target = "import.com",
                    TargetInputSource = "Korean",
                    Priority = 1
                }
            };
            var importPath = Path.Combine(_testConfigDir, "import.json");
            await _configService.ExportRulesAsync(importPath, originalRules);

            // Act
            var importedRules = await _configService.ImportRulesAsync(importPath);

            // Assert
            Assert.NotNull(importedRules);
            Assert.Single(importedRules);
            Assert.Equal("Import Rule", importedRules[0].Name);
            Assert.Equal("import.com", importedRules[0].Target);
        }

        [Fact]
        public async Task ImportRulesAsync_NonExistentFile_ShouldReturnEmptyList()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testConfigDir, "nonexistent.json");

            // Act
            var rules = await _configService.ImportRulesAsync(nonExistentPath);

            // Assert
            Assert.NotNull(rules);
            Assert.Empty(rules);
        }

        [Fact]
        public async Task ResetConfigurationAsync_ExistingConfig_ShouldDeleteConfig()
        {
            // Arrange
            var rules = new List<InputSourceRule>
            {
                new InputSourceRule
                {
                    Name = "Reset Rule",
                    Type = RuleType.Application,
                    Target = "resetapp",
                    TargetInputSource = "English",
                    Priority = 1
                }
            };
            await _configService.SaveRulesAsync(rules);
            Assert.True(File.Exists(_testConfigPath));

            // Act
            var result = await _configService.ResetConfigurationAsync();

            // Assert
            Assert.True(result);
            Assert.False(File.Exists(_testConfigPath));
        }

        [Fact]
        public async Task ExportSettingsAsync_GenericType_ShouldExportSuccessfully()
        {
            // Arrange
            var settings = new TestSettings
            {
                AutoStart = true,
                CheckInterval = 5000,
                Language = "zh-CN"
            };
            var exportPath = Path.Combine(_testConfigDir, "settings.json");

            // Act
            var result = await _configService.ExportSettingsAsync(exportPath, settings);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(exportPath));
        }

        [Fact]
        public async Task ImportSettingsAsync_GenericType_ShouldImportSuccessfully()
        {
            // Arrange
            var originalSettings = new TestSettings
            {
                AutoStart = false,
                CheckInterval = 3000,
                Language = "en-US"
            };
            var importPath = Path.Combine(_testConfigDir, "settings.json");
            await _configService.ExportSettingsAsync(importPath, originalSettings);

            // Act
            var importedSettings = await _configService.ImportSettingsAsync<TestSettings>(importPath);

            // Assert
            Assert.NotNull(importedSettings);
            Assert.Equal(false, importedSettings.AutoStart);
            Assert.Equal(3000, importedSettings.CheckInterval);
            Assert.Equal("en-US", importedSettings.Language);
        }

        public void Dispose()
        {
            _configService?.Dispose();
            
            try
            {
                if (Directory.Exists(_testConfigDir))
                    Directory.Delete(_testConfigDir, true);
            }
            catch
            {
                // 忽略清理错误
            }
        }

        private class TestConfigurationService : ConfigurationService
        {
            private readonly string _configPath;

            public TestConfigurationService(string configPath)
            {
                _configPath = configPath;
                
                // 确保测试目录存在
                var configDir = Path.GetDirectoryName(_configPath);
                if (!string.IsNullOrEmpty(configDir) && !Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
            }

            protected override string GetConfigFilePath()
            {
                return _configPath;
            }

            // 重写SetupFileWatcher以避免文件监控问题
            protected override void SetupFileWatcher()
            {
                // 测试中不设置文件监控
            }

            // 重写SaveRulesAsync以使用测试路径
            public override async Task<bool> SaveRulesAsync(List<InputSourceRule> rules)
            {
                try
                {
                    var config = new ConfigurationData
                    {
                        Rules = rules,
                        LastUpdated = DateTime.Now,
                        Version = "1.0.0"
                    };

                    var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    await File.WriteAllTextAsync(_configPath, json);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            // 重写LoadRulesAsync以使用测试路径
            public override async Task<List<InputSourceRule>> LoadRulesAsync()
            {
                try
                {
                    if (!File.Exists(_configPath))
                        return new List<InputSourceRule>();

                    var json = await File.ReadAllTextAsync(_configPath);
                    var config = JsonSerializer.Deserialize<ConfigurationData>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    return config?.Rules ?? new List<InputSourceRule>();
                }
                catch
                {
                    return new List<InputSourceRule>();
                }
            }

            // 重写ResetConfigurationAsync以使用测试路径
            public override Task<bool> ResetConfigurationAsync()
            {
                try
                {
                    if (File.Exists(_configPath))
                        File.Delete(_configPath);
                        
                    return Task.FromResult(true);
                }
                catch
                {
                    return Task.FromResult(false);
                }
            }

            private class ConfigurationData
            {
                public List<InputSourceRule> Rules { get; set; } = new();
                public DateTime LastUpdated { get; set; }
                public string Version { get; set; } = string.Empty;
            }
        }

        private class TestSettings
        {
            public bool AutoStart { get; set; }
            public int CheckInterval { get; set; }
            public string Language { get; set; } = string.Empty;
        }
    }
}
