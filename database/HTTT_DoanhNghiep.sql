USE [HTTT_DoanhNghiep]
GO
/****** Object:  Table [dbo].[TrangThaiUser]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrangThaiUser](
	[IDTrangThaiUser] [int] IDENTITY(1,1) NOT NULL,
	[TenTrangThaiUser] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_TrangThaiUser] PRIMARY KEY CLUSTERED 
(
	[IDTrangThaiUser] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrangThaiBaiDang]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrangThaiBaiDang](
	[IDTrangThai] [int] IDENTITY(1,1) NOT NULL,
	[TenTrangThai] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_TrangThaiBaiDang] PRIMARY KEY CLUSTERED 
(
	[IDTrangThai] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrangThai_DonHangMua]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrangThai_DonHangMua](
	[IDTrangThai] [int] NOT NULL,
	[TenTrangThai] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TrangThai_DonHangMua] PRIMARY KEY CLUSTERED 
(
	[IDTrangThai] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiSanPham]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiSanPham](
	[MaLoai] [int] IDENTITY(1,1) NOT NULL,
	[TenLoai] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_LoaiSanPham] PRIMARY KEY CLUSTERED 
(
	[MaLoai] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DanhGiaSanPham]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DanhGiaSanPham](
	[IDDanhGia] [int] IDENTITY(1,1) NOT NULL,
	[IDUserMua] [int] NOT NULL,
	[NgayDanhGia] [datetime] NOT NULL,
	[DiemDanhGia] [int] NOT NULL,
	[MaSanPham] [int] NOT NULL,
 CONSTRAINT [PK_DanhGiaSanPham] PRIMARY KEY CLUSTERED 
(
	[IDDanhGia] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[IDUser] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](200) NOT NULL,
	[DiaChi] [nvarchar](max) NOT NULL,
	[Mail] [nvarchar](max) NOT NULL,
	[SoDienThoai] [nvarchar](50) NOT NULL,
	[IDTrangThaiUser] [int] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[IDUser] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BaiDang]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BaiDang](
	[MaSanPham] [int] IDENTITY(1,1) NOT NULL,
	[MaLoai] [int] NOT NULL,
	[TenSP] [nvarchar](max) NOT NULL,
	[MoTa] [nvarchar](max) NOT NULL,
	[GiaSP] [int] NOT NULL,
	[SoLuong] [int] NOT NULL,
	[HinhAnh] [nvarchar](max) NOT NULL,
	[NgayDang] [datetime] NOT NULL,
	[IDTrangThai] [int] NOT NULL,
	[Soluongton] [int] NOT NULL,
	[DanhGia] [float] NOT NULL,
 CONSTRAINT [PK_BaiDang] PRIMARY KEY CLUSTERED 
(
	[MaSanPham] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GioHang_DonHangMua]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GioHang_DonHangMua](
	[IDDonHangMua] [int] IDENTITY(1,1) NOT NULL,
	[IDUserMua] [int] NOT NULL,
	[TongTien] [float] NOT NULL,
	[NgayLap] [datetime] NOT NULL,
	[IDTrangThai] [int] NOT NULL,
 CONSTRAINT [PK_GioHang_DonHangMua] PRIMARY KEY CLUSTERED 
(
	[IDDonHangMua] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTietGioHang]    Script Date: 11/23/2015 16:58:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTietGioHang](
	[IDDonHangMua] [int] NOT NULL,
	[MaSanPham] [int] NOT NULL,
	[SoLuong] [int] NOT NULL,
	[ThanhTien] [float] NOT NULL,
 CONSTRAINT [PK_ChiTietGioHang_1] PRIMARY KEY CLUSTERED 
(
	[IDDonHangMua] ASC,
	[MaSanPham] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_BaiDang_LoaiSanPham]    Script Date: 11/23/2015 16:58:17 ******/
ALTER TABLE [dbo].[BaiDang]  WITH CHECK ADD  CONSTRAINT [FK_BaiDang_LoaiSanPham] FOREIGN KEY([MaLoai])
REFERENCES [dbo].[LoaiSanPham] ([MaLoai])
GO
ALTER TABLE [dbo].[BaiDang] CHECK CONSTRAINT [FK_BaiDang_LoaiSanPham]
GO
/****** Object:  ForeignKey [FK_BaiDang_TrangThaiBaiDang]    Script Date: 11/23/2015 16:58:17 ******/
ALTER TABLE [dbo].[BaiDang]  WITH CHECK ADD  CONSTRAINT [FK_BaiDang_TrangThaiBaiDang] FOREIGN KEY([IDTrangThai])
REFERENCES [dbo].[TrangThaiBaiDang] ([IDTrangThai])
GO
ALTER TABLE [dbo].[BaiDang] CHECK CONSTRAINT [FK_BaiDang_TrangThaiBaiDang]
GO
/****** Object:  ForeignKey [FK_ChiTietGioHang_BaiDang]    Script Date: 11/23/2015 16:58:17 ******/
ALTER TABLE [dbo].[ChiTietGioHang]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietGioHang_BaiDang] FOREIGN KEY([MaSanPham])
REFERENCES [dbo].[BaiDang] ([MaSanPham])
GO
ALTER TABLE [dbo].[ChiTietGioHang] CHECK CONSTRAINT [FK_ChiTietGioHang_BaiDang]
GO
/****** Object:  ForeignKey [FK_ChiTietGioHang_GioHang_DonHangMua1]    Script Date: 11/23/2015 16:58:17 ******/
ALTER TABLE [dbo].[ChiTietGioHang]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietGioHang_GioHang_DonHangMua1] FOREIGN KEY([IDDonHangMua])
REFERENCES [dbo].[GioHang_DonHangMua] ([IDDonHangMua])
GO
ALTER TABLE [dbo].[ChiTietGioHang] CHECK CONSTRAINT [FK_ChiTietGioHang_GioHang_DonHangMua1]
GO
/****** Object:  ForeignKey [FK_GioHang_DonHangMua_TrangThai_DonHangMua]    Script Date: 11/23/2015 16:58:17 ******/
ALTER TABLE [dbo].[GioHang_DonHangMua]  WITH CHECK ADD  CONSTRAINT [FK_GioHang_DonHangMua_TrangThai_DonHangMua] FOREIGN KEY([IDTrangThai])
REFERENCES [dbo].[TrangThai_DonHangMua] ([IDTrangThai])
GO
ALTER TABLE [dbo].[GioHang_DonHangMua] CHECK CONSTRAINT [FK_GioHang_DonHangMua_TrangThai_DonHangMua]
GO
/****** Object:  ForeignKey [FK_GioHang_DonHangMua_User]    Script Date: 11/23/2015 16:58:17 ******/
ALTER TABLE [dbo].[GioHang_DonHangMua]  WITH CHECK ADD  CONSTRAINT [FK_GioHang_DonHangMua_User] FOREIGN KEY([IDUserMua])
REFERENCES [dbo].[User] ([IDUser])
GO
ALTER TABLE [dbo].[GioHang_DonHangMua] CHECK CONSTRAINT [FK_GioHang_DonHangMua_User]
GO
/****** Object:  ForeignKey [FK_User_TrangThaiUser]    Script Date: 11/23/2015 16:58:17 ******/
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_TrangThaiUser] FOREIGN KEY([IDTrangThaiUser])
REFERENCES [dbo].[TrangThaiUser] ([IDTrangThaiUser])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_TrangThaiUser]
GO
