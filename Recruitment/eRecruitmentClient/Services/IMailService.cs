using System.Threading.Tasks;
using System.Threading;
using eRecruitmentClient.Models;

namespace eRecruitmentClient.Services
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}
