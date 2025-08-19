#if WINDOWS
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InputSourceManager
{
    public partial class InputSourceManager
    {
        // Windows API 导入
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutList(int nBuff, [Out] IntPtr[] lpList);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        // 消息常量
        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const uint INPUTLANGCHANGE_FORWARD = 0x0002;
        private const uint INPUTLANGCHANGE_BACKWARD = 0x0001;

        public override async Task<string> GetCurrentApplicationAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var foregroundWindow = GetForegroundWindow();
                    if (foregroundWindow == IntPtr.Zero)
                        return null;

                    // 获取进程ID
                    GetWindowThreadProcessId(foregroundWindow, out uint processId);
                    if (processId == 0)
                        return null;

                    // 获取进程信息
                    var process = Process.GetProcessById((int)processId);
                    return process?.ProcessName;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"获取当前应用程序时出错: {ex.Message}");
                    return null;
                }
            });
        }

        public override async Task<string> GetCurrentInputSourceAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var foregroundWindow = GetForegroundWindow();
                    if (foregroundWindow == IntPtr.Zero)
                        return null;

                    // 获取窗口线程ID
                    GetWindowThreadProcessId(foregroundWindow, out uint threadId);
                    if (threadId == 0)
                        return null;

                    // 获取键盘布局
                    var keyboardLayout = GetKeyboardLayout(threadId);
                    if (keyboardLayout == IntPtr.Zero)
                        return null;

                    // 将布局句柄转换为语言ID
                    var langId = (ushort)(keyboardLayout.ToInt64() & 0xFFFF);
                    
                    return GetLanguageName(langId);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"获取当前输入法时出错: {ex.Message}");
                    return null;
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
                    
                    if (count == 0)
                        return new string[0];

                    var languages = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        var langId = (ushort)(layouts[i].ToInt64() & 0xFFFF);
                        languages[i] = GetLanguageName(langId);
                    }

                    return languages;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"获取可用输入法时出错: {ex.Message}");
                    return new string[0];
                }
            });
        }

        public override async Task<bool> SwitchToInputSourceAsync(string languageName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 获取桌面窗口句柄
                    var desktopWindow = GetDesktopWindow();
                    if (desktopWindow == IntPtr.Zero)
                        return false;

                    // 发送输入法切换请求
                    var result = PostMessage(desktopWindow, WM_INPUTLANGCHANGEREQUEST, 
                                          IntPtr.Zero, IntPtr.Zero);
                    
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"切换输入法时出错: {ex.Message}");
                    return false;
                }
            });
        }

        private string GetLanguageName(ushort langId)
        {
            // 常见的语言ID映射
            return langId switch
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
                0x0416 => "葡萄牙语 (巴西)",
                0x0816 => "葡萄牙语 (葡萄牙)",
                0x0419 => "俄语",
                0x0406 => "丹麦语",
                0x040B => "芬兰语",
                0x040D => "希伯来语",
                0x040E => "匈牙利语",
                0x040F => "冰岛语",
                0x0413 => "荷兰语",
                0x0414 => "挪威语",
                0x0415 => "波兰语",
                0x0418 => "罗马尼亚语",
                0x041B => "斯洛伐克语",
                0x041D => "瑞典语",
                0x041E => "泰语",
                0x041F => "土耳其语",
                _ => $"未知语言 (0x{langId:X4})"
            };
        }
    }
}
#endif


