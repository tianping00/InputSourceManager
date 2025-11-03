# 部署指南 - Windows版本推送与构建

**日期**: 2025-01-03  
**版本**: v1.0.7  
**状态**: 准备推送

---

## 📦 准备推送的内容

我们已经完成了5个重要的提交，准备推送到GitHub并触发自动构建：

```
53d96aa - docs: 添加Windows版本完成总结文档
9ec78af - feat: 完善Windows版本UI功能
82aaba9 - docs: 添加与InputSourcePro的功能对比文档
cc6a432 - chore: 触发自动构建
0046b7b - fix: 修复关键bug - 输入法切换、非交互崩溃和网站规则匹配
```

---

## 🚀 推送方法

### 方法1: 使用个人访问令牌 (PAT) - 最简单 ⭐推荐

#### 步骤1: 生成访问令牌
1. 访问: https://github.com/settings/tokens
2. 点击: **"Generate new token"** → **"Generate new token (classic)"**
3. 设置:
   - **Note**: InputSourceManager Deploy Token
   - **Expiration**: 根据需要选择（建议90天或永不过期）
   - **Scopes**: 勾选 **`repo`** (完整仓库访问权限)
4. 点击: **"Generate token"**
5. **重要**: 立即复制生成的token（只显示一次！）

#### 步骤2: 推送代码
```bash
# 在项目根目录执行
cd /home/gh/InputSource
git push origin master
```

#### 步骤3: 输入凭据
```
Username for 'https://github.com': tianping00
Password for 'https://tianping00@github.com': [粘贴刚才生成的token]
```

**完成后**: GitHub Actions 会自动开始构建！

---

### 方法2: 使用SSH密钥 - 长期解决方案

#### 步骤1: 生成SSH密钥
```bash
# 生成新的SSH密钥
ssh-keygen -t ed25519 -C "your_email@example.com"

# 按提示操作:
# - 文件位置: 直接回车（默认~/.ssh/id_ed25519）
# - 密码: 可以设置密码或直接回车
```

#### 步骤2: 复制公钥
```bash
# 显示公钥内容
cat ~/.ssh/id_ed25519.pub
```

**复制输出的完整内容**（以 `ssh-ed25519` 开头）

#### 步骤3: 添加到GitHub
1. 访问: https://github.com/settings/ssh/new
2. 填写:
   - **Title**: InputSourceManager (或其他描述性名称)
   - **Key**: 粘贴刚才复制的公钥内容
3. 点击: **"Add SSH key"**

#### 步骤4: 修改远程URL并推送
```bash
# 切换到SSH URL
git remote set-url origin git@github.com:tianping00/InputSourceManager.git

# 测试SSH连接
ssh -T git@github.com
# 应该看到: Hi tianping00! You've successfully authenticated...

# 推送代码
git push origin master
```

**完成后**: GitHub Actions 会自动开始构建！

---

## 📊 GitHub Actions 自动构建流程

### 触发条件
- ✅ Push 到 `master` 或 `main` 分支
- ✅ 推送以 `v` 开头的标签（如 `v1.0.7`）

### 构建流程
```
1. 检出代码 (checkout@v4)
2. 设置.NET 8 SDK (setup-dotnet@v4)
3. 还原NuGet包
4. 发布框架依赖版本 (win-x64, fxdep)
5. 发布自包含版本 (win-x64, selfcontained)
6. 验证exe文件生成
7. 打包为zip文件
8. 上传构建产物 (Artifacts)
9. (可选) 创建GitHub Release
```

### 输出产物
构建完成后，您可以从Actions页面下载：
- `InputSourceManager-Windows-fxdep.zip` (~10MB)
- `InputSourceManager-Windows-selfcontained.zip` (~100MB)

---

## 🎯 查看构建状态

### 实时查看
1. 访问: https://github.com/tianping00/InputSourceManager/actions
2. 点击最新的 workflow run
3. 实时查看构建日志

### 下载产物
1. 在Actions页面找到完成的构建
2. 滚动到底部，点击 **"Artifacts"** 区域
3. 下载 `windows-builds.zip`
4. 解压后包含两个zip文件

---

## 🏷️ 创建正式发布 (可选)

### 创建标签并推送
```bash
# 创建带注释的标签
git tag -a v1.0.7 -m "Windows版本完整实现

主要更新:
- 实现完整的UI功能（规则管理、设置页面）
- 修复输入法切换逻辑
- 修复非交互环境崩溃问题
- 新增网站规则专用执行方法
- 完善开机自启动功能
- 添加详细文档

功能完成度: 100%
"

# 推送标签（会触发Release自动创建）
git push origin v1.0.7
```

### Release自动创建
当推送标签后，GitHub Actions会自动：
- 创建GitHub Release
- 附加构建的zip文件
- 生成发布说明

访问: https://github.com/tianping00/InputSourceManager/releases

---

## ✅ 验证清单

推送前确认:
- [ ] 所有代码已提交到本地仓库
- [ ] 无未保存的更改
- [ ] 核心项目编译成功
- [ ] 无Linter错误
- [ ] README中的触发信息已更新

推送后验证:
- [ ] GitHub Actions显示正在构建
- [ ] 构建成功完成（绿色✓）
- [ ] Artifacts可以下载
- [ ] 生成的zip文件大小合理
- [ ] (如果创建标签) Release已创建

---

## 🐛 常见问题

### Q: 推送提示 "Authentication failed"
**A**: 检查token权限是否正确设置了 `repo` 作用域

### Q: GitHub Actions失败
**A**: 
1. 查看详细错误日志
2. 检查项目文件是否有编译错误
3. 确认 .csproj 文件格式正确

### Q: 无法下载Artifacts
**A**: 
1. 构建必须完全成功
2. 等待构建完成（黄色⏳ → 绿色✓）
3. Artifacts在构建完成后才可下载

### Q: Release没有自动创建
**A**: 
1. 标签必须以 `v` 开头
2. 构建job必须成功
3. 检查 workflow 配置中的条件

---

## 📚 相关链接

- **GitHub仓库**: https://github.com/tianping00/InputSourceManager
- **Actions页面**: https://github.com/tianping00/InputSourceManager/actions
- **Releases页面**: https://github.com/tianping00/InputSourceManager/releases
- **设置Tokens**: https://github.com/settings/tokens
- **设置SSH Keys**: https://github.com/settings/ssh

---

## 📞 需要帮助?

如果遇到问题，请检查：
1. `.github/workflows/release-windows.yml` 是否正确
2. GitHub Actions 的详细错误信息
3. 项目是否能本地编译成功

---

**祝部署顺利！** 🚀

---

*最后更新: 2025-01-03*
