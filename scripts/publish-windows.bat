@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

cd /d "%~dp0\.."

if "%1"=="--self-contained" (
    echo 正在构建自包含 Windows 版本...
    set OUT_DIR=publish\win-selfcontained
    set ZIP_NAME=InputSourceManager-Windows-selfcontained.zip
    set SELF_CONTAINED=true
    set SINGLE_FILE=true
) else (
    echo 正在构建框架依赖 Windows 版本...
    set OUT_DIR=publish\win-fxdep
    set ZIP_NAME=InputSourceManager-Windows-fxdep.zip
    set SELF_CONTAINED=false
    set SINGLE_FILE=false
)

echo 目标运行时: win-x64
echo 自包含: %SELF_CONTAINED%
echo 单文件: %SINGLE_FILE%
echo.

if exist "%OUT_DIR%" rmdir /s /q "%OUT_DIR%"
mkdir "%OUT_DIR%"

echo 正在还原包...
dotnet restore InputSourceManager.sln

echo 正在发布 Windows 应用程序...
dotnet publish InputSourceManager.Windows\InputSourceManager.Windows.csproj -c Release -r win-x64 --self-contained %SELF_CONTAINED% -p:PublishSingleFile=%SINGLE_FILE% -p:PublishTrimmed=false -o "%OUT_DIR%"

if exist "%ZIP_NAME%" del "%ZIP_NAME%"

echo 正在创建压缩包...
powershell -Command "Compress-Archive -Path '%OUT_DIR%\*' -DestinationPath '%ZIP_NAME%' -Force"

if exist "%ZIP_NAME%" (
    echo ✅ 已成功创建 %ZIP_NAME%
    echo 输出位置: %CD%\%ZIP_NAME%
) else (
    echo ❌ 创建压缩包失败
    exit /b 1
)

echo.
echo 🎉 构建完成！
pause
