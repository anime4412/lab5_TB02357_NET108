using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThuVien.DAL;
using QuanLyThuVien.Models;
using QuanLyThuVien.Utils;

namespace QuanLyThuVien.Controllers
{
    /// <summary>
    /// BÀI 1 (CRUD Sách) & BÀI 3 (Lọc theo Thể loại + Tìm kiếm theo Tên sách)
    /// </summary>
    public class BooksController : Controller
    {
        private readonly BookDAL _bookDal;
        private readonly CategoryDAL _categoryDal;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(BookDAL bookDal, CategoryDAL categoryDal, IWebHostEnvironment webHostEnvironment)
        {
            _bookDal = bookDal;
            _categoryDal = categoryDal;
            _webHostEnvironment = webHostEnvironment;
        }

        // BÀI 3: Hỗ trợ Lọc theo loại sách (categoryId) & Tìm kiếm theo tên/ISBN/tác giả (searchString)
        [Authorize(Policy = "EmployeeOrManager")]
        public IActionResult Index(int? categoryId, string? searchString)
        {
            var categories = _categoryDal.GetAll();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentSearch = searchString;

            var books = _bookDal.GetFiltered(categoryId, searchString);
            return View(books);
        }

        [Authorize(Policy = "EmployeeOrManager")]
        public IActionResult Details(int id)
        {
            var book = _bookDal.GetById(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [Authorize(Policy = "ManagerOnly")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name");
            return View(new Book());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> Create(Book book, IFormFile? coverFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (coverFile != null)
                    {
                        book.CoverImage = await FileUtil.UploadFileAsync(coverFile, "books", _webHostEnvironment.WebRootPath);
                    }
                    _bookDal.Insert(book);
                    TempData["Success"] = $"Đã thêm cuốn sách \"{book.Title}\" thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi thêm sách (kiểm tra trùng mã ISBN): {ex.Message}");
                }
            }
            ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name", book.CategoryId);
            return View(book);
        }

        [Authorize(Policy = "ManagerOnly")]
        public IActionResult Edit(int id)
        {
            var book = _bookDal.GetById(id);
            if (book == null) return NotFound();

            ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name", book.CategoryId);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> Edit(int id, Book book, IFormFile? coverFile)
        {
            if (id != book.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    if (coverFile != null)
                    {
                        book.CoverImage = await FileUtil.UploadFileAsync(coverFile, "books", _webHostEnvironment.WebRootPath);
                    }
                    _bookDal.Update(book);
                    TempData["Success"] = $"Đã cập nhật cuốn sách \"{book.Title}\" thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật sách: {ex.Message}");
                }
            }
            ViewBag.Categories = new SelectList(_categoryDal.GetAll(), "Id", "Name", book.CategoryId);
            return View(book);
        }

        [Authorize(Policy = "ManagerOnly")]
        public IActionResult Delete(int id)
        {
            var book = _bookDal.GetById(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var existing = _bookDal.GetById(id);
                if (existing != null && !string.IsNullOrEmpty(existing.CoverImage))
                {
                    FileUtil.DeleteFile(existing.CoverImage, _webHostEnvironment.WebRootPath);
                }
                _bookDal.Delete(id);
                TempData["Success"] = "Xóa sách khỏi thư viện thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể xóa sách: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
