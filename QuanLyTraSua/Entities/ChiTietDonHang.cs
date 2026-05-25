using System;

namespace QuanLyTraSua.Entities
{
    /// <summary>
    /// Thực thể ChiTietDonHang - Chi tiết từng món trong đơn hàng (Giỏ hàng)
    /// Quan hệ Master-Detail với DonHang
    /// ĐÓNG GÓI: Validation thông qua properties
    /// </summary>
    public class ChiTietDonHang
    {
        private int _id;
        private int _donHangId;
        private int _sanPhamId;
        private int _soLuong;
        private decimal _donGiaBan;
        private string _tuyChonThem = string.Empty;
        private decimal _thanhTien;

        // ===== ĐÓNG GÓI: Properties =====
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int DonHangId
        {
            get => _donHangId;
            set
            {
                if (value < 0)
                    throw new ArgumentException("ID Đơn hàng không hợp lệ!");
                _donHangId = value;
            }
        }

        public int SanPhamId
        {
            get => _sanPhamId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("ID Sản phẩm không hợp lệ!");
                _sanPhamId = value;
            }
        }

        public int SoLuong
        {
            get => _soLuong;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Số lượng phải lớn hơn 0!");
                _soLuong = value;
                TinhLaiThanhTien();
            }
        }

        public decimal DonGiaBan
        {
            get => _donGiaBan;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Đơn giá không được âm!");
                _donGiaBan = value;
                TinhLaiThanhTien();
            }
        }

        /// <summary>
        /// Tùy chọn thêm: "Size L, 50% Đường, Ít đá" 
        /// Chuỗi này được dùng để tính Đa hình cộng thêm tiền
        /// </summary>
        public string TuyChonThem
        {
            get => _tuyChonThem;
            set => _tuyChonThem = value ?? string.Empty;
        }

        public decimal ThanhTien
        {
            get => _thanhTien;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Thành tiền không được âm!");
                _thanhTien = value;
            }
        }

        // Navigation properties (không lưu DB)
        public SanPham? SanPham { get; set; }

        // Constructor
        public ChiTietDonHang() { }

        public ChiTietDonHang(int donHangId, int sanPhamId, int soLuong, decimal donGiaBan, string tuyChonThem = "")
        {
            DonHangId = donHangId;
            SanPhamId = sanPhamId;
            _soLuong = soLuong;       // Set trực tiếp để tránh gọi TinhLaiThanhTien trước khi có DonGia
            _donGiaBan = donGiaBan;
            TuyChonThem = tuyChonThem;
            TinhLaiThanhTien();
        }

        private void TinhLaiThanhTien()
        {
            _thanhTien = _donGiaBan * _soLuong;
        }

        public override string ToString() =>
            $"{SanPham?.TenSanPham ?? $"SP#{SanPhamId}"} x{SoLuong} [{TuyChonThem}] = {ThanhTien:N0}đ";
    }
}
