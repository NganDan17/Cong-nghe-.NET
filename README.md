# Quản lý thi tại trung tâm ngoại ngữ

Ứng dụng Windows Forms dùng .NET Framework 4.7.2 và SQL Server.

## Chuẩn bị một lần trên máy mới

- Cài Visual Studio với **.NET desktop development** và **.NET Framework 4.7.2 development tools**.
- Cài SQL Server/SQL Server Express. Tài khoản Windows chạy khởi tạo cần được phép tạo database trên SQL Server.
- Cài SAP Crystal Reports for Visual Studio và runtime x86/x64 phù hợp khi dùng chức năng in báo cáo. Các DLL managed có trong source không thay thế bộ runtime native.
- Tải/clone vào đường dẫn ngắn, ví dụ `D:\NET`. Tránh thư mục Downloads lồng nhiều cấp.

## Khởi tạo và chạy

Mở PowerShell tại thư mục gốc kho và chạy:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\KhoiTao.ps1 -SqlServer '.\SQLEXPRESS'
```

Đổi `SqlServer` theo tên instance thật của máy nhận. Script tạo database mẫu `QL_NGOAINGU` và ghi kết nối Windows Authentication vào `Database.Local.config` ở thư mục project. Script không xóa database cũ, không đặt mật khẩu `sa` và không in thông tin xác thực.

Nếu database đúng của dự án đã có sẵn, dùng:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\KhoiTao.ps1 -SqlServer '.\SQLEXPRESS' -SuDungDuLieuHienCo
```

Mở `Nhom09_QuanLyThiTaiTrungTamNgoaiNgu/demo/Nhom09_QLyNgoaiNgu/Nhom09_QLyNgoaiNgu.sln` bằng Visual Studio, Restore NuGet Packages nếu cần, Build rồi **F5**. `Ctrl+Shift+B` trong Visual Studio chỉ build, không chạy ứng dụng.

Có thể chọn database riêng bằng `-Database TenDatabase`. Để dùng SQL login thay Windows Authentication, sửa tệp cấu hình cục bộ đã tạo hoặc đặt biến môi trường `QL_NGOAINGU_CONNECTION_STRING`. Không commit tệp cấu hình cục bộ chứa mật khẩu.

Tài khoản đăng nhập ứng dụng là tài khoản mẫu trong `database/01_KhoiTao.sql`, khác tài khoản Windows/SQL Server. Chỉ dùng dữ liệu này cho học tập; đổi mật khẩu mẫu trước khi dùng thật.

## Những điểm đã sửa

- Đọc kết nối từ cấu hình cục bộ, bỏ tên máy và mật khẩu SQL ghi trong mã C#.
- Khởi tạo schema/dữ liệu mẫu qua script có kiểm tra database đã tồn tại.
- Không chạy lại lệnh xóa database từ tệp SQL cũ.
- Giữ thư viện Crystal cần để tham chiếu ở `lib/CrystalReports`; bỏ cache Visual Studio và bản build cũ khỏi phiên bản mới.

Máy chỉ cần khởi tạo một lần; các lần sau mở solution rồi F5. Muốn nạp lại dữ liệu cần thực hiện riêng có sao lưu, script không tự ghi đè.

Mật khẩu SQL của máy cũ đã được bỏ khỏi mã hiện hành nhưng có thể vẫn nằm trong lịch sử Git. Nếu còn dùng mật khẩu đó trên SQL Server thật, cần đổi tại máy chủ.
