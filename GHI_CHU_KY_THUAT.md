# Ghi chú kỹ thuật

Hướng dẫn mở ứng dụng nằm trong [README](README.md). Tệp này dành cho người cần xem cách tổ chức và phạm vi kiểm tra.

- Windows Forms, .NET Framework 4.7.2, SQL Server, Microsoft.Data.SqlClient và Crystal Reports.
- `KhoiTao.ps1` tạo database mẫu mới bằng tài khoản Windows đang chạy script. Database đã tồn tại được chặn; chỉ tái sử dụng khi truyền `-SuDungDuLieuHienCo` và cấu trúc đúng.
- Kết nối nằm ở `Database.Local.config` trong thư mục project, được sao chép khi build và bị Git bỏ qua. Biến môi trường `QL_NGOAINGU_CONNECTION_STRING` có thể ghi đè kết nối khi cần triển khai riêng.
- Có thể chọn tên database khác bằng tham số `-Database` của script.
- Schema/dữ liệu mẫu nằm ở `database/01_KhoiTao.sql`. Tệp SQL cũ chỉ thông báo dùng script khởi tạo, không xóa database nữa.
- Thư viện Crystal managed dùng để tham chiếu nằm ở `lib/CrystalReports` trong project; vẫn cần cài runtime native phù hợp.
- Bản build cũ, `bin`, `obj` và cache Visual Studio được bỏ khỏi phiên bản hiện hành.

Đã kiểm tra biên dịch C# với thư viện tham chiếu Framework 4.7.2, tạo database sạch, đọc dữ liệu bằng lớp kết nối đã biên dịch, tái sử dụng database và chặn ghi đè. Chưa kiểm thử toàn bộ giao diện hoặc chức năng in với Crystal runtime trên máy Windows mới hoàn toàn.

Mật khẩu SQL của máy cũ đã được bỏ khỏi mã hiện hành nhưng vẫn có thể nằm trong lịch sử Git. Nếu còn dùng mật khẩu đó trên SQL Server thật, cần đổi tại máy chủ. Tài khoản mẫu trong README chỉ dành cho học tập.
