using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using QuanLyTraSua.Entities;
using QuanLyTraSua.DAL.Interfaces;

namespace QuanLyTraSua.DAL
{
    /// <summary>
    /// DAL DonHang - Thực thi interface IDonHangDAL
    /// </summary>
    public class DonHangDAL : IDonHangDAL
    {
        private readonly IChiTietDonHangDAL _chiTietDAL;

        public DonHangDAL()
        {
            _chiTietDAL = new ChiTietDonHangDAL();
        }

        public List<DonHang> LayTatCa()
        {
            var danhSach = new List<DonHang>();
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"SELECT dh.*, nd.HoTen FROM DonHang dh
                           LEFT JOIN NguoiDung nd ON dh.KhachHangId = nd.Id
                           ORDER BY dh.ThoiGianDat DESC";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocDongData(reader));
            return danhSach;
        }

        public List<DonHang> LayTheoKhachHang(int khachHangId)
        {
            var danhSach = new List<DonHang>();
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"SELECT dh.*, nd.HoTen FROM DonHang dh
                           LEFT JOIN NguoiDung nd ON dh.KhachHangId = nd.Id
                           WHERE dh.KhachHangId=@KhachHangId
                           ORDER BY dh.ThoiGianDat DESC";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@KhachHangId", khachHangId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocDongData(reader));
            return danhSach;
        }

        public List<DonHang> LayTheoTrangThai(string trangThai)
        {
            var danhSach = new List<DonHang>();
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"SELECT dh.*, nd.HoTen FROM DonHang dh
                           LEFT JOIN NguoiDung nd ON dh.KhachHangId = nd.Id
                           WHERE dh.TrangThai=@TrangThai
                           ORDER BY dh.ThoiGianDat DESC";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@TrangThai", trangThai);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocDongData(reader));
            return danhSach;
        }

        public DonHang? LayTheoId(int id)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"SELECT dh.*, nd.HoTen FROM DonHang dh
                           LEFT JOIN NguoiDung nd ON dh.KhachHangId = nd.Id
                           WHERE dh.Id=@Id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var dh = DocDongData(reader);
                // Load chi tiết
                dh.DanhSachChiTiet = _chiTietDAL.LayTheodonHang(id);
                return dh;
            }
            return null;
        }

        public int Them(DonHang donHang)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                string sql = @"INSERT INTO DonHang (KhachHangId, ThoiGianDat, TongTien, TrangThai, GhiChu)
                               VALUES (@KhachHangId, @ThoiGianDat, @TongTien, @TrangThai, @GhiChu);
                               SELECT last_insert_rowid();";
                using var cmd = new SqliteCommand(sql, conn, transaction);
                cmd.Parameters.AddWithValue("@KhachHangId", donHang.KhachHangId);
                cmd.Parameters.AddWithValue("@ThoiGianDat", donHang.ThoiGianDat.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@TongTien", donHang.TongTien);
                cmd.Parameters.AddWithValue("@TrangThai", donHang.TrangThai);
                cmd.Parameters.AddWithValue("@GhiChu", donHang.GhiChu ?? "");

                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                donHang.Id = newId;

                // Thêm chi tiết đơn hàng
                foreach (var ct in donHang.DanhSachChiTiet)
                {
                    ct.DonHangId = newId;
                    string sqlCt = @"INSERT INTO ChiTietDonHang (DonHangId, SanPhamId, SoLuong, DonGiaBan, TuyChonThem, ThanhTien)
                                     VALUES (@DonHangId, @SanPhamId, @SoLuong, @DonGiaBan, @TuyChonThem, @ThanhTien)";
                    using var cmdCt = new SqliteCommand(sqlCt, conn, transaction);
                    cmdCt.Parameters.AddWithValue("@DonHangId", ct.DonHangId);
                    cmdCt.Parameters.AddWithValue("@SanPhamId", ct.SanPhamId);
                    cmdCt.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                    cmdCt.Parameters.AddWithValue("@DonGiaBan", ct.DonGiaBan);
                    cmdCt.Parameters.AddWithValue("@TuyChonThem", ct.TuyChonThem ?? "");
                    cmdCt.Parameters.AddWithValue("@ThanhTien", ct.ThanhTien);
                    cmdCt.ExecuteNonQuery();
                }

                transaction.Commit();
                return newId;
            }
            catch
            {
                transaction.Rollback();
                return -1;
            }
        }

        public bool CapNhatTrangThai(int id, string trangThai)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand(
                "UPDATE DonHang SET TrangThai=@TrangThai WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@TrangThai", trangThai);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int id)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                // Xóa chi tiết trước
                using var cmd1 = new SqliteCommand("DELETE FROM ChiTietDonHang WHERE DonHangId=@Id", conn, transaction);
                cmd1.Parameters.AddWithValue("@Id", id);
                cmd1.ExecuteNonQuery();

                // Xóa đơn hàng
                using var cmd2 = new SqliteCommand("DELETE FROM DonHang WHERE Id=@Id", conn, transaction);
                cmd2.Parameters.AddWithValue("@Id", id);
                cmd2.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        private static DonHang DocDongData(SqliteDataReader reader)
        {
            var dh = new DonHang
            {
                Id = Convert.ToInt32(reader["Id"]),
                KhachHangId = Convert.ToInt32(reader["KhachHangId"]),
                ThoiGianDat = DateTime.Parse(reader["ThoiGianDat"].ToString()!),
                TongTien = Convert.ToDecimal(reader["TongTien"]),
                GhiChu = reader["GhiChu"].ToString() ?? ""
            };
            dh.TrangThai = reader["TrangThai"].ToString()!;

            // Gán tên khách hàng nếu JOIN
            if (reader.GetOrdinal("HoTen") >= 0)
            {
                try
                {
                    dh.KhachHang = new NguoiDung { HoTen = reader["HoTen"].ToString() ?? "" };
                }
                catch { }
            }

            return dh;
        }
    }

    /// <summary>
    /// DAL ChiTietDonHang - Thực thi interface IChiTietDonHangDAL
    /// </summary>
    public class ChiTietDonHangDAL : IChiTietDonHangDAL
    {
        public List<ChiTietDonHang> LayTheodonHang(int donHangId)
        {
            var danhSach = new List<ChiTietDonHang>();
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"SELECT ct.*, sp.TenSanPham, sp.Loai FROM ChiTietDonHang ct
                           LEFT JOIN SanPham sp ON ct.SanPhamId = sp.Id
                           WHERE ct.DonHangId=@DonHangId";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@DonHangId", donHangId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocDongData(reader));
            return danhSach;
        }

        public bool Them(ChiTietDonHang chiTiet)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"INSERT INTO ChiTietDonHang (DonHangId, SanPhamId, SoLuong, DonGiaBan, TuyChonThem, ThanhTien)
                           VALUES (@DonHangId, @SanPhamId, @SoLuong, @DonGiaBan, @TuyChonThem, @ThanhTien)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@DonHangId", chiTiet.DonHangId);
            cmd.Parameters.AddWithValue("@SanPhamId", chiTiet.SanPhamId);
            cmd.Parameters.AddWithValue("@SoLuong", chiTiet.SoLuong);
            cmd.Parameters.AddWithValue("@DonGiaBan", chiTiet.DonGiaBan);
            cmd.Parameters.AddWithValue("@TuyChonThem", chiTiet.TuyChonThem ?? "");
            cmd.Parameters.AddWithValue("@ThanhTien", chiTiet.ThanhTien);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ThemNhieu(List<ChiTietDonHang> danhSach)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                foreach (var ct in danhSach)
                {
                    string sql = @"INSERT INTO ChiTietDonHang (DonHangId, SanPhamId, SoLuong, DonGiaBan, TuyChonThem, ThanhTien)
                                   VALUES (@DonHangId, @SanPhamId, @SoLuong, @DonGiaBan, @TuyChonThem, @ThanhTien)";
                    using var cmd = new SqliteCommand(sql, conn, transaction);
                    cmd.Parameters.AddWithValue("@DonHangId", ct.DonHangId);
                    cmd.Parameters.AddWithValue("@SanPhamId", ct.SanPhamId);
                    cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                    cmd.Parameters.AddWithValue("@DonGiaBan", ct.DonGiaBan);
                    cmd.Parameters.AddWithValue("@TuyChonThem", ct.TuyChonThem ?? "");
                    cmd.Parameters.AddWithValue("@ThanhTien", ct.ThanhTien);
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool Xoa(int id)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("DELETE FROM ChiTietDonHang WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool XoaTheodonHang(int donHangId)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("DELETE FROM ChiTietDonHang WHERE DonHangId=@DonHangId", conn);
            cmd.Parameters.AddWithValue("@DonHangId", donHangId);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static ChiTietDonHang DocDongData(SqliteDataReader reader)
        {
            var ct = new ChiTietDonHang
            {
                Id = Convert.ToInt32(reader["Id"]),
                DonHangId = Convert.ToInt32(reader["DonHangId"]),
                SanPhamId = Convert.ToInt32(reader["SanPhamId"]),
                TuyChonThem = reader["TuyChonThem"].ToString() ?? ""
            };

            // Đặt riêng để tránh tính toán lại trước khi có đủ dữ liệu
            ct.DonGiaBan = Convert.ToDecimal(reader["DonGiaBan"]);
            ct.SoLuong = Convert.ToInt32(reader["SoLuong"]);
            ct.ThanhTien = Convert.ToDecimal(reader["ThanhTien"]);

            // Attach tên sản phẩm nếu có
            try
            {
                string loai = reader["Loai"].ToString() ?? "TraSua";
                ct.SanPham = SanPhamFactory.TaoSanPham(loai, ct.SanPhamId,
                    reader["TenSanPham"].ToString()!, ct.DonGiaBan, "", true);
            }
            catch { }

            return ct;
        }
    }
}
