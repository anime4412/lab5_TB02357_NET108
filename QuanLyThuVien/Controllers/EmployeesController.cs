using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyThuVien.DAL;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    /// <summary>
    /// BÀI 1 (4 ĐIỂM): Xây dựng CRUD cho Nhân viên (Employees)
    /// Chỉ Quản lý (Admin) có quyền quản lý hồ sơ nhân sự (ManagerOnly)
    /// </summary>
    [Authorize(Policy = "ManagerOnly")]
    public class EmployeesController : Controller
    {
        private readonly EmployeeDAL _employeeDal;

        public EmployeesController(EmployeeDAL employeeDal)
        {
            _employeeDal = employeeDal;
        }

        public IActionResult Index()
        {
            return View(_employeeDal.GetAll());
        }

        public IActionResult Details(int id)
        {
            var emp = _employeeDal.GetById(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        public IActionResult Create()
        {
            return View(new Employee());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _employeeDal.Insert(employee);
                    TempData["Success"] = $"Thêm nhân viên \"{employee.FullName}\" thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi thêm nhân viên (kiểm tra trùng Mã NV hoặc Email): {ex.Message}");
                }
            }
            return View(employee);
        }

        public IActionResult Edit(int id)
        {
            var emp = _employeeDal.GetById(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee employee)
        {
            if (id != employee.Id) return BadRequest();

            // Cho phép giữ nguyên mật khẩu cũ nếu để trống khi sửa
            if (string.IsNullOrWhiteSpace(employee.Password))
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _employeeDal.Update(employee);
                    TempData["Success"] = $"Cập nhật thông tin nhân viên \"{employee.FullName}\" thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật nhân viên: {ex.Message}");
                }
            }
            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var emp = _employeeDal.GetById(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _employeeDal.Delete(id);
                TempData["Success"] = "Đã xóa nhân viên khỏi hệ thống!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Không thể xóa nhân viên: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
