using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using ProjectBase.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;

namespace ProjectBase.Controllers
{
    [Route("/Account")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        public AccountController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return PartialView("_RegisterPartial", new RegisterModel());
        }


        //Hàm đăng kí         
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data"); // Trả về lỗi 400 nếu model là null
            }

            if (string.IsNullOrEmpty(model.fullname))
            {
                ModelState.AddModelError("Fullname", "Username cannot be blank");
            }
            if (string.IsNullOrEmpty(model.password))
            {
                ModelState.AddModelError("Password", "Password cannot be blank");
            }
            else if (!IsValidPassword(model.password))
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
            }
            if (string.IsNullOrEmpty(model.email))
            {
                ModelState.AddModelError("Email", "Email cannot be blank");
            }
            // Kiểm tra điều kiện trường email
            if (!string.IsNullOrEmpty(model.email) && !IsValidEmail(model.email))
            {
                ModelState.AddModelError("Email", "Invalid email format");
            }
            if (string.IsNullOrEmpty(model.Phone) || !IsValidPhoneNumber(model.Phone))
            {
                ModelState.AddModelError("Phone", "Invalid phone number format");
            }

            if (model.password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password do not match");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.email == model.email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email address is already in use.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo mã xác minh
                    string token = Guid.NewGuid().ToString();

                    // Thêm đối tượng User vào cơ sở dữ liệu
                    var newUser = new RegisterModel
                    {
                        fullname = model.fullname,
                        Phone = model.Phone,
                        password = EncryptionHelper.GetMd5Hash(model.password),
                        email = model.email,
                        gender = model.gender ? true : false,
                        verificationToken = token,
                        status = 0,
                        RoleID = 2,
                    };

                    _context.Users.Add(newUser.toUser());
                    await _context.SaveChangesAsync();
                    // Gửi email xác minhz`
                    await EmailHelper.SendVerifyLink(newUser.email, token);
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    return StatusCode(500, "Internal server error: " + ex.StackTrace );
                }
            }

            return Ok(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
        [HttpGet]
        [Route("VerifyAccount")]
        public async Task<IActionResult> VerifyAccount(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Error");
            }

            // Tìm người dùng với mã xác minh
            var user = await _context.Users.FirstOrDefaultAsync(u => u.verificationToken == token);
            if (user != null)
            {
                user.status = 1; // Đã kích hoạt
                user.verificationToken = null; // Xóa mã xác minh sau khi xác minh thành công
                await _context.SaveChangesAsync();

                return RedirectToAction("VerificationSuccess");
            }

            return RedirectToAction("Error");
        }
        //kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        //kiểm tra dịnh dạng số điện thoại
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            var phoneRegex = @"^(\+?\d{1,3}[- ]?)?\d{10}$";
            return Regex.IsMatch(phoneNumber, phoneRegex);
        }
        // Hàm kiểm tra định dạng mật khẩu
        private bool IsValidPassword(string password)
        {
            // Yêu cầu mật khẩu phải có ít nhất một chữ cái hoa, một chữ cái thường, một chữ số, và một ký tự đặc biệt
            var hasUpperCaseLetter = new Regex(@"[A-Z]+");
            var hasLowerCaseLetter = new Regex(@"[a-z]+");
            var hasDigit = new Regex(@"[0-9]+");
            var hasSpecialCharacter = new Regex(@"[\W]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasUpperCaseLetter.IsMatch(password) &&
                   hasLowerCaseLetter.IsMatch(password) &&
                   hasDigit.IsMatch(password) &&
                   hasSpecialCharacter.IsMatch(password) &&
                   hasMinimum8Chars.IsMatch(password);
        }
        [HttpGet]
        [Route("VerificationSuccess")]
        public IActionResult VerificationSuccess()
        {
            return View();
        }
        [HttpGet]
        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }
        //hàm đăng nhập
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string hashedPassword = EncryptionHelper.GetMd5Hash(model.password);
            // Tìm kiếm một người dùng trong cơ sở dữ liệu với email và mật khẩu mã hóa
            var user = await _context.Users
                .Include(u => u.Role) // Bao gồm thông tin vai trò của người dùng
                .FirstOrDefaultAsync(u => u.email == model.email && u.password == hashedPassword);

            // Kiểm tra xem người dùng với thông tin đăng nhập đã cung cấp có tồn tại không
            if (user != null)
            {
                if (user.status == 0)
                {
                    return Json(new { success = false, message = "Your account is not activated yet. You need check your gmail and verify account." });
                }
                // Nếu người dùng tồn tại và tài khoản đã được kích hoạt, tạo claims cho người dùng
                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.fullname),
                            new Claim(ClaimTypes.Name, user.email),
                            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                            new Claim(ClaimTypes.Role, user.Role.RoleName) // Thêm claim vai trò
                        };
                // Tạo một identity chứa các claims
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                // Đăng nhập người dùng với identity đã tạo
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                // Thiết lập một cookie với email của người dùng
                Response.Cookies.Append("UserEmail", model.email);
                return Json(new { success = true });

            }
            return Json(new { success = false, message = "Invalid email or password." });

        }

        //hàm đăng xuất
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        //hàm thay đổi mật khẩu
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out long userId))
            {
                var user = _context.Users.Find(userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                string currentHashedPassword = EncryptionHelper.GetMd5Hash(model.CurrentPassword);
                if (user.password != currentHashedPassword)
                {
                    return Json(new { success = false, message = "Current password is incorrect." });
                }
                // Kiểm tra xem new password có trùng current password không
                string newHashedPassword = EncryptionHelper.GetMd5Hash(model.NewPassword);
                if (user.password == newHashedPassword)
                {
                    return Json(new { success = false, message = "New password must be different from current password." });
                }
                if (!IsValidPassword(model.NewPassword))
                {
                    return Json(new { success = false, message = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character." });
                }

                // Kiểm tra xem new password và renew password có trùng nhau không
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    return Json(new { success = false, message = "New password and renew password do not match." });
                }

                // Nếu trùng khớp, tiến hành cập nhật mật khẩu mới
                user.password = EncryptionHelper.GetMd5Hash(model.NewPassword);
                _context.Update(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Password changed successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Invalid user identifier." });
            }
        }
        //hàm lấy lại mật khẩu
        [HttpGet]
        [Route("ResetPasswordRequest")]
        public IActionResult ResetPasswordRequest()
        {
            return View("~/Views/User/ResetPasswordRequest.cshtml");
        }
        [HttpPost]
        [Route("ResetPasswordRequest")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.email == model.email);
                if (user == null)
                {
                    return Json(new { success = false, message = "No user found with this email address." });
                }

                var token = Guid.NewGuid().ToString();
                user.PasswordResetToken = token;
                // Lấy giá trị cấu hình thời gian hết hạn từ appsettings.json
                int expirationHours = _config.GetValue<int>("PasswordResetLinkExpirationHours");
                // Tính thời gian hết hạn của token
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                user.PasswordResetTokenExpires = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddHours(expirationHours); 

                await _context.SaveChangesAsync();

                await EmailHelper.SendResetPasswordLink(model.email, token);

                return Json(new { success = true, message = "Password reset email sent.", token = token });

            }
            return Json(new { success = false, message = "Invalid request." });
        }

        [HttpPost]
        [Route("ResetPasswordConfirm")]
        public async Task<IActionResult> ResetPasswordConfirm([FromBody] ResetPasswordConfirmModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Invalid data.", errors });
            }

            string cleanedToken = model.Token.Replace("\"", ""); // Loại bỏ dấu ngoặc kép nếu có

            if (string.IsNullOrEmpty(cleanedToken))
            {
                return Json(new { success = false, message = "Token is required." });
            }


            if (!IsValidPassword(model.NewPassword))
            {
                return Json(new { success = false, message = "Password does not meet complexity requirements." });
            }

            if (model.NewPassword != model.ReNewPassword)
            {
                return Json(new { success = false, message = "Passwords do not match." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == cleanedToken);
            if (user == null)
            {
                return Json(new { success = false, message = "Invalid token." });
            }

            // Lấy múi giờ Việt Nam
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamCurrentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            if (user.PasswordResetTokenExpires < vietnamCurrentTime)
            {
                return Json(new { success = false, message = "Token expired." });
            }

            user.password = EncryptionHelper.GetMd5Hash(model.NewPassword); // Ensure you have this method implemented
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Password has been reset successfully." });
        }
        [HttpGet]
        [Route("ResetPasswordConfirm")]
        public IActionResult ResetPasswordConfirm(string token)
        {
            ViewBag.Token = token; // Đặt token vào ViewBag
            return View("~/Views/User/ResetPasswordConfirm.cshtml");
        }
        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out long userId))
            {
                var user = await _context.Users
                    .Include(u => u.Role) // Bao gồm thông tin vai trò của người dùng
                    .FirstOrDefaultAsync(u => u.ID == userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }
                var avatarUrl = user.profile_picture;
                var userDetails = new
                {
                    UserID = user.ID,
                    FullName = user.fullname,
                    Email = user.email,
                    Phone = user.Phone,
                    Gender = user.gender,
                    Role = user.Role.RoleName,
                    Status = user.status,
                    Address = user.address,       // Thêm trường địa chỉ
                    DOB = user.Dob,       // Thêm trường ngày sinh
                    Description = user.description, // Thêm trường mô tả
                    AvatarUrl = avatarUrl
                };

                return Json(new { success = true, userDetails });
            }
            else
            {
                return Json(new { success = false, message = "Invalid user identifier." });
            }
        }
        [HttpPost]
        [Route("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserProfileModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out long userId))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                user.fullname = model.FullName;
                user.Phone = model.Phone;
                if (!IsValidPhoneNumber(model.Phone))
                {
                    return Json(new { success = false, message = "Invalid phone number format." });
                }
                user.gender = model.Gender;
                user.address = model.Address;
                user.Dob = model.DOB;
                user.description = model.Description;

                if (model.Avatar != null)
                {
                    try
                    {
                        // Generate a unique file name based on userId and current timestamp
                        var fileName = $"{userId}_{DateTime.Now.Ticks}{Path.GetExtension(model.Avatar.FileName)}";
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture", "avatar");
                        var filePath = Path.Combine(directoryPath, fileName);

                        // Ensure the directory exists
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Save the file to the specified path
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(stream);
                        }

                        // Update the avatar path in the database
                        user.profile_picture = $"/picture/avatar/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        // Log the error (consider using a logging framework like Serilog, NLog, etc.)
                        Console.Error.WriteLine($"Failed to save avatar: {ex.Message}");
                        return Json(new { success = false, message = "Failed to save avatar." });
                    }
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Profile updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Invalid user identifier." });
            }
        }
        [HttpGet]
        [Route("GetUserAvatar/{userId}")]
        public async Task<IActionResult> GetUserAvatar(long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null || string.IsNullOrEmpty(user.profile_picture))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.profile_picture.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = GetContentType(filePath);

            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        private string GetContentType(string filePath)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(filePath, out contentType))
            {
                contentType = "application/octet-stream"; // Default content type
            }
            return contentType;
        }
    }
}
  