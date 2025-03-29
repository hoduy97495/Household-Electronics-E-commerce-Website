using DoAnCoSo.Models;
using Microsoft.AspNetCore.Mvc;
using DoAnCoSo.Helpers;
using DoAnCoSo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DoAnCoSo.Services;

namespace DoAnCoSo.Controllers
{
    public class CartController : Controller
    {
        private readonly DacsContext dbcontext;
        private readonly IVnPayService _vnPayservice;
        public CartController(DacsContext context, IVnPayService vnPayservice)
        {
            dbcontext = context;
            _vnPayservice = vnPayservice;
        }
        //const string CART_KEY = "MYCART";
        //public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int maSanPham, int soLuong = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaSanPham == maSanPham);
            if (item == null)
            {
                var sanPham = dbcontext.SanPhams.SingleOrDefault(p => p.MaSanPham == maSanPham);
                if (sanPham == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {maSanPham}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaSanPham = sanPham.MaSanPham,
                    TenSp = sanPham.TenSp,
                    Gia = sanPham.Gia ?? 0,
                    HinhSp = sanPham.HinhSp ?? string.Empty,
                    SoLuong = soLuong


                };
                gioHang.Add(item);

            }
            else
            {
                item.SoLuong += soLuong;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("index");
        }

        public IActionResult UpdateCart([FromBody] List<CartItem> items)
        {
            var gioHang = Cart;

            foreach (var updatedItem in items)
            {
                var item = gioHang.SingleOrDefault(p => p.MaSanPham == updatedItem.MaSanPham);
                if (item != null)
                {
                    if (updatedItem.SoLuong > 0)
                    {
                        item.SoLuong = updatedItem.SoLuong;
                    }
                    else
                    {
                        gioHang.Remove(item);
                    }
                }
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return Json(new { success = true, total = gioHang.Sum(i => i.ThanhTien) });
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaSanPham == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }
        //public async Task<IActionResult> Remove(int maSanPham)
        //{
        //    var gioHang = Cart;
        //    gioHang.RemoveAll(p => p.MaSanPham == maSanPham);
        //    if (gioHang.Count == 0)
        //    {
        //        HttpContext.Session.Remove(CART_KEY);
        //    }
        //    else
        //    {
        //        HttpContext.Session.Set(CART_KEY, gioHang);
        //    }
        //    return Json(new { success = true });
        //}
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {

            if (Cart.Count == 0)
            {
                return Redirect("/");
            }

            return View(Cart);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model, string payment = "COD")
        {
            if (ModelState.IsValid)
            {
                if (payment == "Thanh Toán VNPay")
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = Cart.Sum(p => p.ThanhTien),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.Ten} {model.SDT}",
                        FullName = model.Ten,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                }


                var taiKhoanId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == "CustomerID").Value);
                var taiKhoan = new TaiKhoan();

                if (model.GiongKhachHang)
                {
                    taiKhoan = dbcontext.TaiKhoans.SingleOrDefault(tk => tk.MaTk == taiKhoanId);
                }
                double tongTien = Cart.Sum(item => item.ThanhTien);

                var donhang = new DonDatHang
                {
                    MaTk = taiKhoanId,
                    Ten = model.Ten ?? taiKhoan.Ten,
                    DiaChi = model.DiaChi ?? taiKhoan.DiaChi,
                    Sdt = model.SDT ?? taiKhoan.Sdt,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "Grab",
                    TrangThai = 1,
                    GhiChu = model.GhiChu,
                    Tong = tongTien
                };

                var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY);

                if (cart == null || !cart.Any())
                {
                    ModelState.AddModelError(string.Empty, "Giỏ hàng của bạn đang trống.");
                    return View(model);
                }

                using (var transaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        dbcontext.Add(donhang);
                        dbcontext.SaveChanges();

                        var ctdh = new List<ChiTietDonHang>();
                        foreach (var item in cart)
                        {
                            var sanPham = dbcontext.SanPhams.SingleOrDefault(sp => sp.MaSanPham == item.MaSanPham);
                            if (sanPham == null)
                            {
                                transaction.Rollback();
                                ModelState.AddModelError(string.Empty, $"Sản phẩm với mã {item.MaSanPham} không tồn tại.");
                                return View(model);
                            }



                            // Trừ số lượng sản phẩm
                            sanPham.SoLuong -= item.SoLuong;
                            
                            // Tăng số lượt mua
                            sanPham.LuotMua = (sanPham.LuotMua ?? 0) + item.SoLuong;

                            ctdh.Add(new ChiTietDonHang
                            {
                                MaDdh = donhang.MaDdh,
                                SoLuong = item.SoLuong,
                                Gia = item.Gia,
                                TenSp =item.TenSp,
                                MaSanPham = item.MaSanPham
                            });
                        }

                        dbcontext.AddRange(ctdh);


                        dbcontext.SaveChanges();

                        // Xóa giỏ hàng sau khi đặt hàng thành công
                        HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                        transaction.Commit();
                        TempData["OrderDetails"] = donhang;


                        return View("Success");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Xử lý exception ở đây, ví dụ:
                        ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình đặt hàng. Vui lòng thử lại sau.");
                        // Hoặc ghi log lỗi
                        Console.WriteLine("Error occurred during checkout: " + ex.Message);
                    }
                }
            }

            // Nếu không thành công, quay lại trang checkout với model hiện tại
            return View(model);
        }
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }
        [Authorize]
        public IActionResult PaymentSuccess()
        {
          
            return View("Success");
        }
        [Authorize]
        [HttpGet]
        [Route("Cart/PaymentCallBack")]
        public IActionResult PaymentCalBack()
        {
			var response = _vnPayservice.PaymentExecute(Request.Query);

			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode ?? "Unknown error"}";
				return RedirectToAction("PaymentFail");
			}

			// Extract cart from session
			var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY); 

			if (cart == null || !cart.Any())
			{
				TempData["Message"] = "Giỏ hàng của bạn đang trống.";
				return RedirectToAction("PaymentFail");
			}

			var taiKhoanId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == "CustomerID").Value);
			var taiKhoan = dbcontext.TaiKhoans.SingleOrDefault(tk => tk.MaTk == taiKhoanId);
			double tongTien = cart.Sum(item => item.ThanhTien);

			var donhang = new DonDatHang
			{
				MaTk = taiKhoanId,
				Ten = taiKhoan.Ten,
				DiaChi = taiKhoan.DiaChi,
				Sdt = taiKhoan.Sdt,
				NgayDat = DateTime.Now,
				CachThanhToan = "VNPay",
				CachVanChuyen = "Grab",
				TrangThai = 1,
				GhiChu = string.Empty,
				Tong = tongTien
			};

			using (var transaction = dbcontext.Database.BeginTransaction())
			{
				try
				{
					dbcontext.Add(donhang);
					dbcontext.SaveChanges();

					var ctdh = new List<ChiTietDonHang>();
					foreach (var item in cart)
					{
						var sanPham = dbcontext.SanPhams.SingleOrDefault(sp => sp.MaSanPham == item.MaSanPham);
						if (sanPham == null)
						{
							transaction.Rollback();
							TempData["Message"] = $"Sản phẩm với mã {item.MaSanPham} không tồn tại.";
							return RedirectToAction("PaymentFail");
						}

						// Trừ số lượng sản phẩm
						sanPham.SoLuong -= item.SoLuong;

						// Tăng số lượt mua
						sanPham.LuotMua = (sanPham.LuotMua ?? 0) + item.SoLuong;

						ctdh.Add(new ChiTietDonHang
						{
							MaDdh = donhang.MaDdh,
							SoLuong = item.SoLuong,
							Gia = item.Gia,
							TenSp = item.TenSp,
							MaSanPham = item.MaSanPham
						});
					}

					dbcontext.AddRange(ctdh);
					dbcontext.SaveChanges();

					// Xóa giỏ hàng sau khi đặt hàng thành công
					HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

					transaction.Commit();
                    TempData["OrderDetails"] = donhang;
                    TempData["Message"] = "Thanh toán VNPay thành công";
                    //return RedirectToAction("PaymentSuccess");
                    return View("Success");
                }
				catch (Exception ex)
				{
					transaction.Rollback();
					TempData["Message"] = "Đã xảy ra lỗi trong quá trình đặt hàng. Vui lòng thử lại sau.";
					Console.WriteLine("Error occurred during payment callback: " + ex.Message);
					return RedirectToAction("PaymentFail");
				}
			}
		}
       
    }
}
