# Quizzy — Project Completion Checklist

> Mục tiêu: hoàn thiện dự án theo từng phần độc lập. Mỗi giai đoạn phải đi theo thứ tự **sửa → build → test → Quality Gate**. Chỉ chuyển sang giai đoạn tiếp theo khi Quality Gate hiện tại đã đạt.

## Quy ước

- `[ ]` Chưa thực hiện
- `[x]` Đã hoàn thành và có kết quả kiểm tra
- Không dùng database demo/production cho test có thay đổi dữ liệu.
- Không sửa bảo mật, nghiệp vụ và giao diện trong cùng một giai đoạn.
- Sau mỗi giai đoạn phải ghi lại lệnh test và kết quả.

---

## 1. Nền tảng kiểm thử

### Thực hiện

- [x] Tạo test project riêng.
- [x] Thêm test project vào solution.
- [x] Tạo cấu hình môi trường `Test`.
- [x] Tạo database riêng cho integration test.
- [x] Tạo cơ chế reset database test.
- [x] Tạo WebApplicationFactory cho integration test.
- [x] Tạo smoke test cho các route public.
- [x] Chuẩn hóa một lệnh chạy toàn bộ build và test.

### Kiểm tra

- [x] `dotnet build` thành công.
- [x] `dotnet test` thành công.
- [x] Test database được tạo và reset độc lập.
- [x] Test không thay đổi database demo.
- [x] Ứng dụng vẫn chạy tại port local cấu hình.

### Quality Gate

- [x] Build không có error mới.
- [x] Test project chạy ổn định nhiều lần.
- [x] Có thể kiểm tra dự án từ một clean checkout.
- [x] Không chuyển bước khi test còn phụ thuộc database demo.

---

## 2. SMTP và secret

### Thực hiện

- [x] Thu hồi SMTP password đang nằm trong source.
- [x] Tạo `EmailOptions`.
- [x] Chuyển SMTP email/password sang User Secrets hoặc environment variables.
- [x] Chuyển `EmailHelper` thành service qua dependency injection.
- [x] Đưa SMTP host, port và SSL vào configuration.
- [x] Đưa application base URL vào configuration.
- [x] Tạo fake email sender cho môi trường test.
- [x] Không trả token nhạy cảm trong response production.

### Kiểm tra

- [x] Search toàn repository không còn SMTP password.
- [x] Thiếu SMTP config trả lỗi cấu hình rõ ràng.
- [x] Fake email sender không gửi email ra ngoài.
- [x] Verification email chứa URL đúng.
- [x] Reset password email chứa URL đúng.
- [x] Build và toàn bộ test thành công.

### Quality Gate

- [x] Không còn secret trong source hoặc Git diff.
- [x] Test không gửi email thật.
- [x] Register/reset vẫn gọi đúng email service.

---

## 3. Password hashing

### Thực hiện

- [x] Thay MD5 bằng `PasswordHasher<User>` hoặc ASP.NET Core Identity.
- [x] Thiết kế cột hoặc định dạng nhận biết hash cũ/mới.
- [x] Cho phép tài khoản MD5 cũ đăng nhập một lần để migrate hash.
- [x] Register luôn lưu hash mới.
- [x] Change Password luôn lưu hash mới.
- [x] Reset Password luôn lưu hash mới.
- [x] Xóa đường tạo MD5 cho mật khẩu mới.
- [x] Không log password hoặc password hash.

### Kiểm tra

- [x] Đăng nhập bằng tài khoản hash mới.
- [x] Đăng nhập bằng tài khoản MD5 cũ.
- [x] Xác nhận hash cũ được nâng cấp sau khi đăng nhập.
- [x] Sai password bị từ chối.
- [x] Change Password hoạt động.
- [x] Reset Password hoạt động.
- [x] Hai tài khoản cùng password có hash khác nhau.
- [x] Build và test thành công.

### Quality Gate

- [x] Không có mật khẩu mới nào được lưu bằng MD5.
- [x] Tài khoản seed cũ vẫn truy cập được.
- [x] Không phát sinh regression đăng nhập.

---

## 4. Nullable và build warnings

### 4.1. Request/ViewModel

- [x] Sửa warning trong `LoginModel`.
- [x] Sửa warning trong `RegisterModel`.
- [x] Sửa warning trong `ChangePasswordModel`.
- [x] Sửa warning trong Reset Password models.
- [x] Sửa warning trong Profile models.
- [x] Thêm validation attribute phù hợp.
- [x] Test request thiếu và sai dữ liệu.

### 4.2. Entity models

- [x] Đối chiếu nullable property với schema database.
- [x] Dùng `required` hoặc giá trị mặc định cho field bắt buộc.
- [x] Dùng `?` cho field thực sự được phép null.
- [x] Không gán `string.Empty` hàng loạt khi chưa xác minh nghiệp vụ.
- [x] Kiểm tra migration diff không đổi schema ngoài ý muốn.
- [x] Test đọc dữ liệu seed.

### 4.3. Controllers

- [x] Sửa possible null dereference.
- [x] Sửa possible null assignment.
- [x] Trả `NotFound` cho ID không tồn tại.
- [x] Trả `BadRequest` cho request không hợp lệ.
- [x] Không dùng toán tử `!` chỉ để che warning.

### 4.4. Migration warnings

- [x] Liệt kê migration có class name viết thường.
- [x] Xác định migration nào đã được áp dụng.
- [x] Không rename migration đã áp dụng nếu chưa có chiến lược an toàn.
- [x] Ghi rõ warning nào được giữ lại và lý do.

### Quality Gate

- [x] Build không có nullable warning.
- [x] Warning migration còn lại được phê duyệt rõ ràng.
- [x] Không thay đổi schema ngoài ý muốn.
- [x] Toàn bộ test vẫn xanh.

---

## 5. Authentication và authorization

### Thực hiện

- [x] Rà `[Authorize]` trên từng controller/action.
- [x] Dashboard chỉ cho role phù hợp.
- [x] API riêng tư yêu cầu đăng nhập.
- [x] Lấy User ID từ claims thay vì tin query/body.
- [x] Chặn người dùng xem Practice của người khác.
- [x] Chặn người dùng xem Quiz/Review của người khác.
- [x] Chuẩn hóa `401`, `403` và redirect login.
- [x] Cấu hình cookie `HttpOnly`.
- [x] Cấu hình cookie `SecurePolicy`.
- [x] Cấu hình cookie `SameSite`.
- [x] Kiểm tra thời gian hết hạn phiên.

### Kiểm tra

- [x] Guest truy cập route public.
- [x] Guest truy cập route riêng tư.
- [x] Customer truy cập Dashboard.
- [x] User A truy cập Practice của User B.
- [x] User A truy cập Quiz/Review của User B.
- [x] Marketing/Admin truy cập đúng chức năng.
- [x] Logout xóa phiên.

### Quality Gate

- [x] Không có IDOR.
- [x] Role sai nhận `403`.
- [x] Guest không gọi được API riêng tư.
- [x] Navigation phản ánh đúng quyền.

---

## 6. CSRF, validation và XSS

### Tiến độ theo lát cắt

- [x] Bảo vệ CSRF cho form `SubjectRegister/Register`.
- [x] Test đăng ký môn học có token hợp lệ.
- [x] Test đăng ký môn học thiếu token nhận `400` và không ghi dữ liệu.
- [x] Phát CSRF token dùng chung qua layout và tự gắn header cho AJAX.
- [x] Bảo vệ mutation Registration, Practice và Quiz.
- [x] Test AJAX mutation thiếu header CSRF nhận `400`.
- [x] Bảo vệ CSRF cho các mutation Account còn lại.
- [x] Loại bỏ `Html.Raw` khỏi nội dung câu hỏi.
- [x] Không ghép trực tiếp dữ liệu động vào HTML ở Dashboard/Practice/Quiz/Register.
- [x] Thêm regression test cho các sink XSS đã sửa.
- [x] Không trả `Exception.Message` cho client.
- [x] Chuẩn hóa lỗi hệ thống private API bằng `ProblemDetails` và mã `500`.
- [x] Chặn submit form lặp và mở lại nút sau khi AJAX hoàn tất.
- [x] Làm submit đáp án Quiz idempotent bằng cách tính lại tổng câu đúng.

### Thực hiện

- [x] Thêm anti-forgery token cho form MVC.
- [x] Gửi anti-forgery token trong AJAX mutation.
- [x] Bật validation phía server.
- [x] Không dựa hoàn toàn vào JavaScript validation.
- [x] Chuẩn hóa error response.
- [x] Encode nội dung trước khi đưa vào HTML động.
- [x] Chống double submit.

### Kiểm tra

- [x] POST hợp lệ có token.
- [x] POST thiếu token bị từ chối.
- [x] Request rỗng.
- [x] Request sai kiểu dữ liệu.
- [x] Payload chứa HTML/script.
- [x] Request sửa dữ liệu người khác.
- [x] Double click nút submit.

### Quality Gate

- [x] Mutation thiếu token bị từ chối.
- [x] XSS payload không được thực thi.
- [x] Validation lỗi hiển thị rõ.
- [x] Không tạo dữ liệu rác.

---

## 7. Register, verification và reset password

### Thực hiện

- [x] Chuẩn hóa loading/success/error state.
- [x] Verification token có thời hạn.
- [x] Reset token có thời hạn.
- [x] Token chỉ dùng được một lần.
- [x] Không tiết lộ email có tồn tại hay không.
- [x] Thêm rate limit gửi email.
- [x] Xử lý link sai và hết hạn.
- [x] Hoàn thiện Verification Success.
- [x] Hoàn thiện Error page.

### Kiểm tra

- [x] Register hợp lệ.
- [x] Email trùng.
- [x] Email sai định dạng.
- [x] Password yếu.
- [x] Verification token hợp lệ.
- [x] Verification token sai.
- [x] Verification token hết hạn.
- [x] Reset với email tồn tại.
- [x] Reset với email không tồn tại.
- [x] Reset token dùng lại.
- [x] Password xác nhận không khớp.

### Quality Gate

- [x] Không lộ token trong production response.
- [x] Không user enumeration.
- [x] Fake mailbox nhận đúng email.
- [x] Authentication test xanh.

---

## 8. Subject registration và payment simulation

### Thực hiện

- [x] Chuẩn hóa registration status bằng enum/constants.
- [x] Kiểm tra quyền sở hữu registration.
- [x] Chặn registration trùng.
- [x] Chặn payment lặp.
- [x] Chặn cancel lặp.
- [x] Dùng transaction cho thao tác nhiều bước.
- [x] Giá tiền được lấy từ server/database.
- [x] UI phản ánh đúng trạng thái server.

### Kiểm tra

- [x] Đăng ký mới.
- [x] Đăng ký trùng.
- [x] Đổi package.
- [x] Pay package.
- [x] Cancel package.
- [x] Pay hai lần.
- [x] Cancel hai lần.
- [x] User A sửa registration của User B.
- [x] Đối chiếu database trước/sau.

### Quality Gate

- [x] Trạng thái database chính xác.
- [x] Không có record trùng.
- [x] Không sửa dữ liệu người khác.
- [x] Modal không báo thành công giả.

---

## 9. Practice creation

### Thực hiện

- [x] Validate title.
- [x] Validate number of questions.
- [x] Validate duration.
- [x] Validate difficulty.
- [x] Validate subject registration.
- [x] Kiểm tra ngân hàng đủ câu hỏi.
- [x] Dùng transaction khi tạo Practice và QuizHandle.
- [x] Chống double submit.
- [x] Trả lỗi rõ khi không đủ dữ liệu.

### Kiểm tra

- [x] Tạo Easy Practice.
- [x] Tạo Medium Practice.
- [x] Tạo Hard Practice.
- [x] Số câu bằng 0.
- [x] Số câu âm.
- [x] Số câu vượt ngân hàng.
- [x] Duration không hợp lệ.
- [x] Subject chưa đăng ký.
- [x] Double click submit.
- [x] Giả lập lỗi giữa transaction.

### Quality Gate

- [x] Practice và QuizHandle được tạo đồng bộ.
- [x] Lỗi không để lại dữ liệu dở dang.
- [x] Phân bố difficulty đúng.
- [x] Điều hướng vào Quiz đúng.

---

## 10. Quiz lifecycle

### Thực hiện

- [x] Kiểm tra quyền sở hữu attempt.
- [x] Chuẩn hóa Previous/Next.
- [x] Chuẩn hóa Mark/Unmark.
- [x] Lưu đáp án phía server.
- [x] Xử lý timer phía server.
- [x] Chặn submit nhiều lần.
- [x] Tính điểm từ dữ liệu server.
- [x] Không gửi đáp án đúng trước khi submit.
- [x] Khôi phục trạng thái sau refresh.

### Kiểm tra

- [x] Tải câu đầu tiên.
- [x] Previous/Next.
- [x] Chọn đáp án.
- [x] Đổi đáp án.
- [x] Mark/Unmark.
- [x] Refresh giữa bài.
- [x] Hết thời gian.
- [x] Submit.
- [x] Submit lại.
- [x] User khác truy cập attempt.
- [x] Đối chiếu điểm trong database.

### Quality Gate

- [x] Không lộ đáp án đúng.
- [x] Không sửa attempt đã hoàn thành.
- [x] Điểm chính xác.
- [x] Refresh không mất dữ liệu.

---

## 11. Quiz Review

### Thực hiện

- [x] Chỉ chủ sở hữu được xem review.
- [x] Hiển thị đáp án đã chọn.
- [x] Hiển thị đáp án đúng.
- [x] Xử lý câu bỏ trống.
- [x] Filter correct/incorrect.
- [x] Chuẩn hóa summary.

### Kiểm tra

- [x] Practice hoàn thành.
- [x] Practice chưa hoàn thành.
- [x] Câu đúng.
- [x] Câu sai.
- [x] Câu bỏ trống.
- [x] Filter.
- [x] ID không tồn tại.
- [x] User khác truy cập review.

### Quality Gate

- [x] Review khớp database.
- [x] Không lộ dữ liệu người khác.
- [x] Không lỗi với đáp án null.

---

## 12. Dashboard và reporting

### Thực hiện

- [x] Giới hạn role truy cập.
- [x] Validate khoảng ngày.
- [x] Chuẩn hóa timezone.
- [x] Kiểm tra truy vấn doanh thu.
- [x] Empty state khi không có dữ liệu.
- [x] Error state khi API lỗi.
- [x] Rà N+1 query.

### Kiểm tra

- [x] Khoảng ngày hợp lệ.
- [x] Ngày bắt đầu lớn hơn ngày kết thúc.
- [x] Khoảng không có dữ liệu.
- [x] Customer truy cập.
- [x] Marketing/Admin truy cập.
- [x] So sánh số liệu API với database.
- [x] Chart nhận dataset rỗng.

### Quality Gate

- [x] Số liệu chính xác.
- [x] Không truy cập sai role.
- [x] Chart không lỗi với dữ liệu rỗng.

---

## 13. Frontend cleanup và accessibility

### Thực hiện

- [x] Tách JavaScript lớn khỏi Razor.
  - [x] Dashboard: chuyển 353 dòng sang `wwwroot/js/Dashboard.js`.
  - [x] MyRegistrations: chuyển 387 dòng sang `wwwroot/js/MyRegistrations.js`.
  - [x] Quiz Handle: chuyển 286 dòng sang `wwwroot/js/QuizHandle.js`, cấu hình attempt qua `data-*`.
  - [x] Practice/New Practice: chuyển lần lượt 139 và 151 dòng sang `PracticeList.js` và `NewPractice.js`.
  - [x] Profile/Login/Register/Change Password: tách 336 dòng sang bốn asset riêng, bổ sung dialog/live-region semantics.
  - [x] Blogs/Simulation Exam: tách 228 dòng, đồng thời loại bỏ HTML concatenation từ dữ liệu API.
- [x] Giảm CSS trùng.
  - [x] Audit selector toàn bộ CSS; xóa block `canvas` Dashboard trùng hệt, giữ các cascade responsive/theme có body khác nhau.
- [x] Giảm `!important`.
  - [x] Giảm từ 125 xuống 109; `ResetPassword.css` không còn `!important`.
  - [x] Loại bỏ 12 rule `outline: none/0` ghi đè keyboard focus.
- [x] Xóa selector không dùng sau khi xác minh.
  - [x] Xóa `.dashboard-error`, `.customer-stat-item`, `.customer-stat-title` sau khi quét toàn source.
- [x] Chuẩn hóa button states.
- [x] Chuẩn hóa loading/empty/error states.
  - [x] Dashboard, Practice, MyRegistrations, Simulation, Account và Quiz có trạng thái lỗi/rỗng/loading nội tuyến.
  - [x] Quiz không còn dùng `alert()` cho lỗi API; answer text được escape trước khi tạo markup.
- [x] Chuẩn hóa disabled state.
  - [x] Style disabled dùng chung; Login/Register/Change Password/Profile/New Practice/Payment/Cancel chặn submit lặp.
- [x] Kiểm tra label và accessible name.
- [x] Kiểm tra tab order.
  - [x] Profile fields, Dashboard filters/charts và Subject popup có accessible name.
  - [x] Không có positive `tabindex`; giữ thứ tự focus theo DOM.
- [x] Kiểm tra focus-visible.
- [x] Kiểm tra reduced motion.

### Responsive test

- [x] `375px`.
  - [x] Public shell/routes: không document overflow, không console error.
  - [x] Authenticated routes: đăng nhập thành công; Practice, New Practice, Simulation, Quiz Review và My Registrations không document overflow.
- [x] `768px`.
  - [x] Public shell/routes: không document overflow, mobile menu hoạt động.
  - [x] Authenticated routes: không document overflow; Profile/Change Password modal vừa viewport.
- [x] `1024px`.
  - [x] Public shell/routes: không document overflow, desktop header hoạt động.
  - [x] Authenticated routes: không document overflow; Profile/Change Password modal hoạt động.
- [x] `1440px`.
  - [x] Public shell/routes: không document overflow.
  - [x] Authenticated routes: không document overflow; không console/page error.
- [x] `1920px`.
  - [x] Public shell/routes: không document overflow.
  - [x] Authenticated routes: không document overflow; không console/page error.
- [x] Không overflow ngoài table/carousel chủ động.
  - [x] Public routes; Simulation table và carousel/glare được xác nhận là overflow chủ động.
  - [x] Authenticated routes; Simulation table cuộn trong wrapper, không làm tràn document.

### Interaction test

- [x] Header/menu.
- [x] Login/Register modal.
- [x] Profile/Change Password modal.
- [x] Subject popup.
- [x] Carousel.
- [x] Search/filter/pagination.
- [x] Không có console error.

### Quality Gate

- [x] Không regression giao diện.
- [x] Keyboard navigation sử dụng được.
- [x] Không lỗi console.
- [x] Selector JavaScript hiện tại vẫn hoạt động.

---

## 14. Production readiness

### Thực hiện

- [x] Global exception handling.
- [x] Structured logging.
- [x] Health check.
- [x] Environment configuration.
- [ ] Database migration strategy.
- [x] Rate limiting cho login/register/reset.
- [ ] Security headers.
- [ ] Deployment profile hoặc Docker.
- [ ] CI chạy build và test.
- [ ] Viết lại README với cấu hình chính xác.
- [ ] Không ghi secret vào README.

### Kiểm tra

- [x] Chạy production configuration.
- [x] Thiếu config bắt buộc.
- [ ] Database unavailable.
- [ ] SMTP unavailable.
- [x] Health endpoint.
- [ ] CI từ clean checkout.
- [ ] Deploy staging.
- [ ] Smoke test staging.

### Final Quality Gate

- [ ] `0 error`.
- [ ] `0 warning` hoặc warning còn lại được phê duyệt.
- [ ] Toàn bộ automated test xanh.
- [ ] Không secret trong repository.
- [ ] Không dùng MD5 cho password.
- [ ] Không lỗi JavaScript console.
- [ ] Không test mutation trên database thật.
- [ ] Staging chạy được từ clean deployment.

---

## Bảng tiến độ

| Giai đoạn | Trạng thái | Ngày hoàn thành | Kết quả test/Ghi chú |
|---|---|---|---|
| 1. Nền tảng kiểm thử | Hoàn thành | 28/07/2026 | 6/6 test xanh; clean-copy restore/test đạt; localhost:5152 trả HTTP 200 |
| 2. SMTP và secret | Hoàn thành | 29/07/2026 | 11/11 automated test xanh; gửi email thật và mở link xác minh thành công; source sạch secret; App Password cũ đã thu hồi |
| 3. Password hashing | Hoàn thành | 28/07/2026 | 21/21 test xanh; PBKDF2 có salt; tài khoản MD5 cũ tự nâng cấp khi đăng nhập |
| 4. Nullable warnings | Hoàn thành | 29/07/2026 | Clean Release build 0 warning/0 error; 49/49 test xanh; schema không đổi |
| 5. Authentication/Authorization | Chưa làm | | |
| 6. CSRF/Validation/XSS | Chưa làm | | |
| 7. Register/Verification/Reset | Chưa làm | | |
| 8. Registration/Payment | Chưa làm | | |
| 9. Practice creation | Chưa làm | | |
| 10. Quiz lifecycle | Chưa làm | | |
| 11. Quiz Review | Hoàn thành | 29/07/2026 | Route hoạt động; chỉ review bài đã hoàn thành; summary đúng/sai/bỏ trống tính từ QuizHandle; filter theo trạng thái; 152/152 test pass |
| 12. Dashboard/Reporting | Hoàn thành | 29/07/2026 | UTC date range có validation; revenue aggregate tại DB; role, empty/error states và số liệu được test; 156/156 test pass |
| 13. Frontend cleanup | Đang thực hiện | 29/07/2026 | Script/CSS/UI states và accessibility markup đã audit; label/chart/dialog names đầy đủ, không positive tabindex; 183/183 test pass |
| 14. Production readiness | Chưa làm | | |

## Nhật ký kiểm thử

| Ngày | Giai đoạn | Lệnh/test đã chạy | Kết quả | Lỗi còn lại |
|---|---|---|---|---|
| 28/07/2026 | 1. Nền tảng kiểm thử | `dotnet test SWP391.sln --no-restore` | 6/6 test pass | Warning nền của project chính sẽ xử lý ở Giai đoạn 4 |
| 28/07/2026 | 1. Nền tảng kiểm thử | `dotnet build SWP391.sln --no-restore` | Build thành công, 0 error | Build incremental báo 0 warning |
| 28/07/2026 | 1. Nền tảng kiểm thử | Clean-copy `restore` + `test` | 6/6 test pass | Không sử dụng `bin/obj` của workspace |
| 28/07/2026 | 1. Nền tảng kiểm thử | Local startup smoke test | HTTP 200 tại port 5152 | Tiến trình test đã được dừng sau khi xác minh |
| 28/07/2026 | 2. SMTP và secret | `dotnet test SWP391.sln --no-restore --nologo --verbosity minimal` | 11/11 test pass | Warning nullable/migration cũ sẽ xử lý ở Giai đoạn 4 |
| 28/07/2026 | 2. SMTP và secret | Secret scan + `git diff --check` | Không còn credential SMTP cũ trong source; diff hợp lệ | Cần thu hồi/rotate Gmail App Password trên tài khoản Google |
| 29/07/2026 | 2. SMTP và secret | Manual SMTP + verification link | Email thật được gửi; link gọi đúng `http://localhost:5152`; tài khoản xác minh thành công | Chờ xác nhận App Password cũ đã bị thu hồi |
| 29/07/2026 | 2. SMTP và secret | Thu hồi credential cũ | Người dùng xác nhận Gmail App Password cũ đã bị thu hồi | Không còn lỗi trong phạm vi Giai đoạn 2 |
| 29/07/2026 | 4.1 Request/ViewModel | `dotnet test SWP391.sln -c Release --no-restore --nologo --verbosity minimal` | 26/26 test pass; nhóm request không còn CS8618 | Nullable entity/controller và warning migration tiếp tục ở 4.2–4.4 |
| 29/07/2026 | 4.1 Request/ViewModel | Validation tests + migration/diff check | Request thiếu/sai bị validation từ chối; không có file migration thay đổi; diff hợp lệ | Debug output đang bị ứng dụng PID 16936 khóa nên dùng Release |
| 29/07/2026 | 4.2 Entity — User/Role | `dotnet test SWP391.sln -c Release --no-restore --nologo --verbosity minimal` | 28/28 test pass; đọc được 6 role seed; collection navigation khởi tạo an toàn | Các entity còn lại tiếp tục xử lý theo từng cụm |
| 29/07/2026 | 4.2 Entity — User/Role | `dotnet ef migrations has-pending-model-changes --configuration Release --no-build` | Không có model change so với migration snapshot | Không thay đổi schema |
| 29/07/2026 | 4.2 Entity — Subject/Category/PricePackage | `dotnet test SWP391.sln -c Release --no-restore --nologo --verbosity minimal` | 30/30 test pass; EF metadata nullability và collection initialization đạt | Các entity Practice/Quiz/Blog còn lại |
| 29/07/2026 | 4.2 Entity — Subject/Category/PricePackage | Pending model changes + `git diff --check` | Không có model change; diff hợp lệ | Không thay đổi schema |
| 29/07/2026 | 4.2 Entity — Practice/Level/Topic | Release test + pending model changes | 32/32 test pass; đọc được seed Level/Topic; không có model change | Quiz/Blog/Recipe/Slider/Exam còn lại |
| 29/07/2026 | 4.2 Entity — QuizBank/QuizHandle/SimulationExam | Release test + pending model changes + diff check | 34/34 test pass; EF required metadata đạt; không có model/schema change | Blog/Recipe/Slider và ViewModel còn lại |
| 29/07/2026 | 4.2 Entity — Blog/Recipe/Slider | Release test + pending model changes + diff check | 39/39 test pass; toàn bộ entity chính không còn CS8618; không có model/schema change | Chuyển sang 4.3 Controllers; ViewModel trình bày xử lý cùng controller tương ứng |
| 29/07/2026 | 4.3 Controller — Account | Release test + warning filter | 40/40 test pass; AccountController không còn CS8600/01/02/04; account thiếu role bị từ chối rõ ràng | Controllers Practice/Quiz/Blog/Subjects còn lại |
| 29/07/2026 | 4.3 Controller — Practice API | Release test + warning filter + diff check | 42/42 test pass; form thiếu/sai trả 400 trước khi ghi DB; controller không còn nullable warning | Quiz/Blog/Subjects/Review còn lại |
| 29/07/2026 | 4.3 Controller — Quiz/Quiz API/Review | Release test + warning filter | 46/46 test pass; resource thiếu trả 404; submit SQL được parameterize; cụm Quiz không còn nullable warning | Blog/Subjects và ViewModel Home/Blog/Subject còn lại |
| 29/07/2026 | 4.3 Controller — Blogs + ViewModel | Release test + warning filter | 47/47 test pass; blog/tác giả thiếu trả 404; list không null; cụm Blog không còn nullable warning | Subjects và Home/Subject/Registration ViewModel còn lại |
| 29/07/2026 | 4.3 Controller — Subjects + remaining ViewModels | Release test + nullable warning audit | 49/49 test pass; 0 warning CS8600/01/02/03/04/8618 toàn project | Còn 6 dòng CS8981 từ 3 migration cũ, xử lý ở 4.4 |
| 29/07/2026 | 4.4 Migration warnings | DB migration list + localized `CS8981` suppression | Xác nhận 3 migration đã áp dụng; giữ nguyên class/ID; clean Release build 0 warning/0 error | Không còn warning |
| 29/07/2026 | 4. Final Quality Gate | 49 tests + pending model changes + diff check | 49/49 pass; không có pending model changes; diff hợp lệ | Giai đoạn 4 hoàn thành |
| 28/07/2026 | 3. Password hashing | `dotnet test SWP391.sln --no-restore --nologo --verbosity minimal` | 21/21 test pass | Warning nullable/migration cũ sẽ xử lý ở Giai đoạn 4 |
| 28/07/2026 | 3. Password hashing | MD5/write-path scan + migration/diff check | MD5 chỉ còn ở nhánh xác minh legacy; không có migration/schema mới; diff hợp lệ | Không có lỗi còn lại trong phạm vi Giai đoạn 3 |
