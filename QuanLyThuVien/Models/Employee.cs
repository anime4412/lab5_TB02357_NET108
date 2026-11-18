using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã nhân viên không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã nhân viên")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(150)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email đăng nhập")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Librarian"; // "Admin" hoặc "Librarian"

        [Display(Name = "Đang hoạt động")]
        public bool Active { get; set; } = true;
    }
}
