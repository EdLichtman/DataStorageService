using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStorageService.Features.EmailClient
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
