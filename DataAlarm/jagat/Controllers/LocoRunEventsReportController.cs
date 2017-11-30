using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Microsoft.Azure.Documents.Client;

using System.Web.Http.Cors;

namespace WabtecOneAPI.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LocoRunEventsReportController : ApiController
    {
        string LocoEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEventsEndpointURI_CosmosDB"];
        string LocoEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEventsPrimaryKey_CosmosDB"];
        string LocoEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEvents_DatabaseId"];
        string LocoEvents_CollectionId = ConfigurationManager.AppSettings["LocoEvents_CollectionId"];

        public HttpResponseMessage GetLocoRunEvents()
        {
            // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
            // Create a new instance of the DocumentClient  
            var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEvents_CollectionId).ToArray().FirstOrDefault();

            var sql = "SELECT * FROM " + LocoEvents_CollectionId;

            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();

            List<Loco> model_Loco = new List<Loco>();
            foreach (var TRRlog in query)
            {
                string s = Convert.ToString(TRRlog);
                model_Loco.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Loco>(s));
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_Loco);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

        }

    }


}
