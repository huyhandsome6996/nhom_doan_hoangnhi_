using System;
using System.Collections.Generic;

namespace QuanLyTraSua.Entities
{
    /// <summary>
    /// Thực thể DonHang - Lưu thông tin đặt món của khách
    /// ĐÓNG GÓI: Validation thông qua properties
    /// </summary>
    public class DonHang
    {
        private int _id;
        private int _khachHangId;
        private DateTime _thoiGianDat = DateTime.Now;
        private decimal _tongTien;
        private string _trangThai = "Chờ xác nhận";
        private string _ghiChu = string.Empty;

        // Danh sách trạng thái hợp lệ
        public static readonly string[] DanhSachTrangThai =
            { "Chờ xác nhận", "Đang pha chế", "Hoàn thành", "Đã hủy" };

        // ===== ĐÓNG GÓI: Properties =====
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int KhachHangId
        {
            get => _khachHangId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("ID Khách hàng không hợp lệ!");
                _khachHangId = value;
            }
        }

        public DateTime ThoiGianDat
        {
            get => _thoiGianDat;
            set => _thoiGianDat = value;
        }

        public decimal TongTien
        {
            get => _tongTien;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Tổng tiền không được âm!");
                _tongTien = value;
            }
        }

        public string TrangThai
        {
            get => _trangThai;
            set
            {
                if (Array.IndexOf(DanhSachTrangThai, value) == -1)
                    throw new ArgumentException($"Trạng thái '{value}' không hợp lệ!");
                _trangThai = value;
            }
        }

        public string GhiChu
        {
            get => _ghiChu;
            set => _ghiChu = value ?? string.Empty;
        }

        // Navigation property (không lưu DB)
        public NguoiDung? KhachHang { get; set; }
        public List<ChiTietDonHang> DanhSachChiTiet { get; set; } = new();

        // Constructor
        public DonHang() { }

        public DonHang(int khachHangId, string ghiChu = "")
        {
            KhachHangId = khachHangId;
            ThoiGianDat = DateTime.Now;
            TrangThai = "Chờ xác nhận";
            GhiChu = ghiChu;
        }

        // Phương thức tính lại tổng tiền từ ChiTiet
        public void TinhLaiTongTien()
        {
            decimal tong = 0;
            foreach (var ct in DanhSachChiTiet)
                tong += ct.ThanhTien;
            TongTien = tong;
        }

        public bool CoTheHuy() => _trangThai == "Chờ xác nhận";

        public override string ToString() =>
            $"Đơn #{Id} | {ThoiGianDat:dd/MM/yyyy HH:mm} | {TrangThai} | {TongTien:N0}đ";
    }
}
