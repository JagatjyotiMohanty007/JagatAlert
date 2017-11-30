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
    public class DayInLifeController : ApiController
    {
        string LORDEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LORDEndpointURI_CosmosDB"];
        string LORDPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LORDPrimaryKey_CosmosDB"];
        string LORD_DatabaseId = ConfigurationManager.AppSettings["DayinLife_DatabaseId"];
        string LORD_CollectionId = ConfigurationManager.AppSettings["DayinLife_CollectionId"];

        [HttpGet]
        public HttpResponseMessage GetlatlngDetails()
        {
            // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
            // Create a new instance of the DocumentClient  ok
            var client = new DocumentClient(new Uri(LORDEndpointURI_CosmosDB), LORDPrimaryKey_CosmosDB);

            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LORD_DatabaseId).AsEnumerable().FirstOrDefault();

            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LORD_CollectionId).ToArray().FirstOrDefault();


            DateTime dt1 = DateTime.UtcNow.AddDays(-2);
            DateTime dt2 = DateTime.UtcNow;

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dt1.ToUniversalTime() - origin;
            string from = Math.Floor(diff.TotalSeconds).ToString();

            DateTime origin1 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff1 = dt2.ToUniversalTime() - origin;
            string to = Math.Floor(diff1.TotalSeconds).ToString();

            //var sql = "select locoMAP.LocoID as locomotiveId from " + LORD_CollectionId;

            //var sql = "select  a.LocoID as locomotiveId from " + LORD_CollectionId + " a join b in a.messageList Where b.messageTimeInt >= " + from + " and " + "b.messageTimeInt <= " + to;
            var sql = "select  a.id as locomotiveId from " + LORD_CollectionId + " a join b in a.messageList Where b.messageTimeInt >= " + from + " and " + "b.messageTimeInt <= " + to;

            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            List<dayinlife> model_LocoEnf = new List<dayinlife>();
            foreach (var TRRlog in query)
            {
                try
                {
                    string s = Convert.ToString(TRRlog);
                    model_LocoEnf.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<dayinlife>(s));
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
        [Route("api/DayInLife/GetbyLocoID/{LococID}/{Subdiv}")]
        public HttpResponseMessage GetbyLocoID(string LococID,int Subdiv)
        {
            try
            {
                // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
                // Create a new instance of the DocumentClient  
                var client = new DocumentClient(new Uri(LORDEndpointURI_CosmosDB), LORDPrimaryKey_CosmosDB);

                // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LORD_DatabaseId).AsEnumerable().FirstOrDefault();

                // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LORD_CollectionId).ToArray().FirstOrDefault();


                 string sql= "SELECT * FROM b in locoMAP.messageList where b.locoID = "+ "\"" + LococID + "\""+ " and " + "b.headEndPTCSubdiv=" + Subdiv; 
                //string sql = "SELECT b.headEndPTCSubdiv FROM b in locoMAP.messageList where b.locoID = " + "\"" + LococID + "\"" + " and " + "b.headEndPTCSubdiv=" + Subdiv;


                var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

                var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();

                List<latlngDetails> model_Loco = new List<latlngDetails>();
                foreach (var TRRlog in query)
                {
                    string s = Convert.ToString(TRRlog);
                    model_Loco.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<latlngDetails>(s));
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_Loco);

                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(jsonString);
                return response;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        [HttpGet]
        [Route("api/DayInLife/GetsubdivbyLocoID/{LococID}")]
        public HttpResponseMessage GetsubdivbyLocoID(string LococID)
        {
            try
            {
                // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
                // Create a new instance of the DocumentClient  
                var client = new DocumentClient(new Uri(LORDEndpointURI_CosmosDB), LORDPrimaryKey_CosmosDB);

                // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LORD_DatabaseId).AsEnumerable().FirstOrDefault();

                // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LORD_CollectionId).ToArray().FirstOrDefault();
               
                string sql = "SELECT b.headEndPTCSubdiv FROM b in locoMAP.messageList where b.locoID = " + "\"" + LococID + "\"";

                var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

                var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();

                List<subdiv> model_Loco = new List<subdiv>();
                foreach (var TRRlog in query)
                {
                    string s = Convert.ToString(TRRlog);
                    model_Loco.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<subdiv>(s));
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_Loco);

                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(jsonString);
                return response;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        [HttpGet]
        [Route("api/DayInLife/GetStringlinechartdata/{from}/{to}")]
        public HttpResponseMessage GetStringlinechartdata(int from, int to)
        {
            try
            {
                // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
                // Create a new instance of the DocumentClient  
                var client = new DocumentClient(new Uri(LORDEndpointURI_CosmosDB), LORDPrimaryKey_CosmosDB);

                // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LORD_DatabaseId).AsEnumerable().FirstOrDefault();

                // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LORD_CollectionId).ToArray().FirstOrDefault();

                string sql = "SELECT a.LocoID,b.messageTime,b.messageTimeInt,b.railroadSCAC,b.headEndTrackName,b.headEndPTCSubdiv,b.headEndMilepost,b.headEndCurrentPositionLat,b.headEndCurrentPositionLon,b.directionOfTravel,b.locomotiveState from " + LORD_CollectionId + " a join b in a.messageList where  b.messageTimeInt >= "  + from +  " and " + "b.messageTimeInt <= "  + to  ;
                //string sql = "SELECT b.headEndPTCSubdiv FROM b in locoMAP.messageList where b.locoID = " + "\"" + LococID + "\"" + " and " + "b.headEndPTCSubdiv=" + Subdiv;
                

                var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

                var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();

                List<stringlineclass> model_Loco = new List<stringlineclass>();
                foreach (var TRRlog in query)
                {
                    string s = Convert.ToString(TRRlog);
                    model_Loco.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<stringlineclass>(s));
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_Loco);

                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(jsonString);
                return response;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
