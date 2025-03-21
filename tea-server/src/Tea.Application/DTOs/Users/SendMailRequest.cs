namespace Tea.Application.DTOs.Users
{
    public class SendMailRequest
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Content { get; set; }
    }
}
