# 📋 NHẬT KÝ HOẠT ĐỘNG DỰ ÁN
## Hệ thống Quản Lý Quán Trà Sữa Take Away (C# WinForms + WebView2 + SQLite)

Tài liệu này ghi nhận chi tiết các bước thiết lập, phát triển, tối ưu hóa mã nguồn và sửa lỗi trong suốt quá trình xây dựng sản phẩm.

---

### 1. THIẾT LẬP MÔI TRƯỜNG & KHỞI TẠO DỰ ÁN

| Thời gian | File / Thư mục | Lệnh Terminal / Công cụ | Mô tả công việc & Thay đổi |
| :--- | :--- | :--- | :--- |
| **2026-05-25 20:24** | `.git/` | `git checkout -b feature/US00-khoi-tao-du-an` | Khởi tạo nhánh phát triển tính năng cốt lõi. |
| **2026-05-25 20:25** | `QuanLyTraSua/` | `dotnet new winforms -n QuanLyTraSua -o ./QuanLyTraSua -f net8.0` | Khởi tạo cấu trúc dự án Windows Forms (.NET 8). |
| **2026-05-25 20:25** | `QuanLyTraSua.csproj` | `dotnet add package Microsoft.Data.Sqlite`<br>`dotnet add package Microsoft.Web.WebView2` | Cài đặt các thư viện kết nối cơ sở dữ liệu SQLite và nhân hiển thị Chromium WebView2. |
| **2026-05-25 20:25** | Thư mục dự án | *Khởi tạo cấu trúc thư mục 3 tầng:*<br>`Entities/`, `DAL/`, `GUI/`, `wwwroot/` | Tổ chức mã nguồn theo mô hình phân tầng chuẩn để dễ dàng bảo trì và mở rộng. |

---

### 2. XÂY DỰNG MÔ HÌNH DỮ LIỆU & LỚP ĐỐI TƯỢNG (ENTITIES)
*Áp dụng các tính chất cơ bản của lập trình hướng đối tượng (OOP):*

| File / Lớp | Nguyên lý OOP áp dụng | Nội dung chi tiết |
| :--- | :--- | :--- |
| `Entities/NguoiDung.cs` | **Đóng gói (Encapsulation)** | Định nghĩa thuộc tính người dùng. Ràng buộc kiểm tra tính hợp lệ của `TenDangNhap`, độ dài `MatKhau` và giới hạn các vai trò (`Admin`, `NhanVien`, `KhachHang`). |
| `Entities/SanPham.cs` | **Kế thừa & Đa hình** | Xây dựng lớp cha trừu tượng (`abstract class SanPham`). Kế thừa bởi 2 lớp con `TraSua` và `Topping`. Định nghĩa phương thức đa hình `TinhTien(string option)` để tính phụ phí theo size hoặc topping đi kèm. |
| `Entities/DonHang.cs` | **Đóng gói** | Quản lý thông tin đơn hàng, tự động tính tổng tiền từ danh sách các chi tiết đơn hàng tương ứng. |
| `Entities/ChiTietDonHang.cs` | **Đóng gói** | Quản lý thông tin từng món ăn trong giỏ hàng. Ràng buộc số lượng bán lớn hơn 0 và tính toán thành tiền tự động. |

---

### 3. THIẾT KẾ TẦNG TRUY XUẤT DỮ LIỆU (DAL - DATA ACCESS LAYER)

| Đường dẫn File | Tính chất OOP | Chi tiết thiết kế & Hoạt động |
| :--- | :--- | :--- |
| `DAL/Interfaces/IDALInterfaces.cs` | **Tính Trừu tượng (Abstraction)** | Khai báo các interface CRUD hệ thống: `INguoiDungDAL`, `ISanPhamDAL`, `IDonHangDAL`, `IChiTietDonHangDAL` tách biệt hoàn toàn định nghĩa và cài đặt chi tiết. |
| `DAL/DatabaseHelper.cs` | **Đóng gói hệ thống** | Thiết lập SQLite Connection. Tự động chạy câu lệnh tạo bảng (`CREATE TABLE IF NOT EXISTS`) và thực hiện nạp dữ liệu mẫu (Seed Data) mặc định nếu cơ sở dữ liệu trống. |
| `DAL/NguoiDungDAL.cs` | **Hiện thực hóa Trừu tượng** | Triển khai interface `INguoiDungDAL` để xử lý kiểm tra tài khoản, thông tin đăng nhập từ cơ sở dữ liệu. |
| `DAL/SanPhamDAL.cs` | **Hiện thực hóa Trừu tượng** | Triển khai interface `ISanPhamDAL`. Sử dụng mẫu thiết kế Factory (`SanPhamFactory`) để tạo ra đúng đối tượng con `TraSua` hoặc `Topping` tương ứng khi tải dữ liệu từ database lên. |
| `DAL/DonHangDAL.cs` | **Hiện thực hóa Trừu tượng** | Triển khai interface `IDonHangDAL` & `IChiTietDonHangDAL`. Sử dụng **Giao tác SQLite (SQLite Transaction)** bảo đảm an toàn dữ liệu khi thực hiện lưu trữ đồng thời đơn hàng và các chi tiết đơn hàng (quan hệ Master-Detail). |

---

### 4. PHÁT TRIỂN GIAO DIỆN DESKTOP & FRONTEND (GUI & WWWROOT)

| Thời gian | Thành phần phát triển | Mô tả chức năng & Thay đổi giao diện |
| :--- | :--- | :--- |
| **2026-05-25 20:31** | `GUI/MainForm.cs`<br>`Program.cs` | Thiết lập điều khiển WebView2 để render mã nguồn HTML. Đăng ký chạy ngầm dưới góc màn hình (**System Tray**), bắt sự kiện `OnFormClosing` để ẩn ứng dụng thay vì tắt hẳn. |
| **2026-05-25 20:32** | `wwwroot/css/style.css` | Thiết kế hệ thống giao diện tối hiện đại (Dark Mode), sử dụng font chữ `Be Vietnam Pro`, các hiệu ứng kính mờ (glassmorphism) và các chuyển động siêu mượt. |
| **2026-05-25 20:32** | `wwwroot/login.html` | Thiết kế màn hình đăng nhập, truyền tải dữ liệu tài khoản và mật khẩu an toàn qua Bridge kết nối WebView2. |
| **2026-05-25 20:33** | `wwwroot/order.html` | Thiết kế giao diện đặt món cho khách hàng: Hiển thị danh sách menu, giỏ hàng, bảng tùy chọn Size & tỷ lệ Đường/Đá. |
| **2026-05-25 20:36** | `wwwroot/admin.html`<br>`wwwroot/js/admin.js` | Thiết kế trang tổng quản trị: Theo dõi biểu đồ doanh thu, quản lý danh sách đơn hàng, quản lý menu và quản trị phân quyền tài khoản người dùng. |
| **2026-05-25 20:37** | `QuanLyTraSua.csproj` | Thêm thuộc tính build để tự động copy toàn bộ nội dung thư mục `wwwroot` sang thư mục đầu ra (Output Directory) khi biên dịch dự án. |

---

### 5. TINH CHỈNH & SỬA LỖI HỆ THỐNG (BUG FIXING & OPTIMIZATION)

| Thời gian | File sửa lỗi | Lệnh Terminal | Mục đích & Chi tiết xử lý lỗi kỹ thuật |
| :--- | :--- | :--- | :--- |
| **2026-05-25 20:38** | `GUI/MainForm.cs` | `dotnet build` | Sửa lỗi biên dịch do sai lệch chuẩn hàm giao tiếp WebView2: Sửa `PostWebMessage` thành `PostWebMessageAsString`. Dự án build thành công 100% với 0 lỗi và 0 cảnh báo. |
| **2026-05-25 20:48** | `QuanLyTraSua.csproj` | `dotnet run` | Nâng cấp cấu hình `TargetFramework` từ `.NET 8` lên `.NET 10-windows` để tương thích hoàn toàn với nền tảng máy chạy thử nghiệm. |
| **2026-05-25 21:05** | `Entities/ChiTietDonHang.cs` | — | Sửa đổi ràng buộc thuộc tính `DonHangId`: Chuyển điều kiện `value <= 0` thành `value < 0` để cho phép gán tạm thời ID = 0 đối với các món ăn đang trong giỏ hàng (chưa lưu cơ sở dữ liệu). |
| **2026-05-25 21:05** | `wwwroot/login.html`<br>`wwwroot/order.html` | — | Sửa lỗi redirect đăng nhập bằng cách hỗ trợ kiểm tra linh hoạt cả hai định dạng viết hoa và viết thường của thuộc tính (`id` / `Id` và `vaiTro` / `VaiTro`). |
| **2026-05-25 21:09** | `GUI/MainForm.cs` | — | Khắc phục lỗi kẹt nút đăng nhập "Đang xử lý...": Cấu hình `PropertyNameCaseInsensitive = true` cho bộ biên dịch JSON của C# nhằm nhận diện chính xác dữ liệu viết thường từ JavaScript truyền qua. |
| **2026-05-25 21:12** | `wwwroot/js/bridge.js` | — | **Tích hợp Chế độ Trình duyệt (Fallback Mode):** Tạo cơ chế giả lập Mock Backend bằng cách lưu trữ dữ liệu tạm thời vào `localStorage` của trình duyệt nếu phát hiện ứng dụng đang chạy bên ngoài WebView2. Hỗ trợ chạy thử nghiệm giao diện trực tiếp trên mọi trình duyệt web thông thường mà không cần C#. |
| **2026-05-25 21:12** | `wwwroot/js/admin.js` | — | Khắc phục lỗi vòng lặp truy vấn vô hạn (infinite loop) gửi request liên tục từ Dashboard gây chậm trình duyệt. Chuyển hàm `loadDashboard()` thành `renderDashboard()` khi nhận gói tin cập nhật dữ liệu. |
