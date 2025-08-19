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

### 下载预构建版本

#### Windows 版本下载
- **框架依赖版本** (需要安装 .NET 8.0 Desktop Runtime): [InputSourceManager-Windows-fxdep.zip](https://github.com/your-username/InputSourceManager/releases/latest/download/InputSourceManager-Windows-fxdep.zip)
- **自包含版本** (无需额外安装): [InputSourceManager-Windows-selfcontained.zip](https://github.com/your-username/InputSourceManager/releases/latest/download/InputSourceManager-Windows-selfcontained.zip)

> **注意**: 自包含版本文件较大 (~100MB)，但无需安装 .NET Runtime。框架依赖版本文件较小 (~10MB)，但需要先安装 .NET 8.0 Desktop Runtime。

### 快速开始

#### 1. 克隆项目
```bash
git clone <your-repo-url>
cd InputSource
```

#### 2. 构建项目
```bash
# Linux/macOS
./build.sh

# Windows
build.bat
```

#### 3. 运行程序
```bash
# 跨平台版本
dotnet run --project InputSourceManager

# Windows 版本（仅 Windows）
dotnet run --project InputSourceManager.Windows
```

## 📋 规则配置示例

### 应用程序规则
```json
{
  "name": "代码编辑器使用英文",
  "type": "Application",
  "target": "code",
  "targetInputSource": "英语 (美国)",
  "priority": 2
}
```

### 网站规则
```json
{
  "name": "中文网站使用中文",
  "type": "Website",
  "target": "zhihu.com",
  "targetInputSource": "中文 (简体)",
  "priority": 1
}
```

### 进程规则
```json
{
  "name": "终端使用英文",
  "type": "Process",
  "target": "terminal",
  "targetInputSource": "英语 (美国)",
  "priority": 1
}
```

## 🔧 高级配置

### 自定义规则优先级
- **高优先级 (3-5)**：系统关键应用程序
- **中优先级 (1-2)**：常用应用程序
- **低优先级 (0)**：一般应用程序

### 浏览器检测支持
- Chrome/Chromium
- Microsoft Edge
- Firefox
- Opera
- Brave

### 配置文件位置
- **Windows**: `%APPDATA%\InputSourceManager\config.json`
- **Linux/macOS**: `~/.config/InputSourceManager/config.json`

## 🚧 开发计划

### 即将推出
- [ ] WPF 图形用户界面
- [ ] 系统托盘集成
- [ ] 全局快捷键支持
- [ ] 输入法指示器
- [ ] 浏览器扩展支持

### 长期规划
- [ ] 机器学习优化规则
- [ ] 云端规则同步
- [ ] 多显示器支持
- [ ] 插件系统

## 🤝 贡献指南

我们欢迎所有形式的贡献！

### 如何贡献
1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

### 开发环境设置
```bash
# 安装 .NET 8.0 SDK
# 克隆项目
# 运行测试
dotnet test

# 构建项目
dotnet build
```

### 构建 Windows 版本

#### 使用脚本 (推荐)
```bash
# PowerShell 脚本
.\scripts\publish-windows.ps1                    # 框架依赖版本
.\scripts\publish-windows.ps1 -SelfContained     # 自包含版本

# Windows 批处理文件
.\scripts\publish-windows.bat                     # 框架依赖版本
.\scripts\publish-windows.bat --self-contained    # 自包含版本
```

#### 手动构建
```bash
# 框架依赖版本
dotnet publish InputSourceManager.Windows/InputSourceManager.Windows.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=false

# 自包含版本
dotnet publish InputSourceManager.Windows/InputSourceManager.Windows.csproj -c Release -r win-x64 --self-contained true -p:PublishTrimmed=false -p:PublishSingleFile=true
```

构建完成后，可执行文件将位于 `InputSourceManager.Windows/bin/Release/net8.0-windows/win-x64/` 目录中。

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

- 本项目受到 [InputSourcePro](https://github.com/runjuu/InputSourcePro) 的启发
- 感谢所有贡献者的辛勤工作
- 特别感谢 .NET 社区的支持

## 📞 联系我们

- 项目主页：GitHub
- 问题反馈：GitHub Issues
- 功能建议：GitHub Discussions
- 贡献代码：GitHub Pull Requests

---

⭐ 如果这个项目对你有帮助，请给我们一个 Star！
