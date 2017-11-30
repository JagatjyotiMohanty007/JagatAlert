using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using System.Web.Http.Cors;
namespace WabtecOneAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EnforcementDashboardController : ApiController
    {

        string LORDEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LORDEndpointURI_CosmosDB"];
        string LORDPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LORDPrimaryKey_CosmosDB"];
        string LocoEnfEvents_DatabaseId = ConfigurationManager.AppSettings["DayinLife_DatabaseId"];
        string LocoEnfEvents_CollectionId = ConfigurationManager.AppSettings["EnforcementDashboard_CollectionId"];

        [HttpGet]

        [Route("api/EnforcementDashboard/GetAllLocoEnfData")]

        public HttpResponseMessage GetAllLocoEnfData()
        {
            HttpResponseMessage response = null;
            try
            {

                var client = new DocumentClient(new Uri(LORDEndpointURI_CosmosDB), LORDPrimaryKey_CosmosDB);
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEnfEvents_DatabaseId).AsEnumerable().FirstOrDefault();
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEnfEvents_CollectionId).ToArray().FirstOrDefault();
                // var sql = GetSQL();  

                //Previous 24hours Data   
                string dt1 = DateTime.Now.AddDays(-10).ToString("o");
                string dt2 = DateTime.Now.ToString("o"); 

                var sql = "SELECT * FROM b in " + LocoEnfEvents_CollectionId + ".messageList where b.warningEnforcement =\"Enforcement\" and (b.enforcement_date_time >= " + "\"" + dt1 + "\"" + " and " + "b.enforcement_date_time <= " + "\"" + dt2 + "\"" + ")";

                var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

                // for 30 Days Data
                DateTime dt3 = DateTime.Now.AddDays(-30);
                DateTime dt4 = DateTime.Now;

                DateTime dt5 = DateTime.UtcNow.AddDays(-30);
                DateTime dt6 = DateTime.UtcNow;

                string dt7 = DateTime.UtcNow.AddDays(-30).ToString("o");
                string dt8 = DateTime.UtcNow.ToString("o");

                //"Msglogtime": "2017-08-13T04:42:35.0000000Z",

                //string Date1 = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
                //string Date2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string Date1 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                string Date2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



                //var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();
                List<LocoEnforcement_Dash> model_LocoEnf = new List<LocoEnforcement_Dash>();
                foreach (var TRRlog in query)
                {
                    string s = Convert.ToString(TRRlog);
                    model_LocoEnf.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<LocoEnforcement_Dash>(s));
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_LocoEnf);
                // HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpResponseMessage();
                response.Content = new StringContent(jsonString);

            }

            catch (Exception ex)
            {

            }
            return response;
        }
    }
}