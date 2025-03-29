using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DoAnCoSo.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/danhmuc")]
    [Authorize(Roles = "Admin")]
    public class DanhMucController : Controller
    {
        private readonly DacsContext _context;

        public DanhMucController(DacsContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> DanhMuc(int? page)
        {
            int pageSize = 12;
            int pageNumber = page ?? 1;
            var danhMucs = await _context.DanhMucs.ToListAsync();
            var pagedList = danhMucs.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        [Route("ThemDanhMucMoi")]
        [HttpGet]
        public IActionResult ThemDanhMucMoi()
        {
            return View();
        }

        [Route("ThemDanhMucMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDanhMucMoi([Bind("TenDanhMuc")] DanhMuc danhMuc)
        {
             if (!string.IsNullOrEmpty(danhMuc.TenDanhMuc))
            {
                _context.Add(danhMuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DanhMuc));
            }
            return View(danhMuc);
        }

        [Route("SuaDanhMuc")]
        [HttpGet]
        public async Task<IActionResult> SuaDanhMuc(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc);
        }

        [Route("SuaDanhMuc")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaDanhMuc([Bind("MaDanhMuc, TenDanhMuc")] DanhMuc danhMuc)
        {

            try
            {
                var danhMucDb = await _context.DanhMucs.FirstOrDefaultAsync(c => c.MaDanhMuc == danhMuc.MaDanhMuc);
                if (danhMucDb != null)
                {
                    danhMucDb.TenDanhMuc = danhMuc.TenDanhMuc;
                    _context.Update(danhMucDb);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DanhMuc));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Sửa danh mục thất bại");
                    return View(danhMuc);
                }
              
              
            }
            catch (Exception ex )
            {

                 ModelState.AddModelError(string.Empty, "Sửa danh mục thất bại");
               
            }
           
            return View(danhMuc);
        }

        [Route("XoaDanhMuc")]
        [HttpGet]
        public async Task<IActionResult> XoaDanhMuc(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc);
        }

        [Route("XoaDanhMuc")]
        [HttpPost, ActionName("XoaDanhMuc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDanhMucConfirmed(int id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc != null)
            {
                _context.DanhMucs.Remove(danhMuc);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Danh mục đã được xóa!";
            }
            else
            {
                TempData["Message"] = "Không tìm thấy danh mục!";
            }

            return RedirectToAction(nameof(DanhMuc));
        }

        private bool DanhMucExists(int id)
        {
            return _context.DanhMucs.Any(e => e.MaDanhMuc == id);
        }
    }
}
