using static DoAnCoSo.Areas.Admin.Controllers.PhieuNhapController;

namespace DoAnCoSo.Areas.Admin.Models
{
    public class PhieuNhapViewModel
    {
        public int NhaCungCapId { get; set; }
        public List<ChiTietPhieuNhapViewModel> ChiTietPhieuNhaps { get; set; }
    }
}
