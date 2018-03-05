using System.Threading.Tasks;

namespace AuthorizationCenter.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
