using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string? TenSp { get; set; }

    public string? HinhSp { get; set; }

    public int? SoLuong { get; set; }

    public double? Gia { get; set; }

    public int? LuotMua { get; set; }

    public string? MoTa { get; set; }

    public string? ChiTiet { get; set; }

    public DateTime? NgayTao { get; set; }

    public int? MaNsx { get; set; }

    public int? MaNcc { get; set; }

    public int? MaDanhMuc { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<CtphieuNhap> CtphieuNhaps { get; set; } = new List<CtphieuNhap>();

    public virtual DanhMuc? MaDanhMucNavigation { get; set; }

    public virtual NhaCungCap? MaNccNavigation { get; set; }

    public virtual NhaSanXuat? MaNsxNavigation { get; set; }
}
