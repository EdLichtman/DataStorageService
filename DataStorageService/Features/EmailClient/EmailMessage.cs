using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStorageService.Endpoints.DataStorage;

namespace DataStorageService.Features.EmailClient
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            ToAddresses = new List<EmailAddress>();
            FromAddresses = new List<EmailAddress>();
            AttachmentList = new List<EmailFileRequest>();
        }

        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> FromAddresses { get; set; }
        
        public string Subject { get; set; }
        public string Content { get; set; }
        public IList<EmailFileRequest> AttachmentList { get; set; }
    }
}
