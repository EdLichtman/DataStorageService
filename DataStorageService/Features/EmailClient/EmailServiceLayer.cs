using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStorageService.Endpoints.DataStorage;

namespace DataStorageService.Features.EmailClient
{
    public interface IEmailServiceLayer
    {
        void SendFile(string clientKey, EmailFileRequest fileRequest, string messageContent);
    }
    public class EmailServiceLayer : IEmailServiceLayer
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly IEmailService _emailService;

        public EmailServiceLayer(IEmailConfiguration emailConfiguration, IEmailService emailService)
        {
            _emailConfiguration = emailConfiguration;
            _emailService = emailService;
        }

        public void SendFile(string clientKey, EmailFileRequest fileRequest, string messageContent = null)
        {
            var toEmailAddress = "e.lichtman2@gmail.com";
            var message = new EmailMessage();
            message.FromAddresses.Add(new EmailAddress()
            {
                Address = _emailConfiguration.DefaultFromAddress,
                Name = "DataStorageService"
            });
            message.ToAddresses.Add(new EmailAddress
            {
                Address = toEmailAddress,
                Name = "AggregateDataRecipient"
            });

            message.Subject = "Import Results";
            message.Content = messageContent;
            message.AttachmentList.Add(fileRequest);

            _emailService.Send(message);
        }
    }
}
