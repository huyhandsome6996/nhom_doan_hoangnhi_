using System;
using System.Windows.Forms;
using System.Drawing;
using QuanLyTraSua.DAL;
using QuanLyTraSua.GUI;

namespace QuanLyTraSua
{
    internal static class Program
    {
        private static NotifyIcon? _trayIcon;
        private static MainForm? _mainForm;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Khởi tạo database
            DatabaseHelper.KhoiTao();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Tạo System Tray Icon
            KhoiTaoSystemTray();

            _mainForm = new MainForm();
            _mainForm.Show();

            Application.Run();
        }

        private static void KhoiTaoSystemTray()
        {
            var contextMenu = new ContextMenuStrip();

            contextMenu.Items.Add("🧋 Mở ứng dụng", null, (s, e) => HienThiForm());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("❌ Thoát", null, (s, e) => ThoatUngDung());

            _trayIcon = new NotifyIcon
            {
                Text = "Quán Trà Sữa",
                Icon = SystemIcons.Application,
                ContextMenuStrip = contextMenu,
                Visible = true
            };

            // Double-click vào tray icon => hiện cửa sổ
            _trayIcon.DoubleClick += (s, e) => HienThiForm();

            // Balloon tip khi khởi động
            _trayIcon.ShowBalloonTip(2000,
                "Quán Trà Sữa",
                "Ứng dụng đang chạy ngầm. Double-click để mở.",
                ToolTipIcon.Info);
        }

        private static void HienThiForm()
        {
            if (_mainForm == null) return;

            _mainForm.Show();
            _mainForm.WindowState = FormWindowState.Normal;
            _mainForm.BringToFront();
            _mainForm.Activate();
        }

        private static void ThoatUngDung()
        {
            _trayIcon!.Visible = false;
            _trayIcon.Dispose();
            Application.Exit();
        }
    }
}