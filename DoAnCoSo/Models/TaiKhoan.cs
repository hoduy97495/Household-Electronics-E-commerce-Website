using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class TaiKhoan
{
    public int MaTk { get; set; }

    public string? Ten { get; set; }

    public string? Sdt { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public DateTime? NgaySinh { get; set; }

    public int? MaDdh { get; set; }

    public string? MatKhau { get; set; }

    public string? RandomKey { get; set; }

    public int? VaiTro { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; set; } = new List<DonDatHang>();
}
