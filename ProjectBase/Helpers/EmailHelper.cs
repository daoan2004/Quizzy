using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProjectBase.Services
{
    public class EmailHelper
    {
        // Phương thức để gửi email không đồng bộ
        public static async Task SendVerifyLink(string to, string token)
        {
            // Tạo URL liên kết xác minh với mã token
            var verifyLink = $"https://localhost:7137/Account/VerifyAccount?token={token}";

            // Địa chỉ email người gửi và tên hiển thị tùy chọn
            var fromAddress = new MailAddress("group5swp391@gmail.com", "Group5");

            // Mật khẩu cho tài khoản email người gửi (thường là mật khẩu ứng dụng cụ thể)
            const string fromPassword = "nzme usqj uwwl bowo";

            // Địa chỉ email người nhận
            var toAddress = new MailAddress(to);

            // Cấu hình client SMTP
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",                // Tên máy chủ SMTP
                Port = 587,                               // Số cổng cho giao tiếp SMTP
                EnableSsl = true,                         // Kích hoạt mã hóa SSL/TLS
                DeliveryMethod = SmtpDeliveryMethod.Network, // Chỉ định phương thức gửi (qua mạng)
                UseDefaultCredentials = false,            // Không sử dụng thông tin xác thực mặc định
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword) // Cung cấp thông tin xác thực để xác thực
            };

            // Tạo thông điệp email
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "[Group5]Please Verify Your Email Address to Complete Your Account Registration", // Đặt tiêu đề của email
                Body = $"Click <a href='{verifyLink}'>here</a> to verify your Account.", // Đặt nội dung email với liên kết xác minh
                IsBodyHtml = true               // Chỉ định rằng nội dung là HTML
            })
            {
                // Gửi email không đồng bộ
                await smtp.SendMailAsync(message);
            }
        }
        public static async Task SendResetPasswordLink(string to, string token)
        {
            var resetLink = $"https://localhost:7137/Account/ResetPasswordConfirm?token={token}";
            var fromAddress = new MailAddress("group5swp391@gmail.com", "Group5");
            const string fromPassword = "nzme usqj uwwl bowo";
            var toAddress = new MailAddress(to);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "[Group5] Reset Your Password",
                Body = $"Click <a href='{resetLink}'>here</a> to reset your password.",
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
    

}
