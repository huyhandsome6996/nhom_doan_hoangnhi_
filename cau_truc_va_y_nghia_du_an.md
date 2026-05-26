# 📁 CẤU TRÚC VÀ Ý NGHĨA THÀNH PHẦN DỰ ÁN

Tài liệu này phân tích chi tiết cấu trúc thư mục, chức năng của từng file mã nguồn, và cách các nguyên lý lập trình hướng đối tượng (OOP) được áp dụng để bạn dễ dàng báo cáo trước giáo viên.

---

## 🌳 1. SƠ ĐỒ CẤU TRÚC THƯ MỤC

```text
nhom_doan_hoangnhi_/
├── QuanLyTraSua/                # Mã nguồn chính của dự án WinForms C#
│   ├── DAL/                     # Tầng truy cập dữ liệu (Data Access Layer)
│   │   ├── Interfaces/          # Các giao diện trừu tượng (Interface) định nghĩa CRUD
│   │   │   └── IDALInterfaces.cs
│   │   ├── DatabaseHelper.cs    # Khởi tạo kết nối SQLite và tạo bảng/nạp dữ liệu mẫu
│   │   ├── DonHangDAL.cs        # Thực thi lưu/đọc hóa đơn và chi tiết hóa đơn
│   │   ├── NguoiDungDAL.cs      # Thực thi nghiệp vụ đăng nhập & CRUD người dùng
│   │   └── SanPhamDAL.cs        # Thực thi CRUD sản phẩm trà sữa & topping
│   ├── Entities/                # Tầng mô hình dữ liệu (Business Objects / Entities)
│   │   ├── SanPham.cs           # Lớp cha trừu tượng (abstract class) chứa thông tin chung
│   │   ├── TraSua.cs            # Lớp con Trà Sữa (Kế thừa từ SanPham)
│   │   ├── Topping.cs           # Lớp con Topping (Kế thừa từ SanPham)
│   │   ├── NguoiDung.cs         # Lớp thông tin tài khoản người dùng
│   │   ├── DonHang.cs           # Lớp hóa đơn
│   │   └── ChiTietDonHang.cs    # Lớp chi tiết mặt hàng trong hóa đơn
│   ├── GUI/                     # Tầng Giao diện Windows Forms
│   │   ├── MainForm.cs          # Form chính chứa WebView2 để kết nối HTML/JS với C#
│   │   └── MainForm.Designer.cs # Thiết kế layout kéo thả của Form chính
│   ├── wwwroot/                 # Thư mục chứa giao diện Web chạy trong WebView2
│   │   ├── css/
│   │   │   └── style.css        # Thiết kế giao diện (Dark Mode, màu cam chủ đạo)
│   │   ├── js/
│   │   │   ├── admin.js         # Logic trang quản lý (Dashboard, Đơn hàng, Menu, User)
│   │   │   └── bridge.js        # Cầu nối giả lập chạy được trên cả Trình duyệt thường
│   │   ├── admin.html           # Trang quản trị dành cho Admin & Nhân viên
│   │   ├── login.html           # Trang đăng nhập tài khoản
│   │   └── order.html           # Trang gọi món dành cho Khách hàng
│   ├── Program.cs               # Điểm khởi chạy chương trình (Main)
│   └── QuanLyTraSua.csproj      # File cấu hình build dự án C#
├── QuanLyTraSua.Tests/          # Dự án chạy kiểm thử tự động (Unit Test xUnit)
├── nhat_ky_hoat_dong.md         # Nhật ký ghi nhận các câu lệnh và công việc đã thực hiện
├── huong_dan_su_dung.md         # Tài liệu cấu hình tài khoản và hướng dẫn chạy chương trình
└── cau_truc_va_y_nghia_du_an.md # Tệp này (Phục vụ báo cáo thuyết trình)
```

---

## 🔍 2. Ý NGHĨA CHI TIẾT TỪNG THƯ MỤC VÀ TỆP

### 1. Thư mục `Entities/` (Lớp Đối Tượng)
Chứa các đối tượng cốt lõi đại diện cho các thực thể ngoài đời thực của quán trà sữa:
* **`NguoiDung.cs`**: Lưu thông tin nhân viên, admin và khách hàng.
* **`SanPham.cs`**: Định nghĩa thông tin chung của sản phẩm (ID, tên, giá bán, loại, ảnh).
  * *OOP áp dụng:* **Tính trừu tượng (Abstraction)** - được khai báo là một lớp `abstract`.
* **`TraSua.cs` & `Topping.cs`**: 
  * *OOP áp dụng:* **Kế thừa (Inheritance)** - kế thừa từ lớp `SanPham`.
  * *OOP áp dụng:* **Đa hình (Polymorphism)** - override phương thức `TinhTien()` (Trà sữa có phụ phí size, còn Topping thì giữ nguyên giá cơ bản).
* **`DonHang.cs`**: Đại diện cho 1 hóa đơn đặt trà sữa. Tự động tính tổng tiền bằng cách cộng dồn danh sách chi tiết đơn.
* **`ChiTietDonHang.cs`**: Lưu thông tin cụ thể (món nào, số lượng bao nhiêu, giá tiền bao nhiêu) trong một hóa đơn.

### 2. Thư mục `DAL/` (Tầng Truy Xuất Dữ Liệu)
Làm việc trực tiếp với file Cơ sở dữ liệu SQLite:
* **`Interfaces/IDALInterfaces.cs`**:
  * *OOP áp dụng:* **Tính trừu tượng (Abstraction)** - chứa các interface cam kết các hàm CRUD sẽ phải thực thi mà không quan tâm bên dưới là database gì (SQL Server, SQLite hay MySQL).
* **`DatabaseHelper.cs`**: Tạo file database `trasua.db`. Nếu mở lên thấy trống, nó sẽ tự động chạy lệnh tạo bảng và nạp tài khoản, món trà sữa mẫu để tránh bị lỗi trống dữ liệu.
* **`NguoiDungDAL.cs`, `SanPhamDAL.cs`, `DonHangDAL.cs`**: Thực hiện các thao tác đọc ghi dữ liệu như `SELECT`, `INSERT`, `UPDATE`, `DELETE`.
  * *Ví dụ Đa hình:* Trong `SanPhamDAL.cs`, hàm `DocDongData` sử dụng `SanPhamFactory.TaoSanPham` để tự sinh ra đối tượng `TraSua` hoặc `Topping` tùy theo loại đọc được từ SQLite.

### 3. Thư mục `GUI/` (Tầng Giao Diện C# WinForms)
* **`MainForm.cs`**: Chứa điều khiển **WebView2** (nhân Chromium tương tự trình duyệt Edge). Thay vì vẽ nút bấm xấu xí của Windows ngày xưa, C# chỉ đóng vai trò làm Backend chạy ngầm kết nối cơ sở dữ liệu và mở một cửa sổ hiển thị file HTML/CSS/JS tuyệt đẹp.
  * C# nhận thông điệp JSON từ JavaScript gửi lên (như yêu cầu `dangNhap`, `laySanPham`, `datMon`) để tương tác với cơ sở dữ liệu SQLite, sau đó gửi trả kết quả phản hồi lại giao diện.

### 4. Thư mục `wwwroot/` (Giao Diện Frontend HTML/CSS/JS)
Chứa trang web chạy bên trong cửa sổ WebView2:
* **`login.html`**: Trang đăng nhập trực quan, xử lý hiệu ứng nhập tài khoản.
* **`order.html`**: Trang chọn món, cho phép khách hàng nhấn chọn món, chỉnh sửa đường, đá, size, và đặt hàng.
* **`admin.html`**: Giao diện quản trị viên gồm bảng theo dõi thống kê doanh thu, quản lý tình trạng đơn hàng và kho hàng.
* **`js/bridge.js`**: File thông minh giả lập cơ chế giao tiếp của WebView2. Nếu bạn nhấp đúp chạy trực tiếp HTML trên Chrome, file này sẽ giả lập SQLite thành `localStorage` của trình duyệt giúp chạy thử cực kỳ mượt mà.
* **`js/admin.js`**: Chứa logic xử lý các hoạt động nút bấm trên trang Admin.

---

## 🎓 3. CÁC NGUYÊN LÝ OOP ÁP DỤNG ĐỂ GIẢI THÍCH CHO THẦY CÔ

Khi thầy cô hỏi: *"Em đã áp dụng lập trình hướng đối tượng như thế nào trong đồ án này?"*, bạn hãy trả lời như sau:

1. **Đóng gói (Encapsulation):**
   * Các thuộc tính của lớp đều được đặt trong thuộc tính `{ get; set; }` có kiểm tra giá trị (Ví dụ: `ChiTietDonHang.SoLuong` kiểm tra nếu gán giá trị `<= 0` sẽ ném ra ngoại lệ). Các dữ liệu nhạy cảm được đóng gói ẩn đi, chỉ truy cập qua hàm.

2. **Kế thừa (Inheritance):**
   * Em tạo lớp cha `SanPham` chứa các thuộc tính chung như `Id`, `TenSanPham`, `GiaCoBan`. Hai lớp con `TraSua` và `Topping` kế thừa từ `SanPham` để sử dụng lại mã nguồn mà không cần khai báo lại các thuộc tính đó.

3. **Đa hình (Polymorphism):**
   * Thể hiện ở phương thức `abstract decimal TinhTien(string option)` trong lớp cha `SanPham`. Lớp con `TraSua` cài đặt lại phương thức này để cộng thêm `5.000đ` nếu khách chọn `Size L`, còn lớp con `Topping` cài đặt phương thức này trả về đúng giá trị cơ bản gốc.
   * Ngoài ra còn áp dụng thông qua Factory Pattern (`SanPhamFactory.TaoSanPham`) để tự động tạo đúng đối tượng lớp con tùy theo dữ liệu đọc từ SQLite.

4. **Trừu tượng (Abstraction):**
   * Em sử dụng các `interface` như `ISanPhamDAL`, `IDonHangDAL` ở tầng dữ liệu. Tầng GUI chỉ gọi thông qua giao diện này để lấy dữ liệu mà không cần biết chi tiết truy vấn SQLite viết như thế nào. Giúp hệ thống dễ nâng cấp sau này.
