using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class NhaSanXuat
{
    public int MaNsx { get; set; }

    public string? TenNsx { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
