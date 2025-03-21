using Tea.Application.DTOs.Users;

namespace Tea.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendMailAsync(CancellationToken cancellationToken, SendMailRequest request);
    }
}