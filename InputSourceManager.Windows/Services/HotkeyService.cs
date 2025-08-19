using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InputSourceManager.Windows
{
	[Flags]
	public enum HotkeyModifiers
	{
		None = 0,
		Alt = 1,
		Control = 2,
		Shift = 4,
		Win = 8
	}

	public sealed class HotkeyService : IDisposable
	{
		private const int WM_HOTKEY = 0x0312;
		[DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
		[DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private readonly Dictionary<int, Action> _handlers = new();
		private IntPtr _windowHandle = IntPtr.Zero;
		private int _nextId = 1;

		public void RegisterHotkey(IntPtr hwnd, HotkeyModifiers modifiers, Keys key, Action handler)
		{
			_windowHandle = hwnd;
			var id = _nextId++;
			_handlers[id] = handler;
			RegisterHotKey(hwnd, id, (int)modifiers, (int)key);
		}

		public void UnregisterAll()
		{
			if (_windowHandle == IntPtr.Zero) return;
			foreach (var id in _handlers.Keys)
			{
				UnregisterHotKey(_windowHandle, id);
			}
			_handlers.Clear();
		}

		public bool TryHandleHotkeyMessage(IntPtr msg, IntPtr wParam)
		{
			if (msg.ToInt32() != WM_HOTKEY) return false;
			var id = wParam.ToInt32();
			if (_handlers.TryGetValue(id, out var handler))
			{
				handler();
				return true;
			}
			return false;
		}

		public void Dispose()
		{
			UnregisterAll();
		}
	}
}
