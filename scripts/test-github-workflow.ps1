# 测试GitHub Actions工作流构建命令
# 这个脚本模拟GitHub Actions的构建步骤，用于本地测试

param(
    [switch]$Clean,
    [switch]$Verbose
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

Push-Location "$PSScriptRoot\.."
try {
    Write-Host "=== 测试GitHub Actions工作流构建 ===" -ForegroundColor Cyan
    Write-Host "当前目录: $(Get-Location)" -ForegroundColor Yellow
    Write-Host ""

    # 清理输出目录
    if ($Clean -or (Test-Path "out")) {
        Write-Host "清理输出目录..." -ForegroundColor Yellow
        if (Test-Path "out") { Remove-Item "out" -Recurse -Force }
    }

    # 创建输出目录
    New-Item -ItemType Directory -Force -Path "out" | Out-Null
    New-Item -ItemType Directory -Force -Path "out/win-fxdep" | Out-Null
    New-Item -ItemType Directory -Force -Path "out/win-selfcontained" | Out-Null

    # 检查.NET SDK
    Write-Host "检查.NET SDK..." -ForegroundColor Yellow
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK版本: $dotnetVersion" -ForegroundColor Green

    # 还原包
    Write-Host "还原包..." -ForegroundColor Yellow
    dotnet restore InputSourceManager.sln
    if ($LASTEXITCODE -ne 0) {
        throw "包还原失败"
    }
    Write-Host "✅ 包还原成功" -ForegroundColor Green

    # 发布框架依赖版本
    Write-Host "发布框架依赖版本..." -ForegroundColor Yellow
    dotnet publish InputSourceManager.Windows/InputSourceManager.Windows.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:PublishTrimmed=false -o out/win-fxdep
    if ($LASTEXITCODE -ne 0) {
        throw "框架依赖版本发布失败"
    }
    Write-Host "✅ 框架依赖版本发布成功" -ForegroundColor Green

    # 发布自包含版本
    Write-Host "发布自包含版本..." -ForegroundColor Yellow
    dotnet publish InputSourceManager.Windows/InputSourceManager.Windows.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -o out/win-selfcontained
    if ($LASTEXITCODE -ne 0) {
        throw "自包含版本发布失败"
    }
    Write-Host "✅ 自包含版本发布成功" -ForegroundColor Green

    # 验证exe文件生成
    Write-Host "验证exe文件生成..." -ForegroundColor Yellow
    $fxdepExe = Get-ChildItem "out/win-fxdep/*.exe" -ErrorAction SilentlyContinue
    if ($fxdepExe) {
        Write-Host "✅ 框架依赖版本exe文件: $($fxdepExe.Name)" -ForegroundColor Green
        Write-Host "   文件大小: $([math]::Round($fxdepExe.Length / 1MB, 2)) MB" -ForegroundColor Gray
    } else {
        throw "框架依赖版本未生成exe文件"
    }

    $selfExe = Get-ChildItem "out/win-selfcontained/*.exe" -ErrorAction SilentlyContinue
    if ($selfExe) {
        Write-Host "✅ 自包含版本exe文件: $($selfExe.Name)" -ForegroundColor Green
        Write-Host "   文件大小: $([math]::Round($selfExe.Length / 1MB, 2)) MB" -ForegroundColor Gray
    } else {
        throw "自包含版本未生成exe文件"
    }

    Write-Host "✅ 两个版本都成功生成了exe文件" -ForegroundColor Green

    # 显示输出目录内容
    if ($Verbose) {
        Write-Host "`n输出目录内容:" -ForegroundColor Cyan
        Write-Host "框架依赖版本:" -ForegroundColor Yellow
        Get-ChildItem "out/win-fxdep" | ForEach-Object { Write-Host "  $($_.Name)" -ForegroundColor Gray }
        
        Write-Host "自包含版本:" -ForegroundColor Yellow
        Get-ChildItem "out/win-selfcontained" | ForEach-Object { Write-Host "  $($_.Name)" -ForegroundColor Gray }
    }

    # 创建压缩包
    Write-Host "`n创建压缩包..." -ForegroundColor Yellow
    Compress-Archive -Path "out/win-fxdep/*" -DestinationPath "out/InputSourceManager-Windows-fxdep.zip" -Force
    Compress-Archive -Path "out/win-selfcontained/*" -DestinationPath "out/InputSourceManager-Windows-selfcontained.zip" -Force

    $fxdepZip = Get-Item "out/InputSourceManager-Windows-fxdep.zip"
    $selfZip = Get-Item "out/InputSourceManager-Windows-selfcontained.zip"

    Write-Host "✅ 已创建发布包:" -ForegroundColor Green
    Write-Host "  - InputSourceManager-Windows-fxdep.zip ($([math]::Round($fxdepZip.Length / 1MB, 2)) MB)" -ForegroundColor Gray
    Write-Host "  - InputSourceManager-Windows-selfcontained.zip ($([math]::Round($selfZip.Length / 1MB, 2)) MB)" -ForegroundColor Gray

    Write-Host "`n🎉 测试完成！所有步骤都成功执行" -ForegroundColor Green
    Write-Host "输出位置: $(Get-Location)\out" -ForegroundColor Cyan

} catch {
    Write-Host "❌ 测试失败: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}
