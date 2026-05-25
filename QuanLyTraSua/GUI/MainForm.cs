using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using QuanLyTraSua.DAL;
using QuanLyTraSua.Entities;

namespace QuanLyTraSua.GUI
{
    public class MainForm : Form
    {
        private WebView2 _webView = null!;
        private NguoiDung? _nguoiDungHienTai;

        private readonly NguoiDungDAL _nguoiDungDAL = new();
        private readonly SanPhamDAL _sanPhamDAL = new();
        private readonly DonHangDAL _donHangDAL = new();
        private readonly ChiTietDonHangDAL _chiTietDAL = new();

        public MainForm()
        {
            InitializeComponent();
            InitWebView();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Quán Trà Sữa - Take Away";
            this.Width = 1280;
            this.Height = 800;
            this.MinimumSize = new System.Drawing.Size(1024, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);

            // Icon ứng dụng
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images", "icon.ico");
                if (File.Exists(iconPath))
                    this.Icon = new System.Drawing.Icon(iconPath);
            }
            catch { }

            _webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(_webView);
        }

        private async void InitWebView()
        {
            string userDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "QuanLyTraSua", "WebView2");

            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await _webView.EnsureCoreWebView2Async(env);

            _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;

            // Đăng ký handler nhận message từ JS
            _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            // Thiết lập virtual host để serve file HTML nội bộ
            string wwwrootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "app.local", wwwrootPath, CoreWebView2HostResourceAccessKind.Allow);

            // Load trang đăng nhập
            _webView.CoreWebView2.Navigate("https://app.local/login.html");
        }

        private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string msgJson = e.TryGetWebMessageAsString();
                var msg = JsonSerializer.Deserialize<WebMessage>(msgJson);
                if (msg == null) return;

                string response = msg.Action switch
                {
                    "dangNhap" => XuLyDangNhap(msg.Data),
                    "dangXuat" => XuLyDangXuat(),
                    "laySanPham" => XuLyLaySanPham(msg.Data),
                    "themSanPham" => XuLyThemSanPham(msg.Data),
                    "suaSanPham" => XuLySuaSanPham(msg.Data),
                    "xoaSanPham" => XuLyXoaSanPham(msg.Data),
                    "datMon" => XuLyDatMon(msg.Data),
                    "layDonHang" => XuLyLayDonHang(msg.Data),
                    "capNhatTrangThai" => XuLyCapNhatTrangThai(msg.Data),
                    "layNguoiDung" => XuLyLayNguoiDung(),
                    "themNguoiDung" => XuLyThemNguoiDung(msg.Data),
                    "suaNguoiDung" => XuLySuaNguoiDung(msg.Data),
                    "xoaNguoiDung" => XuLyXoaNguoiDung(msg.Data),
                    _ => JsonSerializer.Serialize(new { ok = false, loi = "Hành động không xác định" })
                };

                _webView.CoreWebView2.PostWebMessageAsString(response);
            }
            catch (Exception ex)
            {
                var errResponse = JsonSerializer.Serialize(new { ok = false, loi = ex.Message });
                _webView.CoreWebView2.PostWebMessageAsString(errResponse);
            }
        }

        // ===== XỬ LÝ ĐĂNG NHẬP =====
        private string XuLyDangNhap(JsonElement data)
        {
            string tenDN = data.GetProperty("tenDangNhap").GetString() ?? "";
            string matKhau = data.GetProperty("matKhau").GetString() ?? "";
            var nd = _nguoiDungDAL.DangNhap(tenDN, matKhau);
            if (nd != null)
            {
                _nguoiDungHienTai = nd;
                return JsonSerializer.Serialize(new
                {
                    ok = true,
                    action = "dangNhap",
                    nguoiDung = new { nd.Id, nd.HoTen, nd.VaiTro, nd.TenDangNhap }
                });
            }
            return JsonSerializer.Serialize(new { ok = false, action = "dangNhap", loi = "Tên đăng nhập hoặc mật khẩu sai!" });
        }

        private string XuLyDangXuat()
        {
            _nguoiDungHienTai = null;
            return JsonSerializer.Serialize(new { ok = true, action = "dangXuat" });
        }

        // ===== XỬ LÝ SẢN PHẨM =====
        private string XuLyLaySanPham(JsonElement data)
        {
            string loai = "";
            try { loai = data.GetProperty("loai").GetString() ?? ""; } catch { }

            var danhSach = string.IsNullOrEmpty(loai)
                ? _sanPhamDAL.LayTatCa()
                : _sanPhamDAL.LayTheoLoai(loai);

            var items = danhSach.ConvertAll(sp => new
            {
                sp.Id, sp.TenSanPham, sp.GiaCoBan, sp.Loai, sp.HinhAnh, sp.DangBan
            });
            return JsonSerializer.Serialize(new { ok = true, action = "laySanPham", data = items });
        }

        private string XuLyThemSanPham(JsonElement data)
        {
            try
            {
                string loai = data.GetProperty("loai").GetString()!;
                var sp = SanPhamFactory.TaoSanPham(loai, 0,
                    data.GetProperty("tenSanPham").GetString()!,
                    data.GetProperty("giaCoBan").GetDecimal(),
                    data.GetProperty("hinhAnh").GetString() ?? "",
                    true);
                bool ok = _sanPhamDAL.Them(sp);
                return JsonSerializer.Serialize(new { ok, action = "themSanPham" });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { ok = false, action = "themSanPham", loi = ex.Message });
            }
        }

        private string XuLySuaSanPham(JsonElement data)
        {
            try
            {
                string loai = data.GetProperty("loai").GetString()!;
                var sp = SanPhamFactory.TaoSanPham(loai,
                    data.GetProperty("id").GetInt32(),
                    data.GetProperty("tenSanPham").GetString()!,
                    data.GetProperty("giaCoBan").GetDecimal(),
                    data.GetProperty("hinhAnh").GetString() ?? "",
                    data.GetProperty("dangBan").GetBoolean());
                bool ok = _sanPhamDAL.Sua(sp);
                return JsonSerializer.Serialize(new { ok, action = "suaSanPham" });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { ok = false, action = "suaSanPham", loi = ex.Message });
            }
        }

        private string XuLyXoaSanPham(JsonElement data)
        {
            int id = data.GetProperty("id").GetInt32();
            bool ok = _sanPhamDAL.Xoa(id);
            return JsonSerializer.Serialize(new { ok, action = "xoaSanPham" });
        }

        // ===== XỬ LÝ ĐẶT MÓN =====
        private string XuLyDatMon(JsonElement data)
        {
            if (_nguoiDungHienTai == null)
                return JsonSerializer.Serialize(new { ok = false, action = "datMon", loi = "Chưa đăng nhập!" });
            try
            {
                var donHang = new DonHang(_nguoiDungHienTai.Id,
                    data.GetProperty("ghiChu").GetString() ?? "");

                var items = data.GetProperty("items").EnumerateArray();
                foreach (var item in items)
                {
                    int spId = item.GetProperty("sanPhamId").GetInt32();
                    int sl = item.GetProperty("soLuong").GetInt32();
                    string tuyChon = item.GetProperty("tuyChonThem").GetString() ?? "";

                    // ĐA HÌNH: TinhTien() trả về giá đúng theo loại sản phẩm
                    var sp = _sanPhamDAL.LayTheoId(spId);
                    if (sp == null) continue;
                    decimal donGia = sp.TinhTien(tuyChon);

                    var ct = new ChiTietDonHang(0, spId, sl, donGia, tuyChon);
                    donHang.DanhSachChiTiet.Add(ct);
                }

                donHang.TinhLaiTongTien();
                int newId = _donHangDAL.Them(donHang);
                return JsonSerializer.Serialize(new { ok = newId > 0, action = "datMon", donHangId = newId });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { ok = false, action = "datMon", loi = ex.Message });
            }
        }

        // ===== XỬ LÝ ĐƠN HÀNG =====
        private string XuLyLayDonHang(JsonElement data)
        {
            List<DonHang> danhSach;
            try
            {
                string filter = data.GetProperty("filter").GetString() ?? "";
                if (filter == "cuaToi" && _nguoiDungHienTai != null)
                    danhSach = _donHangDAL.LayTheoKhachHang(_nguoiDungHienTai.Id);
                else if (!string.IsNullOrEmpty(filter) && filter != "cuaToi")
                    danhSach = _donHangDAL.LayTheoTrangThai(filter);
                else
                    danhSach = _donHangDAL.LayTatCa();
            }
            catch { danhSach = _donHangDAL.LayTatCa(); }

            var items = danhSach.ConvertAll(dh => new
            {
                dh.Id, dh.KhachHangId,
                ThoiGianDat = dh.ThoiGianDat.ToString("dd/MM/yyyy HH:mm"),
                dh.TongTien, dh.TrangThai, dh.GhiChu,
                TenKhachHang = dh.KhachHang?.HoTen ?? "N/A"
            });
            return JsonSerializer.Serialize(new { ok = true, action = "layDonHang", data = items });
        }

        private string XuLyCapNhatTrangThai(JsonElement data)
        {
            int id = data.GetProperty("id").GetInt32();
            string trangThai = data.GetProperty("trangThai").GetString()!;
            bool ok = _donHangDAL.CapNhatTrangThai(id, trangThai);
            return JsonSerializer.Serialize(new { ok, action = "capNhatTrangThai" });
        }

        // ===== XỬ LÝ NGƯỜI DÙNG =====
        private string XuLyLayNguoiDung()
        {
            var danhSach = _nguoiDungDAL.LayTatCa();
            var items = danhSach.ConvertAll(nd => new
            {
                nd.Id, nd.TenDangNhap, nd.HoTen, nd.VaiTro
            });
            return JsonSerializer.Serialize(new { ok = true, action = "layNguoiDung", data = items });
        }

        private string XuLyThemNguoiDung(JsonElement data)
        {
            try
            {
                var nd = new NguoiDung(0,
                    data.GetProperty("tenDangNhap").GetString()!,
                    data.GetProperty("matKhau").GetString()!,
                    data.GetProperty("hoTen").GetString()!,
                    data.GetProperty("vaiTro").GetString() ?? "KhachHang");
                bool ok = _nguoiDungDAL.Them(nd);
                return JsonSerializer.Serialize(new { ok, action = "themNguoiDung" });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { ok = false, action = "themNguoiDung", loi = ex.Message });
            }
        }

        private string XuLySuaNguoiDung(JsonElement data)
        {
            try
            {
                var nd = new NguoiDung(
                    data.GetProperty("id").GetInt32(),
                    data.GetProperty("tenDangNhap").GetString()!,
                    data.GetProperty("matKhau").GetString()!,
                    data.GetProperty("hoTen").GetString()!,
                    data.GetProperty("vaiTro").GetString() ?? "KhachHang");
                bool ok = _nguoiDungDAL.Sua(nd);
                return JsonSerializer.Serialize(new { ok, action = "suaNguoiDung" });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { ok = false, action = "suaNguoiDung", loi = ex.Message });
            }
        }

        private string XuLyXoaNguoiDung(JsonElement data)
        {
            int id = data.GetProperty("id").GetInt32();
            bool ok = _nguoiDungDAL.Xoa(id);
            return JsonSerializer.Serialize(new { ok, action = "xoaNguoiDung" });
        }

        // ===== FORM LIFECYCLE =====
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide(); // Thu vào system tray thay vì đóng
            }
            base.OnFormClosing(e);
        }
    }

    // DTO nhận message từ JS
    public class WebMessage
    {
        public string Action { get; set; } = "";
        public JsonElement Data { get; set; }
    }
}
