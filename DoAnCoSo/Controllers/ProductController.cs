using DoAnCoSo.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DoAnCoSo.Controllers
{
    public class ProductController : Controller
    {
        DacsContext db;
        public ProductController(DacsContext dacsContext)
        {
            db = dacsContext;
        }
        public IActionResult Index(int? page, string? searchQuery,int? maDanhMuc)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listsanpham = db.SanPhams.ToList();
            if (!string.IsNullOrEmpty( searchQuery))
            {
                listsanpham = listsanpham.Where(c => c.TenSp.ToLower().Contains(searchQuery)).ToList();
            }
            if (maDanhMuc != null)
            {
                listsanpham = listsanpham.Where(c => c.MaDanhMuc == maDanhMuc).ToList();
            }
            PagedList<SanPham> lst = new PagedList<SanPham>(listsanpham, pageNumber, pageSize);
            ViewBag.SearchQuery = searchQuery;
            ViewBag.PageNumber = pageNumber;
            ViewBag.MaDanhMuc = maDanhMuc;
            return View(lst);
        }


    }

}
