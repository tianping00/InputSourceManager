using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using InputSourceManager.Services;

namespace InputSourceManager.Windows
{
	public partial class MainWindow : Window
	{
		private readonly InputSourceManager.InputSourceManagerBase _manager;
		private readonly RuleEngineService _ruleEngine;
		private readonly BrowserDetectionService _browserService;
		private readonly HotkeyService _hotkeyService;
		private readonly TrayService _trayService;
		private readonly IndicatorWindow _indicator;
		private readonly System.Windows.Threading.DispatcherTimer _timer;
		private readonly UrlReceiverService _urlReceiver;
		private readonly ConfigurationService _configService;
		private readonly StartupService _startupService;
		private readonly CancellationTokenSource _cts = new();

		public MainWindow()
		{
			InitializeComponent();
			_manager = new InputSourceManager.WindowsInputSourceManager();
			_browserService = new BrowserDetectionService();
			_ruleEngine = new RuleEngineService(_manager, _browserService);
			_configService = new ConfigurationService();
			_startupService = new StartupService();
			_hotkeyService = new HotkeyService();
			_trayService = new TrayService(this);
			_indicator = new IndicatorWindow();
			_urlReceiver = new UrlReceiverService();

			Loaded += OnLoaded;
			Closed += OnClosed;

			_timer = new System.Windows.Threading.DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(1200)
			};
			_timer.Tick += async (s, e) => await RefreshStatusAndAutoSwitchAsync();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			TxtVersion.Text = $"v1.0.7";
			
			// 初始化开机自启动状态
			ChkAutoStart.IsChecked = _startupService.IsStartupEnabled();
			
			// 初始化规则页面
			RulesPageControl.Initialize(_configService, _ruleEngine, _manager);
			
			// 初始化设置页面
			SettingsPageControl.Initialize(_startupService, _manager, _configService);
			
			// 设置窗口消息钩子
			var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
			source.AddHook(WndProc);
			
			// 注册默认热键
			RegisterDefaultHotkey();
			
			// 订阅 URL 接收事件
			_urlReceiver.UrlReceived += OnUrlReceived;
			
			// 启动服务
			_ = RefreshStatusAsync();
			_timer.Start();
			_ = _urlReceiver.StartAsync(_cts.Token);
		}

		private void OnClosed(object? sender, EventArgs e)
		{
			_cts.Cancel();
			_urlReceiver.UrlReceived -= OnUrlReceived;
			_hotkeyService.Dispose();
			_trayService.Dispose();
			_urlReceiver.Dispose();
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (_hotkeyService.TryHandleHotkeyMessage((IntPtr)msg, wParam))
			{
				handled = true;
			}
			return IntPtr.Zero;
		}

		// URL 接收事件处理
		private void OnUrlReceived(object? sender, UrlReceivedEventArgs e)
		{
			try
			{
				// 在 UI 线程上更新状态
				Dispatcher.Invoke(() =>
				{
					TxtCurrentUrl.Text = e.Url;
					TxtCurrentDomain.Text = e.Domain;
					TxtLastUpdate.Text = DateTime.Now.ToString("HH:mm:ss");
				});

				// 执行规则匹配
				_ = Task.Run(async () =>
				{
					try
					{
						var ruleExecuted = await _ruleEngine.ExecuteRulesAsync(e.Domain, "Website");
						if (ruleExecuted)
						{
							Dispatcher.Invoke(() =>
							{
								TxtRuleStatus.Text = "规则已执行";
								TxtRuleStatus.Foreground = System.Windows.Media.Brushes.Green;
							});
						}
					}
					catch (Exception ex)
					{
						Dispatcher.Invoke(() =>
						{
							TxtRuleStatus.Text = $"规则执行失败: {ex.Message}";
							TxtRuleStatus.Foreground = System.Windows.Media.Brushes.Red;
						});
					}
				});
			}
			catch (Exception ex)
			{
				Dispatcher.Invoke(() =>
				{
					TxtRuleStatus.Text = $"处理URL时出错: {ex.Message}";
					TxtRuleStatus.Foreground = System.Windows.Media.Brushes.Red;
				});
			}
		}

		private async Task RefreshStatusAndAutoSwitchAsync()
		{
			await RefreshStatusAsync();
			if (ChkAutoSwitch.IsChecked == true)
			{
				await TryExecuteRulesAsync();
			}
		}

		private async Task RefreshStatusAsync()
		{
			var app = await _manager.GetCurrentApplicationAsync();
			var layout = await _manager.GetCurrentInputSourceAsync();
			TxtCurrentApp.Text = app ?? "-";
			TxtCurrentLayout.Text = layout ?? "-";
		}

		private async Task TryExecuteRulesAsync()
		{
			var app = await _manager.GetCurrentApplicationAsync();
			var layout = await _manager.GetCurrentInputSourceAsync();
			var executed = await _ruleEngine.ExecuteRulesAsync(app ?? string.Empty, layout ?? string.Empty);
			if (executed)
			{
				ShowIndicator(await _manager.GetCurrentInputSourceAsync());
				TxtStatus.Text = "已根据规则切换";
			}
		}

		private void RegisterDefaultHotkey()
		{
			var helper = new WindowInteropHelper(this);
			helper.EnsureHandle();
			_hotkeyService.RegisterHotkey(helper.Handle, HotkeyModifiers.Control | HotkeyModifiers.Alt, System.Windows.Forms.Keys.Space, OnToggleHotkey);
		}

		private async void OnToggleHotkey()
		{
			var layout = await _manager.GetCurrentInputSourceAsync();
			var target = (layout?.Contains("中文") ?? false) ? "英语 (美国)" : "中文 (简体)";
			if (await _manager.SwitchToInputSourceAsync(target))
			{
				ShowIndicator(target);
				TxtStatus.Text = "快捷键切换完成";
			}
		}

		private void ShowIndicator(string? text)
		{
			if (string.IsNullOrWhiteSpace(text)) return;
			_indicator.SetText(text!);
			_indicator.ShowTemporarily();
		}

		private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
		{
			await RefreshStatusAsync();
		}

		private void BtnToggle_Click(object sender, RoutedEventArgs e)
		{
			OnToggleHotkey();
		}

		private void BtnMinimizeToTray_Click(object sender, RoutedEventArgs e)
		{
			_trayService.MinimizeToTray();
		}

		private void BtnApplyHotkey_Click(object sender, RoutedEventArgs e)
		{
			_hotkeyService.UnregisterAll();
			RegisterDefaultHotkey();
			TxtStatus.Text = "快捷键已应用";
		}
	}
}
