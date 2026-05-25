using System;

namespace QuanLyTraSua.Entities
{
    /// <summary>
    /// Thực thể NguoiDung - Quản lý tài khoản đăng nhập (Khách hàng, Nhân viên, Admin)
    /// ĐÓNG GÓI: Validation thông qua properties
    /// </summary>
    public class NguoiDung
    {
        private int _id;
        private string _tenDangNhap = string.Empty;
        private string _matKhau = string.Empty;
        private string _hoTen = string.Empty;
        private string _vaiTro = "KhachHang";

        // ===== ĐÓNG GÓI: Properties với validation =====
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string TenDangNhap
        {
            get => _tenDangNhap;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên đăng nhập không được để trống!");
                _tenDangNhap = value.Trim();
            }
        }

        public string MatKhau
        {
            get => _matKhau;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mật khẩu không được để trống!");
                if (value.Length < 4)
                    throw new ArgumentException("Mật khẩu phải có ít nhất 4 ký tự!");
                _matKhau = value;
            }
        }

        public string HoTen
        {
            get => _hoTen;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Họ tên không được để trống!");
                _hoTen = value.Trim();
            }
        }

        public string VaiTro
        {
            get => _vaiTro;
            set
            {
                var vaiTroHopLe = new[] { "Admin", "NhanVien", "KhachHang" };
                if (Array.IndexOf(vaiTroHopLe, value) == -1)
                    throw new ArgumentException($"Vai trò '{value}' không hợp lệ! (Admin/NhanVien/KhachHang)");
                _vaiTro = value;
            }
        }

        // Constructor mặc định
        public NguoiDung() { }

        // Constructor đầy đủ
        public NguoiDung(int id, string tenDangNhap, string matKhau, string hoTen, string vaiTro = "KhachHang")
        {
            Id = id;
            TenDangNhap = tenDangNhap;
            MatKhau = matKhau;
            HoTen = hoTen;
            VaiTro = vaiTro;
        }

        public bool LaAdmin() => _vaiTro == "Admin";
        public bool LaNhanVien() => _vaiTro == "NhanVien" || _vaiTro == "Admin";

        public override string ToString() => $"[{VaiTro}] {HoTen} ({TenDangNhap})";
    }
}
