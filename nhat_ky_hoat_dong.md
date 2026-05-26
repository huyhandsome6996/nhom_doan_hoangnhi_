# 📋 NHẬT KÝ HOẠT ĐỘNG DỰ ÁN
## Hệ thống Quản Lý Quán Trà Sữa Take Away (C# WinForms + WebView2 + SQLite)

Tài liệu này ghi nhận chi tiết các bước thiết lập, phát triển, tối ưu hóa mã nguồn và sửa lỗi trong suốt quá trình xây dựng sản phẩm.

---

### 1. THIẾT LẬP MÔI TRƯỜNG & KHỞI TẠO DỰ ÁN

| Thời gian | Tệp / Thành phần | Lệnh Terminal / Công cụ | Mô tả chi tiết & Hoạt động |
| :--- | :--- | :--- | :--- |
| **2026-05-25 20:24** | `.git/` | `git checkout -b feature/US00-khoi-tao-du-an` | Khởi tạo nhánh phát triển tính năng cốt lõi. |
| **2026-05-25 20:25** | `QuanLyTraSua/` | `dotnet new winforms -n QuanLyTraSua -o ./QuanLyTraSua -f net8.0` | Khởi tạo cấu trúc dự án Windows Forms (.NET 8). |
| **2026-05-25 20:25** | `QuanLyTraSua.csproj` | `dotnet add package Microsoft.Data.Sqlite`<br>`dotnet add package Microsoft.Web.WebView2` | Cài đặt các thư viện kết nối cơ sở dữ liệu SQLite và nhân hiển thị Chromium WebView2. |
| **2026-05-25 20:25** | Thư mục dự án | — | Khởi tạo cấu trúc thư mục 3 tầng: `Entities/`, `DAL/`, `GUI/`, `wwwroot/`. |

---

### 2. XÂY DỰNG MÔ HÌNH DỮ LIỆU & LỚP ĐỐI TƯỢNG (ENTITIES)

| Thời gian | Tệp / Lớp | Lệnh Terminal / Công cụ | Mô tả chi tiết & Hoạt động |
| :--- | :--- | :--- | :--- |
| **2026-05-25 20:26** | `Entities/NguoiDung.cs` | — | **Đóng gói (Encapsulation):** Định nghĩa thuộc tính người dùng, ràng buộc kiểm tra tính hợp lệ của `TenDangNhap`, độ dài `MatKhau` và giới hạn các vai trò (`Admin`, `NhanVien`, `KhachHang`). |
| **2026-05-25 20:27** | `Entities/SanPham.cs` | — | **Kế thừa & Đa hình:** Xây dựng lớp cha trừu tượng (`abstract class SanPham`) và 2 lớp con `TraSua`, `Topping`. Override phương thức đa hình `TinhTien()` cộng thêm phụ phí size cho trà sữa. |
| **2026-05-25 20:27** | `Entities/DonHang.cs` | — | **Đóng gói:** Quản lý thông tin đơn hàng, tự động tính tổng tiền từ danh sách các chi tiết đơn hàng tương ứng. |
| **2026-05-25 20:28** | `Entities/ChiTietDonHang.cs` | — | **Đóng gói:** Quản lý thông tin từng món ăn trong giỏ hàng. Ràng buộc số lượng lớn hơn 0 và tự tính thành tiền. |
| **2026-05-25 20:28** | Toàn bộ Entities | `dotnet build` | Biên dịch thử để đảm bảo các lớp đối tượng Entities không lỗi cú pháp C#. |

---

### 3. THIẾT KẾ TẦNG TRUY XUẤT DỮ LIỆU (DAL - DATA ACCESS LAYER)

| Thời gian | Tệp / Lớp | Lệnh Terminal / Công cụ | Mô tả chi tiết & Hoạt động |
| :--- | :--- | :--- | :--- |
| **2026-05-25 20:29** | `DAL/Interfaces/` | — | **Tính Trừu tượng (Abstraction):** Khai báo các interface CRUD hệ thống: `INguoiDungDAL`, `ISanPhamDAL`, `IDonHangDAL`, `IChiTietDonHangDAL`. |
| **2026-05-25 20:29** | `DAL/DatabaseHelper.cs` | — | **Đóng gói hệ thống:** Thiết lập SQLite Connection, tự động khởi tạo database, bảng dữ liệu và thực hiện nạp dữ liệu mẫu (Seed Data) mặc định nếu cơ sở dữ liệu trống. |
| **2026-05-25 20:30** | `DAL/NguoiDungDAL.cs` | — | **Hiện thực hóa Trừu tượng:** Cài đặt interface `INguoiDungDAL` để xử lý kiểm tra tài khoản đăng nhập từ database. |
| **2026-05-25 20:30** | `DAL/SanPhamDAL.cs` | — | **Hiện thực hóa & Đa hình:** Cài đặt `ISanPhamDAL`, sử dụng `SanPhamFactory` tạo đúng đối tượng con `TraSua`/`Topping` khi đọc dữ liệu từ SQLite. |
| **2026-05-25 20:31** | `DAL/DonHangDAL.cs` | — | **Hiện thực hóa Trừu tượng:** Cài đặt `IDonHangDAL` & `IChiTietDonHangDAL`. Áp dụng **SQLite Transaction** để lưu trữ đồng thời đơn hàng và các chi tiết đơn hàng (quan hệ Master-Detail). |
| **2026-05-25 20:31** | Toàn bộ DAL | `dotnet build` | Biên dịch toàn bộ tầng DAL để xác minh tích hợp kết nối database SQLite thành công. |

---

### 4. PHÁT TRIỂN GIAO DIỆN DESKTOP & FRONTEND (GUI & WWWROOT)

| Thời gian | Tệp / Thành phần | Lệnh Terminal / Công cụ | Mô tả chi tiết & Hoạt động |
| :--- | :--- | :--- | :--- |
| **2026-05-25 20:31** | `GUI/MainForm.cs`<br>`Program.cs` | — | Khởi tạo control WebView2, đăng ký chạy ngầm ứng dụng dưới góc màn hình (**System Tray**), bắt sự kiện đóng để ẩn thay vì tắt hẳn. |
| **2026-05-25 20:32** | `wwwroot/css/style.css` | — | Thiết kế hệ thống giao diện Dark Mode hiện đại, sử dụng font chữ `Be Vietnam Pro` cùng hiệu ứng kính mờ (glassmorphism). |
| **2026-05-25 20:32** | `wwwroot/login.html` | — | Thiết kế màn hình đăng nhập, gửi dữ liệu tài khoản/mật khẩu an toàn qua Bridge WebView2. |
| **2026-05-25 20:33** | `wwwroot/order.html` | — | Thiết kế giao diện đặt món cho khách hàng: menu, giỏ hàng, tùy chọn Size & tỷ lệ Đường/Đá. |
| **2026-05-25 20:36** | `wwwroot/admin.html`<br>`wwwroot/js/admin.js` | — | Thiết kế trang quản trị: xem doanh thu, quản lý danh sách đơn hàng, quản lý sản phẩm và phân quyền người dùng. |
| **2026-05-25 20:37** | `QuanLyTraSua.csproj` | — | Thêm cấu hình build tự động copy thư mục `wwwroot` sang thư mục đầu ra (Output) khi biên dịch. |
| **2026-05-25 20:37** | Toàn bộ GUI | `dotnet build` | Biên dịch ứng dụng Desktop để chuẩn bị cho giai đoạn kiểm thử tích hợp. |

---

### 5. TINH CHỈNH & SỬA LỖI HỆ THỐNG (BUG FIXING & OPTIMIZATION)

| Thời gian | Tệp / Thành phần | Lệnh Terminal / Công cụ | Mô tả chi tiết & Hoạt động |
| :--- | :--- | :--- | :--- |
| **2026-05-25 20:38** | `GUI/MainForm.cs` | `dotnet build` | Sửa lỗi biên dịch do sai lệch chuẩn hàm giao tiếp WebView2: Sửa `PostWebMessage` thành `PostWebMessageAsString`. |
| **2026-05-25 20:48** | `QuanLyTraSua.csproj` | `dotnet run` | Nâng cấp cấu hình `TargetFramework` từ `.NET 8` lên `.NET 10-windows` để tương thích hoàn toàn với nền tảng máy chạy thử nghiệm. |
| **2026-05-25 21:05** | `Entities/ChiTietDonHang.cs` | `dotnet test` | Sửa đổi ràng buộc thuộc tính `DonHangId`: Chuyển điều kiện `value <= 0` thành `value < 0` để cho phép gán tạm thời ID = 0 đối với các món ăn đang trong giỏ hàng (chưa lưu cơ sở dữ liệu). Chạy test kiểm chứng 12/12 pass. |
| **2026-05-25 21:05** | `wwwroot/login.html`<br>`wwwroot/order.html` | — | Sửa lỗi redirect đăng nhập bằng cách hỗ trợ kiểm tra linh hoạt cả hai định dạng viết hoa và viết thường của thuộc tính (`id` / `Id` và `vaiTro` / `VaiTro`). |
| **2026-05-25 21:09** | `GUI/MainForm.cs` | — | Khắc phục lỗi kẹt nút đăng nhập "Đang xử lý...": Cấu hình `PropertyNameCaseInsensitive = true` cho bộ biên dịch JSON của C# nhằm nhận diện chính xác dữ liệu viết thường từ JavaScript truyền qua. |
| **2026-05-25 21:12** | `wwwroot/js/bridge.js` | — | **Tích hợp Chế độ Trình duyệt (Fallback Mode):** Tạo cơ chế giả lập Mock Backend bằng cách lưu trữ dữ liệu tạm thời vào `localStorage` của trình duyệt nếu phát hiện ứng dụng đang chạy bên ngoài WebView2. Hỗ trợ chạy thử nghiệm giao diện trực tiếp trên mọi trình duyệt web thông thường mà không cần C#. |
| **2026-05-25 21:12** | `wwwroot/js/admin.js` | — | Khắc phục lỗi vòng lặp truy vấn vô hạn (infinite loop) gửi request liên tục từ Dashboard gây chậm trình duyệt. Chuyển hàm `loadDashboard()` thành `renderDashboard()` khi nhận gói tin cập nhật dữ liệu. |
| **2026-05-25 21:42** | `wwwroot/admin.html`<br>`wwwroot/js/admin.js`<br>`wwwroot/order.html`<br>`wwwroot/js/bridge.js` | `git push origin main` | **Thêm tính năng Đính kèm ảnh sản phẩm cho Admin:** Tích hợp bộ chọn file cục bộ (chuyển đổi Base64) và trường nhập URL ảnh trực tiếp vào modal thêm/sửa sản phẩm. Cập nhật giao diện danh sách Menu của Admin (hiển thị thumbnail) và giao diện đặt món của Khách hàng (hiển thị ảnh thực tế thay cho emoji). |
| **2026-05-25 22:05** | `Entities/NguoiDung.cs`<br>`Entities/SanPham.cs`<br>`Entities/DonHang.cs`<br>`Entities/ChiTietDonHang.cs` | `dotnet test` | **Đơn giản hóa mã nguồn C#:** Chuyển đổi toàn bộ biểu thức Lambda rút gọn và switch expressions sang cấu trúc rẽ nhánh `if-else` truyền thống và khối mã thuộc tính get/set tiêu chuẩn. Bổ sung chú thích tiếng Việt cực kỳ chi tiết từng thuộc tính để phục vụ thuyết trình báo cáo môn học. Kiểm tra Unit Test đạt 12/12 thành công. |
| **2026-05-25 22:05** | `huong_dan_su_dung.md`<br>`cau_truc_va_y_nghia_du_an.md` | `git push origin main` | **Bổ sung tài liệu học thuật:** Viết tài liệu hướng dẫn cài đặt/chạy thử (WinForms & Trình duyệt thường) và sơ đồ cấu trúc/ý nghĩa phân tầng dự án chi tiết kèm phân tích 4 trụ cột OOP phục vụ phản biện đồ án. |
