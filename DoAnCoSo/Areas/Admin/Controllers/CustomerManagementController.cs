using DoAnCoSo.Areas.Admin.Models;
using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/khachhang")]
    [Authorize(Roles = "Admin")]
    public class CustomerManagementController : Controller
    {
        private readonly DacsContext _context;

        public CustomerManagementController(DacsContext context)
        {
            _context = context;
        }
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index(string? searchQuery, int? page)
        {

            var khachHang = await _context.TaiKhoans.Where(c => c.VaiTro == 3).ToListAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                khachHang = khachHang.Where(c => c.Ten.Contains(searchQuery) || c.Sdt.Contains(searchQuery) || c.Email.Contains(searchQuery)).ToList();
            }
            int pageSize = 10; // Số bản ghi trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại
            var pagedList = khachHang.ToPagedList(pageNumber, pageSize); // Tạo trang dữ liệu

            ViewBag.SearchQuery = searchQuery; // Lưu lại searchQuery để truyền cho view     
            ViewBag.PageNumber = pageNumber;

            return View(pagedList);
        }
        [Route("CreateCustomer")]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [Route("EditCustomer")]
        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var khachhang = await _context.TaiKhoans.FirstOrDefaultAsync(c => c.MaTk == id);
            if (khachhang == null) return RedirectToAction("Error", "Home");

            CreateCustomerDto customerDto = new CreateCustomerDto();
            customerDto.MaTk = khachhang.MaTk;
            customerDto.Ten = khachhang.Ten;
            customerDto.Sdt = khachhang.Sdt;
            customerDto.DiaChi = khachhang.DiaChi;
            customerDto.Email = khachhang.Email;
            customerDto.MatKhau = khachhang.MatKhau;
            customerDto.XacNhanMatKhau = khachhang.MatKhau;

            if (khachhang.NgaySinh != null)
            {
                customerDto.NgaySinh = khachhang.NgaySinh.Value;
            }
           
            return View(customerDto);


        }
        [Route("EditCustomer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(CreateCustomerDto khachHang)
        {
            if (khachHang.MaTk == null) return RedirectToAction("Index");

            TaiKhoan taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(c=>c.MaTk == khachHang.MaTk.Value);
            taiKhoan.Ten = khachHang.Ten;
            taiKhoan.Sdt = khachHang.Sdt;
            taiKhoan.DiaChi = khachHang.DiaChi;
            taiKhoan.Email = khachHang.Email;
            if (taiKhoan.MatKhau != khachHang.MatKhau)
            {
                taiKhoan.RandomKey = MyUtil.GenerateRandomKey();
                taiKhoan.MatKhau = khachHang.MatKhau.ToMd5Hash(taiKhoan.RandomKey);
            }
            taiKhoan.NgaySinh = khachHang.NgaySinh;
            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [Route("CreateCustomer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto khachHang)
        {

            if (khachHang != null)
            {
                var checkExist = _context.TaiKhoans.FirstOrDefault(c => c.Email == khachHang.Email);
                if (checkExist != null)
                {
                    ModelState.AddModelError("", "Tài khoản đã tồn tại.");
                    return View();
                }

                TaiKhoan taiKhoan = new TaiKhoan();
                taiKhoan.Ten = khachHang.Ten;
                taiKhoan.Sdt = khachHang.Sdt;
                taiKhoan.DiaChi = khachHang.DiaChi;
                taiKhoan.Email = khachHang.Email;
                taiKhoan.RandomKey = MyUtil.GenerateRandomKey();
                taiKhoan.MatKhau = khachHang.MatKhau.ToMd5Hash(taiKhoan.RandomKey);          
                taiKhoan.NgaySinh = khachHang.NgaySinh;
                taiKhoan.VaiTro = 3;
                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();


                return RedirectToAction("Index");
            }
            else
            {

                ModelState.AddModelError("", "Lỗi.");
            }


            return View();
        }

        [Route("DeleteCustomer")]
        [HttpPost]
        public JsonResult Delete(int maKh)
        {
            if (maKh != 0)
            {
                var khachhang = _context.TaiKhoans.FirstOrDefault(c => c.MaTk == maKh);
                if (khachhang == null)
                {
                    return Json(0);
                }
                _context.TaiKhoans.Remove(khachhang);
                _context.SaveChanges();
                return Json(1);
            }
            return Json(0);
        }

    }
}

