using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace InputSourceManager.Services.Linux
{
    /// <summary>
    /// Linux开机自启动服务
    /// 支持 systemd user service 和 XDG autostart
    /// </summary>
    public class LinuxStartupService
    {
        private const string DesktopEntryName = "inputsourcemanager.desktop";
        private const string DesktopEntryTemplate = @"[Desktop Entry]
Type=Application
Name=Input Source Manager
Comment=自动切换输入法工具
Exec={0} --daemon
Terminal=false
Categories=Utility;
X-GNOME-Autostart-enabled=true
";

        /// <summary>
        /// 检查是否已设置开机自启动
        /// </summary>
        public bool IsStartupEnabled()
        {
            try
            {
                // 检查 XDG autostart
                var autostartDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config", "autostart");
                
                if (!Directory.Exists(autostartDir))
                {
                    return false;
                }

                var desktopFile = Path.Combine(autostartDir, DesktopEntryName);
                return File.Exists(desktopFile);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 启用开机自启动
        /// </summary>
        public bool EnableStartup()
        {
            try
            {
                // 创建 XDG autostart 目录
                var autostartDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config", "autostart");
                
                Directory.CreateDirectory(autostartDir);

                // 获取可执行文件路径
                var exePath = GetExecutablePath();
                if (string.IsNullOrEmpty(exePath))
                {
                    return false;
                }

                // 创建 desktop entry 文件
                var desktopFile = Path.Combine(autostartDir, DesktopEntryName);
                var content = string.Format(DesktopEntryTemplate, exePath);
                File.WriteAllText(desktopFile, content, Encoding.UTF8);

                // 设置可执行权限
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "chmod",
                        Arguments = $"+x \"{desktopFile}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit(1000);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 禁用开机自启动
        /// </summary>
        public bool DisableStartup()
        {
            try
            {
                var autostartDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".config", "autostart");
                
                var desktopFile = Path.Combine(autostartDir, DesktopEntryName);
                
                if (File.Exists(desktopFile))
                {
                    File.Delete(desktopFile);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为 daemon 模式
        /// </summary>
        public bool IsDaemonMode()
        {
            var args = Environment.GetCommandLineArgs();
            return args.Length > 1 && args[1] == "--daemon";
        }

        /// <summary>
        /// 获取可执行文件路径
        /// </summary>
        private string? GetExecutablePath()
        {
            try
            {
                // 尝试获取当前进程的可执行文件路径
                var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                {
                    return exePath;
                }

                // 备选方案：使用 AppContext.BaseDirectory
                var baseDir = AppContext.BaseDirectory;
                if (!string.IsNullOrEmpty(baseDir))
                {
                    var possiblePath = Path.Combine(baseDir, "InputSourceManager");
                    if (File.Exists(possiblePath))
                    {
                        return possiblePath;
                    }
                }

                // 尝试在 PATH 中查找
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "which",
                        Arguments = "InputSourceManager",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit(1000);
                
                if (process.ExitCode == 0)
                {
                    var path = process.StandardOutput.ReadToEnd().Trim();
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }
            catch { }

            return null;
        }
    }
}

