using DoAnCoSo.Models;

namespace DoAnCoSo.Areas.Admin.Models
{
    public class OrderViewModel
    {
        public List<ChiTietDonHang> chiTietDonHangs { get; set; }
        public DonDatHang donHang { get; set; }
        public TaiKhoan khachHang { get; set; }
    }
}
