using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã ISBN không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã ISBN")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên sách không được để trống")]
        [StringLength(250)]
        [Display(Name = "Tên sách")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Thể loại")]
        public int? CategoryId { get; set; }

        [StringLength(150)]
        [Display(Name = "Tác giả")]
        public string? Author { get; set; }

        [Range(1800, 2100, ErrorMessage = "Năm xuất bản không hợp lệ")]
        [Display(Name = "Năm xuất bản")]
        public int PublishYear { get; set; } = 2024;

        [Range(0, 100000000, ErrorMessage = "Giá bìa không hợp lệ")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá bìa (VNĐ)")]
        public decimal Price { get; set; } = 0;

        [Range(0, 10000, ErrorMessage = "Số lượng không hợp lệ")]
        [Display(Name = "Tổng số lượng")]
        public int Quantity { get; set; } = 1;

        [Range(0, 10000, ErrorMessage = "Số lượng khả dụng không hợp lệ")]
        [Display(Name = "Số lượng khả dụng")]
        public int AvailableQuantity { get; set; } = 1;

        [StringLength(255)]
        [Display(Name = "Ảnh bìa")]
        public string? CoverImage { get; set; }

        [Display(Name = "Mô tả sách")]
        public string? Description { get; set; }

        [Display(Name = "Đang lưu hành")]
        public bool Active { get; set; } = true;

        // Navigation property Code First
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [NotMapped]
        public string? CategoryName => Category?.Name;
    }
}
