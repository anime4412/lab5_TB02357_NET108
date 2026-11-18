using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email đã đăng ký")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        [Display(Name = "Email nhân viên")]
        public string Email { get; set; } = string.Empty;
    }
}
