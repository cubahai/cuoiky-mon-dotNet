namespace BanHangDienTu.Models.Constants;

public static class PaymentMethodConstants
{
    public const string Cod = "COD";
    public const string BankTransfer = "BankTransfer";

    public static string ToDisplayName(string? method) => method switch
    {
        BankTransfer => "Chuyển khoản ngân hàng",
        _ => "Thanh toán khi nhận hàng (COD)"
    };
}
