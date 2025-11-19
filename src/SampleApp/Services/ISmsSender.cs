using System.Threading.Tasks;

namespace SampleApp.Services;

public interface ISmsSender
{
    Task SendSmsAsync(string number, string message);
}
