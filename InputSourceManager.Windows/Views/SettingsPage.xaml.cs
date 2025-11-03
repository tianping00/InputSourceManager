using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using InputSourceManager.Services;

namespace InputSourceManager.Windows.Views
{
    public partial class SettingsPage : UserControl
    {
        private StartupService? _startupService;
        private InputSourceManager.InputSourceManagerBase? _manager;
        private ConfigurationService? _configService;

        public SettingsPage()
        {
            InitializeComponent();
        }

        public void Initialize(StartupService startupService, InputSourceManager.InputSourceManagerBase manager, ConfigurationService configService)
        {
            _startupService = startupService;
            _manager = manager;
            _configService = configService;

            // 加载设置
            LoadSettings();
        }

        private async void LoadSettings()
        {
            // 加载开机自启动状态
            if (_startupService != null)
            {
                ChkAutoStart.IsChecked = _startupService.IsStartupEnabled();
            }

            // 加载可用输入法
            if (_manager != null)
            {
                try
                {
                    var inputSources = await _manager.GetAvailableInputSourcesAsync();
                    if (inputSources.Length > 0)
                    {
                        DataGridInputSources.ItemsSource = inputSources.OrderBy(s => s);
                    }
                }
                catch (Exception ex)
                {
                    // 如果加载输入法列表失败，记录但不阻止页面显示
                    System.Diagnostics.Debug.WriteLine($"加载输入法列表失败: {ex.Message}");
                }
            }
        }

        private void ChkAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_startupService != null)
                {
                    var success = _startupService.EnableStartup();
                    if (!success)
                    {
                        ChkAutoStart.IsChecked = false;
                        MessageBox.Show("设置开机自启动失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("开机自启动已启用", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置开机自启动时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChkAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_startupService != null)
                {
                    var success = _startupService.DisableStartup();
                    if (!success)
                    {
                        ChkAutoStart.IsChecked = true;
                        MessageBox.Show("取消开机自启动失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("开机自启动已禁用", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"取消开机自启动时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnResetConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "确定要重置所有配置吗？此操作不可恢复。",
                    "确认重置",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes && _configService != null)
                {
                    var success = _configService.ResetConfigurationAsync().Result;
                    if (success)
                    {
                        MessageBox.Show("配置已重置，请重新启动程序以应用更改。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("重置配置失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重置配置时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}