using System;

namespace InputSourceManager.Models
{
    public class InputSourceRule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public RuleType Type { get; set; }
        public string Target { get; set; } = string.Empty; // 应用程序名称或网站URL
        public string TargetInputSource { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public int Priority { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastUsed { get; set; }
        public int UsageCount { get; set; } = 0;
    }

    public enum RuleType
    {
        Application,  // 基于应用程序
        Website,      // 基于网站
        Process       // 基于进程名
    }
}
