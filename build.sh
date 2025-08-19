#!/bin/bash

echo "=== Input Source Manager 构建脚本 ==="
echo "检测到操作系统: $(uname -s)"
echo

# 检查 .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "错误: 未找到 .NET SDK"
    echo "请先安装 .NET 8.0 SDK:"
    echo "  Ubuntu/Debian: sudo apt install dotnet-sdk-8.0"
    echo "  CentOS/RHEL: sudo yum install dotnet-sdk-8.0"
    echo "  macOS: brew install --cask dotnet-sdk"
    exit 1
fi

echo "找到 .NET SDK: $(dotnet --version)"
echo

# 构建跨平台版本
echo "构建跨平台版本..."
dotnet build InputSourceManager

if [ $? -eq 0 ]; then
    echo "✅ 跨平台版本构建成功！"
    echo
    echo "运行程序:"
    echo "  dotnet run --project InputSourceManager"
    echo
    echo "发布程序:"
    echo "  dotnet publish InputSourceManager -c Release"
else
    echo "❌ 跨平台版本构建失败"
    exit 1
fi

echo
echo "注意: Windows 版本需要在 Windows 系统上构建"
echo "在 Windows 上运行: dotnet build InputSourceManager.Windows"

