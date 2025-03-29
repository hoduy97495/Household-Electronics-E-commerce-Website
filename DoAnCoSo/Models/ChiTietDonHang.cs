using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class ChiTietDonHang
{
    public int MaCtdh { get; set; }

    public int? MaSanPham { get; set; }

    public int? MaDdh { get; set; }

    public string? TenSp { get; set; }

    public int? SoLuong { get; set; }

    public double? Gia { get; set; }

    public virtual DonDatHang? MaDdhNavigation { get; set; }

    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
