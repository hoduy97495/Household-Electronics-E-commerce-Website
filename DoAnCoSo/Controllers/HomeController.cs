using DoAnCoSo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing.Printing;
using X.PagedList;

namespace DoAnCoSo.Controllers
{

    public class HomeController : Controller
    {
        DacsContext db = new DacsContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //Phân Trang
        public IActionResult Index()
        {
            //int pageSize = 12;
            //int pageNumber=page==null || page <0?1:page.Value;
            //var listsanpham = db.SanPhams.ToList();
            //PagedList<SanPham> lst=new PagedList<SanPham>(listsanpham,pageNumber,pageSize);
            var lstDGD = db.SanPhams.Where(n => n.MaDanhMuc == 1).ToList();
            //Gán vào viewbag
            ViewBag.ListDGD = lstDGD;

            var lstTBD = db.SanPhams.Where(n => n.MaDanhMuc == 2).ToList();
            //Gán vào viewbag
            ViewBag.ListTBD = lstTBD;

            return View();
        }
        public IActionResult ChiTietSanPham(int maSanPham)
        {
            var sanPham = db.SanPhams
       .Include(sp => sp.MaNsxNavigation)
       .SingleOrDefault(x => x.MaSanPham == maSanPham);
            //var sanPham=db.SanPhams.SingleOrDefault(x=>x.MaSanPham == maSanPham);

            if (sanPham == null)
            {
                return NotFound(); // Tr? v? k?t qu? 404 n?u s?n ph?m không t?n t?i
            }
            ViewBag.TenNsx = sanPham.MaNsxNavigation?.TenNsx;
            return View(sanPham);
        }
        public IActionResult LienHe()
        {
            return View(); // Tr? v? view "lienhe.cshtml"
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
