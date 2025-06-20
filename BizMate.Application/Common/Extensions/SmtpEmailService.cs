using BizMate.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace BizMate.Application.Common.Extensions
{
    public class SmtpEmailService : IEmailService
    {
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly IConfiguration _config;

        public SmtpEmailService(ILogger<SmtpEmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode, DateTime expiredAt)
        {
            var subject = "Mã xác thực OTP";

            var body = $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
            </head>
            <body style="font-family: Arial, sans-serif; background-color: #f6f9fc; padding: 20px;">
                <table width="100%" cellpadding="0" cellspacing="0" style="max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);">
                    <tr>
                        <td style="padding: 20px 30px;">
                            <h2 style="color: #007BFF;">Mã xác thực OTP</h2>
                            <p>Xin chào,</p>
                            <p>Đây là mã OTP <strong style="color: #007BFF;">BizMate</strong> của bạn:</p>
                            <p style="font-size: 24px; font-weight: bold; color: #007BFF;">{otpCode}</p>
                            <p>Mã có hiệu lực đến: <strong>{expiredAt:HH:mm:ss dd/MM/yyyy}</strong></p>
                            <p style="margin-top: 30px;">Trân trọng,<br/>Đội ngũ BizMate</p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

            using var smtpClient = new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(
                    _config["Smtp:Username"],
                    _config["Smtp:Password"]
                ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Smtp:FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Đã gửi email OTP tới: {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email OTP.");
                throw new InvalidOperationException("Không thể gửi email xác thực OTP. Vui lòng thử lại sau.");
            }
        }
    }
}
