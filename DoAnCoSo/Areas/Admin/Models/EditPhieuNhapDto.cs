namespace DoAnCoSo.Areas.Admin.Models
{
    public class EditPhieuNhapDto
    {
        public int MaPN { get; set; }
        public int NhaCungCapId { get; set; }
        public List<ChiTietPhieuNhapViewModel> ChiTietPhieuNhaps { get; set; }
    }
}
