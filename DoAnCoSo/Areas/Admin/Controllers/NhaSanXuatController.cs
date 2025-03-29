using DoAnCoSo.Areas.Admin.Models;
using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/nhasanxuat")]
    [Authorize(Roles = "Admin")]
    public class NhaSanXuatController : Controller
    {
        private readonly DacsContext _context;

        public NhaSanXuatController(DacsContext context)
        {
            _context = context;
        }
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index(string? searchQuery, int? page)
        {
            var khachHang = await _context.NhaSanXuats.ToListAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                khachHang = khachHang.Where(c => c.TenNsx.Contains(searchQuery) || c.Sdt.Contains(searchQuery) || c.Email.Contains(searchQuery)).ToList();
            }
            int pageSize = 10; // Số bản ghi trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại
            var pagedList = khachHang.ToPagedList(pageNumber, pageSize); // Tạo trang dữ liệu

            ViewBag.SearchQuery = searchQuery;      
            ViewBag.PageNumber = pageNumber;

            return View(pagedList);
        }
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaSanXuat nhaSanXuat)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem nhà sản xuất đã tồn tại hay chưa
                var checkExist = _context.NhaSanXuats.FirstOrDefault(c => c.Email == nhaSanXuat.Email);
                if (checkExist != null)
                {
                    ModelState.AddModelError("", "Nhà sản xuất đã tồn tại.");
                    return View(nhaSanXuat);
                }

                // Tạo mới một đối tượng nhà sản xuất từ dữ liệu được gửi từ form
                _context.NhaSanXuats.Add(nhaSanXuat);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, hiển thị form tạo mới lại với các thông báo lỗi
            return View(nhaSanXuat);
        }
        [Route("Edit")]
        public IActionResult Edit(int maNsx)
        {
            var nhaSanXuat = _context.NhaSanXuats.FirstOrDefault(c => c.MaNsx == maNsx);
            EditNhaSanXuatDto editNhaSanXuatDto = new EditNhaSanXuatDto();
            if (nhaSanXuat != null)
            {
                
                editNhaSanXuatDto.MaNsx = nhaSanXuat.MaNsx;
                editNhaSanXuatDto.TenNsx = nhaSanXuat.TenNsx;
                editNhaSanXuatDto.DiaChi = nhaSanXuat.DiaChi;
                editNhaSanXuatDto.Email = nhaSanXuat.Email;
                editNhaSanXuatDto.Sdt = nhaSanXuat.Sdt;
            }
           
            return View(editNhaSanXuatDto);
        }
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NhaSanXuat nhaSanXuat)
        {
            if (nhaSanXuat != null)
            {
                _context.NhaSanXuats.Update(nhaSanXuat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [Route("Delete")]
        [HttpPost]
        public JsonResult Delete(int maNsx)
        {
            if (maNsx != 0)
            {
                var nhaSanXuat = _context.NhaSanXuats.FirstOrDefault(c => c.MaNsx == maNsx);
                if (nhaSanXuat == null)
                {
                    return Json(0);
                }
                _context.NhaSanXuats.Remove(nhaSanXuat);
                _context.SaveChanges();
                return Json(1);
            }
            return Json(0);
        }
    }
}
