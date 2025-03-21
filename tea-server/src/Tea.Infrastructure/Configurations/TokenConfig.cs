namespace Tea.Infrastructure.Configurations
{
    public class TokenConfig
    {
        public static string ConfigName = "JWTSetting";
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpiredByMinutes { get; set; }
        public int RefreshTokenExpiredByHours { get; set; }
    }
}
