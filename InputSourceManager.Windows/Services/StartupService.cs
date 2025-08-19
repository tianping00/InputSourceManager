using System;
using System.IO;
using Microsoft.Win32;
using System.Security.Principal;

namespace InputSourceManager.Windows
{
    public class StartupService
    {
        private const string AppName = "InputSourceManager";
        private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string TaskName = "InputSourceManager_Startup";

        /// <summary>
        /// 检查是否已设置开机自启动
        /// </summary>
        public bool IsStartupEnabled()
        {
            try
            {
                // 检查注册表
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (key != null)
                {
                    var value = key.GetValue(AppName);
                    return value != null && value.ToString().Contains(AppName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"检查开机自启动状态时出错: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// 设置开机自启动
        /// </summary>
        public bool EnableStartup()
        {
            try
            {
                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (string.IsNullOrEmpty(exePath))
                {
                    exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
                }

                if (string.IsNullOrEmpty(exePath))
                {
                    return false;
                }

                // 使用注册表方式
                using var key = Registry.CurrentUser.CreateSubKey(RegistryKey);
                if (key != null)
                {
                    key.SetValue(AppName, $"\"{exePath}\" --startup");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"设置开机自启动时出错: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// 禁用开机自启动
        /// </summary>
        public bool DisableStartup()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                if (key != null)
                {
                    key.DeleteValue(AppName, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"禁用开机自启动时出错: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// 获取启动参数
        /// </summary>
        public bool IsStartupMode()
        {
            var args = Environment.GetCommandLineArgs();
            return args.Length > 1 && args[1] == "--startup";
        }

        /// <summary>
        /// 检查管理员权限
        /// </summary>
        public bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}

