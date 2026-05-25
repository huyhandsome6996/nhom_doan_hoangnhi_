using System.Collections.Generic;
using QuanLyTraSua.Entities;

namespace QuanLyTraSua.DAL.Interfaces
{
    /// <summary>
    /// TRỪU TƯỢNG: Interface cho DAL NguoiDung
    /// </summary>
    public interface INguoiDungDAL
    {
        NguoiDung? DangNhap(string tenDangNhap, string matKhau);
        List<NguoiDung> LayTatCa();
        NguoiDung? LayTheoId(int id);
        bool Them(NguoiDung nguoiDung);
        bool Sua(NguoiDung nguoiDung);
        bool Xoa(int id);
        bool KiemTraTenDangNhapTonTai(string tenDangNhap);
    }

    /// <summary>
    /// TRỪU TƯỢNG: Interface cho DAL SanPham
    /// </summary>
    public interface ISanPhamDAL
    {
        List<SanPham> LayTatCa();
        List<SanPham> LayTheoLoai(string loai); // "TraSua" hoặc "Topping"
        SanPham? LayTheoId(int id);
        bool Them(SanPham sanPham);
        bool Sua(SanPham sanPham);
        bool Xoa(int id);
        bool DoiTrangThai(int id, bool dangBan);
    }

    /// <summary>
    /// TRỪU TƯỢNG: Interface cho DAL DonHang
    /// </summary>
    public interface IDonHangDAL
    {
        List<DonHang> LayTatCa();
        List<DonHang> LayTheoKhachHang(int khachHangId);
        List<DonHang> LayTheoTrangThai(string trangThai);
        DonHang? LayTheoId(int id);
        int Them(DonHang donHang);
        bool CapNhatTrangThai(int id, string trangThai);
        bool Xoa(int id);
    }

    /// <summary>
    /// TRỪU TƯỢNG: Interface cho DAL ChiTietDonHang
    /// </summary>
    public interface IChiTietDonHangDAL
    {
        List<ChiTietDonHang> LayTheodonHang(int donHangId);
        bool Them(ChiTietDonHang chiTiet);
        bool ThemNhieu(List<ChiTietDonHang> danhSach);
        bool Xoa(int id);
        bool XoaTheodonHang(int donHangId);
    }
}
