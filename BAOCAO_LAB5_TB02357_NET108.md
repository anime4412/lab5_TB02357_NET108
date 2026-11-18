# BÁO CÁO THỰC HÀNH LAB 5: CRUD SÁCH/NHÂN VIÊN, CẤP LẠI MẬT KHẨU, LỌC/TÌM KIẾM & OTP (EF CORE CODE FIRST)
**MÔN HỌC: DỰ ÁN .NET 1 (NET108 / SOF1022)**  
**LỚP: DỰ ÁN 1 — NHÓM THỰC HIỆN: NHÓM 5**  
**MÃ SINH VIÊN: TB02357**  
**ĐỀ TÀI: HỆ THỐNG QUẢN LÝ THƯ VIỆN ĐIỆN TỬ (LIBRARY MANAGEMENT SYSTEM)**  

---

## MỤC TIÊU BÀI THỰC HÀNH LAB 5
1. **Xây dựng CRUD Sách & Nhân viên (Bài 1 - 4 điểm)**: Hoàn thiện `BooksController` (kèm upload ảnh bìa qua `FileUtil`) và `EmployeesController` trên nền **Entity Framework Core 8.0 Code First**.
2. **Chức năng Quên mật khẩu / Cấp lại mật khẩu ngẫu nhiên (Bài 2 - 2 điểm)**: Sinh mật khẩu ngẫu nhiên an toàn bằng `RandomNumberGenerator`, lưu vào SQL Server qua `EmployeeDAL` và gửi về Email nhân viên qua `EmailService`.
3. **Nâng cấp Lọc & Tìm kiếm Sách (Bài 3 - 2 điểm)**: Lọc danh sách sách theo Thể loại (`categoryId`) và Tìm kiếm theo Tên sách / ISBN / Tác giả (`searchString`) bằng LINQ EF Core.
4. **Ứng dụng AI tìm hiểu về OTP & hướng tích hợp vào Quên mật khẩu (Bài 4 - 2 điểm)**.

---

## PHẦN I: ÁP DỤNG BÀI HỌC

### BÀI 1 (4 ĐIỂM): XÂY DỰNG CRUD CHO SÁCH VÀ NHÂN VIÊN (EF CORE CODE FIRST)

#### 1. Kiến trúc EF Core Code First (`Data/LibraryDbContext.cs` & `Migrations/`)
* Các Entity `Employee`, `Category`, `Book` được cấu hình Data Annotations (`[Key]`, `[Required]`, `[StringLength]`, `[Column(TypeName = "decimal(18,2)")]`) và Unique Index (`EmployeeCode`, `Email`, `ISBN`).
* Sinh CSDL bằng lệnh `dotnet ef migrations add InitialCreate` và `dotnet ef database update`.

#### 2. `BooksController.cs` & `BookDAL.cs`
* **Xem (`Index`, `Details`)**: Cho phép `EmployeeOrManager` (Thủ thư & Quản lý).
* **Thêm, Sửa, Xóa (`Create`, `Edit`, `Delete`)**: Gắn `[Authorize(Policy = "ManagerOnly")]` — Chỉ Admin thao tác, hỗ trợ upload ảnh bìa vào `wwwroot/uploads/books/` qua `FileUtil`.

#### 3. `EmployeesController.cs` & `EmployeeDAL.cs`
* Gắn `[Authorize(Policy = "ManagerOnly")]` bảo vệ toàn bộ Controller.
* Cung cấp đầy đủ `Index`, `Details`, `Create`, `Edit` (giữ nguyên mật khẩu cũ nếu không nhập mới), `Delete`.

---

### BÀI 2 (2 ĐIỂM): XÂY DỰNG CHỨC NĂNG CẤP LẠI MẬT KHẨU CHO NHÂN VIÊN

Khi người dùng bấm **"Quên mật khẩu?"** trên màn hình Đăng nhập (`/Account/ForgotPassword`):
1. `AccountController.ForgotPassword(model)` kiểm tra Email nhân viên đang hoạt động (`Active = true`).
2. `EmployeeDAL.ResetRandomPassword(email)` dùng `System.Security.Cryptography.RandomNumberGenerator` sinh mật khẩu ngẫu nhiên 8 ký tự (gồm chữ hoa, chữ thường, số, ký tự đặc biệt) và lưu vào CSDL qua `_context.SaveChanges()`.
3. `EmailService.SendResetPasswordEmailAsync(...)` gửi email SMTP chứa mật khẩu mới tới hòm thư nhân viên (có chế độ mô phỏng Offline nếu chưa cấu hình App Password để thuận tiện kiểm thử).

```csharp
public (Employee? Employee, string? NewPassword) ResetRandomPassword(string email)
{
    var emp = _context.Employees.FirstOrDefault(e => e.Email == email && e.Active);
    if (emp == null) return (null, null);

    string newPassword = GenerateSecureRandomPassword(8);
    emp.Password = newPassword;
    _context.SaveChanges();

    return (emp, newPassword);
}
```

---

## PHẦN II: NÂNG CẤP TÍNH NĂNG

### BÀI 3 (2 ĐIỂM): LỌC SÁCH THEO THỂ LOẠI & TÌM KIẾM THEO TÊN

Tại `BookDAL.GetFiltered(int? categoryId, string? searchString)` và `BooksController.Index(...)`:
```csharp
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
```
* Trên `Views/Books/Index.cshtml`, thanh lọc gồm dropdown `categoryId` (`-- Tất cả thể loại --`) và ô input `searchString`, giữ nguyên giá trị đang lọc/tìm kiếm sau khi submit form `GET`.

---

### BÀI 4 (2 ĐIỂM): SỬ DỤNG AI TÌM HIỂU VỀ MẬT KHẨU DÙNG MỘT LẦN (OTP)

#### Prompt sử dụng với AI:
> *"OTP (One-Time Password) là gì? Tại sao dùng OTP bảo mật hơn gửi mật khẩu mới trực tiếp qua Email? Làm thế nào để tích hợp xác thực OTP vào tính năng Quên mật khẩu hiện tại trong ASP.NET Core MVC (EF Core Code First)?"*

#### 1. OTP (One-Time Password) là gì?
* **Định nghĩa**: OTP (Mật khẩu dùng một lần) là một chuỗi mã xác thực ngẫu nhiên (thường gồm 6 chữ số) chỉ có giá trị sử dụng **đúng 1 lần duy nhất** trong khoảng thời gian ngắn (thường từ **2 đến 5 phút**).
* **Ưu điểm bảo mật so với gửi mật khẩu thẳng qua Email**:
  - Ngăn chặn rủi ro kẻ xấu chiếm tài khoản ngay lập tức nếu nhân viên chưa kịp bấm vào link/đổi mật khẩu: nếu gửi mật khẩu tĩnh qua email và email bị rò rỉ, kẻ xấu có thể đăng nhập trực tiếp; còn OTP chỉ là mã xác minh tạm thời, hết hạn rất nhanh và bắt buộc người dùng tự nhập mật khẩu mới của chính mình.
  - Phù hợp chuẩn xác thực 2 yếu tố (2FA) và bảo mật ứng dụng web hiện đại.

#### 2. Quy trình & Cách tích hợp OTP vào tính năng Quên mật khẩu hiện tại
Để nâng cấp tính năng `/Account/ForgotPassword` hiện tại sang quy trình **OTP**, ta thực hiện theo 4 bước sau:

* **Bước 1 — Bổ sung trường lưu OTP trong Entity `Employee` (hoặc bảng `PasswordResetOtp` bằng EF Core Code First)**:
  ```csharp
  public string? ResetOtpCode { get; set; }
  public DateTime? ResetOtpExpiry { get; set; }
  ```
  Sau đó chạy Migration: `dotnet ef migrations add AddOtpFields` và `dotnet ef database update`.

* **Bước 2 — Bước yêu cầu quên mật khẩu (`POST /Account/ForgotPassword`)**:
  - Kiểm tra Email tồn tại.
  - Sinh mã OTP 6 chữ số ngẫu nhiên bằng `RandomNumberGenerator.GetInt32(100000, 999999).ToString()`.
  - Lưu `emp.ResetOtpCode = otp;` và `emp.ResetOtpExpiry = DateTime.UtcNow.AddMinutes(5);` xuống CSDL.
  - Gửi Email chứa **mã OTP 6 số** (kèm thông báo hết hạn sau 5 phút) thay vì gửi mật khẩu mới. Chuyển hướng sang trang `VerifyOtp`.

* **Bước 3 — Bước xác thực OTP & Đặt lại mật khẩu mới (`POST /Account/VerifyOtp`)**:
  - Người dùng nhập: `Email`, `OtpCode`, `NewPassword`, `ConfirmPassword`.
  - Hệ thống kiểm tra 3 điều kiện:
    1. `emp.ResetOtpCode == model.OtpCode` (khớp mã OTP).
    2. `emp.ResetOtpExpiry != null && emp.ResetOtpExpiry > DateTime.UtcNow` (chưa hết hạn 5 phút).
  - Nếu hợp lệ: Cập nhật mật khẩu mới (`emp.Password = ...`), đồng thời **xóa bỏ mã OTP** (`emp.ResetOtpCode = null; emp.ResetOtpExpiry = null;`) để đảm bảo OTP chỉ dùng **1 lần duy nhất**, gọi `_context.SaveChanges()` và chuyển về trang `Login`.

---

## TỔNG KẾT
* Hoàn thành 100% 4 bài của **Lab 5** theo đúng đề tài **Hệ thống Quản lý Thư viện Điện tử** và chuẩn **EF Core 8.0 Code First**.
* Dự án biên dịch thành công **0 Warning(s), 0 Error(s)**.
