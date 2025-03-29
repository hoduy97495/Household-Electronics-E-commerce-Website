using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class DonDatHang
{
    public int MaDdh { get; set; }

    public DateTime? NgayDat { get; set; }

    public double? Tong { get; set; }

    public int? MaTk { get; set; }

    public int? SoLuong { get; set; }

    public string? CachThanhToan { get; set; }

    public string? Sdt { get; set; }

    public string? DiaChi { get; set; }

    public string? Ten { get; set; }

    public string? CachVanChuyen { get; set; }

    public string? GhiChu { get; set; }

    public double? PhiVanChuyen { get; set; }

    public int? TrangThai { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual TaiKhoan? MaTkNavigation { get; set; }
}
