using System.Threading.Tasks;

namespace AuthorizationCenter.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
