using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using QuanLyTraSua.Entities;
using QuanLyTraSua.DAL.Interfaces;

namespace QuanLyTraSua.DAL
{
    /// <summary>
    /// DAL NguoiDung - Thực thi interface INguoiDungDAL
    /// TRỪU TƯỢNG: Implement interface
    /// </summary>
    public class NguoiDungDAL : INguoiDungDAL
    {
        public NguoiDung? DangNhap(string tenDangNhap, string matKhau)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = "SELECT * FROM NguoiDung WHERE TenDangNhap=@TenDangNhap AND MatKhau=@MatKhau";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            cmd.Parameters.AddWithValue("@MatKhau", matKhau);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return DocDongData(reader);
            return null;
        }

        public List<NguoiDung> LayTatCa()
        {
            var danhSach = new List<NguoiDung>();
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM NguoiDung ORDER BY VaiTro, HoTen", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                danhSach.Add(DocDongData(reader));
            return danhSach;
        }

        public NguoiDung? LayTheoId(int id)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM NguoiDung WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return DocDongData(reader);
            return null;
        }

        public bool Them(NguoiDung nguoiDung)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"INSERT INTO NguoiDung (TenDangNhap, MatKhau, HoTen, VaiTro)
                           VALUES (@TenDangNhap, @MatKhau, @HoTen, @VaiTro)";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@TenDangNhap", nguoiDung.TenDangNhap);
            cmd.Parameters.AddWithValue("@MatKhau", nguoiDung.MatKhau);
            cmd.Parameters.AddWithValue("@HoTen", nguoiDung.HoTen);
            cmd.Parameters.AddWithValue("@VaiTro", nguoiDung.VaiTro);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Sua(NguoiDung nguoiDung)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            string sql = @"UPDATE NguoiDung SET TenDangNhap=@TenDangNhap, MatKhau=@MatKhau,
                           HoTen=@HoTen, VaiTro=@VaiTro WHERE Id=@Id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", nguoiDung.Id);
            cmd.Parameters.AddWithValue("@TenDangNhap", nguoiDung.TenDangNhap);
            cmd.Parameters.AddWithValue("@MatKhau", nguoiDung.MatKhau);
            cmd.Parameters.AddWithValue("@HoTen", nguoiDung.HoTen);
            cmd.Parameters.AddWithValue("@VaiTro", nguoiDung.VaiTro);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int id)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("DELETE FROM NguoiDung WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool KiemTraTenDangNhapTonTai(string tenDangNhap)
        {
            using var conn = DatabaseHelper.TaoKetNoi();
            conn.Open();
            using var cmd = new SqliteCommand("SELECT COUNT(*) FROM NguoiDung WHERE TenDangNhap=@TenDangNhap", conn);
            cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            var count = Convert.ToInt64(cmd.ExecuteScalar() ?? 0);
            return count > 0;
        }

        private static NguoiDung DocDongData(SqliteDataReader reader)
        {
            return new NguoiDung(
                id: Convert.ToInt32(reader["Id"]),
                tenDangNhap: reader["TenDangNhap"].ToString()!,
                matKhau: reader["MatKhau"].ToString()!,
                hoTen: reader["HoTen"].ToString()!,
                vaiTro: reader["VaiTro"].ToString()!
            );
        }
    }
}
