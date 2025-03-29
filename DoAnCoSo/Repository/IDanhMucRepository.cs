using DoAnCoSo.Models;

namespace DoAnCoSo.Repository
{
    public interface IDanhMucRepository
    {
        DanhMuc Add(DanhMuc danhMuc);
        DanhMuc Update(DanhMuc danhMuc);
        DanhMuc Delete(int maDanhMuc);
        DanhMuc GetDanhMuc(int maDanhMuc);
        IEnumerable<DanhMuc> GetAllDanhMuc();

    }
}
