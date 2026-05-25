# 📋 NHẬT KÝ HOẠT ĐỘNG DỰ ÁN
## Hệ thống Quản Lý Quán Trà Sữa Take Away

---

| Thời gian | Hành động | Đường dẫn File/Folder | Lệnh Terminal | Chi tiết thay đổi |
|---|---|---|---|---|
| 2026-05-25 20:24 | Khởi tạo nhánh Git | `.git/` | `git checkout -b feature/US00-khoi-tao-du-an` | Tạo nhánh mới để phát triển |
| 2026-05-25 20:25 | Tạo dự án WinForms | `QuanLyTraSua/` | `dotnet new winforms -n QuanLyTraSua -o ./QuanLyTraSua -f net8.0` | Scaffold dự án WinForms .NET 8 |
| 2026-05-25 20:25 | Cài NuGet packages | `QuanLyTraSua.csproj` | `dotnet add package Microsoft.Data.Sqlite; dotnet add package Microsoft.Web.WebView2` | Cài SQLite và WebView2 |
| 2026-05-25 20:25 | Tạo cấu trúc thư mục | `Entities/, DAL/, DAL/Interfaces/, GUI/, wwwroot/` | PowerShell `New-Item -ItemType Directory` | Tạo 3-Tier + wwwroot |
| 2026-05-25 20:26 | Tạo Entity NguoiDung | `Entities/NguoiDung.cs` | — | ĐÓNG GÓI: validation properties TenDangNhap, MatKhau, VaiTro |
| 2026-05-25 20:26 | Tạo Entity SanPham | `Entities/SanPham.cs` | — | ĐÓNG GÓI + KẾ THỪA: lớp cha abstract SanPham; lớp con TraSua, Topping; ĐA HÌNH: virtual TinhTien() |
| 2026-05-25 20:27 | Tạo Entity DonHang | `Entities/DonHang.cs` | — | ĐÓNG GÓI: validation TrangThai, TongTien; quan hệ 1-N với ChiTietDonHang |
| 2026-05-25 20:27 | Tạo Entity ChiTietDonHang | `Entities/ChiTietDonHang.cs` | — | ĐÓNG GÓI: validation SoLuong > 0, ThanhTien >= 0 |
| 2026-05-25 20:27 | Tạo DAL Interfaces | `DAL/Interfaces/IDALInterfaces.cs` | — | TRỪU TƯỢNG: INguoiDungDAL, ISanPhamDAL, IDonHangDAL, IChiTietDonHangDAL |
| 2026-05-25 20:27 | Tạo DatabaseHelper | `DAL/DatabaseHelper.cs` | — | Khởi tạo SQLite, tạo schema, seed dữ liệu mặc định |
| 2026-05-25 20:27 | Tạo NguoiDungDAL | `DAL/NguoiDungDAL.cs` | — | Implement INguoiDungDAL; CRUD NguoiDung |
| 2026-05-25 20:28 | Tạo SanPhamDAL | `DAL/SanPhamDAL.cs` | — | Implement ISanPhamDAL; ĐA HÌNH: SanPhamFactory.TaoSanPham() |
| 2026-05-25 20:28 | Tạo DonHangDAL | `DAL/DonHangDAL.cs` | — | Implement IDonHangDAL + IChiTietDonHangDAL; Transaction khi tạo đơn hàng |
| 2026-05-25 20:31 | Tạo MainForm | `GUI/MainForm.cs` | — | WinForms + WebView2; System Tray hide; message handler |
| 2026-05-25 20:31 | Tạo Program.cs | `Program.cs` | — | Entry point; System Tray Icon; double-click mở form |
| 2026-05-25 20:32 | Tạo CSS design system | `wwwroot/css/style.css` | — | Dark theme, Be Vietnam Pro font, glassmorphism, animations |
| 2026-05-25 20:32 | Tạo trang đăng nhập | `wwwroot/login.html` | — | Form đăng nhập; giao tiếp WebView2 message API |
| 2026-05-25 20:33 | Tạo trang đặt món | `wwwroot/order.html` | — | Giao diện khách hàng; menu grid; giỏ hàng; modal tùy chọn size/đường |
| 2026-05-25 20:36 | Tạo trang admin | `wwwroot/admin.html` | — | Dashboard; quản lý đơn hàng (Master-Detail); CRUD menu; CRUD tài khoản |
| 2026-05-25 20:37 | Tạo admin.js | `wwwroot/js/admin.js` | — | Logic JS toàn bộ admin; filter; modal; message handler |
| 2026-05-25 20:37 | Cập nhật csproj | `QuanLyTraSua.csproj` | — | Thêm copy wwwroot vào output |
| 2026-05-25 20:38 | Fix lỗi build | `GUI/MainForm.cs` | `dotnet build` | Sửa `PostWebMessage` → `PostWebMessageAsString` |
| 2026-05-25 20:38 | **BUILD THÀNH CÔNG** | — | `dotnet build` → **0 errors, 0 warnings** | ✅ Dự án compile OK |
| 2026-05-25 20:48 | Nâng cấp Framework | `QuanLyTraSua.csproj` | — | Đổi `TargetFramework` thành `net10.0-windows` để tương thích runtime máy |
| 2026-05-25 20:48 | Chạy chương trình | — | `dotnet run --project QuanLyTraSua\QuanLyTraSua.csproj` | Khởi chạy ứng dụng Desktop WinForms thành công |
| 2026-05-25 20:51 | Dừng chương trình chạy ngầm | — | Stop-Process / Terminate | Tắt tiến trình ứng dụng để giải phóng tài nguyên và file khóa |
| 2026-05-25 21:05 | Sửa logic validation DonHangId | `Entities/ChiTietDonHang.cs` | — | Sửa kiểm tra `value <= 0` thành `value < 0` để cho phép ID = 0 đối với chi tiết đơn hàng tạm thời (chưa lưu DB) |
| 2026-05-25 21:05 | Sửa lỗi redirect khi đăng nhập | `wwwroot/login.html`, `wwwroot/order.html` | — | Sửa thuộc tính kiểm tra phiên đăng nhập để hỗ trợ cả PascalCase (`Id`, `VaiTro`) và camelCase, sửa lỗi kẹt màn hình "đang xử lý" |
