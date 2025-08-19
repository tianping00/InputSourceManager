param(
  [switch]$SelfContained
)

$ErrorActionPreference = 'Stop'

Push-Location "$PSScriptRoot\.."
try {
  $rid = 'win-x64'
  $out = if ($SelfContained) { 'publish\win-selfcontained' } else { 'publish\win-fxdep' }
  if (Test-Path $out) { Remove-Item $out -Recurse -Force }
  New-Item -ItemType Directory -Force -Path $out | Out-Null

  $sc = if ($SelfContained) { 'true' } else { 'false' }
  $single = if ($SelfContained) { 'true' } else { 'false' }
  $trim = if ($SelfContained) { 'false' } else { 'false' }

  Write-Host "目标运行时: $rid" -ForegroundColor Cyan
  Write-Host "自包含: $sc" -ForegroundColor Cyan
  Write-Host "单文件: $single" -ForegroundColor Cyan
  Write-Host ""

  Write-Host "正在还原包..." -ForegroundColor Yellow
  dotnet restore .\InputSourceManager.sln

  Write-Host "正在发布 Windows 应用程序..." -ForegroundColor Yellow
  dotnet publish .\InputSourceManager.Windows\InputSourceManager.Windows.csproj -c Release -r $rid --self-contained $sc -p:PublishSingleFile=$single -p:PublishTrimmed=$trim -o $out

  $zip = if ($SelfContained) { 'InputSourceManager-Windows-selfcontained.zip' } else { 'InputSourceManager-Windows-fxdep.zip' }
  if (Test-Path ".\$zip") { Remove-Item ".\$zip" -Force }
  
  Write-Host "正在创建压缩包..." -ForegroundColor Yellow
  Compress-Archive -Path "$out\*" -DestinationPath ".\$zip" -Force
  
  Write-Host "✅ 已成功创建 $zip" -ForegroundColor Green
  Write-Host "输出位置: $(Get-Location)\$zip" -ForegroundColor Green
}
finally {
  Pop-Location
}

