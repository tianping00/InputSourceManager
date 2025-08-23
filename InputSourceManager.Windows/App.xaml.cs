using System.Windows;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace InputSourceManager.Windows
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			try
			{
				// 在调试模式下跳过运行时检查
				#if !DEBUG
				if (!IsDotNetDesktopRuntimeInstalled())
				{
					const string url = "https://dotnet.microsoft.com/en-us/download/dotnet/8.0/runtime";
					var result = MessageBox.Show(
						"未检测到 .NET 8.0 Desktop Runtime。\n请先安装后再运行本程序。\n\n下载地址:\n" + url + "\n\n是否现在打开浏览器前往下载页面？",
						"缺少运行时",
						MessageBoxButton.YesNo,
						MessageBoxImage.Warning);

					if (result == MessageBoxResult.Yes)
					{
						try
						{
							Process.Start(new ProcessStartInfo
							{
								FileName = url,
								UseShellExecute = true
							});
						}
						catch { }
					}

					Shutdown();
					return;
				}
				#endif

				base.OnStartup(e);
				this.DispatcherUnhandledException += (s, ex) =>
				{
					MessageBox.Show($"发生未处理异常: {ex.Exception.Message}\n\n堆栈跟踪:\n{ex.Exception.StackTrace}", 
						"错误", MessageBoxButton.OK, MessageBoxImage.Error);
					ex.Handled = true;
				};
			}
			catch (Exception ex)
			{
				MessageBox.Show($"应用程序启动失败: {ex.Message}\n\n详细信息:\n{ex}", 
					"启动错误", MessageBoxButton.OK, MessageBoxImage.Error);
				throw;
			}
		}

		private bool IsDotNetDesktopRuntimeInstalled()
		{
			try
			{
				// 检查常见安装路径是否存在 8.0.x 版本
				var candidates = new[]
				{
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "shared", "Microsoft.WindowsDesktop.App"),
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet", "shared", "Microsoft.WindowsDesktop.App")
				};

				foreach (var basePath in candidates)
				{
					if (!string.IsNullOrWhiteSpace(basePath) && Directory.Exists(basePath))
					{
						var has8 = Directory.GetDirectories(basePath)
							.Select(Path.GetFileName)
							.Any(v => v != null && v.StartsWith("8.0.", StringComparison.OrdinalIgnoreCase));
						if (has8) return true;
					}
				}
			}
			catch { }

			return false;
		}
	}
}
