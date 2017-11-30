using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Microsoft.Azure.Documents.Client;

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http.Cors;
namespace WabtecOneAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LocoRunEventsController : ApiController
    {
        string LocoEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEventsEndpointURI_CosmosDB"];
        string LocoEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEventsPrimaryKey_CosmosDB"];
        string LocoEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEvents_DatabaseId"];
        string LocoEvents_CollectionId = ConfigurationManager.AppSettings["LocoEvents_CollectionId"];


        [HttpGet]
        //[ActionName("GetTrainRunEvents")]
        //[Route("api/{TrainRunEvents}/{GetTrainRunEvents}")]
        public HttpResponseMessage GetLocoRunEvents()
        {
             // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
            // Create a new instance of the DocumentClient  
            var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEvents_CollectionId).ToArray().FirstOrDefault();

            var sql = "SELECT * FROM "+ LocoEvents_CollectionId;

            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();

            //JsonResult v = Json(query);
            //JsonResult v1 = Json(query1);
            //JObject jsonresult = JObject.Parse();

            List<Loco> model_TRR = new List<Loco>();
            foreach (var TRRlog in query)
            {
                string s = Convert.ToString(TRRlog);
                model_TRR.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Loco>(s));
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_TRR);
            //JObject json = JObject.Parse(jsonString);
            //JArray json = JArray.Parse(jsonString);
            //return json.ToString();

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

        }

        //[HttpGet]
        //public HttpResponseMessage GetTrainRunEvents()  // Hierarchical Structured
        //{
        //    //return "Manasa";

        //    // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
        //    // Create a new instance of the DocumentClient  
        //    var client = new DocumentClient(new Uri(TREventsEndpointURI_CosmosDB), TREventsPrimaryKey_CosmosDB);

        //    // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
        //    var database = client.CreateDatabaseQuery().Where(db => db.Id == TREvents_DatabaseId).AsEnumerable().FirstOrDefault();

        //    // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
        //    var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == TREvents_CollectionId).ToArray().FirstOrDefault();

        //    var sql = "SELECT * FROM " + TREvents_CollectionId;
        //    var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

        //    List<TRRRootChild> model_TRRRootChild = new List<TRRRootChild>();
        //    foreach (var TRRlog in query)
        //    {
        //        string s = Convert.ToString(TRRlog);
        //        model_TRRRootChild.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<TRRRootChild>(s));
        //    }
        //    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_TRRRootChild);

        //    //JArray json = JArray.Parse(jsonString);

        //    HttpResponseMessage response = new HttpResponseMessage();
        //    response.Content = new StringContent(jsonString);
        //    return response;
        //}

    }

    public class BrowserJsonFormatter : JsonMediaTypeFormatter
    {
        public BrowserJsonFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            this.SerializerSettings.Formatting = Formatting.Indented;
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
    public class LocoRootChild
    {
        public string trainId { get; set; }
        public List<Locomotive> locomotives { get; set; }
        public int departureLocation { get; set; }
        public string initializationTime { get; set; }
        public string departureTime { get; set; }
        public int delay { get; set; }
        public int arrivalLocation { get; set; }
        public string arrivalTime { get; set; }
        public int activeTime { get; set; }
       
    }
    public class Locomotive
    {
        public int locomotiveId { get; set; }
        public string onboardSoftwareVersion { get; set; }
        public string initializationTime { get; set; }
    }


}
