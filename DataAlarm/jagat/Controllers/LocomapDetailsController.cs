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
    public class LocomapDetailsController : ApiController
    {
        string LORDEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LORDEndpointURI_CosmosDB"];
        string LORDPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LORDPrimaryKey_CosmosDB"];
        string LORD_DatabaseId = ConfigurationManager.AppSettings["DayinLife_DatabaseId"];
        string LORD_CollectionId = ConfigurationManager.AppSettings["DayinLife_CollectionId"];

        [HttpGet]
        public HttpResponseMessage GetbyLocoID(string LococID)
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

                //var sql = "SELECT * FROM " + TREvents_CollectionId;
                // string sql = GetSearchQuerey(TrainId, LocoId, DepLoc, ArrLoc);

                string sql = "select a.LocoID as locomotiveId, b.messageTime , b.railroadSCAC,b.headEndTrackName,b.headEndPTCSubdiv,b.headEndMilepost,b.headEndCurrentPositionLat,b.headEndCurrentPositionLon,b.directionOfTravel,b.locomotiveState FROM " + LORD_CollectionId + " a JOIN b IN a.messageList where a.LocoID=" + LococID;

                //var sql = "SELECT * FROM " + TREvents_CollectionId + " Where " + TREvents_CollectionId + "." + "initializationTime " + ">= " + " '2017-06-19 22:22:55' " + " AND " + TREvents_CollectionId + "." + "initializationTime <= " + " '2017-06-20 23:24:07' ";

                // SELECT* FROM c WHERE c.initializationTime >= "2017-06-19 22:22:55" AND c.initializationTime <= "2017-06-20 23:24:07"

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
    }
}
