using ABP.Core.Application.Dtos.Email;

namespace ABP.Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}
