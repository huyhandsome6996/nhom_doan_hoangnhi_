# 💾 MÃ SQL TẠO BẢNG TỰ ĐỘNG CHO MICROSOFT ACCESS

Trong Microsoft Access, bạn **KHÔNG THỂ** chạy toàn bộ mã SQL cùng một lúc giống như MySQL hay SQL Server. Bạn phải copy và chạy **từng đoạn lệnh một** theo đúng thứ tự dưới đây.

### 📝 Hướng dẫn chạy mã SQL trong Access:
1. Mở file Access của bạn.
2. Trên thanh menu (Ribbon), chọn thẻ **Create** -> nhấn vào **Query Design**.
3. Cửa sổ *Show Table* hiện lên, bạn nhấn **Close (Đóng)** nó đi.
4. Ở góc trên cùng bên trái, nhấn vào chữ **SQL View** (hoặc chuột phải vào tab query chọn SQL View).
5. Copy một đoạn code dưới đây, dán vào cửa sổ SQL và nhấn nút **Run** ❗️ (Dấu chấm than màu đỏ) để chạy.
6. Xóa code cũ đi, copy đoạn tiếp theo và chạy tiếp cho đến khi hết.

---

### BƯỚC 1: TẠO BẢNG NGƯỜI DÙNG (Chạy đầu tiên)
*Copy đoạn code này dán vào SQL View và nhấn Run:*

```sql
CREATE TABLE NguoiDung (
    Id AUTOINCREMENT PRIMARY KEY,
    TenDangNhap VARCHAR(50) NOT NULL,
    MatKhau VARCHAR(50) NOT NULL,
    HoTen VARCHAR(100) NOT NULL,
    VaiTro VARCHAR(20) DEFAULT 'KhachHang'
);
```

---

### BƯỚC 2: TẠO BẢNG SẢN PHẨM (Chạy thứ 2)
*Xóa code cũ, copy đoạn này dán vào và nhấn Run:*

```sql
CREATE TABLE SanPham (
    Id AUTOINCREMENT PRIMARY KEY,
    TenSanPham VARCHAR(150) NOT NULL,
    GiaCoBan CURRENCY NOT NULL,
    Loai VARCHAR(20),
    HinhAnh MEMO,
    DangBan YESNO
);
```

---

### BƯỚC 3: TẠO BẢNG ĐƠN HÀNG VÀ NỐI KHÓA NGOẠI (Chạy thứ 3)
*Bảng này chứa khóa ngoại `KhachHangId` nối tới bảng `NguoiDung`:*

```sql
CREATE TABLE DonHang (
    Id AUTOINCREMENT PRIMARY KEY,
    KhachHangId INTEGER,
    ThoiGianDat DATETIME DEFAULT NOW(),
    TongTien CURRENCY,
    TrangThai VARCHAR(50) DEFAULT 'Chờ xác nhận',
    GhiChu VARCHAR(255),
    CONSTRAINT FK_DonHang_KhachHang FOREIGN KEY (KhachHangId) REFERENCES NguoiDung(Id)
);
```

---

### BƯỚC 4: TẠO BẢNG CHI TIẾT ĐƠN HÀNG VÀ NỐI KHÓA NGOẠI (Chạy cuối cùng)
*Bảng này chứa 2 khóa ngoại nối tới `DonHang` và `SanPham`:*

```sql
CREATE TABLE ChiTietDonHang (
    Id AUTOINCREMENT PRIMARY KEY,
    DonHangId INTEGER,
    SanPhamId INTEGER,
    SoLuong INTEGER NOT NULL,
    DonGiaBan CURRENCY,
    TuyChonThem VARCHAR(255),
    ThanhTien CURRENCY,
    CONSTRAINT FK_ChiTiet_DonHang FOREIGN KEY (DonHangId) REFERENCES DonHang(Id),
    CONSTRAINT FK_ChiTiet_SanPham FOREIGN KEY (SanPhamId) REFERENCES SanPham(Id)
);
```

---
🎉 **HOÀN THÀNH!**
Sau khi chạy xong 4 lệnh trên, bạn hãy vào tab **Database Tools** -> chọn **Relationships**. Kéo cả 4 bảng ra màn hình, bạn sẽ thấy tất cả các đường dây liên kết (Khóa ngoại) đã được tự động nối với nhau cực kỳ chuẩn xác và chuyên nghiệp! 
*(Nếu không thấy bảng, hãy F5 hoặc đóng Access mở lại nhé).*
