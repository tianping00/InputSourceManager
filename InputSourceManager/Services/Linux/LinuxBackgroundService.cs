using System;
using System.Threading;
using System.Threading.Tasks;

namespace InputSourceManager.Services.Linux
{
    /// <summary>
    /// Linux后台服务
    /// 提供 daemon 模式运行能力
    /// </summary>
    public class LinuxBackgroundService
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _monitoringTask;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 状态更新事件
        /// </summary>
        public event EventHandler<string>? StatusUpdated;

        /// <summary>
        /// 启动后台监控
        /// </summary>
        public void Start(Func<Task> monitoringAction)
        {
            if (IsRunning)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            IsRunning = true;

            _monitoringTask = Task.Run(async () =>
            {
                try
                {
                    OnStatusUpdated("后台服务已启动");

                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            await monitoringAction();
                        }
                        catch (Exception ex)
                        {
                            OnStatusUpdated($"监控出错: {ex.Message}");
                        }

                        // 等待1.2秒后继续
                        await Task.Delay(1200, _cancellationTokenSource!.Token);
                    }

                    OnStatusUpdated("后台服务已停止");
                }
                catch (OperationCanceledException)
                {
                    OnStatusUpdated("后台服务已取消");
                }
                catch (Exception ex)
                {
                    OnStatusUpdated($"后台服务错误: {ex.Message}");
                }
                finally
                {
                    IsRunning = false;
                }
            }, _cancellationTokenSource!.Token);
        }

        /// <summary>
        /// 停止后台监控
        /// </summary>
        public async Task StopAsync()
        {
            if (!IsRunning)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            
            if (_monitoringTask != null)
            {
                try
                {
                    await _monitoringTask;
                }
                catch (OperationCanceledException) { }
            }

            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// 触发状态更新事件
        /// </summary>
        protected virtual void OnStatusUpdated(string status)
        {
            StatusUpdated?.Invoke(this, status);
        }
    }
}

