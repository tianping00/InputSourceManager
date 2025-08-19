using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InputSourceManager.Services
{
    public class BrowserDetectionService
    {
        private readonly string[] _browserProcesses = { "chrome", "msedge", "firefox", "opera", "brave", "chromium" };
        private readonly Dictionary<string, DateTime> _browserCache = new();
        private readonly object _lockObject = new object();
        private const int CACHE_TIMEOUT_SECONDS = 5;

        public virtual async Task<string> GetCurrentWebsiteAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 获取所有浏览器进程
                    var browserProcesses = Process.GetProcesses()
                        .Where(p => _browserProcesses.Contains(p.ProcessName.ToLower()))
                        .ToArray();

                    if (browserProcesses.Length == 0)
                        return null;

                    // 检查缓存
                    var cacheKey = string.Join(",", browserProcesses.Select(p => p.ProcessName));
                    if (IsCacheValid(cacheKey))
                    {
                        return GetCachedWebsite(cacheKey);
                    }

                    // 这里需要更复杂的实现来获取实际的URL
                    // 由于安全限制，我们无法直接读取浏览器内存
                    // 在实际应用中，可能需要使用浏览器扩展或其他方法
                    
                    var result = "检测到浏览器，但无法获取具体URL（需要浏览器扩展支持）";
                    
                    // 更新缓存
                    UpdateCache(cacheKey, result);
                    
                    return result;
                }
                catch
                {
                    return null;
                }
            });
        }

        public virtual async Task<bool> IsWebsiteActiveAsync()
        {
            var website = await GetCurrentWebsiteAsync();
            return !string.IsNullOrEmpty(website);
        }

        public async Task<string[]> GetActiveBrowsersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return Process.GetProcesses()
                        .Where(p => _browserProcesses.Contains(p.ProcessName.ToLower()))
                        .Select(p => p.ProcessName)
                        .Distinct()
                        .ToArray();
                }
                catch
                {
                    return new string[0];
                }
            });
        }

        public async Task<bool> IsBrowserRunningAsync(string browserName)
        {
            if (string.IsNullOrEmpty(browserName))
                return false;

            return await Task.Run(() =>
            {
                try
                {
                    return Process.GetProcesses()
                        .Any(p => string.Equals(p.ProcessName, browserName, StringComparison.OrdinalIgnoreCase));
                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task<Dictionary<string, bool>> GetBrowserStatusAsync()
        {
            return await Task.Run(() =>
            {
                var status = new Dictionary<string, bool>();
                
                try
                {
                    foreach (var browser in _browserProcesses)
                    {
                        var isRunning = Process.GetProcesses()
                            .Any(p => string.Equals(p.ProcessName, browser, StringComparison.OrdinalIgnoreCase));
                        status[browser] = isRunning;
                    }
                }
                catch
                {
                    // 如果出错，所有浏览器都标记为未运行
                    foreach (var browser in _browserProcesses)
                    {
                        status[browser] = false;
                    }
                }
                
                return status;
            });
        }

        private bool IsCacheValid(string cacheKey)
        {
            lock (_lockObject)
            {
                if (_browserCache.TryGetValue(cacheKey, out var timestamp))
                {
                    return (DateTime.Now - timestamp).TotalSeconds < CACHE_TIMEOUT_SECONDS;
                }
                return false;
            }
        }

        private string? GetCachedWebsite(string cacheKey)
        {
            lock (_lockObject)
            {
                // 简化缓存，只缓存检测结果，不缓存具体URL
                return _browserCache.ContainsKey(cacheKey) ? "cached-browser-detected" : null;
            }
        }

        private void UpdateCache(string cacheKey, string result)
        {
            lock (_lockObject)
            {
                _browserCache[cacheKey] = DateTime.Now;
                
                // 清理过期缓存
                var expiredKeys = _browserCache
                    .Where(kvp => (DateTime.Now - kvp.Value).TotalSeconds >= CACHE_TIMEOUT_SECONDS * 2)
                    .Select(kvp => kvp.Key)
                    .ToList();
                
                foreach (var key in expiredKeys)
                {
                    _browserCache.Remove(key);
                }
            }
        }

        public void ClearCache()
        {
            lock (_lockObject)
            {
                _browserCache.Clear();
            }
        }
    }
}
