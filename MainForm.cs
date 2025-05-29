using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using CSCore.CoreAudioAPI;

namespace AudioDeviceMonitor
{
    public partial class MainForm : Form
    {
        private CSCore.CoreAudioAPI.MMDeviceEnumerator deviceEnumerator;
        private string lastDeviceId;
        private NotifyIcon trayIcon;

        public MainForm()
        {
            InitializeComponent();

            // 隐藏主窗口，仅显示托盘图标
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;

            // 托盘图标
            trayIcon = new NotifyIcon
            {
                Icon = new Icon("audio_monitor_icon.ico"),
                Visible = true,
                Text = "音频设备监控（双击显示/右键退出）"
            };
            trayIcon.DoubleClick += (s, e) => { this.Visible = true; this.WindowState = FormWindowState.Normal; };

            // 使用 ContextMenuStrip 替代 ContextMenu
            var contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("退出", null, (s, e) =>
            {
                trayIcon.Visible = false;
                Application.Exit();
            });
            contextMenu.Items.Add(exitItem);
            trayIcon.ContextMenuStrip = contextMenu;

            // 初始化 CSCore 监听
            deviceEnumerator = new CSCore.CoreAudioAPI.MMDeviceEnumerator();
            deviceEnumerator.DefaultDeviceChanged += OnDefaultDeviceChanged;

            lastDeviceId = GetCurrentDefaultDeviceId();
        }

        // 如果没有设计器文件，手动添加空方法
        private void InitializeComponent()
        {
            // 如果你用设计器生成窗体，这里可以删除
        }

        private void OnDefaultDeviceChanged(object sender, DefaultDeviceChangedEventArgs e)
        {
            if (e.DataFlow == DataFlow.Render && e.Role == Role.Multimedia)
            {
                string curDeviceId = GetCurrentDefaultDeviceId();
                if (curDeviceId != lastDeviceId)
                {
                    lastDeviceId = curDeviceId;
                    // 检查网易云音乐是否运行
                    var proc = Process.GetProcessesByName("cloudmusic").FirstOrDefault();
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            trayIcon.Visible = false;
            deviceEnumerator.Dispose();
            base.OnFormClosed(e);
        }
    }
}