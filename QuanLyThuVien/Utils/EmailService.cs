using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace QuanLyThuVien.Utils
{
    /// <summary>
    /// BÀI 2 (2 ĐIỂM): Dịch vụ gửi Email cấp lại mật khẩu ngẫu nhiên cho nhân viên
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool SentViaSmtp, string Message)> SendResetPasswordEmailAsync(string toEmail, string fullName, string newPassword)
        {
            var host = _configuration["SmtpSettings:Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(_configuration["SmtpSettings:Port"], out var p) ? p : 587;
            var senderEmail = _configuration["SmtpSettings:SenderEmail"] ?? "";
            var senderName = _configuration["SmtpSettings:SenderName"] ?? "Hệ Thống Thư Viện Điện Tử";
            var smtpPassword = _configuration["SmtpSettings:Password"] ?? "";

            string subject = "[Thư Viện Điện Tử] Cấp lại mật khẩu đăng nhập mới";
            string body = $@"
                <h3>Xin chào {fullName},</h3>
                <p>Hệ thống đã tiếp nhận yêu cầu cấp lại mật khẩu cho tài khoản <strong>{toEmail}</strong>.</p>
                <p>Mật khẩu mới ngẫu nhiên của bạn là: <strong style='color:#0d6efd;font-size:18px;'>{newPassword}</strong></p>
                <p>Vui lòng sử dụng mật khẩu này để đăng nhập lại và đổi mật khẩu cá nhân sau khi vào hệ thống.</p>
                <hr/>
                <small>Hệ thống Quản lý Thư viện Điện tử - Môn NET108</small>";

            // Nếu chưa cấu hình SMTP Password (chạy kiểm thử local/offline), trả về trạng thái mô phỏng để test mượt mà
            if (string.IsNullOrWhiteSpace(smtpPassword) || string.IsNullOrWhiteSpace(senderEmail))
            {
                return (false, $"[Chế độ mô phỏng Offline] Đã tạo và cập nhật mật khẩu mới cho {toEmail} trong CSDL (cấu hình SmtpSettings:Password trong appsettings.json để gửi Gmail thật).");
            }

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(senderEmail, senderName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(senderEmail, smtpPassword),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
                return (true, $"Đã gửi thành công email chứa mật khẩu mới tới {toEmail}!");
            }
            catch (Exception ex)
            {
                return (false, $"Mật khẩu mới đã cập nhật trong CSDL, nhưng gửi SMTP thất bại: {ex.Message}");
            }
        }
    }
}
