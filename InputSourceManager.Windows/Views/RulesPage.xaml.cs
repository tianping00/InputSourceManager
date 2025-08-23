using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private readonly ObservableCollection<InputSourceRule> _rules;
        private InputSourceRule? _selectedRule;

        public RulesPage()
        {
            InitializeComponent();
            _rules = new ObservableCollection<InputSourceRule>();
            
            InitializeControls();
        }

        public void Initialize(ConfigurationService configService, RuleEngineService ruleEngine, InputSourceManager.InputSourceManagerBase inputSourceManager)
        {
            _configService = configService;
            _ruleEngine = ruleEngine;
            _inputSourceManager = inputSourceManager;
            
            _ = LoadRulesAsync();
        }

        private void InitializeControls()
        {
            // 初始化规则类型下拉框
            CmbRuleType.ItemsSource = Enum.GetValues(typeof(RuleType));
            CmbRuleType.SelectedIndex = 0;

            // 绑定规则列表
            DgRules.ItemsSource = _rules;
        }

        private void AddSampleRules()
        {
            if (_rules.Count > 0) return; // 如果已有规则，不添加示例

            var sampleRules = new List<InputSourceRule>
            {
                new InputSourceRule
                {
                    Name = "代码编辑器使用英文",
                    Type = RuleType.Application,
                    Target = "code",
                    TargetInputSource = "英语 (美国)",
                    Priority = 2,
                    IsEnabled = true
                },
                new InputSourceRule
                {
                    Name = "中文网站使用中文",
                    Type = RuleType.Website,
                    Target = "zhihu.com",
                    TargetInputSource = "中文 (简体)",
                    Priority = 1,
                    IsEnabled = true
                },
                new InputSourceRule
                {
                    Name = "英文网站使用英文",
                    Type = RuleType.Website,
                    Target = "stackoverflow.com",
                    TargetInputSource = "英语 (美国)",
                    Priority = 1,
                    IsEnabled = true
                }
            };

            foreach (var rule in sampleRules)
            {
                _rules.Add(rule);
                _ruleEngine?.AddRule(rule);
            }
        }

        private void LoadRulesAsync()
        {
            if (_configService == null || _ruleEngine == null) return;

            try
            {
                var savedRules = _configService.LoadRulesAsync().Result;
                _rules.Clear();
                
                foreach (var rule in savedRules)
                {
                    _rules.Add(rule);
                    _ruleEngine.AddRule(rule);
                }

                if (_rules.Count == 0)
                {
                    AddSampleRules();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载规则失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddRule_Click(object sender, RoutedEventArgs e)
        {
            if (_inputSourceManager == null)
            {
                MessageBox.Show("服务未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 打开规则编辑对话框
            var dialog = new RuleEditDialog(_inputSourceManager, null, OnRuleSaved);
            dialog.Owner = Window.GetWindow(this);
            
            if (dialog.ShowDialog() == true)
            {
                // 规则已保存，刷新列表
                _ = RefreshRulesAsync();
            }
        }

        private void DgRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedRule = DgRules.SelectedItem as InputSourceRule;
        }

        private void BtnEditRule_Click(object sender, RoutedEventArgs e)
        {
            if (_inputSourceManager == null)
            {
                MessageBox.Show("服务未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_selectedRule == null)
            {
                MessageBox.Show("请先选择要编辑的规则", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 打开规则编辑对话框
            var dialog = new RuleEditDialog(_inputSourceManager, _selectedRule, OnRuleSaved);
            dialog.Owner = Window.GetWindow(this);
            
            if (dialog.ShowDialog() == true)
            {
                // 规则已保存，刷新列表
                _ = RefreshRulesAsync();
            }
        }

        private void OnRuleSaved(InputSourceRule rule)
        {
            // 更新规则引擎
            if (_ruleEngine != null)
            {
                if (_rules.Contains(rule))
                {
                    _ruleEngine.UpdateRule(rule);
                }
                else
                {
                    _ruleEngine.AddRule(rule);
                    _rules.Add(rule);
                }
            }
        }

        private void BtnDeleteRule_Click(object sender, RoutedEventArgs e)
        {
            if (_configService == null || _ruleEngine == null)
            {
                MessageBox.Show("服务未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_selectedRule == null)
            {
                MessageBox.Show("请先选择要删除的规则", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"确定要删除规则 '{_selectedRule.Name}' 吗？", "确认删除", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _rules.Remove(_selectedRule);
                _ruleEngine.RemoveRule(_selectedRule.Id);
                _ = SaveRulesAsync();
                _selectedRule = null;
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (_configService == null)
            {
                MessageBox.Show("服务未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "JSON 文件 (*.json)|*.json",
                    FileName = $"InputSourceRules_{DateTime.Now:yyyyMMdd}.json"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var success = _configService.ExportRulesAsync(saveDialog.FileName, _rules.ToList()).Result;
                    if (success)
                    {
                        MessageBox.Show("配置导出成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("配置导出失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出配置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            if (_configService == null || _ruleEngine == null)
            {
                MessageBox.Show("服务未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = "JSON 文件 (*.json)|*.json"
                };

                if (openDialog.ShowDialog() == true)
                {
                    var importedRules = _configService.ImportRulesAsync(openDialog.FileName).Result;
                    if (importedRules.Count > 0)
                    {
                        var result = MessageBox.Show($"导入 {importedRules.Count} 条规则，是否替换现有规则？", 
                            "确认导入", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        
                        if (result == MessageBoxResult.Yes)
                        {
                            _rules.Clear();
                            foreach (var rule in importedRules)
                            {
                                _rules.Add(rule);
                                _ruleEngine.AddRule(rule);
                            }
                            _ = SaveRulesAsync();
                            MessageBox.Show("配置导入成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("导入的文件中没有找到有效规则", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入配置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (_configService == null || _ruleEngine == null)
            {
                MessageBox.Show("服务未初始化", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("确定要重置所有配置吗？这将删除所有自定义规则。", 
                "确认重置", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _configService.ResetConfigurationAsync().Wait();
                    _rules.Clear();
                    _ruleEngine.ClearRules();
                    AddSampleRules();
                    MessageBox.Show("配置已重置为默认值", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"重置配置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SaveRulesAsync()
        {
            if (_configService == null) return;

            try
            {
                await _configService.SaveRulesAsync(_rules.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存规则失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RefreshRulesAsync()
        {
            // 刷新规则列表显示
            DgRules.Items.Refresh();
            await SaveRulesAsync();
        }
    }
}
