@echo off
echo === Input Source Manager 构建脚本 ===
echo.

REM 检查 .NET SDK
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo 错误: 未找到 .NET SDK
    echo 请先安装 .NET 8.0 SDK
    echo 下载地址: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo 找到 .NET SDK: 
dotnet --version
echo.

REM 构建跨平台版本
echo 构建跨平台版本...
dotnet build InputSourceManager
if %errorlevel% neq 0 (
    echo ❌ 跨平台版本构建失败
    pause
    exit /b 1
)

echo ✅ 跨平台版本构建成功！
echo.

REM 构建 Windows 版本
echo 构建 Windows 版本...
dotnet build InputSourceManager.Windows
if %errorlevel% neq 0 (
    echo ❌ Windows 版本构建失败
    echo 这可能是正常的，请检查是否安装了 Windows Desktop SDK
    echo.
) else (
    echo ✅ Windows 版本构建成功！
    echo.
)

echo 运行程序:
echo   跨平台版本: dotnet run --project InputSourceManager
echo   Windows版本: dotnet run --project InputSourceManager.Windows
echo.
echo 发布程序:
echo   跨平台版本: dotnet publish InputSourceManager -c Release
echo   Windows版本: dotnet publish InputSourceManager.Windows -c Release
echo.
pause
