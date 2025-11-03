using System;
using System.Threading.Tasks;

namespace InputSourceManager.Views
{
    /// <summary>
    /// Linux终端用户界面 (TUI)
    /// 提供简洁的终端交互界面
    /// </summary>
    public class LinuxTUI
    {
        /// <summary>
        /// 显示主菜单
        /// </summary>
        public static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("      Input Source Manager - Linux");
            Console.WriteLine("       输入源管理工具 (Linux版本)");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("功能菜单:");
            Console.WriteLine("  1. 查看当前状态");
            Console.WriteLine("  2. 手动切换输入法");
            Console.WriteLine("  3. 规则管理");
            Console.WriteLine("  4. 设置");
            Console.WriteLine("  5. 查看日志");
            Console.WriteLine("  0. 退出");
            Console.WriteLine();
            Console.Write("请选择操作 [0-5]: ");
        }

        /// <summary>
        /// 显示当前状态
        /// </summary>
        public static async Task ShowCurrentStatus(InputSourceManagerBase manager)
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("           当前状态");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            try
            {
                var currentApp = await manager.GetCurrentApplicationAsync();
                var currentInput = await manager.GetCurrentInputSourceAsync();
                var availableInputs = await manager.GetAvailableInputSourcesAsync();

                Console.WriteLine($"当前应用程序: {currentApp}");
                Console.WriteLine($"当前输入法: {currentInput}");
                Console.WriteLine();
                Console.WriteLine("可用输入法:");
                for (int i = 0; i < availableInputs.Length; i++)
                {
                    var marker = availableInputs[i] == currentInput ? "← " : "  ";
                    Console.WriteLine($"{marker}{i + 1}. {availableInputs[i]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取状态失败: {ex.Message}");
            }

            Console.WriteLine();
            Console.Write("按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 显示手动切换菜单
        /// </summary>
        public static async Task ShowSwitchMenu(InputSourceManagerBase manager)
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("           手动切换输入法");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            try
            {
                var availableInputs = await manager.GetAvailableInputSourcesAsync();
                var currentInput = await manager.GetCurrentInputSourceAsync();

                Console.WriteLine("请选择要切换的输入法:");
                for (int i = 0; i < availableInputs.Length; i++)
                {
                    var marker = availableInputs[i] == currentInput ? "← " : "  ";
                    Console.WriteLine($"{marker}{i + 1}. {availableInputs[i]}");
                }
                Console.WriteLine("  0. 返回");

                Console.Write("\n选择 [0-{0}]: ", availableInputs.Length);
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= availableInputs.Length)
                {
                    var target = availableInputs[choice - 1];
                    Console.Write($"\n正在切换到: {target}...");
                    
                    var success = await manager.SwitchToInputSourceAsync(target);
                    
                    Console.Clear();
                    Console.WriteLine("═══════════════════════════════════════════");
                    if (success)
                    {
                        Console.WriteLine("✅ 切换成功！");
                        Console.WriteLine($"   已切换到: {target}");
                    }
                    else
                    {
                        Console.WriteLine("❌ 切换失败");
                        Console.WriteLine($"   无法切换到: {target}");
                    }
                    Console.WriteLine("═══════════════════════════════════════════");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换失败: {ex.Message}");
            }

            Console.WriteLine();
            Console.Write("按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 显示设置菜单
        /// </summary>
        public static async Task ShowSettingsMenu()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("           设置");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("设置选项:");
            Console.WriteLine("  1. 开机自启动");
            Console.WriteLine("  2. 自动切换开关");
            Console.WriteLine("  3. 日志级别");
            Console.WriteLine("  0. 返回");

            Console.Write("\n选择 [0-3]: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowStartupSettings();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 显示开机自启动设置
        /// </summary>
        private static async Task ShowStartupSettings()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("        开机自启动设置");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            var startupService = new Services.Linux.LinuxStartupService();
            var isEnabled = startupService.IsStartupEnabled();

            Console.WriteLine($"当前状态: {(isEnabled ? "已启用" : "已禁用")}");
            Console.WriteLine();
            Console.WriteLine("选项:");
            Console.WriteLine($"  {(isEnabled ? "1. 禁用" : "1. 启用")}");
            Console.WriteLine("  0. 返回");

            Console.Write("\n选择 [0-1]: ");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                bool success;
                if (isEnabled)
                {
                    success = startupService.DisableStartup();
                    Console.WriteLine(success ? "✅ 已禁用开机自启动" : "❌ 禁用失败");
                }
                else
                {
                    success = startupService.EnableStartup();
                    Console.WriteLine(success ? "✅ 已启用开机自启动" : "❌ 启用失败");
                }

                Console.WriteLine();
                Console.Write("按任意键返回...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 显示日志
        /// </summary>
        public static void ShowLogs()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("           日志查看");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("日志功能开发中...");
            Console.WriteLine();

            Console.Write("按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 显示规则管理
        /// </summary>
        public static void ShowRulesManagement()
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("           规则管理");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("规则管理功能开发中...");
            Console.WriteLine();

            Console.Write("按任意键返回...");
            Console.ReadKey();
        }
    }
}

