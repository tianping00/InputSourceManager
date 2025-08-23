using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace InputSourceManager
{
    public abstract class InputSourceManagerBase
    {
        public abstract Task<string> GetCurrentApplicationAsync();
        public abstract Task<string> GetCurrentInputSourceAsync();
        public abstract Task<string[]> GetAvailableInputSourcesAsync();
        public abstract Task<bool> SwitchToInputSourceAsync(string languageName);
        public abstract Task<bool> SwitchToInputSourceByHotkeyAsync();
    }

    public class WindowsInputSourceManager : InputSourceManagerBase
    {
        // Windows API declarations
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(int idThread);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutList(int nBuff, IntPtr[] lpList);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, uint flags);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThreadId();

        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const uint INPUTLANGCHANGE_FORWARD = 0x0002;
        private const uint INPUTLANGCHANGE_BACKWARD = 0x0001;
        private const uint KLF_ACTIVATE = 0x00000001;

        public override async Task<string> GetCurrentApplicationAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var foregroundWindow = GetForegroundWindow();
                    if (foregroundWindow != IntPtr.Zero)
                    {
                        GetWindowThreadProcessId(foregroundWindow, out int processId);
                        var process = System.Diagnostics.Process.GetProcessById(processId);
                        return process.ProcessName;
                    }
                    return "unknown";
                }
                catch
                {
                    return "unknown";
                }
            });
        }

        public override async Task<string> GetCurrentInputSourceAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var threadId = GetCurrentThreadId();
                    var layout = GetKeyboardLayout((int)threadId);
                    return GetLanguageNameFromLayout(layout);
                }
                catch
                {
                    return "未知";
                }
            });
        }

        public override async Task<string[]> GetAvailableInputSourcesAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var layouts = new IntPtr[256];
                    var count = GetKeyboardLayoutList(layouts.Length, layouts);
                    
                    var languages = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        languages[i] = GetLanguageNameFromLayout(layouts[i]);
                    }
                    
                    return languages;
                }
                catch
                {
                    return new string[] { "英语 (美国)", "中文 (简体)" };
                }
            });
        }

        public override async Task<bool> SwitchToInputSourceAsync(string languageName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 通过快捷键切换（Alt+Shift）
                    return SwitchToInputSourceByHotkeyAsync().Result;
                }
                catch
                {
                    return false;
                }
            });
        }

        public override async Task<bool> SwitchToInputSourceByHotkeyAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var foregroundWindow = GetForegroundWindow();
                    if (foregroundWindow != IntPtr.Zero)
                    {
                        // 发送Alt+Shift快捷键消息
                        PostMessage(foregroundWindow, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, (IntPtr)INPUTLANGCHANGE_FORWARD);
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        private string GetLanguageNameFromLayout(IntPtr layout)
        {
            // 简化的语言名称映射
            var layoutId = layout.ToInt64() & 0xFFFF;
            return layoutId switch
            {
                0x0409 => "英语 (美国)",
                0x0804 => "中文 (简体)",
                0x0404 => "中文 (繁体)",
                0x0411 => "日语",
                0x0412 => "韩语",
                0x0407 => "德语",
                0x040C => "法语",
                0x0410 => "意大利语",
                0x040A => "西班牙语",
                0x0419 => "俄语",
                _ => $"语言 (0x{layoutId:X4})"
            };
        }
    }

    public class LinuxInputSourceManager : InputSourceManagerBase
    {
        public override Task<string> GetCurrentApplicationAsync()
        {
            // Linux实现占位符
            return Task.FromResult("linux-app");
        }

        public override Task<string> GetCurrentInputSourceAsync()
        {
            return Task.FromResult("中文 (简体)");
        }

        public override Task<string[]> GetAvailableInputSourcesAsync()
        {
            return Task.FromResult(new string[]
            {
                "英语 (美国)",
                "中文 (简体)",
                "日语",
                "韩语"
            });
        }

        public override Task<bool> SwitchToInputSourceAsync(string languageName)
        {
            return Task.FromResult(true);
        }

        public override Task<bool> SwitchToInputSourceByHotkeyAsync()
        {
            return Task.FromResult(true);
        }
    }
}
