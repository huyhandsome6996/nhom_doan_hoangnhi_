using System;
using System.Collections.Generic;

namespace QuanLyTraSua.Entities
{
    /// <summary>
    /// Thực thể DonHang - Lưu thông tin đặt món của khách
    /// ĐÓNG GÓI: Validation thông qua các thuộc tính getter/setter
    /// </summary>
    public class DonHang
    {
        private int _id;
        private int _khachHangId;
        private DateTime _thoiGianDat = DateTime.Now;
        private decimal _tongTien;
        private string _trangThai = "Chờ xác nhận";
        private string _ghiChu = string.Empty;

        // Danh sách trạng thái hợp lệ để đối chiếu
        public static readonly string[] DanhSachTrangThai = new string[]
        { 
            "Chờ xác nhận", 
            "Đang pha chế", 
            "Hoàn thành", 
            "Đã hủy" 
        };

        // ===== ĐÓNG GÓI (Encapsulation): Các thuộc tính được kiểm tra và đóng gói =====
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int KhachHangId
        {
            get { return _khachHangId; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("ID Khách hàng không hợp lệ!");
                }
                _khachHangId = value;
            }
        }

        public DateTime ThoiGianDat
        {
            get { return _thoiGianDat; }
            set { _thoiGianDat = value; }
        }

        public decimal TongTien
        {
            get { return _tongTien; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Tổng tiền không được âm!");
                }
                _tongTien = value;
            }
        }

        public string TrangThai
        {
            get { return _trangThai; }
            set
            {
                if (Array.IndexOf(DanhSachTrangThai, value) == -1)
                {
                    throw new ArgumentException("Trạng thái '" + value + "' không hợp lệ!");
                }
                _trangThai = value;
            }
        }

        public string GhiChu
        {
            get { return _ghiChu; }
            set { _ghiChu = value ?? string.Empty; }
        }

        // Các thuộc tính liên kết đối tượng (không lưu trong cơ sở dữ liệu SQLite)
        public NguoiDung? KhachHang { get; set; }
        public List<ChiTietDonHang> DanhSachChiTiet { get; set; } = new List<ChiTietDonHang>();

        // Constructor mặc định (không tham số)
        public DonHang() { }

        // Constructor khởi tạo nhanh khi khách đặt hàng
        public DonHang(int khachHangId, string ghiChu = "")
        {
            this.KhachHangId = khachHangId;
            this.ThoiGianDat = DateTime.Now;
            this.TrangThai = "Chờ xác nhận";
            this.GhiChu = ghiChu;
        }

        // Phương thức tính lại tổng tiền từ danh sách chi tiết đơn hàng
        public void TinhLaiTongTien()
        {
            decimal tong = 0;
            foreach (var ct in this.DanhSachChiTiet)
            {
                tong += ct.ThanhTien;
            }
            this.TongTien = tong;
        }

        // Đơn hàng chỉ có thể hủy nếu đang ở trạng thái Chờ xác nhận
        public bool CoTheHuy()
        {
            if (this.TrangThai == "Chờ xác nhận")
            {
                return true;
            }
            return false;
        }

        // Chuyển đối tượng thành chuỗi dạng chữ để dễ hiển thị
        public override string ToString()
        {
            return "Đơn #" + this.Id + " | " + this.ThoiGianDat.ToString("dd/MM/yyyy HH:mm") + " | " + this.TrangThai + " | " + this.TongTien.ToString("N0") + "đ";
        }
    }
}
