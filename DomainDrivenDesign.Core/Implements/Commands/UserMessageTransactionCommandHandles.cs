using System;
using System.Linq;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;

namespace DomainDrivenDesign.Core.Implements.Commands
{
    public class UserMessageTransactionCommandHandles : ICommandHandle<SendEmailToUser>, ICommandHandle<SendSmsToUser>
        , ICommandHandle<ResendUserMessage>
    {
        public void Handle(SendEmailToUser c)
        {
            var defaultFromEmail = SendGridHelper.DefaultFromEmail;
            var tran = new UserMessageTransaction()
            {
                Id = c.MessageTransactionId,
                Content = c.Content,
                From = defaultFromEmail,
                To = c.ToEmail,
                ToName = c.ToName,
                Subject = c.Subject,
                Type = (short)Enums.UserMessageType.Email,
                Status = (short)Enums.UserMessageTransactionStatus.Created,
                ToUserId=c.UserId,
                SendDate=c.SendDate,
                CreatedDate=c.CreatedDate
            };
            using (var db = new CoreDbContext())
            {
                db.UserMessageTransactions.Add(tran);
                db.SaveChanges();
            }
        }

        public void Handle(SendSmsToUser c)
        {
            throw new NotImplementedException();
        }


        public void Handle(ResendUserMessage c)
        {
            UserMessageTransaction tran;
            using (var db = new CoreDbContext())
            {
                tran = db.UserMessageTransactions.SingleOrDefault(i => i.Id == c.MessageTransactionId);
                if (tran != null)
                {
                    tran.Status = (short)Enums.UserMessageTransactionStatus.ReSend;
                    db.SaveChanges();
                }
            }
        }
    }

    public class ResendUserMessage : AdminBaseCommand
    {
        public Guid MessageTransactionId { get; }

        public ResendUserMessage(Guid messageTransactionId, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            MessageTransactionId = messageTransactionId;
        }
    }

    public class SendSmsToUser : AdminBaseCommand
    {
        public Guid MessageTransactionId { get; private set; }
        public string ToPhone { get; private set; }
        public string Content { get; private set; }
        public DateTime SendDate { get; private set; }

        public SendSmsToUser(Guid messageTransactionId, string toPhone, string content, DateTime sendDate
            , Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            MessageTransactionId = messageTransactionId;
            ToPhone = toPhone;
            Content = content;
            SendDate = sendDate;
        }


    }

    public class SendEmailToUser : AdminBaseCommand
    {
        public Guid MessageTransactionId { get; }
        public string ToEmail { get; }
        public string ToName { get; }
        public string Subject { get; }
        public string Content { get; }
        public DateTime SendDate { get; }

        public SendEmailToUser(Guid messageTransactionId, string toEmail, string toName, string subject, string content
            , DateTime sendDate, Guid userId, DateTime createdDate) : base(userId, createdDate)
        {
            MessageTransactionId = messageTransactionId;
            ToEmail = toEmail;
            ToName = toName;
            Subject = subject;
            Content = content;
            SendDate = sendDate;
        }
    }
}