using System.Threading.Tasks;

namespace SampleApp.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}
