# Input Source Manager

一个跨平台的输入源管理工具，支持 Windows 和 Linux 系统，功能媲美 [InputSourcePro](https://github.com/runjuu/InputSourcePro)。

## 🚀 功能特性

### 🥷 自动上下文感知切换
- **应用程序感知**：根据活动应用程序自动切换输入法
- **网站感知**：根据浏览的网站自动切换输入法（需要浏览器扩展支持）
- **进程感知**：支持基于进程名的精确匹配

### 🎯 智能规则引擎
- **优先级系统**：支持规则优先级设置
- **多类型规则**：应用程序、网站、进程三种规则类型
- **使用统计**：跟踪规则使用频率和最后使用时间
- **规则管理**：添加、删除、修改、启用/禁用规则

### 🌐 多语言支持
- **10+ 种语言**：英语、中文、日语、韩语、德语、法语等
- **智能检测**：自动检测系统可用的输入法
- **语言映射**：标准化的语言名称显示

### 💾 配置管理
- **自动保存**：规则自动保存到用户配置目录
- **导入导出**：支持配置文件的导入和导出
- **版本控制**：配置文件版本管理
- **跨设备同步**：配置文件可在不同设备间共享

### 🔍 实时监控
- **状态监控**：实时显示当前应用程序和输入法
- **浏览器检测**：检测主流浏览器进程状态
- **性能优化**：异步操作，不阻塞主线程

## 🏗️ 项目架构

```
InputSource/
├── InputSourceManager/                    # 核心库（跨平台）
│   ├── Models/                           # 数据模型
│   │   └── InputSourceRule.cs           # 输入源规则模型
│   ├── Services/                         # 业务服务
│   │   ├── BrowserDetectionService.cs   # 浏览器检测服务
│   │   ├── RuleEngineService.cs         # 规则引擎服务
│   │   └── ConfigurationService.cs      # 配置管理服务
│   ├── InputSourceManager.cs            # 基础输入源管理类
│   ├── Program.cs                        # 控制台应用程序入口
│   └── InputSourceManager.csproj
├── InputSourceManager.Windows/           # Windows 专用版本
│   ├── InputSourceManager.Windows.cs    # Windows API 实现
│   └── InputSourceManager.Windows.csproj
├── build.sh                              # Linux 构建脚本
├── build.bat                             # Windows 构建脚本
└── README.md                             # 项目说明文档
```

## 🛠️ 安装和使用

### 系统要求
- **跨平台版本**：.NET 8.0 或更高版本
- **Windows 版本**：Windows 10/11 + .NET 8.0 Desktop Runtime

### Download Pre-built Versions

#### Windows Version Downloads
- **Framework-dependent version** (requires .NET 8.0 Desktop Runtime): [InputSourceManager-Windows-fxdep.zip](https://github.com/tianping00/InputSourceManager/releases/latest/download/InputSourceManager-Windows-fxdep.zip)
- **Self-contained version** (no additional installation required): [InputSourceManager-Windows-selfcontained.zip](https://github.com/tianping00/InputSourceManager/releases/latest/download/InputSourceManager-Windows-selfcontained.zip)

> **Note**: The self-contained version has a larger file size (~100MB) but doesn't require .NET Runtime installation. The framework-dependent version has a smaller file size (~10MB) but requires .NET 8.0 Desktop Runtime to be installed first.

### Quick Start

#### 1. Clone the Project
```bash
git clone https://github.com/tianping00/InputSourceManager.git
cd InputSourceManager
```

#### 2. Build the Project
```bash
# Linux/macOS
./build.sh

# Windows
build.bat
```

#### 3. Run the Program
```bash
# Cross-platform version
dotnet run --project InputSourceManager

# Windows version (Windows only)
dotnet run --project InputSourceManager.Windows
```

## 📋 Rule Configuration Examples

### Application Rules
```json
{
  "Type": "Application",
  "Pattern": "notepad.exe",
  "TargetLanguage": "en-US",
  "Priority": 1
}
```

### Website Rules
```json
{
  "Type": "Website",
  "Pattern": "*.github.com",
  "TargetLanguage": "en-US",
  "Priority": 2
}
```

### Process Rules
```json
{
  "Type": "Process",
  "Pattern": "chrome",
  "TargetLanguage": "zh-CN",
  "Priority": 3
}
```

## 🔧 Configuration

### Configuration File Location
- **Windows**: `%APPDATA%\InputSourceManager\config.json`
- **Linux**: `~/.config/InputSourceManager/config.json`

### Configuration File Structure
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

## 🧪 Testing

### Run Tests
```bash
dotnet test InputSourceManager.Tests/InputSourceManager.Tests.csproj
```

### Test Coverage
- **Unit Tests**: 45 test cases
- **Test Framework**: xUnit
- **Coverage**: 100% test coverage for core services

## 📚 Documentation

- **[中文文档](README.zh-CN.md)** - 中文版本的项目说明
- **[项目状态报告](PROJECT_STATUS_REPORT.zh-CN.md)** - 详细的项目状态和功能说明
- **[URL接收器说明](URL_RECEIVER_README.zh-CN.md)** - 浏览器扩展集成的详细说明
- **[发布说明模板](RELEASE_NOTES_TEMPLATE.zh-CN.md)** - 版本发布的说明模板

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Inspired by [InputSourcePro](https://github.com/runjuu/InputSourcePro)
- Built with .NET 8.0 and WPF
- Uses Material Design themes for modern UI

---

**Note**: For Chinese users, please refer to [README.zh-CN.md](README.zh-CN.md) for detailed Chinese documentation.
