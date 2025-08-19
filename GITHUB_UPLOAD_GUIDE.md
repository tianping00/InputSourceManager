# 🚀 GitHub 上传指南

## 📋 前置条件

1. **GitHub 账户**: 确保您已有GitHub账户
2. **Git 配置**: 本地Git已正确配置
3. **项目准备**: 项目代码已提交到本地Git仓库

## 🔗 创建 GitHub 仓库

### 方法1: 通过 GitHub 网页界面

1. 登录 [GitHub](https://github.com)
2. 点击右上角 `+` 号，选择 `New repository`
3. 填写仓库信息：
   - **Repository name**: `InputSourceManager`
   - **Description**: `跨平台输入源管理工具，支持Windows和Linux系统`
   - **Visibility**: 选择 `Public` 或 `Private`
   - **不要**勾选 `Add a README file`（我们已有）
   - **不要**勾选 `Add .gitignore`（我们已有）
4. 点击 `Create repository`

### 方法2: 通过 GitHub CLI

```bash
gh repo create InputSourceManager --public --description "跨平台输入源管理工具"
```

## 🔄 推送到 GitHub

### 1. 添加远程仓库

```bash
# 替换 YOUR_USERNAME 为您的GitHub用户名
git remote add origin https://github.com/YOUR_USERNAME/InputSourceManager.git

# 或者使用SSH（推荐）
git remote add origin git@github.com:YOUR_USERNAME/InputSourceManager.git
```

### 2. 推送代码

```bash
# 推送主分支
git push -u origin master

# 或者如果使用main分支
git branch -M main
git push -u origin main
```

### 3. 创建发布标签

```bash
# 创建版本标签
git tag -a v1.0.0 -m "🎉 第一个正式版本发布"

# 推送标签
git push origin v1.0.0
```

## 🎯 自动发布配置

### GitHub Actions 工作流

项目已包含 `.github/workflows/release-windows.yml` 文件，当您推送标签时会自动：

1. 构建Windows版本
2. 创建发布包
3. 上传到GitHub Releases

### 触发自动发布

```bash
# 创建新版本标签
git tag -a v1.0.1 -m "🔧 修复和改进"

# 推送标签触发工作流
git push origin v1.0.1
```

## 📁 项目结构

```
InputSourceManager/
├── .github/workflows/          # GitHub Actions 工作流
├── InputSourceManager/          # 核心库
├── InputSourceManager.Windows/  # Windows 应用
├── InputSourceManager.Tests/    # 测试项目
├── scripts/                     # 发布脚本
├── docs/                        # 文档
└── README.md                    # 项目说明
```

## 🔧 常用 Git 命令

```bash
# 查看状态
git status

# 查看提交历史
git log --oneline

# 查看远程仓库
git remote -v

# 拉取最新代码
git pull origin master

# 查看分支
git branch -a
```

## 🚨 注意事项

1. **不要上传敏感信息**: 确保 `.gitignore` 正确配置
2. **提交信息规范**: 使用清晰的提交信息
3. **分支管理**: 建议使用 `main` 或 `master` 作为主分支
4. **版本标签**: 使用语义化版本号（如 v1.0.0）

## 🎉 完成后的效果

成功上传后，您将拥有：

- ✅ 完整的源代码仓库
- ✅ 自动构建和发布流程
- ✅ 专业的项目文档
- ✅ 可下载的Windows版本
- ✅ 完整的测试覆盖

## 📞 需要帮助？

如果遇到问题，请检查：

1. Git配置是否正确
2. GitHub仓库是否创建成功
3. 远程仓库URL是否正确
4. 网络连接是否正常

---

**祝您上传成功！** 🚀
