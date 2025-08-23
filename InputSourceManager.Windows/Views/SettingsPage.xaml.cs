using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using InputSourceManager.Services;

namespace InputSourceManager.Windows.Views
{
    public partial class SettingsPage : UserControl
    {
        private StartupService? _startupService;
        private InputSourceManager.InputSourceManagerBase? _inputSourceManager;
        private ConfigurationService? _configService;
        private AppSettings _currentSettings;

        public SettingsPage()
        {
            InitializeComponent();
            _currentSettings = new AppSettings();
        }

        public void Initialize(StartupService startupService, InputSourceManager.InputSourceManagerBase inputSourceManager, ConfigurationService configService)
        {
            _startupService = startupService;
            _inputSourceManager = inputSourceManager;
            _configService = configService;
            
            InitializeControls();
            LoadSettings();
        }

        private void InitializeControls()
        {
            if (_startupService == null) return;

            // 初始化开机自启动状态
            ChkAutoStart.IsChecked = _startupService.IsStartupEnabled();
            
            // 初始化输入法下拉框
            LoadInputSources();
        }

        private void LoadInputSources()
        {
            if (_inputSourceManager == null) return;

            try
            {
                var inputSources = _inputSourceManager.GetAvailableInputSourcesAsync().Result;
                CmbDefaultChinese.ItemsSource = inputSources;
                CmbDefaultEnglish.ItemsSource = inputSources;
                
                // 设置默认选择
                if (inputSources.Length > 0)
                {
                    CmbDefaultChinese.SelectedIndex = 0;
                    CmbDefaultEnglish.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载输入法列表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                // 加载检测间隔
                TxtDetectionInterval.Text = _currentSettings.DetectionInterval.ToString();
                
                // 加载其他设置
                ChkMinimizeToTray.IsChecked = _currentSettings.MinimizeToTray;
                ChkShowIndicator.IsChecked = _currentSettings.ShowIndicator;
                ChkEnableLogging.IsChecked = _currentSettings.EnableLogging;
                ChkEnableDebugMode.IsChecked = _currentSettings.EnableDebugMode;
                ChkEnablePerformanceMonitoring.IsChecked = _currentSettings.EnablePerformanceMonitoring;
                ChkEnableAutoUpdate.IsChecked = _currentSettings.EnableAutoUpdate;
                
                // 加载热键设置
                TxtToggleHotkey.Text = _currentSettings.ToggleHotkey;
                TxtShowWindowHotkey.Text = _currentSettings.ShowWindowHotkey;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载设置失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChkAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            if (_startupService == null) return;

            try
            {
                if (_startupService.EnableStartup())
                {
                    MessageBox.Show("开机自启动已启用", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("启用开机自启动失败，请检查权限设置", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ChkAutoStart.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置开机自启动时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                ChkAutoStart.IsChecked = false;
            }
        }

        private void ChkAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_startupService == null) return;

            try
            {
                if (_startupService.DisableStartup())
                {
                    MessageBox.Show("开机自启动已禁用", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("禁用开机自启动失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ChkAutoStart.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"禁用开机自启动时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                ChkAutoStart.IsChecked = true;
            }
        }

        private void BtnApplyHotkeys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 验证热键格式
                if (!IsValidHotkeyFormat(TxtToggleHotkey.Text) || !IsValidHotkeyFormat(TxtShowWindowHotkey.Text))
                {
                    MessageBox.Show("热键格式不正确，请使用 Ctrl+Alt+Key 格式", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _currentSettings.ToggleHotkey = TxtToggleHotkey.Text;
                _currentSettings.ShowWindowHotkey = TxtShowWindowHotkey.Text;
                
                MessageBox.Show("热键设置已应用，重启程序后生效", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用热键设置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidHotkeyFormat(string hotkey)
        {
            // 简单的热键格式验证
            return hotkey.Contains("Ctrl") || hotkey.Contains("Alt") || hotkey.Contains("Shift");
        }

        private void BtnRefreshInputSources_Click(object sender, RoutedEventArgs e)
        {
            LoadInputSources();
            MessageBox.Show("输入法列表已刷新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnTestInputSources_Click(object sender, RoutedEventArgs e)
        {
            if (_inputSourceManager == null) return;

            try
            {
                var currentInputSource = _inputSourceManager.GetCurrentInputSourceAsync().Result;
                var targetInputSource = currentInputSource?.Contains("中文") == true ? "英语 (美国)" : "中文 (简体)";
                
                if (_inputSourceManager.SwitchToInputSourceAsync(targetInputSource).Result)
                {
                    MessageBox.Show($"测试成功！已切换到: {targetInputSource}", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("测试失败，无法切换输入法", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试输入法时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 保存所有设置
                _currentSettings.DetectionInterval = int.Parse(TxtDetectionInterval.Text);
                _currentSettings.MinimizeToTray = ChkMinimizeToTray.IsChecked ?? false;
                _currentSettings.ShowIndicator = ChkShowIndicator.IsChecked ?? false;
                _currentSettings.EnableLogging = ChkEnableLogging.IsChecked ?? false;
                _currentSettings.EnableDebugMode = ChkEnableDebugMode.IsChecked ?? false;
                _currentSettings.EnablePerformanceMonitoring = ChkEnablePerformanceMonitoring.IsChecked ?? false;
                _currentSettings.EnableAutoUpdate = ChkEnableAutoUpdate.IsChecked ?? false;
                
                // 保存到配置文件
                await SaveSettingsAsync();
                
                MessageBox.Show("设置已保存", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存设置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnResetSettings_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要重置所有设置吗？", "确认重置", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _currentSettings = new AppSettings();
                    LoadSettings();
                    await SaveSettingsAsync();
                    
                    MessageBox.Show("设置已重置为默认值", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"重置设置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnExportSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_configService == null) return;

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "JSON 文件 (*.json)|*.json",
                    FileName = $"InputSourceSettings_{DateTime.Now:yyyyMMdd}.json"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var success = await _configService.ExportSettingsAsync(saveDialog.FileName, _currentSettings);
                    if (success)
                    {
                        MessageBox.Show("设置导出成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("设置导出失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出设置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveSettingsAsync()
        {
            if (_configService == null) return;

            try
            {
                await _configService.SaveSettingsAsync(_currentSettings);
            }
            catch (Exception ex)
            {
                throw new Exception($"保存设置失败: {ex.Message}");
            }
        }
    }

    public class AppSettings
    {
        public int DetectionInterval { get; set; } = 1200;
        public bool MinimizeToTray { get; set; } = true;
        public bool ShowIndicator { get; set; } = true;
        public string ToggleHotkey { get; set; } = "Ctrl+Alt+Space";
        public string ShowWindowHotkey { get; set; } = "Ctrl+Alt+I";
        public string DefaultChineseInputSource { get; set; } = "中文 (简体)";
        public string DefaultEnglishInputSource { get; set; } = "英语 (美国)";
        public bool EnableLogging { get; set; } = false;
        public bool EnableDebugMode { get; set; } = false;
        public bool EnablePerformanceMonitoring { get; set; } = false;
        public bool EnableAutoUpdate { get; set; } = true;
    }
}
