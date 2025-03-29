using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoAnCoSo.Models;
using Microsoft.AspNetCore.Authorization;
using DoAnCoSo.Areas.Admin.Models; // Import model

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    [Authorize(Roles = "Admin")]
    public class HomeAdminController : Controller
    {
        private readonly DacsContext _context;

        public HomeAdminController(DacsContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("GetReportData")]
        public JsonResult GetReportData(DateTime dateStart, DateTime dateEnd)
        {
            DateTime dateStartnew = dateStart.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime dateEndnew = dateEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // Đặt giờ thành 23:59:59

            var reports = new List<ReportViewModel>();

            // Tạo danh sách các ngày cần thống kê
            var dates = Enumerable.Range(0, (dateEnd - dateStart).Days + 1)
                                  .Select(i => dateStart.AddDays(i))
                                  .ToList();

            // Duyệt qua từng ngày và tính toán doanh thu, tổng đơn hàng
            foreach (var date in dates)
            {
                var revenue = _context.DonDatHangs
                            .Where(od => od.NgayDat.Value.Date == date.Date && od.TrangThai == 3)
                            .Sum(od => od.Tong);

                var totalOrders = _context.DonDatHangs
                                    .Where(o => o.NgayDat.Value.Date == date.Date && o.TrangThai == 3)
                                    .Count();

                ReportViewModel reportView = new ReportViewModel
                {
                    DateValue = date,
                    Order = totalOrders,
                    TotalMoney = revenue.Value,
                };

                reports.Add(reportView);
            }
            return Json(reports);
        }


        [Route("sanpham")]
        public async Task<IActionResult> SanPham(int? page, string? searchQuery)
        {
            int pageSize = 12;
            int pageNumber = page ?? 1;
            var sanPhams = await _context.SanPhams.Include(sp => sp.ChiTietDonHangs).ToListAsync();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                sanPhams = sanPhams.Where(c => c.TenSp.ToLower().Contains(searchQuery) ||c.MaSanPham.ToString() == searchQuery).ToList();
            }
            ViewBag.SearchQuery = searchQuery;    
            ViewBag.PageNumber = pageNumber;
            var pagedList = sanPhams.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }
        [Route("chitietsanpham")]
        public async Task<IActionResult> ChiTietSanPham(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var productDetail = await _context.SanPhams
                .Include(sp => sp.MaDanhMucNavigation)
                .Include(sp => sp.MaNccNavigation)
                .Include(sp => sp.MaNsxNavigation)
                .FirstOrDefaultAsync(sp => sp.MaSanPham == productId);

            var productDetailView = new ProductDetailView
            {
                MaSanPham = productDetail.MaSanPham,
                TenSp = productDetail.TenSp,
                HinhSp = productDetail.HinhSp,
                SoLuong = productDetail.SoLuong,
                Gia = productDetail.Gia,
                LuotMua = productDetail.LuotMua,
                MoTa = productDetail.MoTa,
                ChiTiet = productDetail.ChiTiet,
                NgayTao = productDetail.NgayTao,
                TenNsx = productDetail.MaNsxNavigation?.TenNsx ?? string.Empty,
                TenNcc = productDetail.MaNccNavigation?.TenNcc ?? string.Empty,
                TenDanhMuc = productDetail.MaDanhMucNavigation?.TenDanhMuc ?? string.Empty
            };


            return View(productDetailView);
        }

        [Route("ThemSanPhamMoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            LoadViewData();
            return View();
        }

        [Route("ThemSanPhamMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPhamMoi(SanPham sanPham, IFormFile HinhSp)
        {
            if (ModelState.IsValid)
            {
                if (HinhSp != null && HinhSp.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/images/hinhSP", HinhSp.FileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhSp.CopyToAsync(stream);
                    }

                    sanPham.HinhSp = "/" + HinhSp.FileName;
                }
                sanPham.NgayTao = DateTime.Now;
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SanPham));
            }

            LoadViewData();
            return View(sanPham);
        }


        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int? MaSanPham)
        {
            if (MaSanPham == null)
            {
                return NotFound();
            }

            var sanPham = _context.SanPhams.Find(MaSanPham);
            if (sanPham == null)
            {
                return NotFound();
            }

            LoadViewData();
            return View(sanPham);
        }

        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(sanPham).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(SanPham));
            }

            LoadViewData();
            return View(sanPham);
        }

        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(int? MaSanPham)
        {
            if (MaSanPham == null)
            {
                return NotFound();
            }

            var sanPham = _context.SanPhams.Find(MaSanPham);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        [Route("XoaSanPham")]
        [HttpPost, ActionName("XoaSanPham")]
        [ValidateAntiForgeryToken]
        public IActionResult XoaSanPhamConfirmed(int MaSanPham)
        {
            try
            {


                var sanPham = _context.SanPhams.Find(MaSanPham);
                if (sanPham != null)
                {
                    // Không cần xóa riêng ảnh vì nó nằm trong bảng SanPham
                    _context.SanPhams.Remove(sanPham);
                    _context.SaveChanges();
                    TempData["Message"] = "Sản phẩm đã được xóa!";
                }
                else
                {
                    TempData["Message"] = "Không tìm thấy sản phẩm!";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Không thể xóa sản phẩm";

            }

            return RedirectToAction(nameof(SanPham));
        }

        private void LoadViewData()
        {
            // Nạp danh sách các nhà sản xuất từ cơ sở dữ liệu
            var nhaSanXuats = _context.NhaSanXuats.ToList();
            ViewBag.MaNsx = nhaSanXuats.Any() ? new SelectList(nhaSanXuats, "MaNsx", "TenNsx") : new SelectList(new List<NhaSanXuat>(), "MaNsx", "TenNsx");

            // Nạp danh sách các nhà cung cấp từ cơ sở dữ liệu
            var nhaCungCaps = _context.NhaCungCaps.ToList();
            ViewBag.MaNcc = nhaCungCaps.Any() ? new SelectList(nhaCungCaps, "MaNcc", "TenNcc") : new SelectList(new List<NhaCungCap>(), "MaNcc", "TenNcc");

            // Nạp danh sách các danh mục từ cơ sở dữ liệu
            var danhMucs = _context.DanhMucs.ToList();
            ViewBag.MaDanhMuc = danhMucs.Any() ? new SelectList(danhMucs, "MaDanhMuc", "TenDanhMuc") : new SelectList(new List<DanhMuc>(), "MaDanhMuc", "TenDanhMuc");
        }
    }
}
