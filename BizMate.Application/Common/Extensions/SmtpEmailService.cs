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
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var expiredAtVN = TimeZoneInfo.ConvertTimeFromUtc(expiredAt, vietnamTimeZone);

            var subject = "Mã xác thực OTP";

            var body = $$"""
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <style>
                        body {
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            background-color: #f4f6f8;
                            margin: 0;
                            padding: 0;
                        }
                        .container {
                            max-width: 600px;
                            margin: 30px auto;
                            background-color: #ffffff;
                            border-radius: 8px;
                            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                            overflow: hidden;
                        }
                        .header {
                            background-color: #007BFF;
                            color: #ffffff;
                            padding: 20px 30px;
                            text-align: center;
                        }
                        .header h1 {
                            margin: 0;
                            font-size: 24px;
                        }
                        .content {
                            padding: 30px;
                            color: #333333;
                        }
                        .otp-code {
                            font-size: 32px;
                            font-weight: bold;
                            color: #007BFF;
                            text-align: center;
                            margin: 20px 0;
                        }
                        .footer {
                            background-color: #f1f1f1;
                            padding: 20px 30px;
                            text-align: center;
                            font-size: 12px;
                            color: #888888;
                        }
                    </style>
                </head>
                <body>
                    <div class="container">
                        <div class="header">
                            <h1>Mã Xác Thực OTP</h1>
                        </div>
                        <div class="content">
                            <p>Xin chào,</p>
                            <p>Chúng tôi đã nhận được yêu cầu xác thực từ bạn. Dưới đây là mã OTP để tiếp tục:</p>
                            <div class="otp-code">{{otpCode}}</div>
                            <p>Mã này sẽ hết hiệu lực vào: <strong>{{expiredAtVN:HH:mm:ss dd/MM/yyyy}}</strong></p>
                            <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                            <p style="margin-top: 30px;">Trân trọng,<br/><strong>Đội ngũ BizMate</strong></p>
                        </div>
                        <div class="footer">
                            Đây là email tự động, vui lòng không phản hồi.
                        </div>
                    </div>
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
