using System;

namespace QuanLyTraSua.Entities
{
    /// <summary>
    /// Lớp cha trừu tượng SanPham - Menu Trà Sữa
    /// ĐÓNG GÓI: Validation ở properties
    /// KẾ THỪA: Lớp cha cho TraSua và Topping
    /// ĐA HÌNH: Hàm virtual TinhTien() được override ở lớp con
    /// </summary>
    public abstract class SanPham
    {
        private int _id;
        private string _tenSanPham = string.Empty;
        private decimal _giaCoBan;
        private string _loai = string.Empty;
        private string _hinhAnh = string.Empty;
        private bool _dangBan = true;

        // ===== ĐÓNG GÓI (Encapsulation): Bảo vệ các thuộc tính của sản phẩm thông qua Getter/Setter =====
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string TenSanPham
        {
            get { return _tenSanPham; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Tên sản phẩm không được để trống!");
                }
                _tenSanPham = value.Trim();
            }
        }

        public decimal GiaCoBan
        {
            get { return _giaCoBan; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Giá cơ bản không được âm!");
                }
                _giaCoBan = value;
            }
        }

        public string Loai
        {
            get { return _loai; }
            protected set { _loai = value; } // Chỉ có lớp con kế thừa mới được sửa giá trị này
        }

        public string HinhAnh
        {
            get { return _hinhAnh; }
            set { _hinhAnh = value ?? string.Empty; }
        }

        public bool DangBan
        {
            get { return _dangBan; }
            set { _dangBan = value; }
        }

        // ===== ĐA HÌNH (Polymorphism): Phương thức ảo TinhTien() để các lớp con ghi đè (override) =====
        public virtual decimal TinhTien(string tuyChon = "")
        {
            // Mặc định trả về giá cơ bản
            return GiaCoBan;
        }

        // In thông tin sản phẩm ra dạng chuỗi
        public override string ToString()
        {
            return TenSanPham + " - " + GiaCoBan.ToString("N0") + "đ [" + Loai + "]";
        }
    }

    // =======================================================
    // KẾ THỪA (Inheritance): Lớp con TraSua kế thừa lớp cha SanPham
    // =======================================================
    public class TraSua : SanPham
    {
        private const decimal PHU_PHI_SIZE_L = 5000m; // Phụ phí khi chọn size L

        public TraSua()
        {
            this.Loai = "TraSua";
        }

        public TraSua(int id, string tenSanPham, decimal giaCoBan, string hinhAnh = "", bool dangBan = true)
        {
            this.Id = id;
            this.TenSanPham = tenSanPham;
            this.GiaCoBan = giaCoBan;
            this.Loai = "TraSua";
            this.HinhAnh = hinhAnh;
            this.DangBan = dangBan;
        }

        // Ghi đè phương thức TinhTien của lớp cha (Tính đa hình)
        public override decimal TinhTien(string tuyChon = "")
        {
            decimal gia = this.GiaCoBan;

            if (!string.IsNullOrEmpty(tuyChon))
            {
                // Nếu khách chọn Size L thì cộng thêm 5.000đ phụ phí
                if (tuyChon.Contains("Size L", StringComparison.OrdinalIgnoreCase))
                {
                    gia += PHU_PHI_SIZE_L;
                }
            }

            return gia;
        }
    }

    // =======================================================
    // KẾ THỪA (Inheritance): Lớp con Topping kế thừa lớp cha SanPham
    // =======================================================
    public class Topping : SanPham
    {
        public Topping()
        {
            this.Loai = "Topping";
        }

        public Topping(int id, string tenSanPham, decimal giaCoBan, string hinhAnh = "", bool dangBan = true)
        {
            this.Id = id;
            this.TenSanPham = tenSanPham;
            this.GiaCoBan = giaCoBan;
            this.Loai = "Topping";
            this.HinhAnh = hinhAnh;
            this.DangBan = dangBan;
        }

        // Ghi đè phương thức TinhTien của lớp cha (Tính đa hình)
        public override decimal TinhTien(string tuyChon = "")
        {
            // Topping có giá cố định, không cộng thêm tiền theo size
            return this.GiaCoBan;
        }
    }

    // =======================================================
    // TRỪU TƯỢNG (Abstraction): Factory Pattern tạo đối tượng
    // =======================================================
    public static class SanPhamFactory
    {
        // Tạo sản phẩm đúng kiểu dựa trên chuỗi loai nhận được
        public static SanPham TaoSanPham(string loai, int id, string ten, decimal gia, string hinhAnh, bool dangBan)
        {
            if (loai == "TraSua")
            {
                return new TraSua(id, ten, gia, hinhAnh, dangBan);
            }
            else if (loai == "Topping")
            {
                return new Topping(id, ten, gia, hinhAnh, dangBan);
            }
            else
            {
                throw new ArgumentException("Loại sản phẩm '" + loai + "' không hợp lệ!");
            }
        }
    }
}
