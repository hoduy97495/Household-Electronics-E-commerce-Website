using System.ComponentModel.DataAnnotations;

namespace DoAnCoSo.ViewModels
{
    public class RegisterVM
    {
        [Key]
        [Display(Name="Email")]
        [Required(ErrorMessage="*")]
        [EmailAddress(ErrorMessage="Chưa Đúng Định Dạng Email")]
        public string Email { get; set; }

        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "Họ Tên")]
        [Required(ErrorMessage = "*")]
        public string Ten { get; set; }

        [Display(Name = "Số Điện Thoại")]
        [Required(ErrorMessage = "*")]
        public string Sdt { get; set; }
        
        [Display(Name = "Địa Chỉ")]
        [Required(ErrorMessage = "*")]
        public string DiaChi { get; set; }

      

       

    }
}
