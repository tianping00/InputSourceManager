# URL 接收服务使用说明

## 概述

Input Source Manager 现在支持通过浏览器扩展实时接收当前标签页 URL，并自动根据网站规则切换输入法。这实现了与 [InputSourcePro](https://github.com/runjuu/InputSourcePro) 相同的网站感知功能。

## 🚀 功能特性

### 实时 URL 检测
- 监听本地 HTTP 端口 `43219`
- 支持 JSON 格式的 URL 数据
- 自动解析域名并匹配规则
- 实时触发输入法切换

### 智能规则匹配
- 支持网站域名规则
- 优先级系统
- 自动启用/禁用
- 使用统计跟踪

## 🔧 技术实现

### 本地服务
- **端口**: `http://127.0.0.1:43219`
- **端点**: `/tab`
- **方法**: `POST`
- **数据格式**: JSON

### 数据格式
```json
{
  "url": "https://example.com/page"
}
```

### 响应格式
```json
{
  "success": true,
  "domain": "example.com"
}
```

## 📱 浏览器扩展集成

### Chrome/Edge 扩展示例
```javascript
// 在标签页切换时发送 URL
chrome.tabs.onActivated.addListener(async (activeInfo) => {
  const tab = await chrome.tabs.get(activeInfo.tabId);
  if (tab.url && tab.url.startsWith('http')) {
    await fetch('http://127.0.0.1:43219/tab', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        url: tab.url
      })
    });
  }
});
```

### Firefox 扩展示例
```javascript
// 监听标签页切换
browser.tabs.onActivated.addListener(async (activeInfo) => {
  const tab = await browser.tabs.get(activeInfo.tabId);
  if (tab.url && tab.url.startsWith('http')) {
    await fetch('http://127.0.0.1:43219/tab', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        url: tab.url
      })
    });
  }
});
```

## 🧪 测试方法

### 使用 Python 测试脚本
1. 确保 Input Source Manager 正在运行
2. 运行测试脚本：
   ```bash
   python3 test-url-receiver.py
   ```

### 手动测试
使用 curl 命令：
```bash
curl -X POST http://127.0.0.1:43219/tab \
  -H "Content-Type: application/json" \
  -d '{"url":"https://zhihu.com/question/123"}'
```

## ⚙️ 配置说明

### 规则配置
在 WPF 界面的"规则配置"页面中：

1. **添加网站规则**：
   - 规则类型：选择 "Website"
   - 目标：输入域名（如 `zhihu.com`）
   - 目标输入法：选择要切换到的输入法
   - 优先级：设置规则优先级

2. **示例规则**：
   - 中文网站 → 中文输入法
   - 英文网站 → 英文输入法
   - 代码网站 → 英文输入法

### 自动切换设置
- 在"状态"页面勾选"启用自动切换"
- 程序会自动根据规则执行输入法切换

## 🔍 故障排除

### 常见问题

1. **连接失败**
   - 确保 Input Source Manager 正在运行
   - 检查防火墙设置
   - 确认端口 43219 未被占用

2. **规则不生效**
   - 检查规则是否正确配置
   - 确认规则已启用
   - 查看状态栏的错误信息

3. **输入法切换失败**
   - 确认目标输入法已安装
   - 检查 Windows 输入法设置
   - 查看程序日志

### 调试信息
- 状态栏显示当前操作状态
- 控制台输出详细的错误信息
- 规则执行日志记录

## 📋 开发计划

### 即将推出
- [ ] 浏览器扩展官方版本
- [ ] 更智能的域名匹配
- [ ] 正则表达式支持
- [ ] 批量规则导入

### 长期规划
- [ ] 云端规则同步
- [ ] 机器学习优化
- [ ] 多浏览器支持
- [ ] 插件系统

## 🤝 贡献指南

欢迎为浏览器扩展开发贡献力量！

### 扩展开发
1. 支持主流浏览器（Chrome、Firefox、Edge）
2. 轻量级设计，不影响浏览器性能
3. 用户友好的配置界面
4. 完善的错误处理

### 提交方式
- 创建 Pull Request
- 提供详细的测试说明
- 包含示例配置

## 📞 技术支持

- 问题反馈：GitHub Issues
- 功能建议：GitHub Discussions
- 开发讨论：GitHub Discussions

---

⭐ 如果这个功能对你有帮助，请给我们一个 Star！

