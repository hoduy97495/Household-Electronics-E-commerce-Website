using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class TrangThai
{
    public int MaTrangThai { get; set; }

    public string? TenTrangThai { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; set; } = new List<DonDatHang>();
}
