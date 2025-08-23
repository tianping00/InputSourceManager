using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO.Pipes;
using InputSourceManager.Models;

namespace InputSourceManager.Services
{
    public class ConfigurationService : IDisposable
    {
        private readonly string _configFilePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private FileSystemWatcher? _fileWatcher;
        private readonly List<Action<List<InputSourceRule>>> _configChangedCallbacks = new();
        private readonly object _lockObject = new object();

        public ConfigurationService()
        {
            _configFilePath = GetConfigFilePath();
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // 设置文件监控，实现配置热重载
            SetupFileWatcher();
        }

        protected virtual string GetConfigFilePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configDir = Path.Combine(appDataPath, "InputSourceManager");
            
            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);
                
            return Path.Combine(configDir, "config.json");
        }

        protected virtual void SetupFileWatcher()
        {
            try
            {
                var configDir = Path.GetDirectoryName(_configFilePath);
                if (!string.IsNullOrEmpty(configDir))
                {
                    _fileWatcher = new FileSystemWatcher(configDir, "config.json")
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                        EnableRaisingEvents = true
                    };

                    _fileWatcher.Changed += OnConfigFileChanged;
                    _fileWatcher.Created += OnConfigFileChanged;
                }
            }
            catch
            {
                // 如果文件监控设置失败，静默处理
            }
        }

        private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            // 延迟处理，避免文件写入过程中的多次触发
            Task.Delay(100).ContinueWith(_ =>
            {
                try
                {
                    var rules = LoadRulesAsync().Result;
                    NotifyConfigChanged(rules);
                }
                catch
                {
                    // 静默处理错误
                }
            });
        }

        public void RegisterConfigChangedCallback(Action<List<InputSourceRule>> callback)
        {
            if (callback != null)
            {
                lock (_lockObject)
                {
                    _configChangedCallbacks.Add(callback);
                }
            }
        }

        public void UnregisterConfigChangedCallback(Action<List<InputSourceRule>> callback)
        {
            if (callback != null)
            {
                lock (_lockObject)
                {
                    _configChangedCallbacks.Remove(callback);
                }
            }
        }

        private void NotifyConfigChanged(List<InputSourceRule> rules)
        {
            lock (_lockObject)
            {
                foreach (var callback in _configChangedCallbacks)
                {
                    try
                    {
                        callback(rules);
                    }
                    catch
                    {
                        // 静默处理回调错误
                    }
                }
            }
        }

        public virtual async Task<bool> SaveRulesAsync(List<InputSourceRule> rules)
        {
            try
            {
                var config = new ConfigurationData
                {
                    Rules = rules,
                    LastUpdated = DateTime.Now,
                    Version = "1.0.0"
                };

                var json = JsonSerializer.Serialize(config, _jsonOptions);
                
                // 临时禁用文件监控，避免触发重载
                if (_fileWatcher != null)
                    _fileWatcher.EnableRaisingEvents = false;

                await File.WriteAllTextAsync(_configFilePath, json);
                
                // 重新启用文件监控
                if (_fileWatcher != null)
                    _fileWatcher.EnableRaisingEvents = true;
                
                return true;
            }
            catch
            {
                // 重新启用文件监控
                if (_fileWatcher != null)
                    _fileWatcher.EnableRaisingEvents = true;
                return false;
            }
        }

        public virtual async Task<List<InputSourceRule>> LoadRulesAsync()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                    return new List<InputSourceRule>();

                var json = await File.ReadAllTextAsync(_configFilePath);
                var config = JsonSerializer.Deserialize<ConfigurationData>(json, _jsonOptions);
                
                return config?.Rules ?? new List<InputSourceRule>();
            }
            catch
            {
                return new List<InputSourceRule>();
            }
        }

        public async Task<bool> ExportRulesAsync(string filePath, List<InputSourceRule> rules)
        {
            try
            {
                var config = new ConfigurationData
                {
                    Rules = rules,
                    LastUpdated = DateTime.Now,
                    Version = "1.0.0"
                };

                var json = JsonSerializer.Serialize(config, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<InputSourceRule>> ImportRulesAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<InputSourceRule>();

                var json = await File.ReadAllTextAsync(filePath);
                var config = JsonSerializer.Deserialize<ConfigurationData>(json, _jsonOptions);
                
                return config?.Rules ?? new List<InputSourceRule>();
            }
            catch
            {
                return new List<InputSourceRule>();
            }
        }

        public virtual Task<bool> ResetConfigurationAsync()
        {
            try
            {
                if (File.Exists(_configFilePath))
                    File.Delete(_configFilePath);
                    
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<bool> ExportSettingsAsync<T>(string filePath, T settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T?> ImportSettingsAsync<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return default(T);

                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch
            {
                return default(T);
            }
        }

        public async Task<bool> SaveSettingsAsync<T>(T settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(_configFilePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _fileWatcher?.Dispose();
        }

        private class ConfigurationData
        {
            public List<InputSourceRule> Rules { get; set; } = new();
            public DateTime LastUpdated { get; set; }
            public string Version { get; set; } = string.Empty;
        }
    }
}
