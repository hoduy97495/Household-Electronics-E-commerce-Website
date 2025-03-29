using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.Areas.Admin.Models
{
    public class CreateCustomerDto
    {
        public int? MaTk { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có 10 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại phải là số")]
        public string Sdt { get; set; }

        [StringLength(300, ErrorMessage = "Địa chỉ không được quá 300 ký tự")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự", MinimumLength = 6)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("MatKhau", ErrorMessage = "Xác nhận mật khẩu không khớp")]
        public string XacNhanMatKhau { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime NgaySinh { get; set; }
    }
}