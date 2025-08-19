# Input Source Manager Windows 发布说明

## 版本 1.0.0

### 🆕 新功能
- **智能输入法管理**: 根据应用程序、网站、进程自动切换输入法
- **规则引擎**: 支持优先级控制的规则系统，支持通配符匹配
- **浏览器集成**: 支持Chrome、Edge、Firefox、Opera、Brave、Chromium
- **系统托盘**: 最小化到系统托盘，支持开机自启动
- **HTTP服务**: 本地HTTP服务接收浏览器扩展数据
- **配置热重载**: 配置文件修改后自动重新加载

### 🐛 修复
- **Windows API集成**: 修复了真正的Windows输入法切换功能
- **规则匹配逻辑**: 修复了应用程序、网站、进程规则的匹配逻辑
- **线程安全**: 添加了锁机制确保多线程环境下的数据一致性
- **浏览器检测**: 改进了浏览器进程检测和状态监控

### 🔧 改进
- **性能优化**: 智能异步操作，减少不必要的线程切换
- **缓存策略**: 浏览器检测和规则匹配使用缓存提高性能
- **内存管理**: 及时清理过期缓存，避免内存泄漏
- **错误处理**: 完善的错误处理和用户友好的错误提示

### 📦 下载
- **框架依赖版本** (需要 .NET 8.0 Desktop Runtime): [InputSourceManager-Windows-fxdep.zip](https://github.com/tianping00/InputSourceManager/releases/download/v1.0.0/InputSourceManager-Windows-fxdep.zip)
- **自包含版本** (无需额外安装): [InputSourceManager-Windows-selfcontained.zip](https://github.com/tianping00/InputSourceManager/releases/download/v1.0.0/InputSourceManager-Windows-selfcontained.zip)

### 📋 系统要求
- **操作系统**: Windows 10/11 (64位)
- **运行时**: 
  - 框架依赖版本: .NET 8.0 Desktop Runtime
  - 自包含版本: 无需额外运行时
- **内存**: 最低 512MB RAM
- **磁盘空间**: 最低 100MB 可用空间

### 🚀 安装说明
1. 下载对应版本的ZIP文件
2. 解压到任意目录
3. 运行 `InputSourceManager.Windows.exe`
4. 首次运行时会自动创建配置文件

### 🔍 已知问题
- 某些杀毒软件可能会误报，这是误报，可以添加到白名单
- 在Windows 7/8上可能无法正常运行（推荐Windows 10/11）

### 📝 更新日志
- **2025-01-18**: 核心功能修复完成，测试通过
- **2025-01-18**: 添加了完整的测试覆盖
- **2025-01-18**: 优化了性能和内存管理
- **2025-01-18**: 改进了错误处理和用户体验

### 🧪 测试覆盖
- **单元测试**: 45个测试用例
- **测试框架**: xUnit
- **覆盖率**: 核心服务100%测试覆盖
- **测试内容**: 规则引擎、配置服务、浏览器检测、输入源管理器

---
**注意**: 首次运行时会检查 .NET 8.0 Desktop Runtime，如果未安装会提示下载地址。

**技术支持**: 如遇问题，请在GitHub Issues中报告。
