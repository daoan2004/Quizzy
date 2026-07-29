using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using ProjectBase.Models.DAO;
using ProjectBase.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.RateLimiting;

namespace ProjectBase.Controllers
{
    [Route("/Account")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordService _passwordService;

        public AccountController(
            IConfiguration config,
            DataContext context,
            IEmailSender emailSender,
            IPasswordService passwordService)
        {
            _config = config;
            _context = context;
            _emailSender = emailSender;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return PartialView("_RegisterPartial", new RegisterModel());
        }


        //Hàm đăng kí         
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("EmailVerification")]
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
                    var verificationExpirationHours =
                        _config.GetValue<int>("VerificationLinkExpirationHours", 24);

                    // Thêm đối tượng User vào cơ sở dữ liệu
                    var newUser = new User
                    {
                        fullname = model.fullname,
                        Phone = model.Phone,
                        password = string.Empty,
                        email = model.email,
                        gender = model.gender,
                        verificationToken = token,
                        VerificationTokenExpires = DateTime.UtcNow.AddHours(
                            verificationExpirationHours),
                        status = 0,
                        RoleID = 2,
                    };
                    newUser.password = _passwordService.HashPassword(newUser, model.password);

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    // Gửi email xác minhz`
                    await _emailSender.SendVerificationLinkAsync(
                        newUser.email,
                        token,
                        HttpContext.RequestAborted);
                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Problem(
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Unable to register account.");
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
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.verificationToken == token);
            if (user != null &&
                user.VerificationTokenExpires.HasValue &&
                user.VerificationTokenExpires.Value >= DateTime.UtcNow)
            {
                user.status = 1; // Đã kích hoạt
                user.verificationToken = null; // Xóa mã xác minh sau khi xác minh thành công
                user.VerificationTokenExpires = null;
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
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid email or password." });
            }

            var user = await _context.Users
                .Include(u => u.Role) // Bao gồm thông tin vai trò của người dùng
                .FirstOrDefaultAsync(u => u.email == model.email);

            if (user != null)
            {
                var passwordCheck = _passwordService.VerifyPassword(user, model.password);
                if (!passwordCheck.Succeeded)
                {
                    return Json(new { success = false, message = "Invalid email or password." });
                }

                if (user.status == 0)
                {
                    return Json(new { success = false, message = "Your account is not activated yet. You need check your gmail and verify account." });
                }

                if (!user.ID.HasValue ||
                    user.Role is null ||
                    string.IsNullOrWhiteSpace(user.Role.RoleName))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Account role configuration is invalid."
                    });
                }

                if (passwordCheck.NeedsRehash)
                {
                    user.password = _passwordService.HashPassword(user, model.password);
                    await _context.SaveChangesAsync();
                }
                // Nếu người dùng tồn tại và tài khoản đã được kích hoạt, tạo claims cho người dùng
                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.fullname),
                            new Claim(ClaimTypes.Name, user.email),
                            new Claim(ClaimTypes.NameIdentifier, user.ID.Value.ToString()),
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        //hàm thay đổi mật khẩu
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        [ValidateAntiForgeryToken]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid password data.",
                    errors = ModelState.Values
                        .SelectMany(value => value.Errors)
                        .Select(error => error.ErrorMessage)
                });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out long userId))
            {
                var user = _context.Users.Find(userId);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                var currentPasswordCheck = _passwordService.VerifyPassword(user, model.CurrentPassword);
                if (!currentPasswordCheck.Succeeded)
                {
                    return Json(new { success = false, message = "Current password is incorrect." });
                }

                if (_passwordService.VerifyPassword(user, model.NewPassword).Succeeded)
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
                user.password = _passwordService.HashPassword(user, model.NewPassword);
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
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("PasswordReset")]
        [Route("ResetPasswordRequest")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.email == model.email);
                if (user != null)
                {
                    var token = Guid.NewGuid().ToString();
                    user.PasswordResetToken = token;
                    int expirationHours = _config.GetValue<int>("PasswordResetLinkExpirationHours");
                    user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(expirationHours);

                    await _context.SaveChangesAsync();

                    await _emailSender.SendPasswordResetLinkAsync(
                        model.email,
                        token,
                        HttpContext.RequestAborted);
                }

                return Json(new
                {
                    success = true,
                    message = "If an account exists for this email, a password reset link has been sent."
                });

            }
            return Json(new { success = false, message = "Invalid request." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            if (!user.PasswordResetTokenExpires.HasValue ||
                user.PasswordResetTokenExpires.Value < DateTime.UtcNow)
            {
                return Json(new { success = false, message = "Token expired." });
            }

            user.password = _passwordService.HashPassword(user, model.NewPassword);
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
        [Microsoft.AspNetCore.Authorization.Authorize]
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

                if (user.Role is null || string.IsNullOrWhiteSpace(user.Role.RoleName))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Account role configuration is invalid."
                    });
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
        [Microsoft.AspNetCore.Authorization.Authorize]
        [ValidateAntiForgeryToken]
        [Route("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid profile data.",
                    errors = ModelState.Values
                        .SelectMany(value => value.Errors)
                        .Select(error => error.ErrorMessage)
                });
            }

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
            return provider.TryGetContentType(filePath, out var contentType) &&
                   !string.IsNullOrWhiteSpace(contentType)
                ? contentType
                : "application/octet-stream";
        }
    }
}
