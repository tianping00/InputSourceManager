using System;
using System.Threading.Tasks;
using InputSourceManager.Services;
using InputSourceManager.Models;

namespace InputSourceManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 根据操作系统选择合适的实现
            InputSourceManagerBase manager = Environment.OSVersion.Platform == PlatformID.Win32NT 
                ? new WindowsInputSourceManager() 
                : new LinuxInputSourceManager();

            // 检查是否为Linux TUI模式
            if (Environment.OSVersion.Platform != PlatformID.Win32NT && args.Length == 0)
            {
                await RunTUIAsync(manager);
                return;
            }

            Console.WriteLine("=== Input Source Manager ===");
            Console.WriteLine("Windows 输入源管理工具");
            Console.WriteLine("版本: 1.0.0");
            Console.WriteLine();

            var browserService = new BrowserDetectionService();
            var ruleEngine = new RuleEngineService(manager, browserService);
            
            // 添加一些示例规则
            AddSampleRules(ruleEngine);
            
            try
            {
                // 显示可用输入法
                Console.WriteLine("正在获取可用输入法...");
                var inputSources = await manager.GetAvailableInputSourcesAsync();
                
                if (inputSources.Length > 0)
                {
                    Console.WriteLine("可用输入法:");
                    for (int i = 0; i < inputSources.Length; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {inputSources[i]}");
                    }
                }
                else
                {
                    Console.WriteLine("未检测到可用输入法");
                }
                
                Console.WriteLine();
                
                // 显示当前状态
                Console.WriteLine("正在获取当前状态...");
                var currentApp = await manager.GetCurrentApplicationAsync();
                var currentInputSource = await manager.GetCurrentInputSourceAsync();
                var currentWebsite = await browserService.GetCurrentWebsiteAsync();
                
                Console.WriteLine($"当前应用程序: {currentApp ?? "未检测到"}");
                Console.WriteLine($"当前输入法: {currentInputSource ?? "未检测到"}");
                Console.WriteLine($"浏览器状态: {currentWebsite ?? "未检测到浏览器"}");
                
                Console.WriteLine();
                
                // 显示规则信息
                Console.WriteLine("当前规则:");
                var rules = ruleEngine.GetAllRules();
                if (rules.Count > 0)
                {
                    foreach (var rule in rules)
                    {
                        Console.WriteLine($"  - {rule.Name}: {rule.Type} -> {rule.TargetInputSource} (优先级: {rule.Priority})");
                    }
                }
                else
                {
                    Console.WriteLine("  暂无规则");
                }
                
                Console.WriteLine();
                
                // 模拟规则执行
                Console.WriteLine("模拟规则执行...");
                var ruleExecuted = await ruleEngine.ExecuteRulesAsync(
                    currentApp ?? "unknown", 
                    currentInputSource ?? "unknown");
                Console.WriteLine($"规则执行结果: {(ruleExecuted ? "成功" : "无需执行或无匹配规则")}");
                
                Console.WriteLine();
                Console.WriteLine("按任意键退出...");
                try
                {
                    Console.ReadKey();
                }
                catch (InvalidOperationException)
                {
                    // 非交互环境，等待固定时间后退出
                    System.Threading.Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}");
                Console.WriteLine("按任意键退出...");
                try
                {
                    Console.ReadKey();
                }
                catch (InvalidOperationException)
                {
                    // 非交互环境，等待固定时间后退出
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }

        private static void AddSampleRules(RuleEngineService ruleEngine)
        {
            // 添加应用程序规则
            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "记事本使用中文",
                Type = RuleType.Application,
                Target = "notepad",
                TargetInputSource = "中文 (简体)",
                Priority = 1
            });

            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "代码编辑器使用英文",
                Type = RuleType.Application,
                Target = "code",
                TargetInputSource = "英语 (美国)",
                Priority = 2
            });

            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "Visual Studio使用英文",
                Type = RuleType.Application,
                Target = "devenv",
                TargetInputSource = "英语 (美国)",
                Priority = 3
            });

            // 添加网站规则
            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "中文网站使用中文",
                Type = RuleType.Website,
                Target = "*.zhihu.com",
                TargetInputSource = "中文 (简体)",
                Priority = 1
            });

            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "英文网站使用英文",
                Type = RuleType.Website,
                Target = "*.stackoverflow.com",
                TargetInputSource = "英语 (美国)",
                Priority = 1
            });

            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "GitHub使用英文",
                Type = RuleType.Website,
                Target = "*.github.com",
                TargetInputSource = "英语 (美国)",
                Priority = 2
            });

            // 添加进程规则
            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "终端使用英文",
                Type = RuleType.Process,
                Target = "cmd",
                TargetInputSource = "英语 (美国)",
                Priority = 1
            });

            ruleEngine.AddRule(new InputSourceRule
            {
                Name = "PowerShell使用英文",
                Type = RuleType.Process,
                Target = "powershell",
                TargetInputSource = "英语 (美国)",
                Priority = 1
            });
        }

        /// <summary>
        /// 运行TUI主循环
        /// </summary>
        private static async Task RunTUIAsync(InputSourceManagerBase manager)
        {
            while (true)
            {
                Views.LinuxTUI.ShowMainMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await Views.LinuxTUI.ShowCurrentStatus(manager);
                        break;
                    case "2":
                        await Views.LinuxTUI.ShowSwitchMenu(manager);
                        break;
                    case "3":
                        Views.LinuxTUI.ShowRulesManagement();
                        break;
                    case "4":
                        await Views.LinuxTUI.ShowSettingsMenu();
                        break;
                    case "5":
                        Views.LinuxTUI.ShowLogs();
                        break;
                    case "0":
                        Console.WriteLine("\n感谢使用！再见！");
                        return;
                    default:
                        Console.WriteLine("\n❌ 无效选择，请重新输入");
                        await Task.Delay(800);
                        break;
                }
            }
        }
    }
}
