using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    var foregroundWindow = GetForegroundWindow();
                    if (foregroundWindow != IntPtr.Zero)
                    {
                        var threadId = GetWindowThreadProcessId(foregroundWindow, out _);
                        var layout = GetKeyboardLayout(threadId);
                        return GetLanguageNameFromLayout(layout);
                    }
                    return "未知";
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
                    // 获取所有可用的键盘布局
                    var layouts = new IntPtr[256];
                    var count = GetKeyboardLayoutList(layouts.Length, layouts);
                    
                    // 查找目标语言的布局
                    var targetLayout = IntPtr.Zero;
                    foreach (var layout in layouts.Take(count))
                    {
                        var name = GetLanguageNameFromLayout(layout);
                        if (name.Equals(languageName, StringComparison.OrdinalIgnoreCase))
                        {
                            targetLayout = layout;
                            break;
                        }
                    }
                    
                    // 如果找不到目标布局，使用快捷键循环切换
                    if (targetLayout == IntPtr.Zero)
                    {
                        return SwitchToInputSourceByHotkeyAsync().Result;
                    }
                    
                    // 获取前台窗口
                    var foregroundWindow = GetForegroundWindow();
                    if (foregroundWindow != IntPtr.Zero)
                    {
                        // 通过布局ID切换到目标输入法
                        var layoutIdLow = targetLayout.ToInt32() & 0xFFFF;
                        PostMessage(foregroundWindow, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, (IntPtr)layoutIdLow);
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
        // 输入法框架类型枚举
        private enum InputMethodFramework
        {
            Unknown,
            IBus,
            Fcitx,
            None
        }

        // 当前检测到的输入法框架
        private InputMethodFramework _detectedFramework = InputMethodFramework.Unknown;
        private bool _frameworkChecked = false;

        // 语言名称到IBus引擎的映射
        private readonly Dictionary<string, string> _ibusEngineMap = new()
        {
            { "英语 (美国)", "xkb:us::eng" },
            { "英语 (英国)", "xkb:gb::eng" },
            { "中文 (简体)", "pinyin" },
            { "中文 (繁体)", "chewing" },
            { "日语", "mozc" },
            { "韩语", "hangul" },
            { "俄语", "xkb:ru::rus" },
            { "法语", "xkb:fr::fra" },
            { "德语", "xkb:de::ger" },
            { "西班牙语", "xkb:es::spa" },
        };

        // 语言名称到fcitx引擎的映射
        private readonly Dictionary<string, int> _fcitxEngineMap = new()
        {
            { "英语 (美国)", 1 },
            { "英语 (英国)", 2 },
            { "中文 (简体)", 2 },
            { "中文 (繁体)", 3 },
            { "日语", 3 },
            { "韩语", 4 },
            { "俄语", 4 },
            { "法语", 5 },
            { "德语", 6 },
            { "西班牙语", 7 },
        };

        /// <summary>
        /// 检测系统中可用的输入法框架
        /// </summary>
        private InputMethodFramework DetectInputMethodFramework()
        {
            if (_frameworkChecked)
                return _detectedFramework;

            try
            {
                // 检查IBus是否可用
                var ibusProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "which",
                        Arguments = "ibus",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                ibusProcess.Start();
                ibusProcess.WaitForExit();
                if (ibusProcess.ExitCode == 0)
                {
                    _detectedFramework = InputMethodFramework.IBus;
                    _frameworkChecked = true;
                    return _detectedFramework;
                }
            }
            catch { }

            try
            {
                // 检查fcitx是否可用
                var fcitxProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "which",
                        Arguments = "fcitx",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                fcitxProcess.Start();
                fcitxProcess.WaitForExit();
                if (fcitxProcess.ExitCode == 0)
                {
                    _detectedFramework = InputMethodFramework.Fcitx;
                    _frameworkChecked = true;
                    return _detectedFramework;
                }
            }
            catch { }

            _detectedFramework = InputMethodFramework.None;
            _frameworkChecked = true;
            return _detectedFramework;
        }

        /// <summary>
        /// 获取当前前台应用程序名称
        /// </summary>
        public override async Task<string> GetCurrentApplicationAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 尝试使用xdotool获取活动窗口
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "xdotool",
                            Arguments = "getactivewindow getwindowclassname",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    process.WaitForExit(1000); // 超时1秒

                    if (process.ExitCode == 0)
                    {
                        var output = process.StandardOutput.ReadToEnd().Trim();
                        if (!string.IsNullOrEmpty(output))
                        {
                            return output;
                        }
                    }
                }
                catch { }

                // 回退方案：尝试使用wmctrl
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "wmctrl",
                            Arguments = "-a :ACTIVE: -v 2>/dev/null",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    process.WaitForExit(500);

                    if (process.ExitCode == 0)
                    {
                        var output = process.StandardOutput.ReadToEnd().Trim();
                        if (!string.IsNullOrEmpty(output))
                        {
                            return output;
                        }
                    }
                }
                catch { }

                return "unknown";
            });
        }

        /// <summary>
        /// 获取当前输入法
        /// </summary>
        public override async Task<string> GetCurrentInputSourceAsync()
        {
            return await Task.Run(() =>
            {
                var framework = DetectInputMethodFramework();

                if (framework == InputMethodFramework.IBus)
                {
                    return GetCurrentIBusInputSource();
                }
                else if (framework == InputMethodFramework.Fcitx)
                {
                    return GetCurrentFcitxInputSource();
                }

                return "未检测到输入法框架";
            });
        }

        /// <summary>
        /// 获取IBus当前输入法
        /// </summary>
        private string GetCurrentIBusInputSource()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ibus",
                        Arguments = "engine",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit(1000);

                if (process.ExitCode == 0)
                {
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    // 解析输出格式，例如: "pinyin"
                    
                    // 反向查找语言名称
                    foreach (var kvp in _ibusEngineMap)
                    {
                        if (kvp.Value.Contains(output) || output.Contains(kvp.Value))
                        {
                            return kvp.Key;
                        }
                    }

                    return output;
                }
            }
            catch { }

            return "未知";
        }

        /// <summary>
        /// 获取fcitx当前输入法
        /// </summary>
        private string GetCurrentFcitxInputSource()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "fcitx-remote",
                        Arguments = "-c",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit(1000);

                if (process.ExitCode == 0)
                {
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    if (int.TryParse(output, out int engineIndex))
                    {
                        // 反向查找语言名称
                        foreach (var kvp in _fcitxEngineMap)
                        {
                            if (kvp.Value == engineIndex)
                            {
                                return kvp.Key;
                            }
                        }
                    }

                    return $"输入法 {output}";
                }
            }
            catch { }

            return "未知";
        }

        /// <summary>
        /// 获取可用的输入源列表
        /// </summary>
        public override async Task<string[]> GetAvailableInputSourcesAsync()
        {
            return await Task.Run(() =>
            {
                var framework = DetectInputMethodFramework();

                if (framework == InputMethodFramework.IBus)
                {
                    return GetAvailableIBusInputSources();
                }
                else if (framework == InputMethodFramework.Fcitx)
                {
                    return GetAvailableFcitxInputSources();
                }

                // 默认返回常见语言列表
                return new string[]
                {
                    "英语 (美国)",
                    "英语 (英国)",
                    "中文 (简体)",
                    "中文 (繁体)",
                    "日语",
                    "韩语"
                };
            });
        }

        /// <summary>
        /// 获取IBus可用输入法列表
        /// </summary>
        private string[] GetAvailableIBusInputSources()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ibus",
                        Arguments = "list-engine",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit(2000);

                if (process.ExitCode == 0)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    var engines = new List<string>();

                    // 解析输出，尝试匹配已知引擎
                    foreach (var line in output.Split('\n'))
                    {
                        var trimmedLine = line.Trim();
                        if (string.IsNullOrEmpty(trimmedLine))
                            continue;

                        // 查找匹配的引擎映射
                        foreach (var kvp in _ibusEngineMap)
                        {
                            if (trimmedLine.Contains(kvp.Value) || kvp.Value.Contains(trimmedLine))
                            {
                                engines.Add(kvp.Key);
                                break;
                            }
                        }
                    }

                    if (engines.Count > 0)
                    {
                        return engines.ToArray();
                    }
                }
            }
            catch { }

            // 返回默认列表
            return new string[]
            {
                "英语 (美国)",
                "中文 (简体)",
                "日语",
                "韩语"
            };
        }

        /// <summary>
        /// 获取fcitx可用输入法列表
        /// </summary>
        private string[] GetAvailableFcitxInputSources()
        {
            // fcitx的列表获取较复杂，返回默认列表
            return new string[]
            {
                "英语 (美国)",
                "中文 (简体)",
                "日语",
                "韩语"
            };
        }

        /// <summary>
        /// 切换到指定输入源
        /// </summary>
        public override async Task<bool> SwitchToInputSourceAsync(string languageName)
        {
            return await Task.Run(() =>
            {
                var framework = DetectInputMethodFramework();

                if (framework == InputMethodFramework.IBus)
                {
                    return SwitchToIBusInputSource(languageName);
                }
                else if (framework == InputMethodFramework.Fcitx)
                {
                    return SwitchToFcitxInputSource(languageName);
                }

                return false;
            });
        }

        /// <summary>
        /// 切换到IBus输入法
        /// </summary>
        private bool SwitchToIBusInputSource(string languageName)
        {
            try
            {
                if (_ibusEngineMap.TryGetValue(languageName, out var engineName))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "ibus",
                            Arguments = $"engine {engineName}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    process.WaitForExit(1000);

                    return process.ExitCode == 0;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 切换到fcitx输入法
        /// </summary>
        private bool SwitchToFcitxInputSource(string languageName)
        {
            try
            {
                if (_fcitxEngineMap.TryGetValue(languageName, out var engineIndex))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "fcitx-remote",
                            Arguments = $"-s {engineIndex}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    process.WaitForExit(1000);

                    return process.ExitCode == 0;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 使用快捷键切换输入法
        /// </summary>
        public override async Task<bool> SwitchToInputSourceByHotkeyAsync()
        {
            return await Task.Run(() =>
            {
                var framework = DetectInputMethodFramework();

                if (framework == InputMethodFramework.IBus)
                {
                    // IBus默认使用Super+Space切换，模拟发送该快捷键
                    try
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "xdotool",
                                Arguments = "key super+space",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit(500);

                        return process.ExitCode == 0;
                    }
                    catch { }
                }
                else if (framework == InputMethodFramework.Fcitx)
                {
                    // fcitx默认使用Ctrl+Space切换
                    try
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "xdotool",
                                Arguments = "key ctrl+space",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        process.Start();
                        process.WaitForExit(500);

                        return process.ExitCode == 0;
                    }
                    catch { }
                }

                return false;
            });
        }
    }
}
