using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InputSourceManager.Models;

namespace InputSourceManager.Services
{
    public class RuleEngineService
    {
        private readonly List<InputSourceRule> _rules = new();
        private readonly InputSourceManagerBase _inputSourceManager;
        private readonly BrowserDetectionService _browserService;
        private readonly Dictionary<string, InputSourceRule> _ruleCache = new();
        private readonly object _lockObject = new object();

        public RuleEngineService(InputSourceManagerBase inputSourceManager, BrowserDetectionService browserService)
        {
            _inputSourceManager = inputSourceManager;
            _browserService = browserService;
        }

        public async Task<bool> ExecuteRulesAsync(string currentApp, string currentInputSource)
        {
            try
            {
                // 获取匹配的规则
                var matchingRules = await GetMatchingRulesAsync(currentApp);
                
                if (!matchingRules.Any())
                    return false;

                // 按优先级排序，选择最高优先级的规则
                var bestRule = matchingRules.OrderByDescending(r => r.Priority).First();
                
                // 如果当前输入法已经是目标输入法，不需要切换
                if (string.Equals(currentInputSource, bestRule.TargetInputSource, StringComparison.OrdinalIgnoreCase))
                    return false;

                // 执行切换
                var success = await _inputSourceManager.SwitchToInputSourceAsync(bestRule.TargetInputSource);
                
                if (success)
                {
                    // 更新规则使用统计
                    lock (_lockObject)
                    {
                        bestRule.LastUsed = DateTime.Now;
                        bestRule.UsageCount++;
                    }
                }

                return success;
            }
            catch
            {
                return false;
            }
        }

        private async Task<List<InputSourceRule>> GetMatchingRulesAsync(string currentApp)
        {
            var matchingRules = new List<InputSourceRule>();

            // 应用程序规则 - 精确匹配
            var appRules = _rules.Where(r => r.Type == RuleType.Application && 
                                           r.IsEnabled && 
                                           string.Equals(r.Target, currentApp, StringComparison.OrdinalIgnoreCase));
            matchingRules.AddRange(appRules);

            // 进程规则 - 包含匹配
            var processRules = _rules.Where(r => r.Type == RuleType.Process && 
                                               r.IsEnabled && 
                                               currentApp.Contains(r.Target, StringComparison.OrdinalIgnoreCase));
            matchingRules.AddRange(processRules);

            // 网站规则（如果当前有浏览器活动）
            if (await _browserService.IsWebsiteActiveAsync())
            {
                var currentWebsite = await _browserService.GetCurrentWebsiteAsync();
                if (!string.IsNullOrEmpty(currentWebsite))
                {
                    var websiteRules = _rules.Where(r => r.Type == RuleType.Website && 
                                                       r.IsEnabled &&
                                                       IsWebsiteMatch(currentWebsite, r.Target));
                    matchingRules.AddRange(websiteRules);
                }
            }

            return matchingRules;
        }

        private bool IsWebsiteMatch(string currentWebsite, string ruleTarget)
        {
            if (string.IsNullOrEmpty(currentWebsite) || string.IsNullOrEmpty(ruleTarget))
                return false;

            // 支持多种匹配模式
            if (ruleTarget.StartsWith("*."))
            {
                // 通配符匹配：*.example.com
                var domain = ruleTarget.Substring(2);
                return currentWebsite.EndsWith(domain, StringComparison.OrdinalIgnoreCase);
            }
            else if (ruleTarget.StartsWith("*"))
            {
                // 通配符匹配：*example*
                var pattern = ruleTarget.Substring(1);
                return currentWebsite.Contains(pattern, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // 精确匹配
                return string.Equals(currentWebsite, ruleTarget, StringComparison.OrdinalIgnoreCase);
            }
        }

        public void AddRule(InputSourceRule rule)
        {
            if (rule == null || string.IsNullOrEmpty(rule.Name))
                return;

            lock (_lockObject)
            {
                _rules.Add(rule);
                _ruleCache.Clear(); // 清除缓存
            }
        }

        public void RemoveRule(string ruleId)
        {
            if (string.IsNullOrEmpty(ruleId))
                return;

            lock (_lockObject)
            {
                var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
                if (rule != null)
                {
                    _rules.Remove(rule);
                    _ruleCache.Clear(); // 清除缓存
                }
            }
        }

        public void UpdateRule(InputSourceRule updatedRule)
        {
            if (updatedRule == null || string.IsNullOrEmpty(updatedRule.Id))
                return;

            lock (_lockObject)
            {
                var existingRule = _rules.FirstOrDefault(r => r.Id == updatedRule.Id);
                if (existingRule != null)
                {
                    var index = _rules.IndexOf(existingRule);
                    _rules[index] = updatedRule;
                    _ruleCache.Clear(); // 清除缓存
                }
            }
        }

        public List<InputSourceRule> GetAllRules()
        {
            lock (_lockObject)
            {
                return _rules.ToList();
            }
        }

        public void ClearRules()
        {
            lock (_lockObject)
            {
                _rules.Clear();
                _ruleCache.Clear();
            }
        }

        public async Task<List<InputSourceRule>> GetRulesForApplicationAsync(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                return new List<InputSourceRule>();

            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    return _rules.Where(r => r.Type == RuleType.Application && 
                                           string.Equals(r.Target, appName, StringComparison.OrdinalIgnoreCase))
                               .ToList();
                }
            });
        }

        public async Task<List<InputSourceRule>> GetRulesForWebsiteAsync()
        {
            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    return _rules.Where(r => r.Type == RuleType.Website).ToList();
                }
            });
        }

        public async Task<List<InputSourceRule>> GetRulesByTypeAsync(RuleType type)
        {
            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    return _rules.Where(r => r.Type == type).ToList();
                }
            });
        }

        public async Task<InputSourceRule?> GetRuleByIdAsync(string ruleId)
        {
            if (string.IsNullOrEmpty(ruleId))
                return null;

            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    return _rules.FirstOrDefault(r => r.Id == ruleId);
                }
            });
        }

        /// <summary>
        /// 执行网站规则（从URL接收器调用）
        /// </summary>
        public async Task<bool> ExecuteWebsiteRulesAsync(string domain, string currentInputSource)
        {
            try
            {
                // 获取匹配的网站规则
                List<InputSourceRule> websiteRules;
                lock (_lockObject)
                {
                    websiteRules = _rules.Where(r => r.Type == RuleType.Website && 
                                                   r.IsEnabled &&
                                                   IsWebsiteMatch(domain, r.Target))
                                       .ToList();
                }
                
                if (!websiteRules.Any())
                    return false;

                // 按优先级排序，选择最高优先级的规则
                var bestRule = websiteRules.OrderByDescending(r => r.Priority).First();
                
                // 如果当前输入法已经是目标输入法，不需要切换
                if (string.Equals(currentInputSource, bestRule.TargetInputSource, StringComparison.OrdinalIgnoreCase))
                    return false;

                // 执行切换
                var success = await _inputSourceManager.SwitchToInputSourceAsync(bestRule.TargetInputSource);
                
                if (success)
                {
                    // 更新规则使用统计
                    lock (_lockObject)
                    {
                        bestRule.LastUsed = DateTime.Now;
                        bestRule.UsageCount++;
                    }
                }

                return success;
            }
            catch
            {
                return false;
            }
        }
    }
}
