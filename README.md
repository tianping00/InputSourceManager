# Input Source Manager

A cross-platform input source management tool that supports Windows and Linux systems, with functionality comparable to [InputSourcePro](https://github.com/runjuu/InputSourcePro).

## 🚀 Features

### 🥷 Automatic Context-Aware Switching
- **Application Awareness**: Automatically switch input methods based on active applications
- **Website Awareness**: Switch input methods based on browsed websites (requires browser extension support)
- **Process Awareness**: Support for precise matching based on process names

### 🎯 Intelligent Rule Engine
- **Priority System**: Support for rule priority settings
- **Multi-type Rules**: Three rule types: Application, Website, and Process
- **Usage Statistics**: Track rule usage frequency and last usage time
- **Rule Management**: Add, delete, modify, enable/disable rules

### 🌐 Multi-language Support
- **10+ Languages**: English, Chinese, Japanese, Korean, German, French, etc.
- **Smart Detection**: Automatically detect system-available input methods
- **Language Mapping**: Standardized language name display

### 💾 Configuration Management
- **Auto-save**: Rules automatically saved to user configuration directory
- **Import/Export**: Support for configuration file import and export
- **Version Control**: Configuration file version management
- **Cross-device Sync**: Configuration files can be shared between different devices

### 🔍 Real-time Monitoring
- **Status Monitoring**: Real-time display of current applications and input methods
- **Browser Detection**: Detect mainstream browser process status
- **Performance Optimization**: Asynchronous operations, non-blocking main thread

## 🏗️ Project Architecture

```
InputSource/
├── InputSourceManager/                    # Core library (cross-platform)
│   ├── Models/                           # Data models
│   │   └── InputSourceRule.cs           # Input source rule model
│   ├── Services/                         # Business services
│   │   ├── BrowserDetectionService.cs   # Browser detection service
│   │   ├── RuleEngineService.cs         # Rule engine service
│   │   └── ConfigurationService.cs      # Configuration management service
│   ├── InputSourceManager.cs            # Base input source management class
│   ├── Program.cs                        # Console application entry point
│   └── InputSourceManager.csproj
├── InputSourceManager.Windows/           # Windows-specific version
│   ├── InputSourceManager.Windows.cs    # Windows API implementation
│   └── InputSourceManager.Windows.csproj
├── build.sh                              # Linux build script
├── build.bat                             # Windows build script
└── README.md                             # Project documentation
```

## 🛠️ Installation and Usage

### System Requirements
- **Cross-platform version**: .NET 8.0 or higher
- **Windows version**: Windows 10/11 + .NET 8.0 Desktop Runtime

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
