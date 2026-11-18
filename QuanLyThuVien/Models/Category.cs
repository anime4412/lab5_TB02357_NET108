using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thể loại sách không được để trống")]
        [StringLength(150, ErrorMessage = "Tên thể loại tối đa 150 ký tự")]
        [Display(Name = "Tên thể loại")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả thể loại")]
        public string? Description { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh đại diện")]
        public string? Image { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool Active { get; set; } = true;

        // Navigation property Code First
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
