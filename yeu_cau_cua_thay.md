# 📚 YÊU CẦU CỦA THẦY - TÀI LIỆU BẢO VỆ ĐỒ ÁN
## Hệ thống Quản Lý Quán Trà Sữa Take Away

---

## 1. 🏗️ KIẾN TRÚC 3 TẦNG (3-Tier Architecture)

| Tầng | Thư mục | File chính |
|---|---|---|
| **Entity (Tầng thực thể)** | `QuanLyTraSua/Entities/` | `NguoiDung.cs`, `SanPham.cs`, `DonHang.cs`, `ChiTietDonHang.cs` |
| **DAL (Tầng truy cập dữ liệu)** | `QuanLyTraSua/DAL/` | `DatabaseHelper.cs`, `NguoiDungDAL.cs`, `SanPhamDAL.cs`, `DonHangDAL.cs` |
| **GUI (Tầng giao diện)** | `QuanLyTraSua/GUI/` + `wwwroot/` | `MainForm.cs`, `login.html`, `order.html`, `admin.html`, `js/admin.js` |

---

## 2. 🔒 TÍNH ĐÓNG GÓI (Encapsulation)

**File:** `Entities/NguoiDung.cs` — Dòng **15–65**
```csharp
// Validation ở property MatKhau
public string MatKhau {
    get => _matKhau;
    set {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("...");
        if (value.Length < 4) throw new ArgumentException("...");
        _matKhau = value;
    }
}
```

**File:** `Entities/SanPham.cs` — Dòng **30–40**
```csharp
// Validation GiaCoBan >= 0
public decimal GiaCoBan {
    get => _giaCoBan;
    set {
        if (value < 0) throw new ArgumentException("Giá cơ bản không được âm!");
        _giaCoBan = value;
    }
}
```

**File:** `Entities/ChiTietDonHang.cs` — Dòng **44–52**  
```csharp
// Validation SoLuong > 0
public int SoLuong {
    get => _soLuong;
    set {
        if (value <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0!");
        ...
    }
}
```

---

## 3. 🧬 TÍNH KẾ THỪA (Inheritance)

**File:** `Entities/SanPham.cs`

| Lớp | Dòng | Mối quan hệ |
|---|---|---|
| `SanPham` (abstract) | Dòng **11** | Lớp **cha** |
| `TraSua` | Dòng **90** | **Kế thừa** `SanPham` — `public class TraSua : SanPham` |
| `Topping` | Dòng **125** | **Kế thừa** `SanPham` — `public class Topping : SanPham` |

```csharp
// Dòng 90 - KẾ THỪA
public class TraSua : SanPham { ... }

// Dòng 125 - KẾ THỪA
public class Topping : SanPham { ... }
```

---

## 4. 🎭 TÍNH ĐA HÌNH (Polymorphism)

**File:** `Entities/SanPham.cs`

| Hàm | Lớp | Dòng | Hành vi |
|---|---|---|---|
| `virtual TinhTien()` | `SanPham` | ~Dòng **63** | Hàm gốc trả về GiaCoBan |
| `override TinhTien()` | `TraSua` | ~Dòng **108** | **Cộng thêm 5.000đ** nếu chọn **Size L** |
| `override TinhTien()` | `Topping` | ~Dòng **142** | Giá **cố định**, không phụ thuộc size |

```csharp
// Hàm VIRTUAL ở lớp cha (SanPham.cs ~dòng 63)
public virtual decimal TinhTien(string tuyChon = "") { return GiaCoBan; }

// Override ở TraSua (SanPham.cs ~dòng 108) - CỘNG TIỀN SIZE L
public override decimal TinhTien(string tuyChon = "") {
    decimal gia = GiaCoBan;
    if (tuyChon.Contains("Size L")) gia += PHU_PHI_SIZE_L; // +5000đ
    return gia;
}

// Override ở Topping (SanPham.cs ~dòng 142) - GIÁ CỐ ĐỊNH
public override decimal TinhTien(string tuyChon = "") { return GiaCoBan; }
```

**Đa hình được gọi tại:** `GUI/MainForm.cs` — Hàm `XuLyDatMon()` ~dòng **220**:
```csharp
decimal donGia = sp.TinhTien(tuyChon); // GỌI ĐA HÌNH
```

**Và tại:** `DAL/SanPhamDAL.cs` — Hàm `DocDongData()`:
```csharp
return SanPhamFactory.TaoSanPham(loai, id, ten, gia, hinhAnh, dangBan); // FACTORY + ĐA HÌNH
```

---

## 5. 🔲 TÍNH TRỪU TƯỢNG (Abstraction)

**File:** `DAL/Interfaces/IDALInterfaces.cs` — Toàn bộ file

| Interface | Dòng | Mô tả |
|---|---|---|
| `INguoiDungDAL` | Dòng **8** | Interface CRUD cho NguoiDung |
| `ISanPhamDAL` | Dòng **20** | Interface CRUD cho SanPham |
| `IDonHangDAL` | Dòng **31** | Interface CRUD cho DonHang |
| `IChiTietDonHangDAL` | Dòng **43** | Interface CRUD cho ChiTietDonHang |

Lớp implement: `DAL/NguoiDungDAL.cs`, `DAL/SanPhamDAL.cs`, `DAL/DonHangDAL.cs`

**Lớp abstract:** `Entities/SanPham.cs` — `public abstract class SanPham`

---

## 6. 📋 FORM QUẢN LÝ QUAN HỆ 2 ĐỐI TƯỢNG (Master - Detail)

**File giao diện:** `wwwroot/admin.html` — Modal `chiTietModal` (phần cuối file)  
**File logic:** `wwwroot/js/admin.js` — Hàm `xemChiTiet(donHangId)`

- **Master:** `DonHang` — hiển thị thông tin đơn hàng (khách hàng, thời gian, tổng tiền, trạng thái)
- **Detail:** `ChiTietDonHang` — hiển thị danh sách từng món trong đơn hàng

**Backend:** `DAL/DonHangDAL.cs` — Hàm `LayTheoId(int id)` tự động load cả `DanhSachChiTiet`

---

## 7. 🖥️ ĐẶC ĐIỂM DESKTOP APP

| Yêu cầu | Cách thực hiện | File |
|---|---|---|
| Desktop App (.exe) | WinForms + WebView2 | `GUI/MainForm.cs`, `Program.cs` |
| Giao diện HTML/CSS/JS | Render trong WebView2 qua Virtual Host | `wwwroot/*.html` |
| System Tray | `NotifyIcon` + `DoubleClick` hiện form | `Program.cs` dòng ~30-55 |
| Chạy ngầm | `OnFormClosing` → `Hide()` thay vì thoát | `GUI/MainForm.cs` dòng ~326-334 |
| Không mở Browser | `SetVirtualHostNameToFolderMapping` phục vụ file nội bộ | `GUI/MainForm.cs` dòng ~69-74 |

---

## 8. 🗄️ CƠ SỞ DỮ LIỆU SQLite

**File:** `DAL/DatabaseHelper.cs`

- Schema tự tạo khi khởi động
- Dữ liệu mẫu tự seed: 3 tài khoản + 10 sản phẩm
- DB lưu tại: `%AppData%\QuanLyTraSua\quanlytrasua.db`

---

## 9. 👤 TÀI KHOẢN ĐĂNG NHẬP MẶC ĐỊNH

| Tài khoản | Mật khẩu | Vai trò | Trang sau đăng nhập |
|---|---|---|---|
| `admin` | `admin123` | Admin | `admin.html` |
| `nhanvien1` | `nv123456` | Nhân viên | `admin.html` |
| `khachhang1` | `kh123456` | Khách hàng | `order.html` |
