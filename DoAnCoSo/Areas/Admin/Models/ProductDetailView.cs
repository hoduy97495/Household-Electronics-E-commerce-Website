using DoAnCoSo.Models;

namespace DoAnCoSo.Areas.Admin.Models
{
    public class ProductDetailView
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

        public string TenNsx { get; set; }

        public string TenNcc { get; set; }

        public string TenDanhMuc { get; set; }


        
    }
}
