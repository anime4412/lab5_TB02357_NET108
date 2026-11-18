using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QuanLyThuVien.Utils
{
    public static class FileUtil
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public static async Task<string?> UploadFileAsync(IFormFile? file, string targetSubFolder, string webRootPath)
        {
            if (file == null || file.Length == 0) return null;

            if (file.Length > MaxFileSize)
            {
                throw new Exception("Kích thước file vượt quá 5MB cho phép.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (Array.IndexOf(AllowedExtensions, extension) < 0)
            {
                throw new Exception("Định dạng file không được hỗ trợ (chỉ nhận JPG, PNG, WEBP, GIF).");
            }

            var uploadDirectory = Path.Combine(webRootPath, "uploads", targetSubFolder);
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(uploadDirectory, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{targetSubFolder}/{uniqueFileName}";
        }

        public static bool DeleteFile(string? relativePath, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return false;

            var fullPath = Path.Combine(webRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }
    }
}
