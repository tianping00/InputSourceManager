using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using InputSourceManager.Models;
using InputSourceManager.Services;

namespace InputSourceManager.Windows.Views
{
    public partial class RulesPage : UserControl
    {
        private ConfigurationService? _configService;
        private RuleEngineService? _ruleEngine;
        private InputSourceManager.InputSourceManagerBase? _inputSourceManager;
        private readonly List<InputSourceRule> _allRules = new();

        public RulesPage()
        {
            InitializeComponent();
        }

        public void Initialize(ConfigurationService configService, RuleEngineService ruleEngine, InputSourceManager.InputSourceManagerBase inputSourceManager)
        {
            _configService = configService;
            _ruleEngine = ruleEngine;
            _inputSourceManager = inputSourceManager;

            // 加载已保存的规则
            LoadRulesFromEngine();

            // 注册配置变化监听
            if (_configService != null)
            {
                _configService.RegisterConfigChangedCallback(OnRulesChanged);
            }
        }

        private void LoadRulesFromEngine()
        {
            if (_ruleEngine == null) return;

            _allRules.Clear();
            _allRules.AddRange(_ruleEngine.GetAllRules());
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            // 应用筛选
            var filteredRules = _allRules.AsEnumerable();
            if (CmbFilterType.SelectedIndex > 0)
            {
                var filterType = CmbFilterType.SelectedIndex switch
                {
                    1 => RuleType.Application,
                    2 => RuleType.Website,
                    3 => RuleType.Process,
                    _ => RuleType.Application
                };
                filteredRules = filteredRules.Where(r => r.Type == filterType);
            }

            // 按优先级和使用次数排序
            var sortedRules = filteredRules
                .OrderBy(r => r.Priority)
                .ThenByDescending(r => r.UsageCount)
                .ToList();

            DataGridRules.ItemsSource = sortedRules;
            TxtRuleCount.Text = $"共 {sortedRules.Count} 条规则";
        }

        private void OnRulesChanged(List<InputSourceRule> rules)
        {
            // 配置变化时重新加载
            LoadRulesFromEngine();
        }

        private void DataGridRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnEdit.IsEnabled = DataGridRules.SelectedItem != null;
            BtnDelete.IsEnabled = DataGridRules.SelectedItem != null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_inputSourceManager == null || _ruleEngine == null) return;

                var rule = new InputSourceRule
                {
                    Name = "新规则",
                    Type = RuleType.Application,
                    Target = "",
                    TargetInputSource = "英语 (美国)",
                    Priority = 1,
                    IsEnabled = true
                };

                var dialog = new RuleEditDialog(_inputSourceManager, rule, savedRule =>
                {
                    _ruleEngine.AddRule(savedRule);
                    SaveAndRefresh();
                });

                if (dialog.ShowDialog() == true)
                {
                    MessageBox.Show("规则已添加", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加规则失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataGridRules.SelectedItem is not InputSourceRule selectedRule)
                    return;

                if (_ruleEngine == null || _inputSourceManager == null) return;

                // 克隆规则用于编辑
                var editedRule = new InputSourceRule
                {
                    Id = selectedRule.Id,
                    Name = selectedRule.Name,
                    Type = selectedRule.Type,
                    Target = selectedRule.Target,
                    TargetInputSource = selectedRule.TargetInputSource,
                    Priority = selectedRule.Priority,
                    IsEnabled = selectedRule.IsEnabled,
                    CreatedAt = selectedRule.CreatedAt,
                    LastUsed = selectedRule.LastUsed,
                    UsageCount = selectedRule.UsageCount
                };

                var dialog = new RuleEditDialog(_inputSourceManager, editedRule, savedRule =>
                {
                    _ruleEngine.UpdateRule(savedRule);
                    SaveAndRefresh();
                });

                if (dialog.ShowDialog() == true)
                {
                    MessageBox.Show("规则已更新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"编辑规则失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataGridRules.SelectedItem is not InputSourceRule selectedRule)
                    return;

                var result = MessageBox.Show(
                    $"确定要删除规则 \"{selectedRule.Name}\" 吗？",
                    "确认删除",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes && _ruleEngine != null)
                {
                    _ruleEngine.RemoveRule(selectedRule.Id);
                    SaveAndRefresh();
                    MessageBox.Show("规则已删除", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除规则失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "JSON文件|*.json|所有文件|*.*",
                    Title = "导入规则"
                };

                if (dialog.ShowDialog() == true && _configService != null)
                {
                    var rules = _configService.ImportRulesAsync(dialog.FileName).Result;
                    if (rules.Count > 0)
                    {
                        if (_ruleEngine != null)
                        {
                            foreach (var rule in rules)
                            {
                                _ruleEngine.AddRule(rule);
                            }
                        }
                        SaveAndRefresh();
                        MessageBox.Show($"已导入 {rules.Count} 条规则", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入规则失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "JSON文件|*.json|所有文件|*.*",
                    Title = "导出规则",
                    FileName = "rules_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json"
                };

                if (dialog.ShowDialog() == true && _configService != null)
                {
                    var rules = _ruleEngine?.GetAllRules() ?? new List<InputSourceRule>();
                    var success = _configService.ExportRulesAsync(dialog.FileName, rules).Result;
                    if (success)
                    {
                        MessageBox.Show($"已导出 {rules.Count} 条规则", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出规则失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDataGrid();
        }

        private void SaveAndRefresh()
        {
            if (_ruleEngine == null || _configService == null) return;

            var rules = _ruleEngine.GetAllRules();
            _ = _configService.SaveRulesAsync(rules);
            LoadRulesFromEngine();
        }
    }
}