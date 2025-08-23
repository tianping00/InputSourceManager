using System.Windows.Controls;
using InputSourceManager.Services;

namespace InputSourceManager.Windows.Views
{
    public partial class RulesPage : UserControl
    {
        public RulesPage()
        {
            InitializeComponent();
        }

        public void Initialize(ConfigurationService configService, RuleEngineService ruleEngine, InputSourceManager.InputSourceManagerBase inputSourceManager)
        {
            // 简化版本 - 避免异步调用
            // 实际功能将在后续版本中实现
        }
    }
}