using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioDeviceMonitor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                string msg = $"[FATAL] {DateTime.Now:yyyy-MM-dd HH:mm:ss} UnhandledException: {e.ExceptionObject}";
                System.IO.File.AppendAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio_monitor.log"), msg + "\r\n");
                MessageBox.Show(msg, "未处理异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            Application.ThreadException += (s, e) =>
            {
                string msg = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} ThreadException: {e.Exception}";
                System.IO.File.AppendAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio_monitor.log"), msg + "\r\n");
                MessageBox.Show(msg, "线程异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
