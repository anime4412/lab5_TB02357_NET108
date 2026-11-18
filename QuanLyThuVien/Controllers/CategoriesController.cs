using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien.DAL;
using QuanLyThuVien.Models;
using QuanLyThuVien.Utils;

namespace QuanLyThuVien.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryDAL _categoryDal;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(CategoryDAL categoryDal, IWebHostEnvironment webHostEnvironment)
        {
            _categoryDal = categoryDal;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Policy = "EmployeeOrManager")]
        public IActionResult Index()
        {
            return View(_categoryDal.GetAll());
        }

        [Authorize(Policy = "EmployeeOrManager")]
        public IActionResult Details(int id)
        {
            var category = _categoryDal.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [Authorize(Policy = "ManagerOnly")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> Create(Category category, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    category.Image = await FileUtil.UploadFileAsync(imageFile, "categories", _webHostEnvironment.WebRootPath);
                }
                _categoryDal.Insert(category);
                TempData["Success"] = "Thêm thể loại sách mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Authorize(Policy = "ManagerOnly")]
        public IActionResult Edit(int id)
        {
            var category = _categoryDal.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? imageFile)
        {
            if (id != category.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    category.Image = await FileUtil.UploadFileAsync(imageFile, "categories", _webHostEnvironment.WebRootPath);
                }
                _categoryDal.Update(category);
                TempData["Success"] = "Cập nhật thể loại sách thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Authorize(Policy = "ManagerOnly")]
        public IActionResult Delete(int id)
        {
            var category = _categoryDal.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ManagerOnly")]
        public IActionResult DeleteConfirmed(int id)
        {
            var existing = _categoryDal.GetById(id);
            if (existing != null && !string.IsNullOrEmpty(existing.Image))
            {
                FileUtil.DeleteFile(existing.Image, _webHostEnvironment.WebRootPath);
            }
            _categoryDal.Delete(id);
            TempData["Success"] = "Xóa thể loại sách thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
