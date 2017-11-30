using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using System.Web.Http.Cors;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;


namespace WabtecOneAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TwillioController : ApiController
    {
        string twilioid = ConfigurationManager.AppSettings["twilioid"];
        string twiliotoken = ConfigurationManager.AppSettings["twiliotoken"];

        [HttpGet]
        // [ActionName("GetbyDateRange")]
        [Route("api/Twillio/SendTextMsg/{msg}")]
        public void SendTextMsg(string msg)
        {
            try
            {

                TwilioClient.Init(twilioid, twiliotoken);
                var to = new PhoneNumber("+919448123146");
                var message = MessageResource.Create(
                    to,
                    from: new PhoneNumber("+14159686725"),
                    body: msg
                   );
                Console.WriteLine(message.Sid);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        [HttpPost]
        // [ActionName("GetbyDateRange")]
        [Route("api/Twillio/Receiveacknowledgement")]
        public MessagingResponse Receiveacknowledgement(string From, string Body)
        {
            try
            {

                // const string requestBody = Request.Form["Body"];
                var response = new MessagingResponse();
                return response.Message($"Hello {From}. You said {Body}");

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

    }
}

  