using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.Data;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.DAL
{
    /// <summary>
    /// BÀI 1 & BÀI 2: CRUD Nhân viên + Xác thực + Cấp lại mật khẩu ngẫu nhiên (EF Core Code First)
    /// </summary>
    public class EmployeeDAL
    {
        private readonly LibraryDbContext _context;

        public EmployeeDAL(LibraryDbContext context)
        {
            _context = context;
        }

        public Employee? Authenticate(string email, string password)
        {
            return _context.Employees
                .AsNoTracking()
                .FirstOrDefault(e => e.Email == email && e.Password == password && e.Active);
        }

        public List<Employee> GetAll()
        {
            return _context.Employees.OrderBy(e => e.Id).AsNoTracking().ToList();
        }

        public Employee? GetById(int id)
        {
            return _context.Employees.Find(id);
        }

        public Employee? GetByEmail(string email)
        {
            return _context.Employees.FirstOrDefault(e => e.Email == email);
        }

        public int Insert(Employee emp)
        {
            _context.Employees.Add(emp);
            _context.SaveChanges();
            return emp.Id;
        }

        public bool Update(Employee emp)
        {
            var existing = _context.Employees.Find(emp.Id);
            if (existing == null) return false;

            existing.EmployeeCode = emp.EmployeeCode;
            existing.FullName = emp.FullName;
            existing.Email = emp.Email;
            if (!string.IsNullOrWhiteSpace(emp.Password))
            {
                existing.Password = emp.Password;
            }
            existing.Phone = emp.Phone;
            existing.Role = emp.Role;
            existing.Active = emp.Active;

            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var existing = _context.Employees.Find(id);
            if (existing == null) return false;

            _context.Employees.Remove(existing);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// BÀI 2: Đặt lại mật khẩu ngẫu nhiên an toàn 8 ký tự cho nhân viên trong CSDL
        /// </summary>
        public (Employee? Employee, string? NewPassword) ResetRandomPassword(string email)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.Email == email && e.Active);
            if (emp == null) return (null, null);

            string newPassword = GenerateSecureRandomPassword(8);
            emp.Password = newPassword;
            _context.SaveChanges();

            return (emp, newPassword);
        }

        private static string GenerateSecureRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789@#$!";
            Span<byte> randomBytes = stackalloc byte[length];
            RandomNumberGenerator.Fill(randomBytes);

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[randomBytes[i] % validChars.Length];
            }
            return new string(chars);
        }
    }
}
