# Giỏ hàng, đặt hàng COD và lịch sử đơn

## Chạy trên máy phát triển

Từ thư mục chứa `BanHangDienTu.slnx`:

```powershell
dotnet restore BanHangDienTu.slnx
dotnet build BanHangDienTu.slnx --configuration Release
dotnet ef database update --project BanHangDienTu --configuration Release --no-build
dotnet run --project BanHangDienTu
```

SQL Server dùng connection string `DefaultConnection` của ứng dụng. Migration `AddCartAndOrders` thêm ba bảng `CartItems`, `Orders`, `OrderItems`, không xóa bảng hiện có. Khởi động ứng dụng ở Development vẫn cần cấu hình SeedAdmin theo cơ chế sẵn có của dự án.

## Phân lớp

- `Controllers/CartController.cs`: GET giỏ; POST thêm, cập nhật, xóa.
- `Controllers/CheckoutController.cs`: GET/POST xác nhận đặt hàng, GET kết quả.
- `Controllers/OrderHistoryController.cs`: GET danh sách và chi tiết đơn của khách đăng nhập.
- `Services/Implementations/CartService.cs`: kiểm tra số lượng, tạo dữ liệu hiển thị giỏ.
- `Services/Implementations/OrderService.cs`: thông tin giao hàng mặc định, bảo vệ phiên xác nhận, COD và lịch sử.
- `Repositories/Implementations/CartRepository.cs`: giỏ lưu theo UserId, gộp sản phẩm trùng.
- `Repositories/Implementations/OrderRepository.cs`: lưu đơn và transaction tồn kho.
- `Models/Entities`: CartItem, Order, OrderItem; `Models/Enums`: trạng thái đơn, phương thức/trạng thái thanh toán.
- `ViewModels/Cart`, `ViewModels/Checkout`, `ViewModels/Order`: dữ liệu riêng cho giao diện.
- `Views/Cart`, `Views/Checkout`, `Views/OrderHistory`: Razor và Bootstrap theo layout hiện có.

## Quy tắc nghiệp vụ

- Chỉ role Customer có quyền vào các module. Mọi truy vấn giỏ/đơn đều có UserId lấy từ claims.
- Một tài khoản có một giỏ lưu bền trong SQL Server. Số lượng mỗi sản phẩm từ 1 đến 999 và không vượt tồn kho.
- Mua ngay thêm sản phẩm vào giỏ rồi xác nhận toàn bộ giỏ. Chưa hỗ trợ thanh toán một phần giỏ.
- Checkout lấy họ tên, SĐT, địa chỉ từ hồ sơ. Khách có thể sửa cho đơn hiện tại; không ghi đè hồ sơ.
- Giá luôn lấy từ database. Token xác nhận được Data Protection bảo vệ, gắn tài khoản, danh sách sản phẩm, số lượng, giá và hạn 30 phút.
- Giá hoặc nội dung giỏ đổi sau khi mở checkout thì khách phải xác nhận lại. Sản phẩm ngừng bán/hết tồn phải sửa giỏ trước.
- Tạo đơn, chi tiết đơn, trừ tồn, xóa giỏ trong cùng transaction Serializable; câu SQL trừ tồn có điều kiện. Khi tranh chấp/deadlock, trả thông báo xem lại, không tự đặt bằng dữ liệu mới.
- CheckoutKey duy nhất giúp gửi lại cùng yêu cầu trả về đơn cũ. Token mới ở tab khác vẫn không thể đặt lại giỏ đã được xóa.
- Đơn mới: OrderStatus.Pending, PaymentMethod.Cod, PaymentStatus.Unpaid. Phí vận chuyển bằng 0.
- Đơn lưu riêng tên sản phẩm, ảnh, giá lúc mua và thông tin người nhận. Thay đổi sản phẩm/hồ sơ không sửa lịch sử đơn.
- Thời gian lưu UTC, hiển thị UTC+7. Lịch sử 10 đơn/trang, mới nhất trước.
- Tất cả POST đều kiểm tra antiforgery. Không có API phía khách cho thay đổi giá, tổng tiền hoặc trạng thái đơn.

## Tích hợp với Admin

Admin dùng chung `Order`, `OrderItem`, `OrderStatus`, `PaymentStatus`. Khi giao hàng thành công và đã thu COD, phần Admin cập nhật `Delivered` và `Paid`. Không tự đánh dấu Paid lúc khách đặt đơn. Nếu nhóm bổ sung hủy đơn sau này, phải quy định hoàn tồn và chặn xử lý lặp.

Module này chưa triển khai quản trị đơn, thanh toán online, hủy/hoàn tiền, mã giảm giá.

## Kiểm thử tích hợp

```powershell
dotnet build BanHangDienTu.slnx --configuration Release
dotnet run --project BanHangDienTu.IntegrationTests --configuration Release --no-build
```

Đây là executable kiểm thử, không dùng `dotnet test`. Yêu cầu tài khoản SQL có quyền tạo/xóa database. Bộ test dùng cùng SQL Server nhưng tạo database riêng `BanHangDienTu_Test_<GUID>` rồi tự xóa trong finally; không cập nhật dữ liệu ứng dụng. Có thể đặt biến `SHOP_TEST_CONNECTION` để chỉ định SQL Server kiểm thử; tên database luôn được thay bằng tên riêng.

Kiểm tra: thêm/gộp/cập nhật giỏ, tồn kho, phân quyền theo khách, sửa token, giá thay đổi, COD, gửi trùng, rollback khi thiếu hàng, hai khách cùng mua món cuối, snapshot lịch sử và pipeline HTTP thực (đăng nhập, antiforgery, validation, Razor, chuyển trang).
