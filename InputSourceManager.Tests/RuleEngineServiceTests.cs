using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using InputSourceManager.Models;
using InputSourceManager.Services;
using System.Linq;

namespace InputSourceManager.Tests
{
    public class RuleEngineServiceTests
    {
        private readonly MockInputSourceManager _mockManager;
        private readonly MockBrowserDetectionService _mockBrowserService;
        private readonly RuleEngineService _ruleEngine;

        public RuleEngineServiceTests()
        {
            _mockManager = new MockInputSourceManager();
            _mockBrowserService = new MockBrowserDetectionService();
            _ruleEngine = new RuleEngineService(_mockManager, _mockBrowserService);
        }

        [Fact]
        public void AddRule_ValidRule_ShouldAddToRules()
        {
            // Arrange
            var rule = new InputSourceRule
            {
                Name = "Test Rule",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "English",
                Priority = 1
            };

            // Act
            _ruleEngine.AddRule(rule);

            // Assert
            var rules = _ruleEngine.GetAllRules();
            Assert.Contains(rules, r => r.Name == "Test Rule");
        }

        [Fact]
        public void AddRule_NullRule_ShouldNotAdd()
        {
            // Arrange
            InputSourceRule? rule = null;

            // Act
            _ruleEngine.AddRule(rule);

            // Assert
            var rules = _ruleEngine.GetAllRules();
            Assert.Empty(rules);
        }

        [Fact]
        public void AddRule_EmptyName_ShouldNotAdd()
        {
            // Arrange
            var rule = new InputSourceRule
            {
                Name = "",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "English",
                Priority = 1
            };

            // Act
            _ruleEngine.AddRule(rule);

            // Assert
            var rules = _ruleEngine.GetAllRules();
            Assert.Empty(rules);
        }

        [Fact]
        public void RemoveRule_ExistingRule_ShouldRemove()
        {
            // Arrange
            var rule = new InputSourceRule
            {
                Name = "Test Rule",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "English",
                Priority = 1
            };
            _ruleEngine.AddRule(rule);

            // Act
            _ruleEngine.RemoveRule(rule.Id);

            // Assert
            var rules = _ruleEngine.GetAllRules();
            Assert.DoesNotContain(rules, r => r.Id == rule.Id);
        }

        [Fact]
        public void RemoveRule_NonExistentRule_ShouldNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => _ruleEngine.RemoveRule("non-existent-id"));
            Assert.Null(exception);
        }

        [Fact]
        public void UpdateRule_ExistingRule_ShouldUpdate()
        {
            // Arrange
            var rule = new InputSourceRule
            {
                Name = "Test Rule",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "English",
                Priority = 1
            };
            _ruleEngine.AddRule(rule);

            var updatedRule = new InputSourceRule
            {
                Id = rule.Id,
                Name = "Updated Rule",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "Chinese",
                Priority = 2
            };

            // Act
            _ruleEngine.UpdateRule(updatedRule);

            // Assert
            var rules = _ruleEngine.GetAllRules();
            var foundRule = rules.FirstOrDefault(r => r.Id == rule.Id);
            Assert.NotNull(foundRule);
            Assert.Equal("Updated Rule", foundRule.Name);
            Assert.Equal("Chinese", foundRule.TargetInputSource);
            Assert.Equal(2, foundRule.Priority);
        }

        [Fact]
        public async Task ExecuteRulesAsync_NoMatchingRules_ShouldReturnFalse()
        {
            // Act
            var result = await _ruleEngine.ExecuteRulesAsync("unknownapp", "English");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExecuteRulesAsync_MatchingRule_ShouldReturnTrue()
        {
            // Arrange
            var rule = new InputSourceRule
            {
                Name = "Test Rule",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "Chinese",
                Priority = 1
            };
            _ruleEngine.AddRule(rule);

            // Act
            var result = await _ruleEngine.ExecuteRulesAsync("testapp", "English");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExecuteRulesAsync_AlreadyCorrectInputSource_ShouldReturnFalse()
        {
            // Arrange
            var rule = new InputSourceRule
            {
                Name = "Test Rule",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "English",
                Priority = 1
            };
            _ruleEngine.AddRule(rule);

            // Act
            var result = await _ruleEngine.ExecuteRulesAsync("testapp", "English");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetRulesForApplicationAsync_ExistingRules_ShouldReturnRules()
        {
            // Arrange
            var rule1 = new InputSourceRule
            {
                Name = "Rule 1",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "English",
                Priority = 1
            };
            var rule2 = new InputSourceRule
            {
                Name = "Rule 2",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "Chinese",
                Priority = 2
            };
            _ruleEngine.AddRule(rule1);
            _ruleEngine.AddRule(rule2);

            // Act
            var rules = await _ruleEngine.GetRulesForApplicationAsync("testapp");

            // Assert
            Assert.Equal(2, rules.Count);
            Assert.All(rules, r => Assert.Equal("testapp", r.Target));
        }

        [Fact]
        public async Task GetRulesByTypeAsync_ApplicationRules_ShouldReturnOnlyApplicationRules()
        {
            // Arrange
            var appRule = new InputSourceRule
            {
                Name = "App Rule",
                Type = RuleType.Application,
                Target = "testapp",
                TargetInputSource = "English",
                Priority = 1
            };
            var websiteRule = new InputSourceRule
            {
                Name = "Website Rule",
                Type = RuleType.Website,
                Target = "example.com",
                TargetInputSource = "Chinese",
                Priority = 1
            };
            _ruleEngine.AddRule(appRule);
            _ruleEngine.AddRule(websiteRule);

            // Act
            var appRules = await _ruleEngine.GetRulesByTypeAsync(RuleType.Application);
            var websiteRules = await _ruleEngine.GetRulesByTypeAsync(RuleType.Website);

            // Assert
            Assert.Single(appRules);
            Assert.Single(websiteRules);
            Assert.Equal(RuleType.Application, appRules[0].Type);
            Assert.Equal(RuleType.Website, websiteRules[0].Type);
        }

        private class MockInputSourceManager : InputSourceManagerBase
        {
            public override Task<string> GetCurrentApplicationAsync()
            {
                return Task.FromResult("testapp");
            }

            public override Task<string> GetCurrentInputSourceAsync()
            {
                return Task.FromResult("English");
            }

            public override Task<string[]> GetAvailableInputSourcesAsync()
            {
                return Task.FromResult(new[] { "English", "Chinese" });
            }

            public override Task<bool> SwitchToInputSourceAsync(string languageName)
            {
                return Task.FromResult(true);
            }

            public override Task<bool> SwitchToInputSourceByHotkeyAsync()
            {
                return Task.FromResult(true);
            }
        }

        private class MockBrowserDetectionService : BrowserDetectionService
        {
            public override Task<string> GetCurrentWebsiteAsync()
            {
                return Task.FromResult("example.com");
            }

            public override Task<bool> IsWebsiteActiveAsync()
            {
                return Task.FromResult(true);
            }
        }
    }
}
