# Audio Device Monitor

本项目是一个基于 WinForms 的 Windows 托盘工具，用于监控系统默认音频输出设备的变化，并在检测到变化时自动控制网易云音乐（cloudmusic）播放/暂停。

## 功能简介
- 隐藏主窗口，仅显示托盘图标。
- 监听系统默认音频输出设备的切换。
- 检测到切换时，如果网易云音乐正在运行，自动发送 Ctrl+Alt+P 快捷键（控制播放/暂停）。
- 托盘图标右键菜单可退出程序，双击可显示主窗口。

## 依赖环境
- .NET 8.0（或兼容的 Windows 桌面运行环境）
- [CSCore](https://www.nuget.org/packages/CSCore/)（音频设备监听）

## 构建与运行
1. 安装依赖包（如未自动还原）：
   ```powershell
   dotnet restore
   ```
2. 编译项目（Release 模式）：
   ```powershell
   dotnet build -c Release
   ```
3. 运行程序：
   ```powershell
   .\bin\Release\net8.0-windows\audio_monitor.exe
   ```

## 主要文件说明
- `MainForm.cs`：主窗体及核心逻辑
- `audio_monitor.csproj`：项目文件

## 注意事项
- 需在 Windows 系统下运行。
- 若需自定义托盘图标，请将 `.ico` 文件放在程序目录，并在代码中设置。
- 若未用 WinForms 设计器，`InitializeComponent` 方法可为空。

---

如有问题欢迎反馈！

<!-- dotnet nuget list source 

dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

dotnet add package NAudio
dotnet add package CSCore

dotnet build -c Release 
dotnet publish -c Release -r win-x64 --self-contained true -o publish 
C:\git-program\audio_monitor\publish
-->