using AutoMapper;
using DoAnCoSo.Helpers;
using DoAnCoSo.Models;
using DoAnCoSo.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using X.PagedList;

namespace DoAnCoSo.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly DacsContext db;
        private readonly IMapper _mapper;
        public TaiKhoanController(DacsContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;

        }
        #region Register 
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var taiKhoan = _mapper.Map<TaiKhoan>(model);
                taiKhoan.RandomKey = MyUtil.GenerateRandomKey();
                taiKhoan.MatKhau = model.MatKhau.ToMd5Hash(taiKhoan.RandomKey);
                taiKhoan.VaiTro = 3;
                db.Add(taiKhoan);
                db.SaveChanges();
                return RedirectToAction("Index", "Product");
            }
            return View();
        }
        #endregion

        #region Login in
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var taiKhoan = db.TaiKhoans.SingleOrDefault(tk => tk.Email == model.Email);
                if (taiKhoan == null)
                {
                    ModelState.AddModelError("loi", "Không tìm thấy");
                }
                else
                {
                    if (taiKhoan.MatKhau != model.MatKhau.ToMd5Hash(taiKhoan.RandomKey))
                    {
                        ModelState.AddModelError("loi", "Sai mật khẩu");
                    }
                    else
                    {
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, taiKhoan.Email),
                                new Claim(ClaimTypes.Name,taiKhoan.Ten),
                                new Claim("CustomerID", taiKhoan.MaTk.ToString()),



                                new Claim(ClaimTypes.Role,"Customer")


                            };
                        if (taiKhoan.VaiTro == 1) // Kiểm tra nếu tài khoản là admin
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        }
                        

                        var claimsIdentity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return Redirect("/");
                        }

                    }
                }
            }

            return View();
        }
        #endregion
        [Authorize]
        public IActionResult Profile()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "CustomerID")?.Value);

            // Tìm thông tin tài khoản của người dùng từ cơ sở dữ liệu
            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.MaTk == userId);

            // Kiểm tra xem có tồn tại thông tin tài khoản hay không
            if (taiKhoan == null)
            {
                // Trả về trang lỗi hoặc thực hiện xử lý phù hợp
                return NotFound(); // hoặc RedirectToAction("Action", "Controller");
            }

            // Truyền thông tin tài khoản đến view để hiển thị
            return View(taiKhoan);
        }
        [Authorize]
        public IActionResult OrderHistory()
        {
            // Lấy ID của người dùng hiện tại từ Claims
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "CustomerID")?.Value);

            // Lấy danh sách đơn đặt hàng của người dùng từ cơ sở dữ liệu
            var orders = db.DonDatHangs
                           .Where(o => o.MaTk == userId)
                           .OrderByDescending(o => o.NgayDat)
                           .ToList();

            // Truyền danh sách đơn đặt hàng đến view để hiển thị
            return View(orders);
        }
        public IActionResult ChiTietDonHang(int id)
        {
            var chiTietDonHang = db.ChiTietDonHangs.Where(ct => ct.MaDdh == id).ToList();
            return View(chiTietDonHang);
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

      

    }


}


