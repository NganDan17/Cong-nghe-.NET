# Cách chạy dự án Quản lý thi ngoại ngữ

Làm lần lượt từ bước 1 đến bước 6 dưới đây trên máy Windows. Các bước cài đặt và tạo dữ liệu chỉ cần làm lần đầu. Những lần sau xem mục **Mở lại vào lần sau** ở cuối trang.

## Bước 1 — Cài phần mềm

Nếu máy đã có phần mềm nào thì bỏ qua phần đó.

| Phần mềm | Cách cài |
|---|---|
| [Visual Studio 2022 Community](https://aka.ms/vs/17/release/vs_community.exe) | Mở bộ cài, tích **.NET desktop development**, rồi bấm **Install**. Đây là Visual Studio, khác Visual Studio Code. |
| [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) | Trên trang tải, chọn **Developer Pack**, tải về và cài. Nếu báo máy đã có thì bỏ qua. |
| [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) | Tìm mục **Express**, tải bộ cài, mở lên, chọn **Basic** và cài đến khi hoàn tất. |
| [SQL Server Management Studio — SSMS](https://learn.microsoft.com/en-us/ssms/install/install) | Tải và cài theo nút tải trên trang Microsoft. Đây là chương trình dùng để kết nối SQL Server. |
| [SAP Crystal Reports for Visual Studio](https://userapps.support.sap.com/sap/support/knowledge/en/2977776) | Cài bản dành cho **Visual Studio 2022** và bộ **runtime 32-bit (x86)** để dùng màn hình in báo cáo. Ở bước 5 sẽ chọn ứng dụng chạy x86 tương ứng. |

Nếu bộ cài yêu cầu khởi động lại Windows, hãy khởi động lại trước khi làm tiếp.

## Bước 2 — Kiểm tra SQL Server

1. Mở **SQL Server Management Studio**.
2. Ở cửa sổ kết nối, chọn:
   - **Server type:** `Database Engine`.
   - **Server name:** `.\SQLEXPRESS` nếu bạn vừa cài Express theo kiểu Basic.
   - **Authentication:** `Windows Authentication`.
3. Bấm **Connect**. Nếu có lỗi chứng chỉ, tích **Trust server certificate** rồi kết nối lại.
4. Kết nối thành công là khi bên trái hiện máy chủ và thư mục **Databases**.

**Ghi lại tên trong ô Server name.** Lệnh ở bước 4 dùng `.\SQLEXPRESS`. Nếu bạn kết nối bằng tên khác, thay đúng phần đó trong lệnh bằng tên vừa dùng.

## Bước 3 — Tải và mở đúng thư mục dự án

1. Trên trang GitHub của dự án, bấm nút **Code** → **Download ZIP**.
2. Giải nén ở đường dẫn ngắn, ví dụ `D:\NET`.
3. Mở thư mục đã giải nén. Thư mục đúng phải nhìn thấy **cả `README.md` và `KhoiTao.ps1` cùng một chỗ**.
4. Nếu GitHub tạo thêm thư mục `Cong-nghe-.NET-master`, mở tiếp thư mục đó để tìm hai tệp trên.

## Bước 4 — Tạo dữ liệu mẫu

1. Trong thư mục có `KhoiTao.ps1`, bấm vào **thanh địa chỉ** của File Explorer.
2. Gõ `powershell`, nhấn **Enter**. Một cửa sổ PowerShell sẽ mở tại đúng thư mục này.
3. Sao chép nguyên lệnh dưới đây, dán vào PowerShell rồi nhấn **Enter**:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\KhoiTao.ps1 -SqlServer '.\SQLEXPRESS'
```

4. Đợi đến khi thấy **`Da luu cau hinh rieng tai:`** và không có dòng lỗi màu đỏ. Giữ lại tệp cấu hình mà lệnh vừa tạo.

Lệnh này tạo sẵn dữ liệu mẫu để bạn đăng nhập và dùng thử. Bạn không cần tự tạo bảng hay tự nhập dữ liệu bằng tay.

**Nếu báo `Database QL_NGOAINGU da ton tai`:** nếu đây đúng là dữ liệu cũ của dự án, chạy lệnh sau để sử dụng lại:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\KhoiTao.ps1 -SqlServer '.\SQLEXPRESS' -SuDungDuLieuHienCo
```

Nếu chưa chắc database đó là gì, giữ nguyên và gửi nội dung lỗi để được kiểm tra.

## Bước 5 — Mở chương trình

1. Mở lần lượt các thư mục:

   `Nhom09_QuanLyThiTaiTrungTamNgoaiNgu` → `demo` → `Nhom09_QLyNgoaiNgu`.

2. Bấm đúp tệp **`Nhom09_QLyNgoaiNgu.sln`** để mở bằng Visual Studio 2022.
3. Trong **Solution Explorer** bên phải, bấm chuột phải vào dòng trên cùng có chữ **Solution** → **Restore NuGet Packages**, rồi đợi hoàn tất.
4. Bấm chuột phải vào project **Nhom09_QLyNgoaiNgu** → **Set as Startup Project**.
5. Bấm chuột phải project đó → **Properties** → **Build** → đặt **Platform target** thành **x86**, rồi lưu. Cách này dùng cùng bộ Crystal runtime x86 đã cài ở bước 1.
6. Nhấn **F5** hoặc bấm nút **Start** hình tam giác xanh. Cửa sổ đăng nhập sẽ mở.

Nếu bấm `Ctrl + Shift + B`, Visual Studio chỉ kiểm tra/build mã; muốn mở ứng dụng thì bấm **F5**.

## Bước 6 — Đăng nhập dùng thử

Các tài khoản sau có khi bạn tạo dữ liệu mẫu mới ở bước 4:

| Loại tài khoản | Tên đăng nhập | Mật khẩu mẫu |
|---|---|---|
| Quản trị | `admin` | `admin123` |
| Giám thị | `giamthi01` | `giamthi123` |
| Học viên | `hocvien_an` | `123` |

Nhập vào cửa sổ **đăng nhập của ứng dụng**, không nhập các tài khoản này vào SSMS. Nếu dùng database cũ đã đổi mật khẩu, hãy dùng mật khẩu của database cũ.

## Mở lại vào lần sau

Mở `Nhom09_QLyNgoaiNgu.sln` bằng Visual Studio rồi nhấn **F5**. Giữ dịch vụ SQL Server hoạt động và giữ tệp `Database.Local.config` đã tạo. Không cần làm lại bước 4 mỗi lần mở.

## Nếu gặp lỗi

| Bạn thấy gì? | Làm gì tiếp? |
|---|---|
| Không tìm thấy `KhoiTao.ps1` | Quay lại bước 3, mở đúng thư mục có cả README và KhoiTao rồi mở PowerShell lại. |
| Không kết nối được SQL Server | Kiểm tra có Connect được trong SSMS ở bước 2 không; dùng đúng Server name đó trong lệnh bước 4. |
| `CREATE DATABASE permission denied` | Tài khoản Windows đang dùng chưa được SQL Server cho phép tạo dữ liệu. Nhờ người cài SQL Server cấp quyền khởi tạo cho tài khoản đó. |
| `MSB3644` hoặc thiếu `.NET Framework 4.7.2` | Cài **Developer Pack 4.7.2** ở bước 1, sau đó mở lại Visual Studio. |
| Thiếu thư viện NuGet | Thực hiện **Restore NuGet Packages** ở bước 5; máy cần internet khi tải gói còn thiếu. |
| Lỗi `CrystalDecisions`, `ReportAppServer`, `crpe32.dll` hoặc `BadImageFormat` khi in | Kiểm tra đã cài bộ Crystal runtime x86 và đã chọn **Platform target = x86**. |
| `Filename too long` hoặc thiếu tệp sau giải nén | Giải nén lại vào đường dẫn ngắn như `D:\NET`. |

Tệp `Database.Local.config` là cấu hình riêng của máy; giữ nó trên máy và ngoài GitHub. Các tài khoản ở trên chỉ là tài khoản mẫu cho học tập.

[Thông tin kỹ thuật và phạm vi kiểm thử](GHI_CHU_KY_THUAT.md).