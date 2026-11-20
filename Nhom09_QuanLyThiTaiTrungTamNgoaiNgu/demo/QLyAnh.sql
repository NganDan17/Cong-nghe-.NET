
USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QL_NGOAINGU')
BEGIN
    ALTER DATABASE QL_NGOAINGU SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QL_NGOAINGU;
END
GO

CREATE DATABASE QL_NGOAINGU;
GO
USE QL_NGOAINGU;
GO

CREATE TABLE QuyenHan (
    MaQuyen INT PRIMARY KEY IDENTITY(1,1),
    TenQuyen NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE TaiKhoan (
    MaTaiKhoan INT PRIMARY KEY IDENTITY(1,1),
    TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
    MatKhau VARCHAR(50) NOT NULL, 
    Email VARCHAR(100) UNIQUE,
    MaQuyen INT NOT NULL,
    TrangThai BIT DEFAULT 1,
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaQuyen) REFERENCES QuyenHan(MaQuyen)
);
GO

CREATE TABLE NhanVien (
    MaNhanVien INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100),
    SoDienThoai VARCHAR(15),
    Email VARCHAR(100),
    ChucVu NVARCHAR(50),
    MaTaiKhoan INT UNIQUE,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);
GO

CREATE TABLE NhatKyHoatDong (
    MaNhatKy INT PRIMARY KEY IDENTITY(1,1),
    MaTaiKhoan INT,
    ThoiGian DATETIME DEFAULT GETDATE(),
    HanhDong NVARCHAR(255),
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);
GO

CREATE TABLE ThiSinh (
    MaThiSinh INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(10),
    SoDienThoai VARCHAR(15) UNIQUE,
    Email VARCHAR(100) UNIQUE,
    CMND_CCCD VARCHAR(20) NOT NULL UNIQUE,
    DiaChi NVARCHAR(255),
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 1
);
GO

CREATE TABLE GiamThi (
    MaGiamThi INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) UNIQUE,
    Email VARCHAR(100) UNIQUE,
    ChuyenMon NVARCHAR(100),
    MaTaiKhoan INT,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);
GO

CREATE TABLE PhongThi (
    MaPhong VARCHAR(20) PRIMARY KEY,
    TenPhong NVARCHAR(100) NOT NULL,
    SucChua INT NOT NULL,
    DiaDiem NVARCHAR(200)
);
GO

CREATE TABLE QuyDinhChungChi (
    MaQuyDinh INT PRIMARY KEY IDENTITY(1,1),
    LoaiChungChi NVARCHAR(50) NOT NULL,
    DiemToiThieu FLOAT,
    MoTa NVARCHAR(255)
);
GO

CREATE TABLE KyThi (
    MaKyThi INT PRIMARY KEY IDENTITY(1,1),
    TenKyThi NVARCHAR(200) NOT NULL,
    NgayToChuc DATE NOT NULL,
    LoaiChungChi NVARCHAR(50),
    LePhi DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(50)
);
GO

CREATE TABLE DangKyThi (
    MaKyThi INT NOT NULL,
    MaThiSinh INT NOT NULL,
    MaDangKy INT PRIMARY KEY IDENTITY(1,1),
    NgayDangKy DATETIME DEFAULT GETDATE(),
    TrangThaiThanhToan BIT DEFAULT 0,
    FOREIGN KEY (MaThiSinh) REFERENCES ThiSinh(MaThiSinh),
    FOREIGN KEY (MaKyThi) REFERENCES KyThi(MaKyThi),
    UNIQUE(MaThiSinh, MaKyThi)
);
GO

CREATE TABLE ThanhToan (
    MaThanhToan INT PRIMARY KEY IDENTITY(1,1),
    MaDangKy INT NOT NULL,
    SoTien DECIMAL(18,2),
    NgayThanhToan DATETIME DEFAULT GETDATE(),
    PhuongThuc NVARCHAR(50),
    GhiChu NVARCHAR(255),
    FOREIGN KEY (MaDangKy) REFERENCES DangKyThi(MaDangKy)
);
GO

CREATE TABLE CaThi (
    MaCaThi INT PRIMARY KEY IDENTITY(1,1),
    MaKyThi INT NOT NULL,
    MaPhong VARCHAR(20) NOT NULL,
    NgayThi DATE,
    ThoiGianBatDau DATETIME NOT NULL,
    ThoiGianKetThuc DATETIME NOT NULL,
    SiSo INT,
    FOREIGN KEY (MaKyThi) REFERENCES KyThi(MaKyThi),
    FOREIGN KEY (MaPhong) REFERENCES PhongThi(MaPhong)
);
GO

CREATE TABLE PhanCongCoiThi (
    MaPhanCong INT PRIMARY KEY IDENTITY(1,1),
    MaCaThi INT NOT NULL,
    MaGiamThi INT NOT NULL,
    NhiemVu NVARCHAR(100),
    FOREIGN KEY (MaCaThi) REFERENCES CaThi(MaCaThi),
    FOREIGN KEY (MaGiamThi) REFERENCES GiamThi(MaGiamThi),
    UNIQUE(MaCaThi, MaGiamThi)
);
GO

CREATE TABLE DanhSachThi (
    MaDanhSach INT PRIMARY KEY IDENTITY(1,1),
    MaDangKy INT NOT NULL,
    MaCaThi INT NOT NULL,
    SoBaoDanh VARCHAR(20) NOT NULL,
    SoGhe VARCHAR(10),
    FOREIGN KEY (MaDangKy) REFERENCES DangKyThi(MaDangKy),
    FOREIGN KEY (MaCaThi) REFERENCES CaThi(MaCaThi),
    UNIQUE(MaDangKy, MaCaThi)
);
GO

CREATE TABLE KetQuaThi (
    MaKetQua INT PRIMARY KEY IDENTITY(1,1),
    MaDangKy INT NOT NULL,
    DiemNghe FLOAT DEFAULT 0,
    DiemDoc FLOAT DEFAULT 0,
    DiemNoi FLOAT DEFAULT 0,
    DiemViet FLOAT DEFAULT 0,
    TongDiem FLOAT DEFAULT 0,
    KetQua NVARCHAR(50), 
    XepLoai NVARCHAR(50), 
    NgayCapNhat DATETIME DEFAULT GETDATE(),
    GhiChu NVARCHAR(255),
    FOREIGN KEY (MaDangKy) REFERENCES DangKyThi(MaDangKy),
    UNIQUE(MaDangKy)
);
GO

CREATE TABLE ChungChi (
    MaChungChi INT PRIMARY KEY IDENTITY(1,1),
    MaKetQua INT NOT NULL,
    SoChungChi VARCHAR(50) UNIQUE,
    NgayCap DATE DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Đã cấp',
    FOREIGN KEY (MaKetQua) REFERENCES KetQuaThi(MaKetQua)
);
GO


INSERT INTO QuyenHan (TenQuyen) VALUES (N'Admin'), (N'Giám thị'), (N'Học viên');
GO

INSERT INTO PhongThi (MaPhong, TenPhong, SucChua, DiaDiem) VALUES
('P101', N'Phòng lý thuyết A1', 50, N'Tầng 1, Tòa A'),
('P102', N'Phòng lý thuyết A2', 50, N'Tầng 1, Tòa A'),
('P201', N'Phòng thực hành B1', 40, N'Tầng 2, Tòa B'),
('P202', N'Phòng thực hành B2', 40, N'Tầng 2, Tòa B'),
('P301', N'Phòng thi Speaking 01', 5, N'Tầng 3, Tòa A'),
('P302', N'Phòng thi Speaking 02', 5, N'Tầng 3, Tòa A'),
('H1', N'Hội trường lớn', 200, N'Tầng 5, Tòa C'),
('LAB01', N'Phòng Lab 01', 60, N'Tầng 4, Tòa B'),
('LAB02', N'Phòng Lab 02', 60, N'Tầng 4, Tòa B'),
('P505', N'Phòng chờ thi', 100, N'Tầng 5, Tòa A');
GO

INSERT INTO ThiSinh (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, CMND_CCCD, DiaChi) VALUES
(N'Nguyễn Văn An', '2000-01-15', N'Nam', '0901112221', 'vana@gmail.com', '079200001111', N'123 Lê Lợi, Q.1, TP.HCM'),
(N'Trần Thị Bình', '1999-05-20', N'Nữ', '0901112222', 'binhtt@gmail.com', '079200001112', N'456 Hai Bà Trưng, Q.3, TP.HCM'),
(N'Lê Hoàng Cường', '2001-11-30', N'Nam', '0901112223', 'cuonglh@gmail.com', '079200001113', N'789 Nguyễn Trãi, Q.5, TP.HCM'),
(N'Phạm Thị Dung', '2002-02-10', N'Nữ', '0901112224', 'dungpt@gmail.com', '079200001114', N'101 Võ Văn Tần, Q.3, TP.HCM'),
(N'Võ Minh Em', '1998-07-07', N'Nam', '0901112225', 'emvm@gmail.com', '079200001115', N'202 Lý Thường Kiệt, Q.10, TP.HCM'),
(N'Đỗ Thị Hương', '2000-09-25', N'Nữ', '0901112226', 'huongdt@gmail.com', '079200001116', N'303 Trần Hưng Đạo, Q.1, TP.HCM'),
(N'Hoàng Văn Giang', '2001-04-12', N'Nam', '0901112227', 'gianghv@gmail.com', '079200001117', N'404 Điện Biên Phủ, Q.Bình Thạnh, TP.HCM'),
(N'Ngô Thị Kiều', '1997-12-01', N'Nữ', '0901112228', 'kieunt@gmail.com', '079200001118', N'505 Sư Vạn Hạnh, Q.10, TP.HCM'),
(N'Phan Thanh Lâm', '2003-03-03', N'Nam', '0901112229', 'lampt@gmail.com', '079200001119', N'606 CMT8, Q.Tân Bình, TP.HCM'),
(N'Bùi Thị Mơ', '2000-08-19', N'Nữ', '0901112230', 'mobt@gmail.com', '079200001120', N'707 An Dương Vương, Q.5, TP.HCM'),
(N'Phạm Văn Khoa', '1998-02-14', N'Nam', '0909998881', 'khoapv@gmail.com', '079098000001', N'Quận 1, TP.HCM'),
(N'Lý Thị Mộng', '2001-05-30', N'Nữ', '0909998882', 'monglt@gmail.com', '079098000002', N'Quận 3, TP.HCM'),
(N'Trương Tam Phong', '1995-09-09', N'Nam', '0909998883', 'phongtt@gmail.com', '079098000003', N'Thủ Đức, TP.HCM'),
(N'Nguyễn Bích Thủy', '2003-12-20', N'Nữ', '0909998884', 'thuynb@gmail.com', '079098000004', N'Gò Vấp, TP.HCM'),
(N'Trần Đại Quang', '2000-01-01', N'Nam', '0909998885', 'quangtd@gmail.com', '079098000005', N'Bình Thạnh, TP.HCM');
GO

INSERT INTO KyThi (TenKyThi, NgayToChuc, LoaiChungChi, LePhi, TrangThai) VALUES
(N'Kỳ thi IELTS tháng 11/2025', '2025-11-15', 'IELTS Academic', 4650000, N'Đang mở đăng ký'),
(N'Kỳ thi TOEIC 4 kỹ năng tháng 11/2025', '2025-11-20', 'TOEIC', 2890000, N'Đang mở đăng ký'),
(N'Kỳ thi Cambridge KET/PET đợt 2 2025', '2025-12-05', 'PET', 1100000, N'Sắp mở'),
(N'Kỳ thi IELTS tháng 10/2025', '2025-10-15', 'IELTS General', 4650000, N'Đã kết thúc'),
(N'Kỳ thi TOEIC 2 kỹ năng tháng 11/2025', '2025-11-10', 'TOEIC', 1500000, N'Đã kết thúc'),
(N'Kỳ thi Aptis đợt 1 2025', '2025-06-01', 'Aptis', 2000000, N'Đã có kết quả'),
(N'Kỳ thi VSTEP B1-C1 tháng 11/2025', '2025-11-30', 'VSTEP', 1800000, N'Đang mở đăng ký'),
(N'Thi thử IELTS miễn phí 2025', '2025-09-01', 'IELTS Mock Test', 0, N'Đã kết thúc'),
(N'Kỳ thi IELTS tháng 12/2025', '2025-12-15', 'IELTS Academic', 4650000, N'Chưa mở'),
(N'Kỳ thi TOEIC 4 kỹ năng tháng 12/2025', '2025-12-20', 'TOEIC', 2890000, N'Chưa mở'),
(N'Kỳ thi IELTS Cấp tốc tháng 01/2026', '2026-01-15', 'IELTS Academic', 4750000, N'Sắp mở');
GO

INSERT INTO QuyDinhChungChi (LoaiChungChi, DiemToiThieu, MoTa) VALUES
('IELTS 5.5', 5.5, N'Đạt 5.5 Overall, không kỹ năng nào dưới 5.0'),
('IELTS 6.5', 6.5, N'Đạt 6.5 Overall, không kỹ năng nào dưới 6.0'),
('TOEIC 500', 500, N'Tổng điểm Nghe & Đọc đạt 500'),
('TOEIC 750', 750, N'Tổng điểm Nghe & Đọc đạt 750'),
('VSTEP B1', 4.0, N'Đạt 4.0/10'),
('VSTEP B2', 6.0, N'Đạt 6.0/10'),
('VSTEP C1', 8.5, N'Đạt 8.5/10'),
('PET Pass', 140, N'Điểm Cambridge Scale từ 140-152'),
('PET Pass with Merit', 153, N'Điểm Cambridge Scale từ 153-159'),
('KET Pass', 120, N'Điểm Cambridge Scale từ 120-132');
GO

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Email, MaQuyen, TrangThai) VALUES
('admin', 'admin123', 'admin@trungtam.edu.vn', 1, 1),
('giamthi01', 'giamthi123', 'giamthi01@gmail.com', 2, 1),
('giamthi02', 'giamthi123', 'giamthi02@gmail.com', 2, 1),
('hocvien_an', '123', 'vana@gmail.com', 3, 1),
('hocvien_binh', '123', 'binhtt@gmail.com', 3, 1);
GO

INSERT INTO NhanVien (HoTen, SoDienThoai, Email, ChucVu, MaTaiKhoan) VALUES
(N'Trần Văn Admin', '0909123456', 'admin@trungtam1.edu.vn', N'Quản trị hệ thống', NULL);
GO

INSERT INTO GiamThi (HoTen, SoDienThoai, Email, ChuyenMon, MaTaiKhoan) VALUES
(N'Trần Văn', '0909123456', 'admin@trungtam.edu.vn', N'Quản trị hệ thống', 1),
(N'Trịnh Thanh Sơn', '0987654321', 'sontt@gmail.com', N'Giám thị IELTS', 2),
(N'Mai Hồng Ngọc', '0987654322', 'ngocmh@gmail.com', N'Giám thị TOEIC', 3),
(N'Đinh Tiên Hoàng', '0987654323', 'hoangdt@gmail.com', N'Giám thị VSTEP', NULL),
(N'Vũ Thị Hoa', '0987654324', 'hoavt@gmail.com', N'Giám thị Cambridge', NULL),
(N'Nguyễn Tấn Dũng', '0987654325', 'dungnt@gmail.com', N'Giám thị IELTS Speaking', NULL),
(N'Phạm Thu Trang', '0987654326', 'trangpt@gmail.com', N'Giám thị IELTS', NULL),
(N'Hồ Văn Trung', '0987654327', 'trunghv@gmail.com', N'Giám thị TOEIC', NULL),
(N'Lâm Thị Bích', '0987654328', 'bichlt@gmail.com', N'Giám thị VSTEP', NULL),
(N'Cao Văn Lầu', '0987654329', 'laucv@gmail.com', N'Giám thị Cambridge', NULL),
(N'Dương Thị Cẩm', '0987654330', 'camdt@gmail.com', N'Giám thị IELTS', NULL);
GO

INSERT INTO NhatKyHoatDong (MaTaiKhoan, HanhDong) VALUES
(1, N'Đăng nhập hệ thống'), 
(2, N'Giám thị (giamthi01) xác nhận ca thi'), 
(3, N'Giám thị (giamthi02) nhập điểm'),
(1, N'Admin xác nhận thanh toán cho thí sinh Trần Thị Bình'),
(1, N'Cập nhật quyền hạn cho tài khoản: giamthi01'),
(3, N'Xếp lịch thi cho CaThi-001'),
(4, N'Học viên (vana@gmail.com) đăng ký thi'),
(1, N'Xóa tài khoản: hocvien_cu'),
(5, N'Học viên (binhtt@gmail.com) xem kết quả'),
(1, N'Thực hiện sao lưu cơ sở dữ liệu');
GO

INSERT INTO DangKyThi (MaThiSinh, MaKyThi, TrangThaiThanhToan) VALUES
(1, 1, 1), (2, 1, 1), (3, 2, 0), (4, 4, 1), (5, 5, 1), (6, 7, 1), (7, 7, 0), (8, 8, 1), (9, 1, 1), (10, 2, 1),
((SELECT MaThiSinh FROM ThiSinh WHERE Email = 'khoapv@gmail.com'), 2, 1),
((SELECT MaThiSinh FROM ThiSinh WHERE Email = 'monglt@gmail.com'), 2, 0),
((SELECT MaThiSinh FROM ThiSinh WHERE Email = 'phongtt@gmail.com'), 2, 1),
((SELECT MaThiSinh FROM ThiSinh WHERE Email = 'thuynb@gmail.com'), 2, 1),
((SELECT MaThiSinh FROM ThiSinh WHERE Email = 'quangtd@gmail.com'), 2, 0);
GO

INSERT INTO ThanhToan (MaDangKy, SoTien, PhuongThuc, GhiChu) VALUES
(1, 4650000, N'Chuyển khoản', N'CK VCB'), 
(2, 4650000, N'Tiền mặt', NULL), 
(4, 4650000, N'Tiền mặt', NULL),
(5, 1500000, N'Chuyển khoản', N'CK Momo'), 
(6, 1800000, N'Chuyển khoản', N'CK Techcombank'),
(8, 0, N'Miễn phí', N'Thi thử'),
(9, 4650000, N'Tiền mặt', NULL),
(10, 2890000, N'Chuyển khoản', N'CK BIDV'),
(3, 2890000, N'Chuyển khoản', N'Vừa thanh toán'),
(7, 1800000, N'Tiền mặt', N'Nợ lại biên lai'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'khoapv@gmail.com') AND MaKyThi = 2), 2890000, N'Chuyển khoản', N'CK Momo'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'phongtt@gmail.com') AND MaKyThi = 2), 2890000, N'Tiền mặt', N'Thu tại quầy'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'thuynb@gmail.com') AND MaKyThi = 2), 2890000, N'Chuyển khoản', N'CK VCB');
GO

INSERT INTO CaThi (MaKyThi, MaPhong, NgayThi, ThoiGianBatDau, ThoiGianKetThuc, SiSo) VALUES
(1, 'P101', '2025-11-15', '2025-11-15 08:00:00', '2025-11-15 11:00:00', 50),
(1, 'P102', '2025-11-15', '2025-11-15 08:00:00', '2025-11-15 11:00:00', 50),
(1, 'P301', '2025-11-15', '2025-11-15 13:00:00', '2025-11-15 17:00:00', 5),
(2, 'P201', '2025-11-20', '2025-11-20 08:30:00', '2025-11-20 11:30:00', 40),
(2, 'P202', '2025-11-20', '2025-11-20 08:30:00', '2025-11-20 11:30:00', 40),
(4, 'P101', '2025-10-15', '2025-10-15 08:00:00', '2025-10-15 11:00:00', 50),
(5, 'P201', '2025-11-10', '2025-11-10 08:30:00', '2025-11-10 10:30:00', 40),
(7, 'LAB01', '2025-11-30', '2025-11-30 07:30:00', '2025-11-30 10:00:00', 60),
(7, 'LAB01', '2025-11-30', '2025-11-30 13:30:00', '2025-11-30 16:00:00', 60),
(8, 'P101', '2025-09-01', '2025-09-01 08:00:00', '2025-09-01 10:00:00', 50);
GO

INSERT INTO PhanCongCoiThi (MaCaThi, MaGiamThi, NhiemVu) VALUES 
(1, 1, N'Giám thị 1'), (1, 2, N'Giám thị 2'), (2, 3, N'Giám thị 1'), (2, 4, N'Giám thị 2'),
(3, 5, N'Giám khảo Speaking'), (4, 6, N'Giám thị 1'), (4, 7, N'Giám thị 2'), (5, 1, N'Giám thị 1');
GO

INSERT INTO DanhSachThi (MaDangKy, MaCaThi, SoBaoDanh, SoGhe) VALUES 
(1, 1, 'IELTS1125A-001', '01'), (2, 1, 'IELTS1125A-002', '02'), (9, 2, 'IELTS1125B-001', '01'),
(3, 4, 'TOEIC1125A-001', 'A1'), (10, 5, 'TOEIC1125B-001', 'A1'), (4, 6, 'IELTS1025-001', '01'),
(5, 7, 'TOEIC2KN1125-001', '01'), (6, 8, 'VSTEP1125A-001', 'A01'), (7, 8, 'VSTEP1125A-002', 'A02'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'khoapv@gmail.com') AND MaKyThi = 2), 4, 'TOEIC1125A-002', 'A2'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'monglt@gmail.com') AND MaKyThi = 2), 4, 'TOEIC1125A-003', 'A3'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'phongtt@gmail.com') AND MaKyThi = 2), 4, 'TOEIC1125A-004', 'A4'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'thuynb@gmail.com') AND MaKyThi = 2), 4, 'TOEIC1125A-005', 'A5'),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'quangtd@gmail.com') AND MaKyThi = 2), 4, 'TOEIC1125A-006', 'A6');
GO

INSERT INTO KetQuaThi (MaDangKy, DiemNghe, DiemDoc, DiemNoi, DiemViet, TongDiem, GhiChu) VALUES
(4, 7.5, 8.0, 6.5, 7.0, 7.5, NULL), 
(5, 450, 400, NULL, NULL, 850, NULL), 
(8, 6.0, 6.5, NULL, NULL, 6.5, NULL), 
(1, 8.0, 8.5, 7.0, 7.5, 8.0, NULL),
(2, 6.0, 6.5, 5.5, 6.0, 6.0, NULL), 
(3, 350, 300, 120, 110, 880, NULL),
(6, 4.0, 5.0, 6.0, 4.0, 4.5, NULL), 
(7, 3.0, 4.0, 3.0, 2.0, 3.0, NULL),
(9, 7.0, 7.0, 6.0, 6.5, 6.5, NULL), 
(10, 400, 420, 150, 160, 1130, NULL),
((SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'khoapv@gmail.com') AND MaKyThi = 2), 450, 400, NULL, NULL, 850, N'Đạt yêu cầu');
GO

INSERT INTO ChungChi (MaKetQua, SoChungChi, NgayCap, TrangThai) VALUES
(1, 'IELTS-25-0011A', '2025-10-28', N'Đã cấp'),
(2, 'TOEIC-25-0022B', '2025-11-23', N'Đã cấp'),
(3, 'VSTEP-25-1234H', GETDATE(), N'Đang in'),
(4, 'IELTS-25-5678B', GETDATE(), N'Đã cấp'),
(5, 'PET-25-4433Z', GETDATE(), N'Đã cấp'),
((SELECT MaKetQua FROM KetQuaThi WHERE MaDangKy = (SELECT MaDangKy FROM DangKyThi WHERE MaThiSinh = (SELECT MaThiSinh FROM ThiSinh WHERE Email = 'khoapv@gmail.com') AND MaKyThi = 2)), 'TOEIC-25-9999K', GETDATE(), N'Chờ ký');
GO

/* ========================================================== */
/* 4. STORED PROCEDURES (FULL GỐC - FIX LỖI OBJECT NAME)      */
/* ========================================================== */

CREATE PROCEDURE sp_TimKiemKetQuaThi
    @MaKyThi INT,
    @TuKhoa NVARCHAR(100)
AS
BEGIN
    -- Nếu từ khóa rỗng thì hiện tất cả thí sinh của kỳ thi đó
    IF @TuKhoa IS NULL OR @TuKhoa = ''
    BEGIN
        SELECT 
            dk.MaDangKy,
            ds.SoBaoDanh AS [SBD],
            ts.HoTen AS [Họ Tên],
            ts.CMND_CCCD AS [CCCD],
            kq.DiemNghe,
            kq.DiemDoc,
            kq.DiemNoi,
            kq.DiemViet,
            kq.TongDiem,
            kq.KetQua,
            kq.XepLoai,
            kq.GhiChu
        FROM DangKyThi dk
        JOIN ThiSinh ts ON dk.MaThiSinh = ts.MaThiSinh
        JOIN DanhSachThi ds ON dk.MaDangKy = ds.MaDangKy
        LEFT JOIN KetQuaThi kq ON dk.MaDangKy = kq.MaDangKy
        WHERE dk.MaKyThi = @MaKyThi
    END
    ELSE
    BEGIN
        -- Tìm kiếm theo SBD hoặc Họ Tên hoặc CCCD
        SELECT 
            dk.MaDangKy,
            ds.SoBaoDanh AS [SBD],
            ts.HoTen AS [Họ Tên],
            ts.CMND_CCCD AS [CCCD],
            kq.DiemNghe,
            kq.DiemDoc,
            kq.DiemNoi,
            kq.DiemViet,
            kq.TongDiem,
            kq.KetQua,
            kq.XepLoai,
            kq.GhiChu
        FROM DangKyThi dk
        JOIN ThiSinh ts ON dk.MaThiSinh = ts.MaThiSinh
        JOIN DanhSachThi ds ON dk.MaDangKy = ds.MaDangKy
        LEFT JOIN KetQuaThi kq ON dk.MaDangKy = kq.MaDangKy
        WHERE dk.MaKyThi = @MaKyThi
          AND (
              ds.SoBaoDanh LIKE '%' + @TuKhoa + '%' OR 
              ts.HoTen LIKE N'%' + @TuKhoa + '%' OR 
              ts.CMND_CCCD LIKE '%' + @TuKhoa + '%'
          )
    END
END
GO

CREATE PROCEDURE sp_CapNhatDiem
    @MaDangKy INT, @DiemNghe FLOAT, @DiemDoc FLOAT, @DiemNoi FLOAT, @DiemViet FLOAT, @TongDiem FLOAT, @KetQua NVARCHAR(50), @XepLoai NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM KetQuaThi WHERE MaDangKy = @MaDangKy)
        UPDATE KetQuaThi SET DiemNghe = @DiemNghe, DiemDoc = @DiemDoc, DiemNoi = @DiemNoi, DiemViet = @DiemViet, TongDiem = @TongDiem, KetQua = @KetQua, XepLoai = @XepLoai, NgayCapNhat = GETDATE() WHERE MaDangKy = @MaDangKy
    ELSE
        INSERT INTO KetQuaThi (MaDangKy, DiemNghe, DiemDoc, DiemNoi, DiemViet, TongDiem, KetQua, XepLoai, NgayCapNhat) VALUES (@MaDangKy, @DiemNghe, @DiemDoc, @DiemNoi, @DiemViet, @TongDiem, @KetQua, @XepLoai, GETDATE())
END
GO

CREATE PROCEDURE sp_LayDSChoCapChungChi @MaKyThi INT AS
BEGIN
    SELECT kq.MaKetQua, ts.HoTen, ts.NgaySinh, ts.CMND_CCCD, kq.XepLoai, kt.LoaiChungChi 
    FROM KetQuaThi kq JOIN DangKyThi dk ON kq.MaDangKy = dk.MaDangKy JOIN ThiSinh ts ON dk.MaThiSinh = ts.MaThiSinh JOIN KyThi kt ON dk.MaKyThi = kt.MaKyThi 
    WHERE dk.MaKyThi = @MaKyThi AND (kq.KetQua = N'Đạt' OR kq.KetQua = N'Đậu') AND kq.MaKetQua NOT IN (SELECT MaKetQua FROM ChungChi)
END
GO

CREATE PROCEDURE sp_CapChungChi @MaKetQua INT, @SoChungChi VARCHAR(50) AS
BEGIN
    INSERT INTO ChungChi (MaKetQua, SoChungChi, NgayCap, TrangThai) VALUES (@MaKetQua, @SoChungChi, GETDATE(), N'Mới cấp')
END
GO

CREATE PROCEDURE sp_LayDanhSachThanhToan 
    @MaKyThi INT, @TrangThai INT, @TuKhoa NVARCHAR(100) 
AS
BEGIN
    SELECT 
        dk.MaDangKy, 
        ts.HoTen AS [Họ Tên], 
        ts.SoDienThoai AS [SĐT], 
        kt.TenKyThi, 
        kt.LePhi AS [Phải Đóng],
        ISNULL((SELECT SUM(SoTien) FROM ThanhToan WHERE MaDangKy = dk.MaDangKy), 0) AS [Đã Đóng],
        (kt.LePhi - ISNULL((SELECT SUM(SoTien) FROM ThanhToan WHERE MaDangKy = dk.MaDangKy), 0)) AS [Còn Nợ],
        CASE WHEN dk.TrangThaiThanhToan = 1 THEN N'Hoàn tất' ELSE N'Chưa xong' END AS [Trạng Thái],
        dk.NgayDangKy
    FROM DangKyThi dk 
    JOIN ThiSinh ts ON dk.MaThiSinh = ts.MaThiSinh 
    JOIN KyThi kt ON dk.MaKyThi = kt.MaKyThi
    WHERE (@MaKyThi IS NULL OR dk.MaKyThi = @MaKyThi) 
      AND (@TrangThai = -1 OR dk.TrangThaiThanhToan = @TrangThai)
      AND (@TuKhoa IS NULL OR ts.HoTen LIKE N'%' + @TuKhoa + '%' OR ts.SoDienThoai LIKE '%' + @TuKhoa + '%')
END
GO

CREATE OR ALTER PROCEDURE sp_ThucHienThanhToan
    @MaDangKy INT,
    @SoTien DECIMAL(18,2),
    @PhuongThuc NVARCHAR(50),
    @GhiChu NVARCHAR(255)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        
        DECLARE @LePhi DECIMAL(18,2);
        DECLARE @TongDaDong DECIMAL(18,2);
        DECLARE @ConNo DECIMAL(18,2);

        -- Lấy lệ phí kỳ thi
        SELECT @LePhi = k.LePhi
        FROM DangKyThi d 
        JOIN KyThi k ON d.MaKyThi = k.MaKyThi
        WHERE d.MaDangKy = @MaDangKy;

        -- Lấy tổng tiền đã đóng
        SELECT @TongDaDong = ISNULL(SUM(SoTien), 0)
        FROM ThanhToan
        WHERE MaDangKy = @MaDangKy;

        -- Tính số tiền còn nợ
        SET @ConNo = @LePhi - @TongDaDong;

        -- 1. CHẶN ĐÓNG DƯ TIỀN
        IF @SoTien > @ConNo
        BEGIN
            RAISERROR (N'Số tiền thu vượt quá số còn nợ!', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Thêm bản ghi thanh toán
        INSERT INTO ThanhToan (MaDangKy, SoTien, NgayThanhToan, PhuongThuc, GhiChu)
        VALUES (@MaDangKy, @SoTien, GETDATE(), @PhuongThuc, @GhiChu);

        -- 3. Cập nhật trạng thái -> Hoàn tất khi đóng đủ
        IF @TongDaDong + @SoTien >= @LePhi
        BEGIN
            UPDATE DangKyThi
            SET TrangThaiThanhToan = 1
            WHERE MaDangKy = @MaDangKy;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


CREATE PROCEDURE sp_LayDanhSachCaThi @MaKyThi INT AS
BEGIN
    SELECT CT.MaCaThi, CT.NgayThi, CT.ThoiGianBatDau, PT.TenPhong, ISNULL(CT.SiSo, PT.SucChua) AS SucChua, (SELECT COUNT(*) FROM DanhSachThi WHERE MaCaThi = CT.MaCaThi) as DaDK
    FROM CaThi CT JOIN PhongThi PT ON CT.MaPhong = PT.MaPhong WHERE CT.MaKyThi = @MaKyThi
END
GO

CREATE PROCEDURE sp_HocVienDangKyThi @MaThiSinh INT, @MaKyThi INT AS
BEGIN
    IF EXISTS (SELECT 1 FROM DangKyThi WHERE MaThiSinh = @MaThiSinh AND MaKyThi = @MaKyThi) RAISERROR(N'Bạn đã đăng ký rồi!', 16, 1);
    ELSE INSERT INTO DangKyThi (MaThiSinh, MaKyThi, NgayDangKy, TrangThaiThanhToan) VALUES (@MaThiSinh, @MaKyThi, GETDATE(), 0);
END
GO

CREATE PROCEDURE sp_LayDsCanThanhToan
    @MaThiSinh INT
AS
BEGIN
    SELECT 
        DK.MaDangKy,
        KT.TenKyThi,
        KT.NgayToChuc,
        KT.LoaiChungChi,
        KT.LePhi,
        KT.LePhi AS SoTienCanDong -- Để hiển thị
    FROM DangKyThi DK
    JOIN KyThi KT ON DK.MaKyThi = KT.MaKyThi
    WHERE DK.MaThiSinh = @MaThiSinh 
      AND DK.TrangThaiThanhToan = 0 -- 0 là chưa thanh toán xong
END
GO

CREATE PROCEDURE sp_LayKetQuaThiSinh
    @MaThiSinh INT
AS
BEGIN
    SELECT 
        KT.TenKyThi AS TenChungChi, -- Tên kỳ thi
        KT.NgayToChuc AS NgayThi,   -- Ngày thi
        KQ.DiemNghe,
        KQ.DiemDoc,
        KQ.DiemNoi,
        KQ.DiemViet,
        KQ.TongDiem,
        KQ.KetQua,   -- Đạt / Không đạt
        KQ.XepLoai,  -- Ví dụ: IELTS 7.5, TOEIC 850...
        KQ.GhiChu
    FROM KetQuaThi KQ
    JOIN DangKyThi DK ON KQ.MaDangKy = DK.MaDangKy
    JOIN KyThi KT ON DK.MaKyThi = KT.MaKyThi
    WHERE DK.MaThiSinh = @MaThiSinh
    ORDER BY KT.NgayToChuc DESC -- Kỳ thi mới nhất hiện lên đầu
END
GO


CREATE PROCEDURE sp_LayThongTinThiSinh @MaThiSinh INT AS 
BEGIN 
    SELECT * FROM ThiSinh WHERE MaThiSinh = @MaThiSinh 
END
GO

CREATE PROCEDURE sp_CapNhatHoSoThiSinh @MaThiSinh INT, @SoDienThoai VARCHAR(15), @Email VARCHAR(100), @DiaChi NVARCHAR(255), @NgaySinh DATE, @GioiTinh NVARCHAR(10) AS 
BEGIN 
    UPDATE ThiSinh SET SoDienThoai = @SoDienThoai, Email = @Email, DiaChi = @DiaChi, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh WHERE MaThiSinh = @MaThiSinh 
END
GO

CREATE PROCEDURE sp_DoiMatKhauBangEmail @Email VARCHAR(100), @MatKhauCu VARCHAR(50), @MatKhauMoi VARCHAR(50) AS 
BEGIN 
    DECLARE @MaTaiKhoan INT; SELECT @MaTaiKhoan = MaTaiKhoan FROM TaiKhoan WHERE Email = @Email; 
    IF @MaTaiKhoan IS NULL RETURN; 
    IF EXISTS (SELECT 1 FROM TaiKhoan WHERE MaTaiKhoan = @MaTaiKhoan AND MatKhau = @MatKhauCu) 
        UPDATE TaiKhoan SET MatKhau = @MatKhauMoi WHERE MaTaiKhoan = @MaTaiKhoan; 
END
GO

CREATE PROCEDURE sp_LayDSDaCapChungChi @MaKyThi INT AS 
BEGIN 
    SELECT cc.MaChungChi, cc.SoChungChi, ts.HoTen, kq.XepLoai, cc.NgayCap, cc.TrangThai FROM ChungChi cc JOIN KetQuaThi kq ON cc.MaKetQua = kq.MaKetQua JOIN DangKyThi dk ON kq.MaDangKy = dk.MaDangKy JOIN ThiSinh ts ON dk.MaThiSinh = ts.MaThiSinh WHERE dk.MaKyThi = @MaKyThi 
END
GO

CREATE PROCEDURE sp_LayLichThiChoHocVien
    @MaThiSinh INT,
    @TuKhoa NVARCHAR(100) = '',
    @LoaiChungChi NVARCHAR(50) = ''
AS
BEGIN
    SELECT 
        KT.MaKyThi,
        KT.TenKyThi,
        KT.NgayToChuc,
        KT.LoaiChungChi,
        KT.LePhi,
        KT.TrangThai,
        CASE WHEN DK.MaDangKy IS NOT NULL THEN N'Đã đăng ký' ELSE N'Chưa đăng ký' END AS TrangThaiDangKy
    FROM KyThi KT
    LEFT JOIN DangKyThi DK ON KT.MaKyThi = DK.MaKyThi AND DK.MaThiSinh = @MaThiSinh
    WHERE (KT.TrangThai = N'Đang mở đăng ký' OR KT.TrangThai = N'Sắp mở')
      AND (@TuKhoa = '' OR KT.TenKyThi LIKE N'%' + @TuKhoa + '%')
      AND (@LoaiChungChi = '' OR KT.LoaiChungChi LIKE N'%' + @LoaiChungChi + '%')
END
GO

CREATE PROCEDURE sp_LayLichSuThanhToan
    @MaThiSinh INT
AS
BEGIN
    SELECT 
        TT.MaThanhToan,
        KT.TenKyThi,
        TT.SoTien,
        TT.NgayThanhToan,
        TT.PhuongThuc,
        TT.GhiChu
    FROM ThanhToan TT
    JOIN DangKyThi DK ON TT.MaDangKy = DK.MaDangKy
    JOIN KyThi KT ON DK.MaKyThi = KT.MaKyThi
    WHERE DK.MaThiSinh = @MaThiSinh
    ORDER BY TT.NgayThanhToan DESC
END
GO

PRINT N'=== HOÀN TẤT CÀI ĐẶT ĐẦY ĐỦ 100% ===';
