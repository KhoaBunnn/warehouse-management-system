-- =============================================
-- SCRIPT SQL - DỮ LIỆU MẪU ĐỂ TEST HỆ THỐNG
-- Database: QLKhoHang
-- =============================================

USE [QLKhoHang]
GO

-- Xóa dữ liệu cũ (nếu cần)
-- DELETE FROM CT_PhieuXuat
-- DELETE FROM CT_PhieuNhap
-- DELETE FROM PhieuXuat
-- DELETE FROM PhieuNhap
-- DELETE FROM HangHoa
-- DELETE FROM KhachHang
-- DELETE FROM NhaCungCap
-- DELETE FROM NhanVien
-- DELETE FROM LoaiHang
-- DELETE FROM Kho
-- GO

-- =============================================
-- 1. KHO (Kho hàng)
-- =============================================
INSERT INTO [dbo].[Kho] ([MaKho], [TenKho], [DiaChiKho])
VALUES
    ('KHO001', N'Kho chính Hà Nội', N'123 Đường Láng, Đống Đa, Hà Nội'),
    ('KHO002', N'Kho phụ Sài Gòn', N'456 Nguyễn Huệ, Quận 1, TP.HCM'),
    ('KHO003', N'Kho miền Trung', N'789 Lê Lợi, Hải Châu, Đà Nẵng')
GO

-- =============================================
-- 2. LOAIHANG (Loại hàng)
-- =============================================
INSERT INTO [dbo].[LoaiHang] ([MaLoai], [TenLoai])
VALUES
    ('LH001', N'Điện tử'),
    ('LH002', N'Thực phẩm'),
    ('LH003', N'Quần áo'),
    ('LH004', N'Đồ gia dụng'),
    ('LH005', N'Văn phòng phẩm')
GO

-- =============================================
-- 3. NHANVIEN (Nhân viên)
-- =============================================
INSERT INTO [dbo].[NhanVien] ([MaNV], [TenNV], [SDT], [DiaChi])
VALUES
    ('NV001', N'Nguyễn Văn An', '0912345678', N'10 Nguyễn Trãi, Hà Nội'),
    ('NV002', N'Trần Thị Bình', '0923456789', N'20 Lê Duẩn, Hà Nội'),
    ('NV003', N'Lê Văn Cường', '0934567890', N'30 Hoàng Diệu, Hà Nội'),
    ('NV004', N'Phạm Thị Dung', '0945678901', N'40 Trần Phú, Hà Nội'),
    ('NV005', N'Hoàng Văn Em', '0956789012', N'50 Lý Thường Kiệt, Hà Nội')
GO

-- =============================================
-- 4. NHACUNGCAP (Nhà cung cấp)
-- =============================================
INSERT INTO [dbo].[NhaCungCap] ([MaNCC], [TenNCC], [SDT], [DiaChi])
VALUES
    ('NCC001', N'Công ty Điện tử ABC', '0987654321', N'100 Cầu Giấy, Hà Nội'),
    ('NCC002', N'Thực phẩm sạch XYZ', '0976543210', N'200 Ba Đình, Hà Nội'),
    ('NCC003', N'Thời trang Fashion', '0965432109', N'300 Hai Bà Trưng, Hà Nội'),
    ('NCC004', N'Gia dụng Home', '0954321098', N'400 Hoàn Kiếm, Hà Nội'),
    ('NCC005', N'Văn phòng phẩm Office', '0943210987', N'500 Đống Đa, Hà Nội')
GO

-- =============================================
-- 5. KHACHHANG (Khách hàng)
-- =============================================
INSERT INTO [dbo].[KhachHang] ([MaKH], [TenKH], [SDT], [DiaChi])
VALUES
    ('KH001', N'Cửa hàng Điện máy Xanh', '0911111111', N'111 Trần Duy Hưng, Hà Nội'),
    ('KH002', N'Siêu thị Coopmart', '0922222222', N'222 Nguyễn Văn Cừ, TP.HCM'),
    ('KH003', N'Cửa hàng Thời trang', '0933333333', N'333 Lê Lợi, Đà Nẵng'),
    ('KH004', N'Công ty TNHH ABC', '0944444444', N'444 Hoàng Hoa Thám, Hà Nội'),
    ('KH005', N'Cửa hàng Gia dụng', '0955555555', N'555 Giải Phóng, Hà Nội')
GO

-- =============================================
-- 6. HANGHOA (Hàng hóa)
-- =============================================
INSERT INTO [dbo].[HangHoa] ([MaHang], [TenHang], [DonViTinh], [SoLuongTon], [GiaNhap], [GiaXuat], [MaLoai], [MaKho])
VALUES
    -- Điện tử
    ('HH001', N'Laptop Dell XPS 15', N'Cái', 25, 25000000, 28000000, 'LH001', 'KHO001'),
    ('HH002', N'iPhone 15 Pro Max', N'Cái', 50, 30000000, 33000000, 'LH001', 'KHO001'),
    ('HH003', N'Samsung Galaxy S24', N'Cái', 30, 20000000, 22000000, 'LH001', 'KHO001'),
    ('HH004', N'Tai nghe AirPods Pro', N'Cái', 100, 5000000, 5500000, 'LH001', 'KHO001'),
    ('HH005', N'Chuột không dây Logitech', N'Cái', 200, 500000, 650000, 'LH001', 'KHO001'),
    
    -- Thực phẩm
    ('HH006', N'Gạo ST25', N'Kg', 1000, 25000, 30000, 'LH002', 'KHO002'),
    ('HH007', N'Đường trắng', N'Kg', 500, 15000, 18000, 'LH002', 'KHO002'),
    ('HH008', N'Dầu ăn Neptune', N'Chai', 300, 45000, 55000, 'LH002', 'KHO002'),
    ('HH009', N'Mì tôm Hảo Hảo', N'Thùng', 200, 120000, 150000, 'LH002', 'KHO002'),
    ('HH010', N'Nước suối Lavie', N'Thùng', 500, 80000, 100000, 'LH002', 'KHO002'),
    
    -- Quần áo
    ('HH011', N'Áo thun nam', N'Cái', 150, 150000, 200000, 'LH003', 'KHO003'),
    ('HH012', N'Quần jean nữ', N'Cái', 80, 300000, 400000, 'LH003', 'KHO003'),
    ('HH013', N'Áo sơ mi công sở', N'Cái', 120, 250000, 320000, 'LH003', 'KHO003'),
    ('HH014', N'Váy đầm', N'Cái', 60, 400000, 550000, 'LH003', 'KHO003'),
    ('HH015', N'Áo khoác gió', N'Cái', 40, 500000, 700000, 'LH003', 'KHO003'),
    
    -- Đồ gia dụng
    ('HH016', N'Bếp từ Sunhouse', N'Cái', 20, 2000000, 2500000, 'LH004', 'KHO001'),
    ('HH017', N'Nồi cơm điện Tiger', N'Cái', 35, 1500000, 1900000, 'LH004', 'KHO001'),
    ('HH018', N'Máy xay sinh tố', N'Cái', 45, 800000, 1000000, 'LH004', 'KHO001'),
    ('HH019', N'Quạt điện', N'Cái', 60, 600000, 800000, 'LH004', 'KHO001'),
    ('HH020', N'Bàn là hơi nước', N'Cái', 25, 500000, 650000, 'LH004', 'KHO001'),
    
    -- Văn phòng phẩm
    ('HH021', N'Bút bi Thiên Long', N'Cây', 1000, 3000, 5000, 'LH005', 'KHO002'),
    ('HH022', N'Vở học sinh', N'Quyển', 500, 10000, 15000, 'LH005', 'KHO002'),
    ('HH023', N'Bút chì 2B', N'Cây', 800, 2000, 4000, 'LH005', 'KHO002'),
    ('HH024', N'Thước kẻ', N'Cái', 600, 5000, 8000, 'LH005', 'KHO002'),
    ('HH025', N'Tẩy', N'Cục', 700, 3000, 5000, 'LH005', 'KHO002')
GO

-- =============================================
-- 7. PHIEUNHAP (Phiếu nhập)
-- =============================================
INSERT INTO [dbo].[PhieuNhap] ([MaPN], [NgayNhap], [MaNV], [MaNCC])
VALUES
    ('PN001', '2024-12-01', 'NV001', 'NCC001'),
    ('PN002', '2024-12-02', 'NV002', 'NCC002'),
    ('PN003', '2024-12-03', 'NV001', 'NCC003'),
    ('PN004', '2024-12-04', 'NV003', 'NCC001'),
    ('PN005', '2024-12-05', 'NV002', 'NCC004'),
    ('PN006', '2024-12-06', 'NV001', 'NCC005'),
    ('PN007', '2024-12-07', 'NV004', 'NCC002'),
    ('PN008', '2024-12-08', 'NV003', 'NCC003'),
    ('PN009', '2024-12-09', 'NV002', 'NCC001'),
    ('PN010', '2024-12-10', 'NV001', 'NCC004')
GO

-- =============================================
-- 8. CT_PHIEUNHAP (Chi tiết phiếu nhập)
-- =============================================
INSERT INTO [dbo].[CT_PhieuNhap] ([MaPN], [MaHang], [SoLuong], [DonGiaNhap])
VALUES
    -- PN001 - Điện tử
    ('PN001', 'HH001', 10, 25000000),
    ('PN001', 'HH002', 20, 30000000),
    ('PN001', 'HH003', 15, 20000000),
    
    -- PN002 - Thực phẩm
    ('PN002', 'HH006', 500, 25000),
    ('PN002', 'HH007', 200, 15000),
    ('PN002', 'HH008', 100, 45000),
    
    -- PN003 - Quần áo
    ('PN003', 'HH011', 50, 150000),
    ('PN003', 'HH012', 30, 300000),
    ('PN003', 'HH013', 40, 250000),
    
    -- PN004 - Điện tử
    ('PN004', 'HH004', 50, 5000000),
    ('PN004', 'HH005', 100, 500000),
    
    -- PN005 - Gia dụng
    ('PN005', 'HH016', 10, 2000000),
    ('PN005', 'HH017', 15, 1500000),
    ('PN005', 'HH018', 20, 800000),
    
    -- PN006 - Văn phòng phẩm
    ('PN006', 'HH021', 500, 3000),
    ('PN006', 'HH022', 200, 10000),
    ('PN006', 'HH023', 400, 2000),
    
    -- PN007 - Thực phẩm
    ('PN007', 'HH009', 100, 120000),
    ('PN007', 'HH010', 250, 80000),
    
    -- PN008 - Quần áo
    ('PN008', 'HH014', 30, 400000),
    ('PN008', 'HH015', 20, 500000),
    
    -- PN009 - Điện tử
    ('PN009', 'HH001', 15, 25000000),
    ('PN009', 'HH003', 15, 20000000),
    
    -- PN010 - Gia dụng
    ('PN010', 'HH019', 30, 600000),
    ('PN010', 'HH020', 15, 500000)
GO

-- =============================================
-- 9. PHIEUXUAT (Phiếu xuất)
-- =============================================
INSERT INTO [dbo].[PhieuXuat] ([MaPX], [NgayXuat], [MaNV], [MaKH])
VALUES
    ('PX001', '2024-12-11', 'NV001', 'KH001'),
    ('PX002', '2024-12-12', 'NV002', 'KH002'),
    ('PX003', '2024-12-13', 'NV003', 'KH003'),
    ('PX004', '2024-12-14', 'NV001', 'KH001'),
    ('PX005', '2024-12-15', 'NV002', 'KH004'),
    ('PX006', '2024-12-16', 'NV004', 'KH002'),
    ('PX007', '2024-12-17', 'NV003', 'KH005'),
    ('PX008', '2024-12-18', 'NV001', 'KH003'),
    ('PX009', '2024-12-19', 'NV002', 'KH001'),
    ('PX010', '2024-12-20', 'NV004', 'KH004')
GO

-- =============================================
-- 10. CT_PHIEUXUAT (Chi tiết phiếu xuất)
-- =============================================
INSERT INTO [dbo].[CT_PhieuXuat] ([MaPX], [MaHang], [SoLuong], [DonGiaXuat])
VALUES
    -- PX001 - Điện tử
    ('PX001', 'HH001', 5, 28000000),
    ('PX001', 'HH002', 10, 33000000),
    ('PX001', 'HH003', 8, 22000000),
    
    -- PX002 - Thực phẩm
    ('PX002', 'HH006', 200, 30000),
    ('PX002', 'HH007', 100, 18000),
    ('PX002', 'HH008', 50, 55000),
    
    -- PX003 - Quần áo
    ('PX003', 'HH011', 20, 200000),
    ('PX003', 'HH012', 10, 400000),
    ('PX003', 'HH013', 15, 320000),
    
    -- PX004 - Điện tử
    ('PX004', 'HH004', 25, 5500000),
    ('PX004', 'HH005', 50, 650000),
    
    -- PX005 - Gia dụng
    ('PX005', 'HH016', 5, 2500000),
    ('PX005', 'HH017', 8, 1900000),
    ('PX005', 'HH018', 10, 1000000),
    
    -- PX006 - Văn phòng phẩm
    ('PX006', 'HH021', 200, 5000),
    ('PX006', 'HH022', 100, 15000),
    ('PX006', 'HH023', 200, 4000),
    
    -- PX007 - Thực phẩm
    ('PX007', 'HH009', 50, 150000),
    ('PX007', 'HH010', 100, 100000),
    
    -- PX008 - Quần áo
    ('PX008', 'HH014', 15, 550000),
    ('PX008', 'HH015', 10, 700000),
    
    -- PX009 - Điện tử
    ('PX009', 'HH001', 8, 28000000),
    ('PX009', 'HH003', 7, 22000000),
    
    -- PX010 - Gia dụng
    ('PX010', 'HH019', 15, 800000),
    ('PX010', 'HH020', 8, 650000)
GO

-- =============================================
-- KIỂM TRA DỮ LIỆU
-- =============================================
-- SELECT COUNT(*) as SoKho FROM Kho
-- SELECT COUNT(*) as SoLoaiHang FROM LoaiHang
-- SELECT COUNT(*) as SoNhanVien FROM NhanVien
-- SELECT COUNT(*) as SoNhaCungCap FROM NhaCungCap
-- SELECT COUNT(*) as SoKhachHang FROM KhachHang
-- SELECT COUNT(*) as SoHangHoa FROM HangHoa
-- SELECT COUNT(*) as SoPhieuNhap FROM PhieuNhap
-- SELECT COUNT(*) as SoCTPhieuNhap FROM CT_PhieuNhap
-- SELECT COUNT(*) as SoPhieuXuat FROM PhieuXuat
-- SELECT COUNT(*) as SoCTPhieuXuat FROM CT_PhieuXuat
-- GO

PRINT N'✅ Đã thêm dữ liệu mẫu thành công!'
PRINT N'📊 Tổng quan:'
PRINT N'   - 3 Kho'
PRINT N'   - 5 Loại hàng'
PRINT N'   - 5 Nhân viên'
PRINT N'   - 5 Nhà cung cấp'
PRINT N'   - 5 Khách hàng'
PRINT N'   - 25 Hàng hóa'
PRINT N'   - 10 Phiếu nhập'
PRINT N'   - 10 Phiếu xuất'
GO

