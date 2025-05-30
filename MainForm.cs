using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CSCore.CoreAudioAPI;
using Microsoft.Win32;
using System.Reflection;

namespace AudioDeviceMonitor
{
    public partial class MainForm : Form
    {
        private CSCore.CoreAudioAPI.MMDeviceEnumerator deviceEnumerator;
        private string lastDeviceId;
        private NotifyIcon trayIcon;

        // 设备列表控件
        private ListBox deviceListBox;

        public MainForm()
        {
            InitializeComponent();

            Log("程序启动");

            // 设置开机自启动
            SetAutoStart(true);

            // 隐藏主窗口，仅显示托盘图标
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;

            // 托盘图标
            Icon trayIconImage;
            var asm = Assembly.GetExecutingAssembly();
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (var name in names)
                Log("[DEBUG] 资源名: " + name);
            using (var stream = asm.GetManifestResourceStream("AudioDeviceMonitor.audio_monitor_icon.ico"))
            {
                trayIconImage = new Icon(stream);
            }
            trayIcon = new NotifyIcon
            {
                Icon = trayIconImage,
                Visible = true,
                Text = "音频设备监控（双击显示/右键退出）"
            };
            trayIcon.DoubleClick += (s, e) => { this.Visible = true; this.WindowState = FormWindowState.Normal; RefreshDeviceList(); };

            // 使用 ContextMenuStrip 替代 ContextMenu
            var contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("退出", null, (s, e) =>
            {
                trayIcon.Visible = false;
                Application.Exit();
            });
            var disableAutoStartItem = new ToolStripMenuItem("取消开机自启动", null, (s, e) =>
            {
                SetAutoStart(false);
                MessageBox.Show("已取消开机自启动！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
            contextMenu.Items.Add(disableAutoStartItem);
            contextMenu.Items.Add(exitItem);
            trayIcon.ContextMenuStrip = contextMenu;

            // 初始化 CSCore 监听
            deviceEnumerator = new CSCore.CoreAudioAPI.MMDeviceEnumerator();
            deviceEnumerator.DefaultDeviceChanged += OnDefaultDeviceChanged;

            lastDeviceId = GetCurrentDefaultDeviceId();

            // 初始化时刷新设备列表
            RefreshDeviceList();
        }

        // 如果没有设计器文件，手动添加空方法
        private void InitializeComponent()
        {
            this.deviceListBox = new ListBox();
            this.deviceListBox.Dock = DockStyle.Fill;
            this.deviceListBox.Font = new System.Drawing.Font("Consolas", 12F);
            this.Controls.Add(this.deviceListBox);
            this.Text = "音频设备监控 - 设备列表";
            this.ClientSize = new System.Drawing.Size(400, 300);
        }

        private void Log(string message)
        {
            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio_monitor.log");
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\r\n";
            System.IO.File.AppendAllText(logPath, logEntry);
        }

        private void OnDefaultDeviceChanged(object sender, DefaultDeviceChangedEventArgs e)
        {
            Log($"检测到音频设备切换: {e.DataFlow}, {e.Role}");
            if (e.DataFlow == DataFlow.Render && e.Role == Role.Multimedia)
            {
                string curDeviceId = GetCurrentDefaultDeviceId();
                Log($"当前设备ID: {curDeviceId}, 上次设备ID: {lastDeviceId}");
                if (curDeviceId != lastDeviceId)
                {
                    lastDeviceId = curDeviceId;
                    // 检查网易云音乐是否运行
                    var proc = Process.GetProcessesByName("cloudmusic").FirstOrDefault();
                    Log(proc != null ? "检测到网易云音乐进程，发送Ctrl+Alt+P" : "未检测到网易云音乐进程");
                    if (proc != null)
                    {
                        SendCtrlAltP();
                    }
                }
            }
        }

        private string GetCurrentDefaultDeviceId()
        {
            using (var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
            {
                return device.DeviceID;
            }
        }

        private void SendCtrlAltP()
        {
            // 发送Ctrl+Alt+P
            SendKeys.SendWait("^%p");
        }

        // 设置开机自启动
        private void SetAutoStart(bool enable)
        {
            string appName = "AudioDeviceMonitor";
            string exePath = Application.ExecutablePath;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (enable)
                {
                    key.SetValue(appName, '"' + exePath + '"');
                }
                else
                {
                    key.DeleteValue(appName, false);
                }
            }
        }

        private void RefreshDeviceList()
        {
            deviceListBox.Items.Clear();
            using (var devices = deviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
            {
                foreach (var dev in devices)
                {
                    deviceListBox.Items.Add($"{dev.FriendlyName} ({dev.DeviceID})");
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 如果是用户点击关闭按钮（X），则隐藏到托盘而不是退出
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
                this.WindowState = FormWindowState.Minimized;
                return;
            }
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            trayIcon.Visible = false;
            deviceEnumerator.Dispose();
            base.OnFormClosed(e);
        }
    }
}