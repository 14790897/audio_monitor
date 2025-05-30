# Audio Device Monitor

本项目是一个基于 WinForms 的 Windows 托盘工具，用于监控系统默认音频输出设备的变化，并在检测到变化时自动控制网易云音乐（cloudmusic）播放/暂停。

## 功能简介

- 隐藏主窗口，仅显示托盘图标。
- 监听系统默认音频输出设备的切换。
- 检测到切换时，如果网易云音乐正在运行，自动发送 Ctrl+Alt+P 快捷键（控制播放/暂停）。
- 托盘图标右键菜单可退出程序，双击可显示主窗口。
- 主窗口可显示所有音频输出设备列表。
- 支持开机自启动和一键取消自启动。
- 日志功能，所有关键事件写入 `audio_monitor.log`。
- 全局异常捕获，异常信息写入日志并弹窗提示。

## 依赖环境

- .NET 8.0（或兼容的 Windows 桌面运行环境）
- [CSCore](https://www.nuget.org/packages/CSCore/)（音频设备监听）

## 构建与运行

1. 安装依赖包（如未自动还原）：
   ```powershell
   dotnet restore ./AudioDeviceMonitor.csproj
   ```
2. 编译项目（Release 模式）：
   ```powershell
   dotnet build -c Release ./AudioDeviceMonitor.csproj
   ```
3. 运行程序：
   ```powershell
   .\bin\Release\net8.0-windows\AudioDeviceMonitor.exe
   ```

### 推荐发布方式（自包含单文件 EXE， 但大小超过100M）

```powershell
# 生成自包含单文件（无需目标机安装 .NET）
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
# 运行
cd publish
./AudioDeviceMonitor.exe
```

### 仅生成单文件（需目标机有 .NET 8， 大小很小）

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish
```

## 主要文件说明

- `MainForm.cs`：主窗体及核心逻辑
- `AudioDeviceMonitor.csproj`：项目文件
- `audio_monitor_icon.ico`：程序和托盘图标
- `audio_monitor.log`：运行日志

## 注意事项

- 需在 Windows 系统下运行。
- 托盘图标和 exe 图标均已内嵌，无需手动复制 ico 文件。
- 若未用 WinForms 设计器，`InitializeComponent` 方法可为空。
- 日志文件 `audio_monitor.log` 位于 exe 同目录下。
- 若程序无法启动，请用命令行运行并查看日志内容。

## 自动发布

- 推送带 tag（如 v1.0.0）的 commit 到 GitHub，会自动构建并发布 exe 到 Release。
- 详见 `.github/workflows/release-exe.yml`。

---

如有问题欢迎反馈！

<!-- dotnet nuget list source 

dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

dotnet add package NAudio
dotnet add package CSCore

dotnet build -c Release 
dotnet publish -c Release -r win-x64 --self-contained true -o publish 
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

不打包.net

dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish
C:\git-program\audio_monitor\publish
-->