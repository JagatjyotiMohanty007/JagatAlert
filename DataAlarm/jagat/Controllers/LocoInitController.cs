using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using System.Web.Http.Cors;
using Microsoft.Azure.Documents.Linq;

namespace WabtecOneAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LocoInitController : ApiController
    {
        string LocoEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEventsEndpointURI_CosmosDB"];
        string LocoEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEventsPrimaryKey_CosmosDB"];
        string LocoEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEvents_DatabaseId"];
        string LocomotiveInit_CollectionId = ConfigurationManager.AppSettings["LocomotiveInit_CollectionId"];

        [HttpGet]
        //[ActionName("GetTrainRunEvents")]
        //[Route("api/{TrainRunEvents}/{GetTrainRunEvents}")]
        public HttpResponseMessage GetLocoInit()
        {
            // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
            // Create a new instance of the DocumentClient  
            var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

            // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocomotiveInit_CollectionId).ToArray().FirstOrDefault();

            var sql = "SELECT * FROM " + LocomotiveInit_CollectionId;

            var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

            var query1 = client.CreateDocumentQuery(collection.SelfLink, sql).AsEnumerable();


            List<LocoInit> model_LocoInit = new List<LocoInit>();
            foreach (var TRRlog in query)
            {
                string s = Convert.ToString(TRRlog);
                model_LocoInit.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<LocoInit>(s));
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_LocoInit);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

        }


        //public static async Task<str>  GetLocoInit1()
        //{
        //    string continuationToken = null;

        //    var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);
        //    do
        //    {
        //        var queryable = client.CreateDocumentQuery<LocoInit>(
        //          UriFactory.CreateDocumentCollectionUri(LocoEvents_DatabaseId, LocomotiveInit_CollectionId),
        //          new FeedOptions { MaxItemCount = 1, RequestContinuation = continuationToken })
        //            .Where(x => x.group == "MRS").AsDocumentQuery();

        //        var feedResponse = await queryable.ExecuteNextAsync<Post>();
        //        continuationToken = feedResponse.ResponseContinuation;

        //        foreach (var post in feedResponse.ToList())
        //        {
        //            Console.WriteLine(post.Title);
        //        }
        //    } while (continuationToken != null);

        //    return 
        //}

        }
    }
