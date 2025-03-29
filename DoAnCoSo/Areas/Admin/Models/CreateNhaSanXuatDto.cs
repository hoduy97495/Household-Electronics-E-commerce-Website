using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Areas.Admin.Models
{
    public class CreateNhaSanXuatDto
    {
        [Required(ErrorMessage = "Tên nhà sản xuất là trường bắt buộc")]
        public string TenNsx { get; set; }

        [Required(ErrorMessage = "Địa chỉ là trường bắt buộc")]
        public string DiaChi { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Sdt { get; set; }
    }
}
