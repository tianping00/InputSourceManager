using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace InputSourceManager.Windows
{
	public sealed class TrayService : IDisposable
	{
		private readonly NotifyIcon _notifyIcon;
		private readonly Window _mainWindow;

		public TrayService(Window mainWindow)
		{
			_mainWindow = mainWindow;
			_notifyIcon = new NotifyIcon
			{
				Icon = SystemIcons.Information,
				Text = "Input Source Manager",
				Visible = true,
				ContextMenuStrip = BuildMenu()
			};
			_notifyIcon.DoubleClick += (s, e) => RestoreFromTray();
		}

		private ContextMenuStrip BuildMenu()
		{
			var menu = new ContextMenuStrip();
			menu.Items.Add("显示主窗口", null, (s, e) => RestoreFromTray());
			menu.Items.Add("退出", null, (s, e) => System.Windows.Application.Current.Shutdown());
			return menu;
		}

		public void MinimizeToTray()
		{
			_mainWindow.Hide();
			_notifyIcon.BalloonTipTitle = "Input Source Manager";
			_notifyIcon.BalloonTipText = "程序已最小化到系统托盘";
			_notifyIcon.ShowBalloonTip(2000);
		}

		public void RestoreFromTray()
		{
			_mainWindow.Show();
			_mainWindow.WindowState = WindowState.Normal;
			_mainWindow.Activate();
		}

		public void Dispose()
		{
			_notifyIcon.Visible = false;
			_notifyIcon.Dispose();
		}
	}
}





