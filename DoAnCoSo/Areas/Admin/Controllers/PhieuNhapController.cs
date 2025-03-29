using DoAnCoSo.Areas.Admin.Models;
using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using static DoAnCoSo.Areas.Admin.Controllers.PhieuNhapController;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/phieunhap")]
    [Authorize(Roles = "Admin")]
    public class PhieuNhapController : Controller
    {
        private readonly DacsContext _context;

        public PhieuNhapController(DacsContext context)
        {
            _context = context;
        }
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index(string? searchQuery, int? page)
        {

            var phieuNhap = await _context.PhieuNhaps.Include(c => c.MaNccNavigation).ToListAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                phieuNhap = phieuNhap.Where(c => c.MaNccNavigation.TenNcc.ToLower().Contains(searchQuery.ToLower())).ToList();
            }
            int pageSize = 10; // Số bản ghi trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại
            var pagedList = phieuNhap.ToPagedList(pageNumber, pageSize); // Tạo trang dữ liệu

            ViewBag.SearchQuery = searchQuery; // Lưu lại searchQuery để truyền cho view     
            ViewBag.PageNumber = pageNumber;

            return View(pagedList);
        }
        [Route("create")]
        public async Task<IActionResult> Create()
        {

            var sanPhams = await _context.SanPhams.ToListAsync();
            var selectSanphams = new List<SelectListItem>();
            sanPhams.ForEach(s =>
            {

                var sp = new SelectListItem { Text = $"Mã sản phẩm : {s.MaSanPham} Tên sản phẩm : {s.TenSp} ", Value = s.MaSanPham.ToString() };
                selectSanphams.Add(sp);
            });
            var nhaCungCaps = await _context.NhaCungCaps.ToListAsync();
            var selectnhaCungCaps = new List<SelectListItem>();
            nhaCungCaps.ForEach(s =>
            {

                var ncc = new SelectListItem { Text = $"Mã NCC : {s.MaNcc} Tên NCC : {s.TenNcc} ", Value = s.MaNcc.ToString() };
                selectnhaCungCaps.Add(ncc);
            });


            ViewBag.SanPhams = selectSanphams;

            ViewBag.NhaCungCaps = selectnhaCungCaps;

            return View();
        }
        [Route("edit")]
        public async Task<IActionResult> Edit(int maPN)
        {

            var sanPhams = await _context.SanPhams.ToListAsync();
            var selectSanphams = new List<SelectListItem>();
            sanPhams.ForEach(s =>
            {

                var sp = new SelectListItem { Text = $"Mã sản phẩm : {s.MaSanPham} Tên sản phẩm : {s.TenSp} ", Value = s.MaSanPham.ToString() };
                selectSanphams.Add(sp);
            });



            var phieuNhap = await _context.PhieuNhaps.FirstOrDefaultAsync(c => c.MaPn == maPN);
            if (phieuNhap != null)
            {
                ViewBag.SanPhams = selectSanphams;
                ViewBag.CTPNSanPhams = _context.CtphieuNhaps.Where(c => c.MaPn == maPN).ToList();
                ViewBag.PhieuNhap = phieuNhap;
                ViewBag.MaPN = maPN;
                var selectedValue = phieuNhap.MaNcc.ToString(); // Convert selected value to string
                var nhaCungCaps = await _context.NhaCungCaps.ToListAsync();
                var selectnhaCungCaps = nhaCungCaps.Select(s =>
                {
                    var isSelected = s.MaNcc.ToString() == selectedValue; // Check if MaNcc matches the selected value
                    return new SelectListItem
                    {
                        Text = $"Mã NCC : {s.MaNcc} Tên NCC : {s.TenNcc}",
                        Value = s.MaNcc.ToString(),
                        Selected = isSelected
                    };
                }).ToList();

                ViewBag.NhaCungCaps = selectnhaCungCaps;
            }

            return View();
        }
        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> Edit(EditPhieuNhapDto model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // xóa chi tiết phiếu nhập
                    var ctPhieuNhaps = await _context.CtphieuNhaps.Where(c => c.MaPn == model.MaPN).ToListAsync();

                    if (ctPhieuNhaps.Count > 0)
                    {
                        foreach (var chiTiet in ctPhieuNhaps)
                        {
                            var sanPham = _context.SanPhams.FirstOrDefault(c => c.MaSanPham == chiTiet.MaSanPham);
                            if (sanPham != null)
                            {
                                if (sanPham.SoLuong.HasValue)
                                {
                                    sanPham.SoLuong = sanPham.SoLuong -= chiTiet.SoLuongNhap;
                                    _context.SanPhams.Update(sanPham);

                                }
                            }
                        }
                        _context.RemoveRange(ctPhieuNhaps);
                    }
                    if (model.ChiTietPhieuNhaps != null)
                    {
                        

                        //Cập nhật lại NCC
                        var phieuNhap = _context.PhieuNhaps.FirstOrDefault(c => c.MaPn == model.MaPN);
                        phieuNhap.MaNcc = model.NhaCungCapId;
                        _context.PhieuNhaps.Update(phieuNhap);
                        // Tạo chi tiết phiếu nhập
                        foreach (var chiTiet in model.ChiTietPhieuNhaps)
                        {
                            var ctPhieuNhap = new CtphieuNhap
                            {
                                MaPn = phieuNhap.MaPn,
                                MaSanPham = chiTiet.SanPhamId,
                                SoLuongNhap = chiTiet.SoLuong,
                                DonGia = chiTiet.DonGia
                            };
                            _context.CtphieuNhaps.Add(ctPhieuNhap);

                            var sanPham = _context.SanPhams.FirstOrDefault(c => c.MaSanPham == chiTiet.SanPhamId);
                            if (sanPham != null)
                            {
                                if (sanPham.SoLuong.HasValue)
                                {
                                    sanPham.SoLuong = sanPham.SoLuong += chiTiet.SoLuong;
                                    _context.SanPhams.Update(sanPham);
                                }
                            }


                        }

                    }
                    await _context.SaveChangesAsync();

                    // Hoàn tất giao dịch
                    await transaction.CommitAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Rollback giao dịch nếu có lỗi xảy ra
                    transaction.Rollback();
                    // Xử lý lỗi
                    return RedirectToAction("Index");
                }
            }
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(PhieuNhapViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tạo phiếu nhập
                    var phieuNhap = new PhieuNhap
                    {
                        NgayNhap = DateTime.Now,
                        MaNcc = model.NhaCungCapId
                    };
                    await _context.PhieuNhaps.AddAsync(phieuNhap);
                    await _context.SaveChangesAsync();

                    if (model.ChiTietPhieuNhaps != null)
                    {
                        // Tạo chi tiết phiếu nhập
                        foreach (var chiTiet in model.ChiTietPhieuNhaps)
                        {
                            var ctPhieuNhap = new CtphieuNhap
                            {
                                MaPn = phieuNhap.MaPn,
                                MaSanPham = chiTiet.SanPhamId,
                                SoLuongNhap = chiTiet.SoLuong,
                                DonGia = chiTiet.DonGia
                            };
                            _context.CtphieuNhaps.Add(ctPhieuNhap);

                            var sanPham = _context.SanPhams.FirstOrDefault(c => c.MaSanPham == chiTiet.SanPhamId);
                            if (sanPham != null)
                            {
                                if (sanPham.SoLuong.HasValue)
                                {
                                    sanPham.SoLuong = sanPham.SoLuong += chiTiet.SoLuong;
                                    _context.SanPhams.Update(sanPham);
                                }
                            }


                        }
                    }

                    await _context.SaveChangesAsync();

                    // Hoàn tất giao dịch
                    await transaction.CommitAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Rollback giao dịch nếu có lỗi xảy ra
                    transaction.Rollback();
                    // Xử lý lỗi
                    return RedirectToAction("Index");
                }
            }
        }


        [HttpGet]
        [Route("detail")]
        public async Task<IActionResult> Detail(int maPN)
        {

            var phieuNhap = await _context.PhieuNhaps.Include(c => c.MaNccNavigation).Where(c => c.MaPn == maPN).FirstOrDefaultAsync();
            var chiTietSPPhieuNhap = await _context.CtphieuNhaps.Include(c => c.MaSanPhamNavigation).Where(c => c.MaPn == maPN).ToListAsync();

            ViewBag.PhieuNhap = phieuNhap;
            ViewBag.PhieuNhapCT = chiTietSPPhieuNhap;

            return View();

        }
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(int maPN)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ctPhieuNhaps = await _context.CtphieuNhaps.Where(c => c.MaPn == maPN).ToListAsync();

                    if (ctPhieuNhaps.Count > 0)
                    {
                        foreach (var chiTiet in ctPhieuNhaps)
                        {
                            var sanPham = _context.SanPhams.FirstOrDefault(c => c.MaSanPham == chiTiet.MaSanPham);
                            if (sanPham != null)
                            {
                                if (sanPham.SoLuong.HasValue)
                                {
                                    sanPham.SoLuong = sanPham.SoLuong -= chiTiet.SoLuongNhap;
                                    _context.SanPhams.Update(sanPham);

                                }
                            }
                        }
                        _context.RemoveRange(ctPhieuNhaps);
                    }
                    var phieuNhap = await _context.PhieuNhaps.FirstOrDefaultAsync(c => c.MaPn == maPN);
                    if (phieuNhap != null)
                    {
                        _context.PhieuNhaps.Remove(phieuNhap);

                    }
                    await _context.SaveChangesAsync();
                    // Hoàn tất giao dịch
                    await transaction.CommitAsync();
                    return Json(1);
                }
                catch (Exception ex)
                {
                    // Rollback giao dịch nếu có lỗi xảy ra
                    transaction.Rollback();
                    // Xử lý lỗi
                    return Json(0);
                }

            }

        }
    }
}

