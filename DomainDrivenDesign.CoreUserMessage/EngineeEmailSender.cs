using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;

namespace DomainDrivenDesign.CoreUserMessage
{
    public static class EngineeEmailSender
    {
        private static Thread _thread;

        static EngineeEmailSender()
        {
            _thread = new Thread(() =>
              {
                  Loop();
              });
            _thread.Start();
        }

        private static void Loop()
        {
            while (true)
            {
                try
                {
                    List<UserMessageTransaction> emails = new List<UserMessageTransaction>();
                    var created = (short)Enums.UserMessageTransactionStatus.Created;
                    var resend = (short)Enums.UserMessageTransactionStatus.ReSend;
                    var typeEmail = (short)Enums.UserMessageType.Email;
                    using (var db = new CoreDbContext())
                    {
                        emails = db.UserMessageTransactions.Where(i => (i.Status == created || i.Status == resend)
                        && i.Type == typeEmail).ToList();
                    }

                    foreach (var email in emails)
                    {
                        TrySendEmailBySendGrid(email);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.GetMessages());
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public static void Init()
        {

        }

        static void TrySendEmailBySendGrid(UserMessageTransaction tran)
        {
            Task.Run(() =>
            {
                var dtNow = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": ";
                try
                {
                    var toEmails = new List<SendGridRequest.Email>() { new SendGridRequest.Email() { email = tran.To, name = tran.ToName } };
                    var fromEmail = new SendGridRequest.Email() { email = tran.From };
                    var res = SendGridHelper.SendEmail(toEmails, tran.Subject, tran.Content, fromEmail);
                    if (res.StatusCode == HttpStatusCode.Accepted)
                    {
                        UpdateTranStatus(tran.Id, Enums.UserMessageTransactionStatus.Sent, $"{dtNow}Success");
                    }
                    else
                    {
                        UpdateTranStatus(tran.Id, Enums.UserMessageTransactionStatus.Fail, dtNow + res.StatusCode+" " + string.Join(" ", res.errors));
                    }

                }
                catch (Exception ex)
                {
                    UpdateTranStatus(tran.Id, Enums.UserMessageTransactionStatus.Fail, dtNow + ex.GetMessages());
                }
            });
        }


        static void UpdateTranStatus(Guid id, Enums.UserMessageTransactionStatus status, string error)
        {
            using (var db = new CoreDbContext())
            {
                var tran = db.UserMessageTransactions.SingleOrDefault(i => i.Id == id);
                if (tran != null)
                {
                    tran.Status = (short)status;
                    tran.Error = tran.Error + " " + error;
                    db.SaveChanges();
                }
            }
        }


    }
}
