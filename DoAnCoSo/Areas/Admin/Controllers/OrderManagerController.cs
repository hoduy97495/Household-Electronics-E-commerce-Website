using Azure.Core;
using DoAnCoSo.Areas.Admin.Models;
using DoAnCoSo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace DoAnCoSo.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/donhang")]
    [Authorize(Roles = "Admin")]
    public class OrderManagerController : Controller
    {
        private readonly DacsContext _context;

        public OrderManagerController(DacsContext context)
        {
            _context = context;
        }
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index(string? searchQuery, int? page, int? trangThai,int ?status)
        {

            var donHang = await _context.DonDatHangs.Where(c=>c.TrangThai != 5 ).ToListAsync();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                donHang = donHang.Where(c=>c.Sdt.Contains(searchQuery)).ToList();
            }
            if (status != null)
            {
                donHang = donHang.Where(c => c.TrangThai == status).ToList();
            }
            int pageSize = 10; // Số bản ghi trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại
            var pagedList = donHang.ToPagedList(pageNumber, pageSize); // Tạo trang dữ liệu

            ViewBag.SearchQuery = searchQuery; // Lưu lại searchQuery để truyền cho view     
            ViewBag.PageNumber = pageNumber;

            return View(pagedList);
        }
        [Route("updateorder")]
        [HttpPost]
        public async Task< IActionResult> UpdateOrderStatus([FromBody] UpdateStatusOrderRequest request)
        {
            var order = await _context.DonDatHangs.FirstOrDefaultAsync(c=>c.MaDdh == request.OrderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại" });
            }
            order.TrangThai = request.NewStatus;
            _context.Update(order);
            _context.SaveChanges();

            return Json(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công" });
        }
        [Route("detail")]
        public async Task<IActionResult> Detail(int? orderId)
        {
            var order = await _context.DonDatHangs.FirstOrDefaultAsync(c => c.MaDdh == orderId && c.TrangThai != 5);
            var chitietDonHang = await _context.ChiTietDonHangs.Where(c => c.MaDdh == orderId).ToListAsync();
            var khachHang = await _context.TaiKhoans.FirstOrDefaultAsync(c => c.MaTk == order.MaTk);
            if (order == null || !chitietDonHang.Any() || khachHang == null)
            {
                return Redirect("/404");

            }

            OrderViewModel orderViewModel = new OrderViewModel();
            orderViewModel.donHang = order;
            orderViewModel.chiTietDonHangs = chitietDonHang;
            orderViewModel.khachHang = khachHang;
            return View(orderViewModel);
        }
        [Route("DeleteOrder")]
        [HttpPost]
        public JsonResult Delete(int madh)
        {
            if (madh != 0)
            {
                var donHang = _context.DonDatHangs.FirstOrDefault(c => c.MaDdh == madh);
                if (donHang == null)
                {
                    return Json(0);
                }
                donHang.TrangThai = 5;
                _context.DonDatHangs.Update(donHang);
                _context.SaveChanges();
                return Json(1);
            }
            return Json(0);
        }
    }
}
