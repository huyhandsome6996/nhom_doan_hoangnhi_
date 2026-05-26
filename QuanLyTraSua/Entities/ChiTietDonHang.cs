using System;

namespace QuanLyTraSua.Entities
{
    /// <summary>
    /// Thực thể ChiTietDonHang - Chi tiết từng món trong đơn hàng (Giỏ hàng)
    /// Quan hệ Master-Detail với lớp DonHang
    /// ĐÓNG GÓI: Ràng buộc số lượng và giá bán qua thuộc tính
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

        // ===== ĐÓNG GÓI (Encapsulation): Kiểm tra tính hợp lệ dữ liệu đầu vào =====
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int DonHangId
        {
            get { return _donHangId; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("ID Đơn hàng không hợp lệ!");
                }
                _donHangId = value;
            }
        }

        public int SanPhamId
        {
            get { return _sanPhamId; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("ID Sản phẩm không hợp lệ!");
                }
                _sanPhamId = value;
            }
        }

        public int SoLuong
        {
            get { return _soLuong; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Số lượng phải lớn hơn 0!");
                }
                _soLuong = value;
                this.TinhLaiThanhTien(); // Tính lại thành tiền mỗi khi đổi số lượng
            }
        }

        public decimal DonGiaBan
        {
            get { return _donGiaBan; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Đơn giá không được âm!");
                }
                _donGiaBan = value;
                this.TinhLaiThanhTien(); // Tính lại thành tiền mỗi khi đổi đơn giá
            }
        }

        // Tùy chọn thêm như: "Size L, 50% Đường, Ít đá"
        public string TuyChonThem
        {
            get { return _tuyChonThem; }
            set { _tuyChonThem = value ?? string.Empty; }
        }

        public decimal ThanhTien
        {
            get { return _thanhTien; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Thành tiền không được âm!");
                }
                _thanhTien = value;
            }
        }

        // Đối tượng sản phẩm liên kết (không lưu trực tiếp vào cơ sở dữ liệu SQLite)
        public SanPham? SanPham { get; set; }

        // Constructor mặc định (không tham số)
        public ChiTietDonHang() { }

        // Constructor khởi tạo nhanh
        public ChiTietDonHang(int donHangId, int sanPhamId, int soLuong, decimal donGiaBan, string tuyChonThem = "")
        {
            this.DonHangId = donHangId;
            this.SanPhamId = sanPhamId;
            this._soLuong = soLuong;       // Gán trực tiếp vào biến để tránh chạy TinhLaiThanhTien trước khi gán DonGiaBan
            this._donGiaBan = donGiaBan;
            this.TuyChonThem = tuyChonThem;
            this.TinhLaiThanhTien();
        }

        // Phương thức tính thành tiền tự động (Đơn giá x Số lượng)
        private void TinhLaiThanhTien()
        {
            this._thanhTien = this._donGiaBan * this._soLuong;
        }

        // In chi tiết món ăn ra dạng chuỗi
        public override string ToString()
        {
            string tenMon = string.Empty;
            if (this.SanPham != null)
            {
                tenMon = this.SanPham.TenSanPham;
            }
            else
            {
                tenMon = "Sản phẩm #" + this.SanPhamId;
            }
            return tenMon + " x" + this.SoLuong + " [" + this.TuyChonThem + "] = " + this.ThanhTien.ToString("N0") + "đ";
        }
    }
}
