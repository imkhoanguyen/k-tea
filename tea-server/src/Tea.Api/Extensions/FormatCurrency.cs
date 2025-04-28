using CloudinaryDotNet;

namespace Tea.Api.Extensions
{
    public static class FormatCurrency
    {
        public static string ToVndCurrency(this decimal price)
        {
            return price.ToString("#,##0") + " VNĐ";
        }
    }
}
