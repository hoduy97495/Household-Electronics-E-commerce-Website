using System;
using System.Collections.Generic;

namespace DoAnCoSo.Models;

public partial class PhieuNhap
{
    public int MaPn { get; set; }

    public DateTime? NgayNhap { get; set; }

    public int? MaNcc { get; set; }

    public virtual ICollection<CtphieuNhap> CtphieuNhaps { get; set; } = new List<CtphieuNhap>();

    public virtual NhaCungCap? MaNccNavigation { get; set; }
}
