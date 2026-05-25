using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using QuanLyTraSua.Entities;
using QuanLyTraSua.DAL.Interfaces;

namespace QuanLyTraSua.DAL
{
    /// <summary>
    /// DAL SanPham - Thực thi interface ISanPhamDAL
    /// TRỪU TƯỢNG: Implement interface
    /// ĐA HÌNH: Sử dụng SanPhamFactory để tạo đúng kiểu đối tượng từ DB
    /// </summary>
    public class SanPhamDAL : ISanPhamDAL
    {
        public List<SanPham> LayTatCa()
        {
            var danhSach = new List<SanPham>();
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand(
                "SELECT * FROM SanPham ORDER BY Loai, TenSanPham", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocDongData(reader));
            return danhSach;
        }

        public List<SanPham> LayTheoLoai(string loai)
        {
            var danhSach = new List<SanPham>();
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand(
                "SELECT * FROM SanPham WHERE Loai=@Loai AND DangBan=1 ORDER BY TenSanPham", conn);
            cmd.Parameters.AddWithValue("@Loai", loai);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocDongData(reader));
            return danhSach;
        }

        public SanPham? LayTheoId(int id)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM SanPham WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return DocDongData(reader);
            return null;
        }

        public bool Them(SanPham sanPham)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"INSERT INTO SanPham (TenSanPham, GiaCoBan, Loai, HinhAnh, DangBan)
                           VALUES (@TenSanPham, @GiaCoBan, @Loai, @HinhAnh, @DangBan)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@TenSanPham", sanPham.TenSanPham);
            cmd.Parameters.AddWithValue("@GiaCoBan", sanPham.GiaCoBan);
            cmd.Parameters.AddWithValue("@Loai", sanPham.Loai);
            cmd.Parameters.AddWithValue("@HinhAnh", sanPham.HinhAnh ?? "");
            cmd.Parameters.AddWithValue("@DangBan", sanPham.DangBan ? 1 : 0);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Sua(SanPham sanPham)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"UPDATE SanPham SET TenSanPham=@TenSanPham, GiaCoBan=@GiaCoBan,
                           Loai=@Loai, HinhAnh=@HinhAnh, DangBan=@DangBan WHERE Id=@Id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", sanPham.Id);
            cmd.Parameters.AddWithValue("@TenSanPham", sanPham.TenSanPham);
            cmd.Parameters.AddWithValue("@GiaCoBan", sanPham.GiaCoBan);
            cmd.Parameters.AddWithValue("@Loai", sanPham.Loai);
            cmd.Parameters.AddWithValue("@HinhAnh", sanPham.HinhAnh ?? "");
            cmd.Parameters.AddWithValue("@DangBan", sanPham.DangBan ? 1 : 0);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int id)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("DELETE FROM SanPham WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DoiTrangThai(int id, bool dangBan)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand(
                "UPDATE SanPham SET DangBan=@DangBan WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@DangBan", dangBan ? 1 : 0);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// ĐA HÌNH: Dùng SanPhamFactory tạo đúng kiểu TraSua hoặc Topping từ DB
        /// </summary>
        private static SanPham DocDongData(SqliteDataReader reader)
        {
            int id = Convert.ToInt32(reader["Id"]);
            string ten = reader["TenSanPham"].ToString()!;
            decimal gia = Convert.ToDecimal(reader["GiaCoBan"]);
            string loai = reader["Loai"].ToString()!;
            string hinhAnh = reader["HinhAnh"].ToString() ?? "";
            bool dangBan = Convert.ToInt32(reader["DangBan"]) == 1;

            // ĐA HÌNH: SanPhamFactory trả về đúng kiểu TraSua hoặc Topping
            return SanPhamFactory.TaoSanPham(loai, id, ten, gia, hinhAnh, dangBan);
        }
    }
}
