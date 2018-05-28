using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DomainDrivenDesign.Core.Utils
{
    public class SendGridHelper
    {
        private static readonly string ApiUrl = ConfigurationManager.AppSettings["SendGridApiUrl"] ??
                                       "https://api.sendgrid.com/v3/mail/send";

        private static readonly string ApiKey = ConfigurationManager.AppSettings["SendGridApiKey"] ??
                                                   "";

        public static readonly string DefaultFromEmail = ConfigurationManager.AppSettings["SendGridDefaultFromEmail"] ??
                                                          "no-reply@badpaybad.info";

        public static SendGridResponse SendEmail(List<SendGridRequest.Email> toEmails, string subject, string content, SendGridRequest.Email fromEmail = null, bool allowHtml = true)
        {
            var request = new SendGridRequest();
            request.subject = subject;
            request.content.Add(new SendGridRequest.Content()
            {
                value = content,
                type = allowHtml ? "text/html" : "text/plain"
            });

            foreach (var email in toEmails)
            {
                var p = new SendGridRequest.Personalizations();
                p.to.Add(email);
                p.subject = subject;

                request.personalizations.Add(p);
            }

            if (fromEmail == null)
            {
                request.@from.email = DefaultFromEmail;
            }
            else
            {
                request.@from.email = fromEmail.email;
                request.@from.name = fromEmail.name;
            }

            using (var wc = new HttpClient())
            {
                wc.BaseAddress = new Uri("https://api.sendgrid.com");
                wc.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {ApiKey}");
                wc.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                wc.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "sendgrid/custom csharp");
                wc.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var res = wc.PostAsync(ApiUrl,
                      new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")).Result;

                if (res.StatusCode == HttpStatusCode.Accepted)
                {
                    return new SendGridResponse() {StatusCode= HttpStatusCode.Accepted };
                }

                var resContent = res.Content.ReadAsStringAsync().Result;

                var sendGridResponse = JsonConvert.DeserializeObject<SendGridResponse>(resContent);
                sendGridResponse.StatusCode = res.StatusCode;
                return sendGridResponse;
            }
        }

    }

    public class SendGridRequest
    {
        public List<Personalizations> personalizations = new List<Personalizations>();

        public Email from = new Email();

        public string subject;

        public List<Content> content = new List<Content>();

        public class Personalizations
        {
            public List<Email> to = new List<Email>();
            public string subject;
        }
        public class Content
        {
            public string type = "text/html";
            public string value;
        }

        public class Email
        {
            public string name = string.Empty;
            public string email = string.Empty;
        }
    }

    public class SendGridResponse
    {
        public System.Net.HttpStatusCode StatusCode;
        public string StatusCodeText { get { return StatusCode.ToString(); } }
        public List<Error> errors=new List<Error>();

        public class Error
        {
            public string field;
            public string message;
        }
    }
}
