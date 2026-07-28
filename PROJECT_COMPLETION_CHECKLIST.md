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
- [ ] Ứng dụng vẫn chạy tại port local cấu hình.

### Quality Gate

- [x] Build không có error mới.
- [x] Test project chạy ổn định nhiều lần.
- [ ] Có thể kiểm tra dự án từ một clean checkout.
- [x] Không chuyển bước khi test còn phụ thuộc database demo.

---

## 2. SMTP và secret

### Thực hiện

- [ ] Thu hồi SMTP password đang nằm trong source.
- [ ] Tạo `EmailOptions`.
- [ ] Chuyển SMTP email/password sang User Secrets hoặc environment variables.
- [ ] Chuyển `EmailHelper` thành service qua dependency injection.
- [ ] Đưa SMTP host, port và SSL vào configuration.
- [ ] Đưa application base URL vào configuration.
- [ ] Tạo fake email sender cho môi trường test.
- [ ] Không trả token nhạy cảm trong response production.

### Kiểm tra

- [ ] Search toàn repository không còn SMTP password.
- [ ] Thiếu SMTP config trả lỗi cấu hình rõ ràng.
- [ ] Fake email sender không gửi email ra ngoài.
- [ ] Verification email chứa URL đúng.
- [ ] Reset password email chứa URL đúng.
- [ ] Build và toàn bộ test thành công.

### Quality Gate

- [ ] Không còn secret trong source hoặc Git diff.
- [ ] Test không gửi email thật.
- [ ] Register/reset vẫn gọi đúng email service.

---

## 3. Password hashing

### Thực hiện

- [ ] Thay MD5 bằng `PasswordHasher<User>` hoặc ASP.NET Core Identity.
- [ ] Thiết kế cột hoặc định dạng nhận biết hash cũ/mới.
- [ ] Cho phép tài khoản MD5 cũ đăng nhập một lần để migrate hash.
- [ ] Register luôn lưu hash mới.
- [ ] Change Password luôn lưu hash mới.
- [ ] Reset Password luôn lưu hash mới.
- [ ] Xóa đường tạo MD5 cho mật khẩu mới.
- [ ] Không log password hoặc password hash.

### Kiểm tra

- [ ] Đăng nhập bằng tài khoản hash mới.
- [ ] Đăng nhập bằng tài khoản MD5 cũ.
- [ ] Xác nhận hash cũ được nâng cấp sau khi đăng nhập.
- [ ] Sai password bị từ chối.
- [ ] Change Password hoạt động.
- [ ] Reset Password hoạt động.
- [ ] Hai tài khoản cùng password có hash khác nhau.
- [ ] Build và test thành công.

### Quality Gate

- [ ] Không có mật khẩu mới nào được lưu bằng MD5.
- [ ] Tài khoản seed cũ vẫn truy cập được.
- [ ] Không phát sinh regression đăng nhập.

---

## 4. Nullable và build warnings

### 4.1. Request/ViewModel

- [ ] Sửa warning trong `LoginModel`.
- [ ] Sửa warning trong `RegisterModel`.
- [ ] Sửa warning trong `ChangePasswordModel`.
- [ ] Sửa warning trong Reset Password models.
- [ ] Sửa warning trong Profile models.
- [ ] Thêm validation attribute phù hợp.
- [ ] Test request thiếu và sai dữ liệu.

### 4.2. Entity models

- [ ] Đối chiếu nullable property với schema database.
- [ ] Dùng `required` hoặc giá trị mặc định cho field bắt buộc.
- [ ] Dùng `?` cho field thực sự được phép null.
- [ ] Không gán `string.Empty` hàng loạt khi chưa xác minh nghiệp vụ.
- [ ] Kiểm tra migration diff không đổi schema ngoài ý muốn.
- [ ] Test đọc dữ liệu seed.

### 4.3. Controllers

- [ ] Sửa possible null dereference.
- [ ] Sửa possible null assignment.
- [ ] Trả `NotFound` cho ID không tồn tại.
- [ ] Trả `BadRequest` cho request không hợp lệ.
- [ ] Không dùng toán tử `!` chỉ để che warning.

### 4.4. Migration warnings

- [ ] Liệt kê migration có class name viết thường.
- [ ] Xác định migration nào đã được áp dụng.
- [ ] Không rename migration đã áp dụng nếu chưa có chiến lược an toàn.
- [ ] Ghi rõ warning nào được giữ lại và lý do.

### Quality Gate

- [ ] Build không có nullable warning.
- [ ] Warning migration còn lại được phê duyệt rõ ràng.
- [ ] Không thay đổi schema ngoài ý muốn.
- [ ] Toàn bộ test vẫn xanh.

---

## 5. Authentication và authorization

### Thực hiện

- [ ] Rà `[Authorize]` trên từng controller/action.
- [ ] Dashboard chỉ cho role phù hợp.
- [ ] API riêng tư yêu cầu đăng nhập.
- [ ] Lấy User ID từ claims thay vì tin query/body.
- [ ] Chặn người dùng xem Practice của người khác.
- [ ] Chặn người dùng xem Quiz/Review của người khác.
- [ ] Chuẩn hóa `401`, `403` và redirect login.
- [ ] Cấu hình cookie `HttpOnly`.
- [ ] Cấu hình cookie `SecurePolicy`.
- [ ] Cấu hình cookie `SameSite`.
- [ ] Kiểm tra thời gian hết hạn phiên.

### Kiểm tra

- [ ] Guest truy cập route public.
- [ ] Guest truy cập route riêng tư.
- [ ] Customer truy cập Dashboard.
- [ ] User A truy cập Practice của User B.
- [ ] User A truy cập Quiz/Review của User B.
- [ ] Marketing/Admin truy cập đúng chức năng.
- [ ] Logout xóa phiên.

### Quality Gate

- [ ] Không có IDOR.
- [ ] Role sai nhận `403`.
- [ ] Guest không gọi được API riêng tư.
- [ ] Navigation phản ánh đúng quyền.

---

## 6. CSRF, validation và XSS

### Thực hiện

- [ ] Thêm anti-forgery token cho form MVC.
- [ ] Gửi anti-forgery token trong AJAX mutation.
- [ ] Bật validation phía server.
- [ ] Không dựa hoàn toàn vào JavaScript validation.
- [ ] Chuẩn hóa error response.
- [ ] Encode nội dung trước khi đưa vào HTML động.
- [ ] Chống double submit.

### Kiểm tra

- [ ] POST hợp lệ có token.
- [ ] POST thiếu token bị từ chối.
- [ ] Request rỗng.
- [ ] Request sai kiểu dữ liệu.
- [ ] Payload chứa HTML/script.
- [ ] Request sửa dữ liệu người khác.
- [ ] Double click nút submit.

### Quality Gate

- [ ] Mutation thiếu token bị từ chối.
- [ ] XSS payload không được thực thi.
- [ ] Validation lỗi hiển thị rõ.
- [ ] Không tạo dữ liệu rác.

---

## 7. Register, verification và reset password

### Thực hiện

- [ ] Chuẩn hóa loading/success/error state.
- [ ] Verification token có thời hạn.
- [ ] Reset token có thời hạn.
- [ ] Token chỉ dùng được một lần.
- [ ] Không tiết lộ email có tồn tại hay không.
- [ ] Thêm rate limit gửi email.
- [ ] Xử lý link sai và hết hạn.
- [ ] Hoàn thiện Verification Success.
- [ ] Hoàn thiện Error page.

### Kiểm tra

- [ ] Register hợp lệ.
- [ ] Email trùng.
- [ ] Email sai định dạng.
- [ ] Password yếu.
- [ ] Verification token hợp lệ.
- [ ] Verification token sai.
- [ ] Verification token hết hạn.
- [ ] Reset với email tồn tại.
- [ ] Reset với email không tồn tại.
- [ ] Reset token dùng lại.
- [ ] Password xác nhận không khớp.

### Quality Gate

- [ ] Không lộ token trong production response.
- [ ] Không user enumeration.
- [ ] Fake mailbox nhận đúng email.
- [ ] Authentication test xanh.

---

## 8. Subject registration và payment simulation

### Thực hiện

- [ ] Chuẩn hóa registration status bằng enum/constants.
- [ ] Kiểm tra quyền sở hữu registration.
- [ ] Chặn registration trùng.
- [ ] Chặn payment lặp.
- [ ] Chặn cancel lặp.
- [ ] Dùng transaction cho thao tác nhiều bước.
- [ ] Giá tiền được lấy từ server/database.
- [ ] UI phản ánh đúng trạng thái server.

### Kiểm tra

- [ ] Đăng ký mới.
- [ ] Đăng ký trùng.
- [ ] Đổi package.
- [ ] Pay package.
- [ ] Cancel package.
- [ ] Pay hai lần.
- [ ] Cancel hai lần.
- [ ] User A sửa registration của User B.
- [ ] Đối chiếu database trước/sau.

### Quality Gate

- [ ] Trạng thái database chính xác.
- [ ] Không có record trùng.
- [ ] Không sửa dữ liệu người khác.
- [ ] Modal không báo thành công giả.

---

## 9. Practice creation

### Thực hiện

- [ ] Validate title.
- [ ] Validate number of questions.
- [ ] Validate duration.
- [ ] Validate difficulty.
- [ ] Validate subject registration.
- [ ] Kiểm tra ngân hàng đủ câu hỏi.
- [ ] Dùng transaction khi tạo Practice và QuizHandle.
- [ ] Chống double submit.
- [ ] Trả lỗi rõ khi không đủ dữ liệu.

### Kiểm tra

- [ ] Tạo Easy Practice.
- [ ] Tạo Medium Practice.
- [ ] Tạo Hard Practice.
- [ ] Số câu bằng 0.
- [ ] Số câu âm.
- [ ] Số câu vượt ngân hàng.
- [ ] Duration không hợp lệ.
- [ ] Subject chưa đăng ký.
- [ ] Double click submit.
- [ ] Giả lập lỗi giữa transaction.

### Quality Gate

- [ ] Practice và QuizHandle được tạo đồng bộ.
- [ ] Lỗi không để lại dữ liệu dở dang.
- [ ] Phân bố difficulty đúng.
- [ ] Điều hướng vào Quiz đúng.

---

## 10. Quiz lifecycle

### Thực hiện

- [ ] Kiểm tra quyền sở hữu attempt.
- [ ] Chuẩn hóa Previous/Next.
- [ ] Chuẩn hóa Mark/Unmark.
- [ ] Lưu đáp án phía server.
- [ ] Xử lý timer phía server.
- [ ] Chặn submit nhiều lần.
- [ ] Tính điểm từ dữ liệu server.
- [ ] Không gửi đáp án đúng trước khi submit.
- [ ] Khôi phục trạng thái sau refresh.

### Kiểm tra

- [ ] Tải câu đầu tiên.
- [ ] Previous/Next.
- [ ] Chọn đáp án.
- [ ] Đổi đáp án.
- [ ] Mark/Unmark.
- [ ] Refresh giữa bài.
- [ ] Hết thời gian.
- [ ] Submit.
- [ ] Submit lại.
- [ ] User khác truy cập attempt.
- [ ] Đối chiếu điểm trong database.

### Quality Gate

- [ ] Không lộ đáp án đúng.
- [ ] Không sửa attempt đã hoàn thành.
- [ ] Điểm chính xác.
- [ ] Refresh không mất dữ liệu.

---

## 11. Quiz Review

### Thực hiện

- [ ] Chỉ chủ sở hữu được xem review.
- [ ] Hiển thị đáp án đã chọn.
- [ ] Hiển thị đáp án đúng.
- [ ] Xử lý câu bỏ trống.
- [ ] Filter correct/incorrect.
- [ ] Chuẩn hóa summary.

### Kiểm tra

- [ ] Practice hoàn thành.
- [ ] Practice chưa hoàn thành.
- [ ] Câu đúng.
- [ ] Câu sai.
- [ ] Câu bỏ trống.
- [ ] Filter.
- [ ] ID không tồn tại.
- [ ] User khác truy cập review.

### Quality Gate

- [ ] Review khớp database.
- [ ] Không lộ dữ liệu người khác.
- [ ] Không lỗi với đáp án null.

---

## 12. Dashboard và reporting

### Thực hiện

- [ ] Giới hạn role truy cập.
- [ ] Validate khoảng ngày.
- [ ] Chuẩn hóa timezone.
- [ ] Kiểm tra truy vấn doanh thu.
- [ ] Empty state khi không có dữ liệu.
- [ ] Error state khi API lỗi.
- [ ] Rà N+1 query.

### Kiểm tra

- [ ] Khoảng ngày hợp lệ.
- [ ] Ngày bắt đầu lớn hơn ngày kết thúc.
- [ ] Khoảng không có dữ liệu.
- [ ] Customer truy cập.
- [ ] Marketing/Admin truy cập.
- [ ] So sánh số liệu API với database.
- [ ] Chart nhận dataset rỗng.

### Quality Gate

- [ ] Số liệu chính xác.
- [ ] Không truy cập sai role.
- [ ] Chart không lỗi với dữ liệu rỗng.

---

## 13. Frontend cleanup và accessibility

### Thực hiện

- [ ] Tách JavaScript lớn khỏi Razor.
- [ ] Giảm CSS trùng.
- [ ] Giảm `!important`.
- [ ] Xóa selector không dùng sau khi xác minh.
- [ ] Chuẩn hóa button states.
- [ ] Chuẩn hóa loading/empty/error states.
- [ ] Chuẩn hóa disabled state.
- [ ] Kiểm tra label và accessible name.
- [ ] Kiểm tra tab order.
- [ ] Kiểm tra focus-visible.
- [ ] Kiểm tra reduced motion.

### Responsive test

- [ ] `375px`.
- [ ] `768px`.
- [ ] `1024px`.
- [ ] `1440px`.
- [ ] `1920px`.
- [ ] Không overflow ngoài table/carousel chủ động.

### Interaction test

- [ ] Header/menu.
- [ ] Login/Register modal.
- [ ] Profile/Change Password modal.
- [ ] Subject popup.
- [ ] Carousel.
- [ ] Search/filter/pagination.
- [ ] Không có console error.

### Quality Gate

- [ ] Không regression giao diện.
- [ ] Keyboard navigation sử dụng được.
- [ ] Không lỗi console.
- [ ] Selector JavaScript hiện tại vẫn hoạt động.

---

## 14. Production readiness

### Thực hiện

- [ ] Global exception handling.
- [ ] Structured logging.
- [ ] Health check.
- [ ] Environment configuration.
- [ ] Database migration strategy.
- [ ] Rate limiting cho login/register/reset.
- [ ] Security headers.
- [ ] Deployment profile hoặc Docker.
- [ ] CI chạy build và test.
- [ ] Viết lại README với cấu hình chính xác.
- [ ] Không ghi secret vào README.

### Kiểm tra

- [ ] Chạy production configuration.
- [ ] Thiếu config bắt buộc.
- [ ] Database unavailable.
- [ ] SMTP unavailable.
- [ ] Health endpoint.
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
| 1. Nền tảng kiểm thử | Đang kiểm thử | | 6/6 test xanh; còn clean-checkout và xác nhận app chạy lại |
| 2. SMTP và secret | Chưa làm | | |
| 3. Password hashing | Chưa làm | | |
| 4. Nullable warnings | Chưa làm | | |
| 5. Authentication/Authorization | Chưa làm | | |
| 6. CSRF/Validation/XSS | Chưa làm | | |
| 7. Register/Verification/Reset | Chưa làm | | |
| 8. Registration/Payment | Chưa làm | | |
| 9. Practice creation | Chưa làm | | |
| 10. Quiz lifecycle | Chưa làm | | |
| 11. Quiz Review | Chưa làm | | |
| 12. Dashboard/Reporting | Chưa làm | | |
| 13. Frontend cleanup | Chưa làm | | |
| 14. Production readiness | Chưa làm | | |

## Nhật ký kiểm thử

| Ngày | Giai đoạn | Lệnh/test đã chạy | Kết quả | Lỗi còn lại |
|---|---|---|---|---|
| 28/07/2026 | 1. Nền tảng kiểm thử | `dotnet test SWP391.sln --no-restore` | 6/6 test pass | Warning nền của project chính sẽ xử lý ở Giai đoạn 4 |
| 28/07/2026 | 1. Nền tảng kiểm thử | `dotnet build SWP391.sln --no-restore` | Build thành công, 0 error | Build incremental báo 0 warning |
