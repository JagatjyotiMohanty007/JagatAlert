using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using System.Web.Http.Cors;
using System.Net.Mail;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace WabtecOneAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SendGridController : ApiController
    {
        string sendgridkey = ConfigurationManager.AppSettings["sendgridkey"];

        [HttpGet]
        [Route("api/SendGrid/SendMail/{msg}")]

        public void SendMail(string msg)
        {
            try
            {

                var client = new SendGridClient(sendgridkey);
                var msg1 = new SendGridMessage()
                {
                    From = new EmailAddress("wabtecone@gmail.com", " WabtecOne"),
                    Subject = "Notification from the WabtecOne!",
                    PlainTextContent = msg,
                    HtmlContent = msg
                };
                msg1.AddTo(new EmailAddress("wabtecone@gmail.com", "SendGrid Test"));

                var response = client.SendEmailAsync(msg1).Result;

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
    }
}
