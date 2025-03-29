using DoAnCoSo.Models;

namespace DoAnCoSo.Repository
{
    public class DanhMucRepository : IDanhMucRepository
    {
        private readonly DacsContext _context;
        public DanhMucRepository(DacsContext context)
        {
            _context = context;
        }
        public DanhMuc Add(DanhMuc danhMuc)
        {
            _context.DanhMucs.Add(danhMuc);
            _context.SaveChanges();
            return danhMuc;
        }

        public DanhMuc Delete(int maDanhMuc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DanhMuc> GetAllDanhMuc()
        {
            return _context.DanhMucs;
        }

        public DanhMuc GetDanhMuc(int maDanhMuc)
        {
            return _context.DanhMucs.Find(maDanhMuc);
        }

        public DanhMuc Update(DanhMuc danhMuc)
        {
            _context.Update(danhMuc);
            _context.SaveChanges();
            return danhMuc;
        }
    }
}
