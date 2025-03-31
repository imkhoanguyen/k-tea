namespace Tea.Application.DTOs.Users
{
    public class UserLoginResponse
    {
        public required string UserName { get; set; }
        public required string FullName { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime ExpiredDateAccessToken { get; set; }
    }
}
