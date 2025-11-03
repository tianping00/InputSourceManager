using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace InputSourceManager.Services.Linux
{
    /// <summary>
    /// Linux系统托盘服务
    /// 使用 notify-send 和其他 GNOME/KDE 工具
    /// </summary>
    public class LinuxTrayService : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// 显示通知
        /// </summary>
        public void ShowNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            try
            {
                var icon = GetIcon(type);
                var urgency = GetUrgency(type);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "notify-send",
                        Arguments = $"--icon=\"{icon}\" --urgency={urgency} \"{title}\" \"{message}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
            }
            catch { }
        }

        /// <summary>
        /// 显示气球提示（通知）
        /// </summary>
        public void ShowBalloonTip(string title, string message)
        {
            ShowNotification(title, message, NotificationType.Info);
        }

        /// <summary>
        /// 创建系统托盘指示器
        /// 使用 libappindicator 或 gtk3 托盘图标
        /// </summary>
        public async Task<bool> CreateTrayIconAsync(string tooltip)
        {
            // 注：实际系统托盘需要 GTK# 或 libappindicator
            // 这里提供基本实现框架
            return await Task.FromResult(false);
        }

        /// <summary>
        /// 根据通知类型获取图标
        /// </summary>
        private string GetIcon(NotificationType type)
        {
            return type switch
            {
                NotificationType.Info => "dialog-information",
                NotificationType.Warning => "dialog-warning",
                NotificationType.Error => "dialog-error",
                NotificationType.Success => "dialog-information",
                _ => "dialog-information"
            };
        }

        /// <summary>
        /// 根据通知类型获取紧急度
        /// </summary>
        private string GetUrgency(NotificationType type)
        {
            return type switch
            {
                NotificationType.Error => "critical",
                NotificationType.Warning => "normal",
                _ => "low"
            };
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            // 清理资源
        }
    }

    /// <summary>
    /// 通知类型
    /// </summary>
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success
    }
}

