using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using InputSourceManager.Models;
using InputSourceManager.Services;

namespace InputSourceManager.Windows.Views
{
    public partial class RuleEditDialog : Window
    {
        private readonly InputSourceManager _inputSourceManager;
        private readonly InputSourceRule _rule;
        private readonly bool _isNewRule;
        private readonly Action<InputSourceRule> _onSave;

        public RuleEditDialog(InputSourceManager inputSourceManager, InputSourceRule? rule = null, Action<InputSourceRule>? onSave = null)
        {
            InitializeComponent();
            _inputSourceManager = inputSourceManager;
            _onSave = onSave ?? (r => { });
            
            if (rule != null)
            {
                _rule = rule;
                _isNewRule = false;
                Title = "编辑规则";
            }
            else
            {
                _rule = new InputSourceRule();
                _isNewRule = true;
                Title = "新建规则";
            }

            InitializeControls();
            LoadRuleData();
        }

        private void InitializeControls()
        {
            // 初始化规则类型
            CmbRuleType.ItemsSource = Enum.GetValues(typeof(RuleType));
            CmbRuleType.SelectedIndex = 0;

            // 初始化优先级
            var priorities = new[] { 0, 1, 2, 3, 4, 5 };
            CmbPriority.ItemsSource = priorities;
            CmbPriority.SelectedIndex = 1; // 默认优先级 1

            // 加载输入法列表
            LoadInputSources();
        }

        private async void LoadInputSources()
        {
            try
            {
                var inputSources = await _inputSourceManager.GetAvailableInputSourcesAsync();
                CmbTargetInputSource.ItemsSource = inputSources;
                CmbFallbackInputSource.ItemsSource = inputSources;
                
                if (inputSources.Length > 0)
                {
                    CmbTargetInputSource.SelectedIndex = 0;
                    CmbFallbackInputSource.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载输入法列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRuleData()
        {
            if (_isNewRule) return;

            // 加载现有规则数据
            TxtRuleName.Text = _rule.Name;
            CmbRuleType.SelectedItem = _rule.Type;
            TxtTarget.Text = _rule.Target;
            CmbPriority.SelectedItem = _rule.Priority;
            
            // 设置目标输入法
            if (!string.IsNullOrEmpty(_rule.TargetInputSource))
            {
                foreach (var item in CmbTargetInputSource.Items)
                {
                    if (item.ToString() == _rule.TargetInputSource)
                    {
                        CmbTargetInputSource.SelectedItem = item;
                        break;
                    }
                }
            }

            // 设置其他属性（如果有的话）
            // 这里可以根据需要扩展
        }

        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var targetInputSource = CmbTargetInputSource.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(targetInputSource))
                {
                    MessageBox.Show("请先选择目标输入法", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 测试切换到目标输入法
                if (await _inputSourceManager.SwitchToInputSourceAsync(targetInputSource))
                {
                    MessageBox.Show($"测试成功！已切换到: {targetInputSource}", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("测试失败，无法切换到目标输入法", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试规则时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 验证输入
                if (!ValidateInput())
                {
                    return;
                }

                // 保存规则数据
                SaveRuleData();

                // 调用保存回调
                _onSave(_rule);

                // 关闭对话框
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存规则时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            // 验证规则名称
            if (string.IsNullOrWhiteSpace(TxtRuleName.Text))
            {
                MessageBox.Show("请输入规则名称", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtRuleName.Focus();
                return false;
            }

            // 验证目标
            if (string.IsNullOrWhiteSpace(TxtTarget.Text))
            {
                MessageBox.Show("请输入目标（应用程序名或网站域名）", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTarget.Focus();
                return false;
            }

            // 验证目标输入法
            if (CmbTargetInputSource.SelectedItem == null)
            {
                MessageBox.Show("请选择目标输入法", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbTargetInputSource.Focus();
                return false;
            }

            // 验证优先级
            if (CmbPriority.SelectedItem == null)
            {
                MessageBox.Show("请选择优先级", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbPriority.Focus();
                return false;
            }

            // 验证切换延迟
            if (!int.TryParse(TxtSwitchDelay.Text, out var delay) || delay < 0)
            {
                MessageBox.Show("切换延迟必须是大于等于0的整数", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtSwitchDelay.Focus();
                return false;
            }

            return true;
        }

        private void SaveRuleData()
        {
            // 基本信息
            _rule.Name = TxtRuleName.Text.Trim();
            _rule.Type = (RuleType)CmbRuleType.SelectedItem;
            _rule.Target = TxtTarget.Text.Trim();
            _rule.Priority = (int)CmbPriority.SelectedItem;
            _rule.TargetInputSource = CmbTargetInputSource.SelectedItem.ToString();

            // 输入法设置
            if (CmbFallbackInputSource.SelectedItem != null)
            {
                // 这里可以扩展规则模型以支持备用输入法
                // _rule.FallbackInputSource = CmbFallbackInputSource.SelectedItem.ToString();
            }

            // 切换延迟
            if (int.TryParse(TxtSwitchDelay.Text, out var delay))
            {
                // 这里可以扩展规则模型以支持切换延迟
                // _rule.SwitchDelay = delay;
            }

            // 条件设置
            // 这里可以扩展规则模型以支持更多条件
            // _rule.OnlyWhenFocused = ChkOnlyWhenFocused.IsChecked ?? false;
            // _rule.OnlyWhenTyping = ChkOnlyWhenTyping.IsChecked ?? false;
            // _rule.RespectUserChoice = ChkRespectUserChoice.IsChecked ?? false;
            // _rule.ForceSwitch = ChkForceSwitch.IsChecked ?? false;

            // 更新时间
            if (_isNewRule)
            {
                _rule.CreatedAt = DateTime.Now;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            // 设置窗口样式
            if (Owner != null)
            {
                this.Owner = Owner;
            }
        }
    }
}

