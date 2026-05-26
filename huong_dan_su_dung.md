# 📖 HƯỚNG DẪN CẤU HÌNH & SỬ DỤNG DỰ ÁN

Tài liệu này hướng dẫn cách cấu hình, cài đặt và chạy thử nghiệm hệ thống **Quản Lý Quán Trà Sữa Take Away** dành cho người mới clone dự án về máy.

---

## 🛠️ 1. YÊU CẦU PHẦN MỀM (PREREQUISITES)

Để chạy được dự án này trên máy tính cá nhân, bạn cần cài đặt:
1. **.NET SDK v10.0** hoặc tối thiểu **.NET SDK v8.0** (Tải từ [trang chủ Microsoft](https://dotnet.microsoft.com/download)).
2. **Microsoft Edge WebView2 Runtime** (Hầu hết Windows 10/11 đã cài sẵn. Nếu chưa có, tải từ trang chủ Microsoft WebView2).
3. **Visual Studio 2022** (có chọn Cài đặt phát triển Desktop với .NET) hoặc **VS Code**.

---

## 🚀 2. HƯỚNG DẪN CHẠY DỰ ÁN

Dự án này rất linh hoạt, hỗ trợ chạy theo **2 CÁCH** khác nhau:

### 💡 CÁCH 1: Chạy trực tiếp trên trình duyệt Web (Không cần cài .NET/C#)
Do đã được tích hợp thư viện **Mock Backend (`bridge.js`)**, bạn có thể chạy thử toàn bộ giao diện mà không cần chạy code C#:
1. Mở thư mục dự án đã clone về máy.
2. Tìm đến thư mục `QuanLyTraSua/wwwroot/`.
3. Nhấp đúp chuột vào tệp `login.html` (chạy bằng Google Chrome, Edge, Firefox, v.v.).
4. Hệ thống sẽ tự động chuyển sang chế độ giả lập lưu dữ liệu vào `localStorage` của trình duyệt. Bạn có thể đăng nhập, đặt món, quản lý sản phẩm và đơn hàng như thật!

### 💻 CÁCH 2: Chạy ứng dụng WinForms Desktop (Sử dụng C# & SQLite)
Dành cho việc kiểm tra sản phẩm hoàn thiện cuối cùng:
1. Mở cửa sổ dòng lệnh Terminal/PowerShell tại thư mục chính chứa dự án.
2. Chạy lệnh khôi phục gói thư viện:
   ```bash
   dotnet restore
   ```
3. Chạy lệnh khởi động ứng dụng:
   ```bash
   dotnet run --project QuanLyTraSua
   ```
4. Giao diện Desktop sẽ hiển thị. Đồng thời, một biểu tượng icon trà sữa sẽ xuất hiện ở khay hệ thống (System Tray - dưới góc phải màn hình bên cạnh đồng hồ).
   * *Mẹo:* Khi bạn ấn nút đóng (X), ứng dụng sẽ thu nhỏ chạy ẩn dưới khay hệ thống. Để thoát hẳn, nhấp chuột phải vào icon ở khay hệ thống và chọn **Thoát**.

---

## 🔑 3. TÀI KHOẢN ĐĂNG NHẬP MẪU (TEST ACCOUNTS)

Hệ thống đã có sẵn dữ liệu mẫu trong cơ sở dữ liệu để kiểm thử nhanh:

| Tài khoản | Mật khẩu | Vai trò | Chức năng kiểm thử |
| :--- | :--- | :--- | :--- |
| `admin` | `admin123` | **Quản Trị Viên (Admin)** | Xem báo cáo doanh thu, quản lý danh sách đơn hàng, thêm/sửa/xóa món trong menu (bao gồm thêm ảnh sản phẩm), quản lý tài khoản nhân viên. |
| `nhanvien1` | `nv123456` | **Nhân viên phục vụ** | Theo dõi đơn hàng mới, xác nhận đơn, chuyển trạng thái chế biến và hoàn thành đơn hàng. |
| `khachhang1` | `kh123456` | **Khách hàng** | Xem menu trực quan có hình ảnh, thêm món vào giỏ hàng, chọn size (S/M/L) và mức đường/đá, ghi chú đơn hàng và gửi yêu cầu đặt món. |

---

## 📝 4. HƯỚNG DẪN DÙNG CÁC TÍNH NĂNG CHÍNH

### A. Đặt món (Khách hàng)
1. Đăng nhập tài khoản `khachhang1`.
2. Lướt menu trà sữa và topping. Click vào một món.
3. Trong bảng tùy chọn hiện ra, chọn Size (Mặc định, Size L +5.000đ), lượng Đường/Đá và nhập ghi chú.
4. Click **Thêm vào giỏ**.
5. Nhìn sang giỏ hàng bên phải, điều chỉnh số lượng nếu muốn và nhấn **Thanh toán**.

### B. Duyệt đơn hàng (Nhân viên / Admin)
1. Đăng nhập tài khoản `nhanvien1` hoặc `admin`.
2. Tại màn hình chính, đơn hàng mới đặt từ khách hàng sẽ xuất hiện ở mục **Chờ xác nhận**.
3. Nhân viên click **Xác nhận** -> Đơn hàng chuyển sang trạng thái **Đang pha chế**.
4. Sau khi pha chế xong, nhấn **Hoàn thành** -> Đơn hàng đóng lại và tính doanh thu thành công.

### C. Quản lý menu & Ảnh món ăn (Admin)
1. Đăng nhập tài khoản `admin`.
2. Chọn menu **Quản lý Menu** ở cột bên trái.
3. Để thêm món: Click **Thêm Sản Phẩm**. Để sửa món: Click **✏️ Sửa** ở món tương ứng.
4. Tại form nhập liệu:
   * Bạn có thể nhấn **Chọn file từ máy...** để chọn một bức ảnh từ máy tính (ảnh sẽ tự động hiển thị preview).
   * Hoặc bạn có thể dán link ảnh dạng URL vào mục **Link ảnh**.
5. Nhấn **Lưu** để cập nhật. Món ăn sẽ được cập nhật hình ảnh trên menu của khách hàng ngay lập tức!
