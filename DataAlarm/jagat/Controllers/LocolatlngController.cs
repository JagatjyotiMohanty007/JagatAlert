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
    public class LocolatlngController : ApiController
    {
        string LocoEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEventsEndpointURI_CosmosDB"];
        string LocoEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEventsPrimaryKey_CosmosDB"];
        string LocoEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEvents_DatabaseId"];
        string Locolatlng_CollectionId = ConfigurationManager.AppSettings["Locolatlng_CollectionId"];

//        Select a.LocoID as LocoID, b.messageTime as Msglogtime, b.messageType as messagetype
//FROM MRS_UPDATE a JOIN b IN a.messageList WHERE b.messageType = 2083

        [HttpGet]
        
        public HttpResponseMessage GetLocolatlng()
        {
            // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
            // Create a new instance of the DocumentClient  
            var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == Locolatlng_CollectionId).ToArray().FirstOrDefault();

            var sql = "SELECT * FROM " + Locolatlng_CollectionId;

            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();


            List<Locolatlng> model_LocoEnf = new List<Locolatlng>();
            foreach (var TRRlog in query)
            {
                string s = Convert.ToString(TRRlog);
                model_LocoEnf.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Locolatlng>(s));
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_LocoEnf);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

        }
    }
}
