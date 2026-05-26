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

        // ===== ĐÓNG GÓI (Encapsulation): Các thuộc tính được bảo vệ qua các getter/setter =====
        
        // Thuộc tính ID của người dùng
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        // Tên đăng nhập dùng để đăng nhập hệ thống
        public string TenDangNhap
        {
            get { return _tenDangNhap; }
            set
            {
                // Kiểm tra nếu tên đăng nhập trống thì báo lỗi ngay
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Tên đăng nhập không được để trống!");
                }
                _tenDangNhap = value.Trim();
            }
        }

        // Mật khẩu của tài khoản
        public string MatKhau
        {
            get { return _matKhau; }
            set
            {
                // Kiểm tra mật khẩu không được trống và phải có ít nhất 4 ký tự
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Mật khẩu không được để trống!");
                }
                if (value.Length < 4)
                {
                    throw new ArgumentException("Mật khẩu phải có ít nhất 4 ký tự!");
                }
                _matKhau = value;
            }
        }

        // Họ tên đầy đủ hiển thị trên giao diện
        public string HoTen
        {
            get { return _hoTen; }
            set
            {
                // Kiểm tra họ tên không được trống
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Họ tên không được để trống!");
                }
                _hoTen = value.Trim();
            }
        }

        // Vai trò của tài khoản (Admin, NhanVien, KhachHang)
        public string VaiTro
        {
            get { return _vaiTro; }
            set
            {
                // Chỉ cho phép 3 vai trò cố định này
                var vaiTroHopLe = new string[] { "Admin", "NhanVien", "KhachHang" };
                if (Array.IndexOf(vaiTroHopLe, value) == -1)
                {
                    throw new ArgumentException("Vai trò '" + value + "' không hợp lệ! (Phải là Admin, NhanVien hoặc KhachHang)");
                }
                _vaiTro = value;
            }
        }

        // Constructor mặc định (không có tham số)
        public NguoiDung() { }

        // Constructor đầy đủ tham số để khởi tạo đối tượng nhanh
        public NguoiDung(int id, string tenDangNhap, string matKhau, string hoTen, string vaiTro = "KhachHang")
        {
            this.Id = id;
            this.TenDangNhap = tenDangNhap;
            this.MatKhau = matKhau;
            this.HoTen = hoTen;
            this.VaiTro = vaiTro;
        }

        // Kiểm tra xem tài khoản này có phải Quản trị viên hay không
        public bool LaAdmin()
        {
            if (this.VaiTro == "Admin")
            {
                return true;
            }
            return false;
        }

        // Kiểm tra xem tài khoản này có quyền của Nhân viên phục vụ hay không (Admin cũng có quyền này)
        public bool LaNhanVien()
        {
            if (this.VaiTro == "NhanVien" || this.VaiTro == "Admin")
            {
                return true;
            }
            return false;
        }

        // Chuyển đối tượng thành chuỗi dạng chữ để dễ theo dõi/in ra màn hình
        public override string ToString()
        {
            return "[" + this.VaiTro + "] " + this.HoTen + " (" + this.TenDangNhap + ")";
        }
    }
}
