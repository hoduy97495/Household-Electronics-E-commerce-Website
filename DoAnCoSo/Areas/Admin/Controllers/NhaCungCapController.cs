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
    [Route("admin/nhacungcap")]
    [Authorize(Roles = "Admin")]
    public class NhaCungCapController : Controller
    {
        private readonly DacsContext _context;

        public NhaCungCapController(DacsContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> NhaCungCap(int? page)
        {
            int pageSize = 12;
            int pageNumber = page ?? 1;
            var nhaCungCaps = await _context.NhaCungCaps.ToListAsync();
            var pagedList = nhaCungCaps.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        [Route("ThemNhaCungCapMoi")]
        [HttpGet]
        public IActionResult ThemNhaCungCapMoi()
        {
            return View();
        }

        [Route("ThemNhaCungCapMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemNhaCungCapMoi([Bind("TenNcc, DiaChi, Email, Sdt")] NhaCungCap nhaCungCap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhaCungCap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(NhaCungCap));
            }
            return View(nhaCungCap);
        }

        [Route("SuaNhaCungCap")]
        [HttpGet]
        public async Task<IActionResult> SuaNhaCungCap(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }
            return View(nhaCungCap);
        }

        [Route("SuaNhaCungCap")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNhaCungCap(int id, [Bind("MaNcc, TenNcc, DiaChi, Email, Sdt")] NhaCungCap nhaCungCap)
        {
            if (id != nhaCungCap.MaNcc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhaCungCap);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(NhaCungCap));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaCungCapExists(nhaCungCap.MaNcc))
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save changes. The NhaCungCap was deleted by another user.");
                        return View(nhaCungCap);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(nhaCungCap);
        }

        [Route("XoaNhaCungCap")]
        [HttpGet]
        public async Task<IActionResult> XoaNhaCungCap(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
            if (nhaCungCap == null)
            {
                return NotFound();
            }
            return View(nhaCungCap);
        }

        [Route("XoaNhaCungCap")]
        [HttpPost, ActionName("XoaNhaCungCap")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNhaCungCapConfirmed(int id)
        {
            try
            {
                var nhaCungCap = await _context.NhaCungCaps.FindAsync(id);
                if (nhaCungCap != null)
                {
                    _context.NhaCungCaps.Remove(nhaCungCap);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Nhà cung cấp đã được xóa!";
                }
                else
                {
                    TempData["Message"] = "Không tìm thấy nhà cung cấp!";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Không thể xóa nhà cung cấp!";
            }

            return RedirectToAction(nameof(NhaCungCap));
        }

        private bool NhaCungCapExists(int id)
        {
            return _context.NhaCungCaps.Any(e => e.MaNcc == id);
        }
    }
}
