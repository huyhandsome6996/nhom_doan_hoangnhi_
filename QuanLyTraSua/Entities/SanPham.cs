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

        // ===== ĐÓNG GÓI: Properties với validation =====
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string TenSanPham
        {
            get => _tenSanPham;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên sản phẩm không được để trống!");
                _tenSanPham = value.Trim();
            }
        }

        /// <summary>
        /// ĐÓNG GÓI: Validation GiaCoBan >= 0
        /// </summary>
        public decimal GiaCoBan
        {
            get => _giaCoBan;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Giá cơ bản không được âm!");
                _giaCoBan = value;
            }
        }

        public string Loai
        {
            get => _loai;
            protected set => _loai = value; // Chỉ lớp con mới set được
        }

        public string HinhAnh
        {
            get => _hinhAnh;
            set => _hinhAnh = value ?? string.Empty;
        }

        public bool DangBan
        {
            get => _dangBan;
            set => _dangBan = value;
        }

        // ===== ĐA HÌNH: Hàm virtual TinhTien() =====
        /// <summary>
        /// ĐA HÌNH: Lớp con override để tính thêm tiền theo tùy chọn
        /// </summary>
        /// <param name="tuyChon">Chuỗi tùy chọn VD: "Size L, 50% Đường"</param>
        /// <returns>Thành tiền sau khi cộng thêm theo tùy chọn</returns>
        public virtual decimal TinhTien(string tuyChon = "")
        {
            return GiaCoBan;
        }

        public override string ToString() => $"{TenSanPham} - {GiaCoBan:N0}đ [{Loai}]";
    }

    // =======================================================
    // KẾ THỪA: Lớp con TraSua kế thừa từ SanPham
    // =======================================================
    /// <summary>
    /// Lớp con TraSua - KẾ THỪA từ SanPham
    /// ĐA HÌNH: Override TinhTien() cộng thêm tiền khi chọn Size L
    /// </summary>
    public class TraSua : SanPham
    {
        private const decimal PHU_PHI_SIZE_L = 5000m; // Thêm 5k khi chọn Size L

        public TraSua()
        {
            Loai = "TraSua";
        }

        public TraSua(int id, string tenSanPham, decimal giaCoBan, string hinhAnh = "", bool dangBan = true)
        {
            Id = id;
            TenSanPham = tenSanPham;
            GiaCoBan = giaCoBan;
            Loai = "TraSua";
            HinhAnh = hinhAnh;
            DangBan = dangBan;
        }

        /// <summary>
        /// ĐA HÌNH: Override - TraSua cộng thêm tiền khi chọn Size L
        /// </summary>
        public override decimal TinhTien(string tuyChon = "")
        {
            decimal gia = GiaCoBan;

            if (!string.IsNullOrEmpty(tuyChon))
            {
                // Phân tích chuỗi tùy chọn
                if (tuyChon.Contains("Size L", StringComparison.OrdinalIgnoreCase))
                    gia += PHU_PHI_SIZE_L;
            }

            return gia;
        }
    }

    // =======================================================
    // KẾ THỪA: Lớp con Topping kế thừa từ SanPham
    // =======================================================
    /// <summary>
    /// Lớp con Topping - KẾ THỪA từ SanPham
    /// ĐA HÌNH: Override TinhTien() - Topping không cộng thêm tiền theo size
    /// </summary>
    public class Topping : SanPham
    {
        public Topping()
        {
            Loai = "Topping";
        }

        public Topping(int id, string tenSanPham, decimal giaCoBan, string hinhAnh = "", bool dangBan = true)
        {
            Id = id;
            TenSanPham = tenSanPham;
            GiaCoBan = giaCoBan;
            Loai = "Topping";
            HinhAnh = hinhAnh;
            DangBan = dangBan;
        }

        /// <summary>
        /// ĐA HÌNH: Override - Topping KHÔNG cộng thêm tiền theo size
        /// </summary>
        public override decimal TinhTien(string tuyChon = "")
        {
            // Topping: Giá cố định, không phụ thuộc vào size/đường
            return GiaCoBan;
        }
    }

    // =======================================================
    // TRỪU TƯỢNG: Factory Method để tạo SanPham từ Loai
    // =======================================================
    public static class SanPhamFactory
    {
        /// <summary>
        /// TRỪU TƯỢNG: Tạo đối tượng SanPham đúng kiểu dựa trên Loai (Đa hình)
        /// </summary>
        public static SanPham TaoSanPham(string loai, int id, string ten, decimal gia, string hinhAnh, bool dangBan)
        {
            return loai switch
            {
                "TraSua" => new TraSua(id, ten, gia, hinhAnh, dangBan),
                "Topping" => new Topping(id, ten, gia, hinhAnh, dangBan),
                _ => throw new ArgumentException($"Loại sản phẩm '{loai}' không hợp lệ!")
            };
        }
    }
}
