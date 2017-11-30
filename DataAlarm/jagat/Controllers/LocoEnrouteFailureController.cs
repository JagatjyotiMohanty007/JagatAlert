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
    public class LocoEnrouteFailureController : ApiController
    {
        string LocoEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEventsEndpointURI_CosmosDB"];
        string LocoEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEventsPrimaryKey_CosmosDB"];
        string LocoEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEvents_DatabaseId"];
        string Locomotiveenroute_CollectionId = ConfigurationManager.AppSettings["Locomotiveenroute_CollectionId"];

        [HttpGet]
        //[ActionName("GetTrainRunEvents")]
        //[Route("api/{TrainRunEvents}/{GetTrainRunEvents}")]
        public HttpResponseMessage GetLocoEnroutefailure()
        {
            // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
            // Create a new instance of the DocumentClient  
            var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == Locomotiveenroute_CollectionId).ToArray().FirstOrDefault();

            var sql = "SELECT * FROM " + Locomotiveenroute_CollectionId;

            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();


            List<LocoEnrouteFail> model_LocoEnfail = new List<LocoEnrouteFail>();
            foreach (var TRRlog in query)
            {
                string s = Convert.ToString(TRRlog);
                model_LocoEnfail.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<LocoEnrouteFail>(s));
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_LocoEnfail);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

        }
    }
}
