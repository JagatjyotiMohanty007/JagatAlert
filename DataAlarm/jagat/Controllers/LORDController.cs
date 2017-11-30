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
    public class LORDController : ApiController
    {
        string LocoEnfEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEnfEventsEndpointURI_CosmosDB"];
        string LocoEnfEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEnfEventsPrimaryKey_CosmosDB"];
        string LocoEnfEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEnfEvent_DatabaseId"];
        string LocoEnfEvents_CollectionId = ConfigurationManager.AppSettings["LocoEnfEvent_collectionID"];

        [HttpGet]

        //[Route("api/LORD/GetAllLocoEnfData")]
        public HttpResponseMessage GetAllLocoEnfData()
        {


            var client = new DocumentClient(new Uri(LocoEnfEventsEndpointURI_CosmosDB), LocoEnfEventsPrimaryKey_CosmosDB);
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEnfEvents_DatabaseId).AsEnumerable().FirstOrDefault();
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEnfEvents_CollectionId).ToArray().FirstOrDefault();
            // var sql = GetSQL();  

            //Previous 24hours Data   
            //string dt1 = DateTime.UtcNow.AddDays(-1).ToString("o");
            DateTime dt1 = DateTime.UtcNow.AddDays(-30);

            //string dt2 = DateTime.UtcNow.ToString("o");
            DateTime dt2 = DateTime.UtcNow;

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dt1.ToUniversalTime() - origin;
           string from= Math.Floor(diff.TotalSeconds).ToString();

            DateTime origin1 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff1 = dt2.ToUniversalTime() - origin;
            string to = Math.Floor(diff1.TotalSeconds).ToString();           

          var sql= "SELECT a.LocoID as locomotiveId,b.Warning_Enforcement_Type as enforcementtype,b.target_Description as enforcementtarget from " + LocoEnfEvents_CollectionId + " a join b in a.messageList where  b.timeValue >= " + from + " and " + "b.timeValue <= " + to;


            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            List<enfreal> model_LocoEnf = new List<enfreal>();
            foreach (var TRRlog in query)
            {
                try
                {
                    string s = Convert.ToString(TRRlog);
                    model_LocoEnf.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<enfreal>(s));
                }
                catch (Exception ex)
                {

                }

            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_LocoEnf);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

        }

        [HttpGet]
        [Route("api/LORD/Getenforcementdata")]
        public HttpResponseMessage Getenforcementdata()
        {

            var client = new DocumentClient(new Uri(LocoEnfEventsEndpointURI_CosmosDB), LocoEnfEventsPrimaryKey_CosmosDB);
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEnfEvents_DatabaseId).AsEnumerable().FirstOrDefault();
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEnfEvents_CollectionId).ToArray().FirstOrDefault();

            DateTime dt1 = DateTime.UtcNow.AddDays(-3);
            DateTime dt2 = DateTime.UtcNow;

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dt1.ToUniversalTime() - origin;
            string from = Math.Floor(diff.TotalSeconds).ToString();

            DateTime origin1 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff1 = dt2.ToUniversalTime() - origin;
            string to = Math.Floor(diff1.TotalSeconds).ToString();

            var sql = "SELECT b.Warning_Enforcement_Type as enforcementtype,b.Enforcement_Lat,b.Enforcement_Lon from " + LocoEnfEvents_CollectionId + " a join b in a.messageList where  b.timeValue >= " + from + " and " + "b.timeValue <= " + to;

            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            //var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();

            List<enfreallatlng> model_Loco = new List<enfreallatlng>();
            foreach (var TRRlog in query)
            {
                string s = Convert.ToString(TRRlog);
                model_Loco.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<enfreallatlng>(s));
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_Loco);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

        }
    }
}

























