using System;

namespace MailSender
{
    [Serializable]
    public class MailXml
    {
        public string From { get; set; }

        public string FromEmail { get; set; }

        public string To { get; set; }

        public string ToEmail { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}
