using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using QuanLyTraSua.Entities;
using QuanLyTraSua.DAL;

namespace QuanLyTraSua.Tests
{
    public class HeThongQuanLyTraSuaTests : IDisposable
    {
        private readonly string _testDbPath;

        public HeThongQuanLyTraSuaTests()
        {
            // Thiết lập SQLite Database kiểm thử độc lập
            string tempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestDb");
            Directory.CreateDirectory(tempFolder);
            _testDbPath = Path.Combine(tempFolder, $"test_db_{Guid.NewGuid()}.db");
            DatabaseHelper.KhoiTao(_testDbPath);
        }

        public void Dispose()
        {
            // Dọn dẹp Database kiểm thử sau khi hoàn thành
            try
            {
                if (File.Exists(_testDbPath))
                {
                    File.Delete(_testDbPath);
                }
            }
            catch { }
        }

        // =========================================================================
        // 1. KIỂM THỬ ĐÓNG GÓI (Encapsulation)
        // =========================================================================

        [Fact]
        public void NguoiDung_TenDangNhapRong_NemNgoaiLe()
        {
            var user = new NguoiDung();
            Assert.Throws<ArgumentException>(() => user.TenDangNhap = "");
            Assert.Throws<ArgumentException>(() => user.TenDangNhap = "   ");
        }

        [Fact]
        public void NguoiDung_MatKhauNgan_NemNgoaiLe()
        {
            var user = new NguoiDung();
            Assert.Throws<ArgumentException>(() => user.MatKhau = "123");
        }

        [Fact]
        public void NguoiDung_VaiTroKhongHopLe_NemNgoaiLe()
        {
            var user = new NguoiDung();
            Assert.Throws<ArgumentException>(() => user.VaiTro = "SuperAdmin");
        }

        [Fact]
        public void SanPham_GiaCoBanAm_NemNgoaiLe()
        {
            var trasua = new TraSua();
            Assert.Throws<ArgumentException>(() => trasua.GiaCoBan = -1000m);
        }

        [Fact]
        public void ChiTietDonHang_SoLuongKhongHopLe_NemNgoaiLe()
        {
            var ct = new ChiTietDonHang();
            Assert.Throws<ArgumentException>(() => ct.SoLuong = 0);
            Assert.Throws<ArgumentException>(() => ct.SoLuong = -5);
        }

        // =========================================================================
        // 2. KIỂM THỬ KẾ THỪA & ĐA HÌNH (Inheritance & Polymorphism)
        // =========================================================================

        [Fact]
        public void SanPham_TinhKeThua_TraSuaVaToppingLaSanPham()
        {
            var trasua = new TraSua(1, "Trà Sữa Matcha", 40000m);
            var topping = new Topping(2, "Trân Châu Đen", 8000m);

            Assert.True(trasua is SanPham);
            Assert.True(topping is SanPham);
        }

        [Fact]
        public void SanPham_TinhDaHinh_TraSuaTangGiaKhiChonSizeL()
        {
            // Arrange
            SanPham trasua = new TraSua(1, "Trà Sữa Matcha", 40000m);

            // Act
            decimal giaSizeM = trasua.TinhTien("Size M, 50% Đường, Ít đá");
            decimal giaSizeL = trasua.TinhTien("Size L, 50% Đường, Ít đá");

            // Assert
            Assert.Equal(40000m, giaSizeM);
            Assert.Equal(45000m, giaSizeL); // Size L cộng 5k
        }

        [Fact]
        public void SanPham_TinhDaHinh_ToppingGiaCoDinhKeCaKhiChonSizeL()
        {
            // Arrange
            SanPham topping = new Topping(2, "Trân Châu Đen", 8000m);

            // Act
            decimal giaOptionM = topping.TinhTien("Size M");
            decimal giaOptionL = topping.TinhTien("Size L");

            // Assert
            Assert.Equal(8000m, giaOptionM);
            Assert.Equal(8000m, giaOptionL); // Topping không đổi giá
        }

        // =========================================================================
        // 3. KIỂM THỬ CƠ SỞ DỮ LIỆU & DAL (Database & Data Access Layer)
        // =========================================================================

        [Fact]
        public void DatabaseHelper_KhoiTao_CoSeedDuLieuMacDinh()
        {
            var userDAL = new NguoiDungDAL();
            var spDAL = new SanPhamDAL();

            var users = userDAL.LayTatCa();
            var items = spDAL.LayTatCa();

            Assert.NotEmpty(users);
            Assert.NotEmpty(items);
            Assert.Contains(users, u => u.TenDangNhap == "admin" && u.VaiTro == "Admin");
            Assert.Contains(items, sp => sp.TenSanPham == "Trà Sữa Truyền Thống");
        }

        [Fact]
        public void NguoiDungDAL_DangNhapChinhXac_TraVeNguoiDung()
        {
            var userDAL = new NguoiDungDAL();
            
            var user = userDAL.DangNhap("admin", "admin123");
            
            Assert.NotNull(user);
            Assert.Equal("Quản Trị Viên", user.HoTen);
            Assert.Equal("Admin", user.VaiTro);
        }

        [Fact]
        public void NguoiDungDAL_DangNhapSaiMatKhau_TraVeNull()
        {
            var userDAL = new NguoiDungDAL();
            
            var user = userDAL.DangNhap("admin", "wrongpassword");
            
            Assert.Null(user);
        }

        [Fact]
        public void DonHangDAL_ThemDonHang_MasterDetailCungGiaoTacThanhCong()
        {
            // Arrange
            var userDAL = new NguoiDungDAL();
            var spDAL = new SanPhamDAL();
            var dhDAL = new DonHangDAL();

            var khach = userDAL.DangNhap("khachhang1", "kh123456");
            Assert.NotNull(khach);

            var sp1 = spDAL.LayTheoId(1); // Trà Sữa Truyền Thống (35k)
            var sp2 = spDAL.LayTheoId(7); // Thạch Dừa (8k)
            Assert.NotNull(sp1);
            Assert.NotNull(sp2);

            var donHang = new DonHang(khach.Id, "Giao nhanh");
            
            // Món 1: Trà sữa size L (35k + 5k = 40k) x 2 ly = 80k
            var donGia1 = sp1.TinhTien("Size L");
            var ct1 = new ChiTietDonHang(0, sp1.Id, 2, donGia1, "Size L, 100% Đường");
            
            // Món 2: Topping (8k) x 3 = 24k
            var donGia2 = sp2.TinhTien();
            var ct2 = new ChiTietDonHang(0, sp2.Id, 3, donGia2, "");

            donHang.DanhSachChiTiet.Add(ct1);
            donHang.DanhSachChiTiet.Add(ct2);
            donHang.TinhLaiTongTien(); // 80k + 24k = 104k

            // Act
            int donHangId = dhDAL.Them(donHang);

            // Assert
            Assert.True(donHangId > 0);

            // Tải lại đơn hàng từ Database
            var layLaiDh = dhDAL.LayTheoId(donHangId);
            Assert.NotNull(layLaiDh);
            Assert.Equal(104000m, layLaiDh.TongTien);
            Assert.Equal("Chờ xác nhận", layLaiDh.TrangThai);
            Assert.Equal(2, layLaiDh.DanhSachChiTiet.Count);
            
            // Kiểm tra chi tiết sản phẩm 1
            var layLaiCt1 = layLaiDh.DanhSachChiTiet.Find(c => c.SanPhamId == sp1.Id);
            Assert.NotNull(layLaiCt1);
            Assert.Equal(2, layLaiCt1.SoLuong);
            Assert.Equal(40000m, layLaiCt1.DonGiaBan);
            Assert.Equal(80000m, layLaiCt1.ThanhTien);

            // Kiểm tra chi tiết sản phẩm 2
            var layLaiCt2 = layLaiDh.DanhSachChiTiet.Find(c => c.SanPhamId == sp2.Id);
            Assert.NotNull(layLaiCt2);
            Assert.Equal(3, layLaiCt2.SoLuong);
            Assert.Equal(8000m, layLaiCt2.DonGiaBan);
            Assert.Equal(24000m, layLaiCt2.ThanhTien);
        }
    }
}
