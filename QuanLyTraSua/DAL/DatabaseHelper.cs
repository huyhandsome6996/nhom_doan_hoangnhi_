using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace QuanLyTraSua.DAL
{
    /// <summary>
    /// Lớp kết nối và khởi tạo cơ sở dữ liệu SQLite
    /// Singleton Pattern để đảm bảo 1 connection string duy nhất
    /// </summary>
    public static class DatabaseHelper
    {
        private static string _dbPath = string.Empty;
        private static string _connectionString = string.Empty;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    KhoiTao();
                return _connectionString;
            }
        }

        public static void KhoiTao(string? dbPath = null)
        {
            if (dbPath == null)
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "QuanLyTraSua");
                Directory.CreateDirectory(appDataPath);
                _dbPath = Path.Combine(appDataPath, "quanlytrasua.db");
            }
            else
            {
                _dbPath = dbPath;
            }

            _connectionString = $"Data Source={_dbPath};";
            TaoDatabase();
        }

        private static void TaoDatabase()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            string sql = @"
                PRAGMA foreign_keys = ON;

                CREATE TABLE IF NOT EXISTS NguoiDung (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    TenDangNhap TEXT NOT NULL UNIQUE,
                    MatKhau     TEXT NOT NULL,
                    HoTen       TEXT NOT NULL,
                    VaiTro      TEXT NOT NULL DEFAULT 'KhachHang'
                );

                CREATE TABLE IF NOT EXISTS SanPham (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    TenSanPham  TEXT NOT NULL,
                    GiaCoBan    DECIMAL NOT NULL CHECK(GiaCoBan >= 0),
                    Loai        TEXT NOT NULL,
                    HinhAnh     TEXT DEFAULT '',
                    DangBan     INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS DonHang (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    KhachHangId INTEGER NOT NULL,
                    ThoiGianDat DATETIME DEFAULT CURRENT_TIMESTAMP,
                    TongTien    DECIMAL DEFAULT 0,
                    TrangThai   TEXT DEFAULT 'Chờ xác nhận',
                    GhiChu      TEXT DEFAULT '',
                    FOREIGN KEY (KhachHangId) REFERENCES NguoiDung(Id)
                );

                CREATE TABLE IF NOT EXISTS ChiTietDonHang (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    DonHangId   INTEGER NOT NULL,
                    SanPhamId   INTEGER NOT NULL,
                    SoLuong     INTEGER NOT NULL CHECK(SoLuong > 0),
                    DonGiaBan   DECIMAL NOT NULL,
                    TuyChonThem TEXT DEFAULT '',
                    ThanhTien   DECIMAL NOT NULL,
                    FOREIGN KEY (DonHangId) REFERENCES DonHang(Id),
                    FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id)
                );
            ";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();

            // Seed dữ liệu mặc định nếu chưa có
            SeedDuLieu(conn);
        }

        private static void SeedDuLieu(SqliteConnection conn)
        {
            // Kiểm tra đã có admin chưa
            using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM NguoiDung WHERE VaiTro='Admin'", conn);
            var count = Convert.ToInt64(checkCmd.ExecuteScalar() ?? 0);

            if (count == 0)
            {
                // Tạo tài khoản Admin mặc định
                string insertUsers = @"
                    INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, VaiTro) VALUES
                    ('admin', 'admin123', 'Quản Trị Viên', 'Admin'),
                    ('nhanvien1', 'nv123456', 'Nguyễn Văn A', 'NhanVien'),
                    ('khachhang1', 'kh123456', 'Trần Thị B', 'KhachHang');
                ";
                using var userCmd = new SqliteCommand(insertUsers, conn);
                userCmd.ExecuteNonQuery();

                // Tạo menu sản phẩm mặc định
                string insertSanPham = @"
                    INSERT INTO SanPham (TenSanPham, GiaCoBan, Loai, HinhAnh, DangBan) VALUES
                    ('Trà Sữa Truyền Thống', 35000, 'TraSua', '', 1),
                    ('Trà Sữa Matcha', 40000, 'TraSua', '', 1),
                    ('Trà Sữa Khoai Môn', 38000, 'TraSua', '', 1),
                    ('Trà Đào Cam Sả', 32000, 'TraSua', '', 1),
                    ('Trà Sữa Oolong', 42000, 'TraSua', '', 1),
                    ('Trà Sữa Socola', 38000, 'TraSua', '', 1),
                    ('Thạch Dừa', 8000, 'Topping', '', 1),
                    ('Trân Châu Đen', 8000, 'Topping', '', 1),
                    ('Pudding Trứng', 10000, 'Topping', '', 1),
                    ('Thạch Cà Phê', 8000, 'Topping', '', 1);
                ";
                using var spCmd = new SqliteCommand(insertSanPham, conn);
                spCmd.ExecuteNonQuery();
            }
        }

        public static SqliteConnection TaoKetNoi()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
