# Input Source Manager

A cross-platform input source management tool that supports Windows and Linux systems, with functionality comparable to [InputSourcePro](https://github.com/runjuu/InputSourcePro).

**跨平台输入源管理工具，支持Windows和Linux系统，功能媲美InputSourcePro**

## 🚀 Features / 功能特性

### 🥷 Automatic Context-Aware Switching / 自动上下文感知切换
- **Application Awareness**: Automatically switch input methods based on active applications
  **应用程序感知**: 根据活动应用程序自动切换输入法
- **Website Awareness**: Switch input methods based on browsed websites (requires browser extension support)
  **网站感知**: 根据浏览的网站切换输入法（需要浏览器扩展支持）
- **Process Awareness**: Support for precise matching based on process names
  **进程感知**: 支持基于进程名称的精确匹配

### 🎯 Intelligent Rule Engine / 智能规则引擎
- **Priority System**: Support for rule priority settings
  **优先级系统**: 支持规则优先级设置
- **Multi-type Rules**: Three rule types: Application, Website, and Process
  **多类型规则**: 三种规则类型：应用程序、网站和进程
- **Usage Statistics**: Track rule usage frequency and last usage time
  **使用统计**: 跟踪规则使用频率和最后使用时间
- **Rule Management**: Add, delete, modify, enable/disable rules
  **规则管理**: 添加、删除、修改、启用/禁用规则

### 🌐 Multi-language Support / 多语言支持
- **10+ Languages**: English, Chinese, Japanese, Korean, German, French, etc.
  **10+种语言**: 英语、中文、日语、韩语、德语、法语等
- **Smart Detection**: Automatically detect system-available input methods
  **智能检测**: 自动检测系统可用的输入法
- **Language Mapping**: Standardized language name display
  **语言映射**: 标准化的语言名称显示

### 💾 Configuration Management / 配置管理
- **Auto-save**: Rules automatically saved to user configuration directory
  **自动保存**: 规则自动保存到用户配置目录
- **Import/Export**: Support for configuration file import and export
  **导入/导出**: 支持配置文件导入和导出
- **Version Control**: Configuration file version management
  **版本控制**: 配置文件版本管理
- **Cross-device Sync**: Configuration files can be shared between different devices
  **跨设备同步**: 配置文件可以在不同设备间共享

### 🔍 Real-time Monitoring / 实时监控
- **Status Monitoring**: Real-time display of current applications and input methods
  **状态监控**: 实时显示当前应用程序和输入法
- **Browser Detection**: Detect mainstream browser process status
  **浏览器检测**: 检测主流浏览器进程状态
- **Performance Optimization**: Asynchronous operations, non-blocking main thread
  **性能优化**: 异步操作，不阻塞主线程

## 🏗️ Project Architecture / 项目架构

```
InputSource/
├── InputSourceManager/                    # Core library (cross-platform) / 核心库（跨平台）
│   ├── Models/                           # Data models / 数据模型
│   │   └── InputSourceRule.cs           # Input source rule model / 输入源规则模型
│   ├── Services/                         # Business services / 业务服务
│   │   ├── BrowserDetectionService.cs   # Browser detection service / 浏览器检测服务
│   │   ├── RuleEngineService.cs         # Rule engine service / 规则引擎服务
│   │   └── ConfigurationService.cs      # Configuration management service / 配置管理服务
│   ├── InputSourceManager.cs            # Base input source management class / 基础输入源管理类
│   ├── Program.cs                        # Console application entry point / 控制台应用程序入口
│   └── InputSourceManager.csproj
├── InputSourceManager.Windows/           # Windows-specific version / Windows专用版本
│   ├── InputSourceManager.Windows.cs    # Windows API implementation / Windows API实现
│   └── InputSourceManager.Windows.csproj
├── build.sh                              # Linux build script / Linux构建脚本
├── build.bat                             # Windows build script / Windows构建脚本
└── README.md                             # Project documentation / 项目文档
```

## 🛠️ Installation and Usage / 安装和使用

### System Requirements / 系统要求
- **Cross-platform version**: .NET 8.0 or higher / 跨平台版本：.NET 8.0或更高
- **Windows version**: Windows 10/11 + .NET 8.0 Desktop Runtime / Windows版本：Windows 10/11 + .NET 8.0桌面运行时

### Download Pre-built Versions / 下载预构建版本

#### Windows Version Downloads / Windows版本下载
- **Framework-dependent version** (requires .NET 8.0 Desktop Runtime): [InputSourceManager-Windows-fxdep.zip](https://github.com/tianping00/InputSourceManager/releases/latest/download/InputSourceManager-Windows-fxdep.zip)
  **框架依赖版本**（需要.NET 8.0桌面运行时）
- **Self-contained version** (no additional installation required): [InputSourceManager-Windows-selfcontained.zip](https://github.com/tianping00/InputSourceManager/releases/latest/download/InputSourceManager-Windows-selfcontained.zip)
  **自包含版本**（无需额外安装）

> **Note**: The self-contained version has a larger file size (~100MB) but doesn't require .NET Runtime installation. The framework-dependent version has a smaller file size (~10MB) but requires .NET 8.0 Desktop Runtime to be installed first.
> **注意**: 自包含版本文件较大（约100MB）但无需安装.NET运行时。框架依赖版本文件较小（约10MB）但需要先安装.NET 8.0桌面运行时。

### Quick Start / 快速开始

#### 1. Clone the Project / 克隆项目
```bash
git clone https://github.com/tianping00/InputSourceManager.git
cd InputSourceManager
```

#### 2. Build the Project / 构建项目
```bash
# Linux/macOS
./build.sh

# Windows
build.bat
```

#### 3. Run the Program / 运行程序
```bash
# Cross-platform version / 跨平台版本
dotnet run --project InputSourceManager

# Windows version (Windows only) / Windows版本（仅Windows）
dotnet run --project InputSourceManager.Windows
```

## 📋 Rule Configuration Examples / 规则配置示例

### Application Rules / 应用程序规则
```json
{
  "Type": "Application",
  "Pattern": "notepad.exe",
  "TargetLanguage": "en-US",
  "Priority": 1
}
```

### Website Rules / 网站规则
```json
{
  "Type": "Website",
  "Pattern": "*.github.com",
  "TargetLanguage": "en-US",
  "Priority": 2
}
```

### Process Rules / 进程规则
```json
{
  "Type": "Process",
  "Pattern": "chrome",
  "TargetLanguage": "zh-CN",
  "Priority": 3
}
```

## 🔧 Configuration / 配置

### Configuration File Location / 配置文件位置
- **Windows**: `%APPDATA%\InputSourceManager\config.json`
- **Linux**: `~/.config/InputSourceManager/config.json`

### Configuration File Structure / 配置文件结构
```json
{
  "Rules": [
    {
      "Type": "Application",
      "Pattern": "notepad.exe",
      "TargetLanguage": "en-US",
      "Priority": 1,
      "Enabled": true
    }
  ],
  "Settings": {
    "AutoStart": true,
    "ShowIndicator": true,
    "IndicatorDuration": 2000
  }
}
```

## 🧪 Testing / 测试

### Run Tests / 运行测试
```bash
dotnet test InputSourceManager.Tests/InputSourceManager.Tests.csproj
```

### Test Coverage / 测试覆盖
- **Unit Tests**: 45 test cases / 单元测试：45个测试用例
- **Test Framework**: xUnit / 测试框架：xUnit
- **Coverage**: 100% test coverage for core services / 覆盖率：核心服务100%测试覆盖

## 🚀 Browser Extension Integration / 浏览器扩展集成

The project includes a local HTTP service that receives URL information from browser extensions to enable automatic input method switching based on website content.

项目包含一个本地HTTP服务，接收来自浏览器扩展的URL信息，实现基于网站内容的自动输入法切换。

### Service Configuration / 服务配置
- **Port**: Default 8080 (configurable) / 端口：默认8080（可配置）
- **Protocol**: HTTP / 协议：HTTP
- **Security**: Localhost only, no external access / 安全性：仅限本地主机，无外部访问

### API Endpoint / API端点
**POST /url** - Receives URL information from browser extensions / 接收来自浏览器扩展的URL信息

## 🤝 Contributing / 贡献

1. Fork the project / Fork项目
2. Create a feature branch (`git checkout -b feature/AmazingFeature`) / 创建功能分支
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`) / 提交更改
4. Push to the branch (`git push origin feature/AmazingFeature`) / 推送到分支
5. Open a Pull Request / 打开拉取请求

## 📄 License / 许可证

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

本项目采用MIT许可证 - 详情请参阅[LICENSE](LICENSE)文件。

## 🙏 Acknowledgments / 致谢

- Inspired by [InputSourcePro](https://github.com/runjuu/InputSourcePro) / 灵感来自InputSourcePro
- Built with .NET 8.0 and WPF / 使用.NET 8.0和WPF构建
- Uses Material Design themes for modern UI / 使用Material Design主题实现现代UI

---

**Input Source Manager - 智能的跨平台输入法管理工具** 🎯
# Trigger build - Mon Nov 03 10:58:39 CST 2025
