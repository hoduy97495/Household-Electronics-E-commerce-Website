using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class CtphieuNhap
{
    public int MaCtpn { get; set; }

    public double? DonGia { get; set; }

    public int? SoLuongNhap { get; set; }

    public int? MaSanPham { get; set; }

    public int? MaPn { get; set; }

    public virtual PhieuNhap? MaPnNavigation { get; set; }

    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
