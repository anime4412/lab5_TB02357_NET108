using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.Data;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.DAL
{
    /// <summary>
    /// BÀI 1 & BÀI 3: CRUD Sách + Lọc theo thể loại + Tìm kiếm theo tên sách (EF Core Code First)
    /// </summary>
    public class BookDAL
    {
        private readonly LibraryDbContext _context;

        public BookDAL(LibraryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// BÀI 3: Lấy danh sách sách có hỗ trợ Lọc theo loại (categoryId) và Tìm kiếm theo tên (searchString)
        /// </summary>
        public List<Book> GetFiltered(int? categoryId = null, string? searchString = null)
        {
            var query = _context.Books
                .Include(b => b.Category)
                .AsNoTracking()
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim().ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(keyword) ||
                    b.ISBN.ToLower().Contains(keyword) ||
                    (b.Author != null && b.Author.ToLower().Contains(keyword)));
            }

            return query.OrderByDescending(b => b.Id).ToList();
        }

        public List<Book> GetAll() => GetFiltered();

        public Book? GetById(int id)
        {
            return _context.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.Id == id);
        }

        public int Insert(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return book.Id;
        }

        public bool Update(Book book)
        {
            var existing = _context.Books.Find(book.Id);
            if (existing == null) return false;

            existing.ISBN = book.ISBN;
            existing.Title = book.Title;
            existing.CategoryId = book.CategoryId;
            existing.Author = book.Author;
            existing.PublishYear = book.PublishYear;
            existing.Price = book.Price;
            existing.Quantity = book.Quantity;
            existing.AvailableQuantity = book.AvailableQuantity;
            existing.CoverImage = book.CoverImage;
            existing.Description = book.Description;
            existing.Active = book.Active;

            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var existing = _context.Books.Find(id);
            if (existing == null) return false;

            _context.Books.Remove(existing);
            return _context.SaveChanges() > 0;
        }
    }
}
