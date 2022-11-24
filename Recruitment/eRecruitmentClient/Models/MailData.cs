using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRecruitmentClient.Models
{
    public class MailData
    {
        // Receiver
        public List<string> To { get; }
        public List<string> Bcc { get; }

        public List<string> Cc { get; }

        // Sender
        public string? From { get; }

        public string? DisplayName { get; }

        public string? ReplyTo { get; }

        public string? ReplyToName { get; }

        // Content
        [Required(ErrorMessage = "Please input mail's subject")]
        public string Subject { get; }

        [Required(ErrorMessage = "Please input mail's title")]
        public string? Body { get; }

        public MailData()
        {
            To = new List<string>();
            Bcc = new List<string>();
            Cc = new List<string>();
        }

        public MailData(string subject, string body, string to, string from)
        {
            To = new List<string>();
            To.Add(to);
            Bcc = new List<string>();
            Cc = new List<string>();
            Subject = subject;
            Body = body;
            From = from;
        }

        public MailData(List<string> to, string subject, string? body = null, string? from = null, string? displayName = null, string? replyTo = null, string? replyToName = null, List<string>? bcc = null, List<string>? cc = null)
        {
            // Receiver
            To = to;
            Bcc = bcc ?? new List<string>();
            Cc = cc ?? new List<string>();

            // Sender
            From = from;
            DisplayName = displayName;
            ReplyTo = replyTo;
            ReplyToName = replyToName;

            // Content
            Subject = subject;
            Body = body;
        }
    }
}
