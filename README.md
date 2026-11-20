# Cách chạy

**Máy cần có:** Visual Studio với **.NET desktop development**, .NET Framework 4.7.2 Developer Pack, SQL Server và Crystal Reports phù hợp khi in báo cáo.

1. Tải và giải nén dự án vào đường dẫn ngắn, ví dụ `D:\NET`.
2. Mở thư mục có `KhoiTao.ps1`, gõ `powershell` vào thanh địa chỉ, nhấn Enter rồi chạy:

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File .\KhoiTao.ps1 -SqlServer '.\SQLEXPRESS'
   ```

   Đổi `.\SQLEXPRESS` theo tên SQL Server của máy. Nếu database của dự án đã có, thêm `-SuDungDuLieuHienCo` vào cuối lệnh.
3. Mở `Nhom09_QuanLyThiTaiTrungTamNgoaiNgu/demo/Nhom09_QLyNgoaiNgu/Nhom09_QLyNgoaiNgu.sln` bằng Visual Studio. Nếu thiếu gói, chuột phải Solution → **Restore NuGet Packages**.
4. Nhấn **F5**. Đăng nhập mẫu: **`admin` / `admin123`**.

**Lần sau:** chỉ mở `.sln` rồi F5. Giữ tệp `Database.Local.config` đã tạo. Dữ liệu đi kèm là dữ liệu mẫu; không tự nạp đè database đang dùng.