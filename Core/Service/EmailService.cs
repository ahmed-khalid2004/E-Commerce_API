using Microsoft.Extensions.Configuration;
using ServiceAbstracion;
using System.Net;
using System.Net.Mail;

namespace Service
{
    public class EmailService(IConfiguration _configuration) : IEmailService
    {
        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var settings = _configuration.GetSection("EmailSettings");

            var from = settings["From"]!;
            var host = settings["SmtpHost"]!;
            var port = int.Parse(settings["SmtpPort"]!);
            var username = settings["Username"]!;
            var password = settings["Password"]!;

            var message = new MailMessage
            {
                From = new MailAddress(from, "E-Commerce Store"),
                Subject = "Your Password Reset Code",
                IsBodyHtml = true,
                Body = $"""
                    <div style="font-family:Arial,sans-serif;max-width:400px;margin:auto;padding:24px;border:1px solid #eee;border-radius:8px;">
                        <h2 style="color:#333;">Password Reset</h2>
                        <p>Your verification code is:</p>
                        <h1 style="letter-spacing:8px;color:#4f46e5;font-size:40px;">{otp}</h1>
                        <p style="color:#888;font-size:13px;">This code expires in 10 minutes. Do not share it with anyone.</p>
                    </div>
                """
            };
            message.To.Add(toEmail);

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}