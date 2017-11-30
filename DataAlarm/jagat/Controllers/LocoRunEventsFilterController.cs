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

    [Route("api/[controller]")]
    public class LocoRunEventsFilterController : ApiController
    {
        string LocoEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEventsEndpointURI_CosmosDB"];
        string LocoEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEventsPrimaryKey_CosmosDB"];
        string LocoEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEvents_DatabaseId"];
        string LocoEvents_CollectionId = ConfigurationManager.AppSettings["LocoEvents_CollectionId"];

        [HttpGet]
       //[ActionName("GetbyDefDate")]
        [Route("api/LocoRunEventsFilter/GetbyDefDate")]
        public HttpResponseMessage GetbyDefDate()
        {

            try
            {
                //for 7 Days Data
               // string StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
                string StartDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
                string EndDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
               
                var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

                // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

                // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEvents_CollectionId).ToArray().FirstOrDefault();

                //the working copy for hardcoded date date
                //var sql = "SELECT * FROM " + TREvents_CollectionId + " Where " + TREvents_CollectionId + "."+ "initializationTime " + ">= " + " '2017-06-19 22:22:55' " + " AND " + TREvents_CollectionId + "." + "initializationTime <= " + " '2017-06-20 23:24:07' ";

                var sql = "SELECT * FROM " + LocoEvents_CollectionId + " Where " + LocoEvents_CollectionId + "." + "initializationTime " + ">= " + "'" + StartDate + "'" + " AND " + LocoEvents_CollectionId + "." + "initializationTime <= " + "'" + EndDate + "'";

                // SELECT* FROM c WHERE c.initializationTime >= "2017-06-19 22:22:55" AND c.initializationTime <= "2017-06-20 23:24:07"

                var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

              
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
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        [HttpGet]
     // [ActionName("GetbyDateRange")]
        [Route("api/LocoRunEventsFilter/GetbyDateRange/{StartDate}/{EndDate}")]
        public HttpResponseMessage GetbyDateRange(string StartDate, string EndDate)
        {
            try
            {
                //for 7 Days Data
                // string Date1 =  DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
                // string Date2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
                // Create a new instance of the DocumentClient  
                var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

                // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

                // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEvents_CollectionId).ToArray().FirstOrDefault();

                //the working copy for hardcoded date date
                //var sql = "SELECT * FROM " + TREvents_CollectionId + " Where " + TREvents_CollectionId + "."+ "initializationTime " + ">= " + " '2017-06-19 22:22:55' " + " AND " + TREvents_CollectionId + "." + "initializationTime <= " + " '2017-06-20 23:24:07' ";
                if (StartDate.Contains('@'))
                    StartDate = StartDate.Replace('@', ':');

                if (EndDate.Contains('@'))
                    EndDate = EndDate.Replace('@', ':');



                var sql = "SELECT * FROM " + LocoEvents_CollectionId + " Where " + LocoEvents_CollectionId + "." + "initializationTime " + ">= " + "'" + StartDate + "'" + " AND " + LocoEvents_CollectionId + "." + "initializationTime <= " + "'" + EndDate + "'";


                // SELECT* FROM c WHERE c.initializationTime >= "2017-06-19 22:22:55" AND c.initializationTime <= "2017-06-20 23:24:07"

                var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

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
            catch (Exception ex)
            {
                throw (ex);
            }

        }



        [HttpGet]
        [Route("api/LocoRunEventsFilter/GetbyMultiCriteria/{TrainId}/{LocoId}/{ArrLoc}/{DepLoc}")]
        public HttpResponseMessage GetbyMultiCriteria(string TrainId, int LocoId, string ArrLoc, string DepLoc)
        {
            try
            {
                // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
                // Create a new instance of the DocumentClient  
                var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

                // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

                // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEvents_CollectionId).ToArray().FirstOrDefault();

                //var sql = "SELECT * FROM " + TREvents_CollectionId;
                string sql = GetSearchQuerey(TrainId, LocoId, DepLoc, ArrLoc);

                //var sql = "SELECT * FROM " + TREvents_CollectionId + " Where " + TREvents_CollectionId + "." + "initializationTime " + ">= " + " '2017-06-19 22:22:55' " + " AND " + TREvents_CollectionId + "." + "initializationTime <= " + " '2017-06-20 23:24:07' ";

                // SELECT* FROM c WHERE c.initializationTime >= "2017-06-19 22:22:55" AND c.initializationTime <= "2017-06-20 23:24:07"

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
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public string GetSearchQuerey(string TrainId, int LocoId, string ArrLoc, string DepLoc)
        {

            string sql = "";
            sql = "SELECT * FROM " + LocoEvents_CollectionId + " Where ";

            try
            {
                if (TrainId != "0")
                {
                    sql = sql + LocoEvents_CollectionId + "." + "trainId " + "=" + "'" + TrainId + "'" + " AND ";
                }

                if (LocoId != 0)
                {
                    sql = sql + LocoEvents_CollectionId + "." + "locomotiveId " + "=" + LocoId + " AND ";
                }

                if (DepLoc != "0")
                {
                    sql = sql + LocoEvents_CollectionId + "." + "departureLocation " + "=" + "'" + DepLoc + "'" + " AND ";
                }

                if (ArrLoc != "0")
                {
                    sql = sql + LocoEvents_CollectionId + "." + "arrivalLocation " + "=" + "'" + ArrLoc + "'" + " AND ";
                }

                if (ArrLoc != "0")
                {
                    sql = sql + LocoEvents_CollectionId + "." + "locomotiveId " + "=" + "'" + LocoId + "'" + " AND ";
                }
             
                sql = sql.Substring(0, sql.Length - 5);

                return sql;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }



        [HttpGet]
        [Route("api/LocoRunEventsFilter/GetbyMultiCriteria/{TrainId}/{LocoId}/{ArrLoc}/{DepLoc}/{StartDate}/{EndDate}")]
        public HttpResponseMessage GetbyMultiCriteria(string TrainId, int LocoId, string ArrLoc, string DepLoc, string StartDate, string EndDate)
        {
            try
            {
                // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
                // Create a new instance of the DocumentClient  
                var client = new DocumentClient(new Uri(LocoEventsEndpointURI_CosmosDB), LocoEventsPrimaryKey_CosmosDB);

                // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
                var database = client.CreateDatabaseQuery().Where(db => db.Id == LocoEvents_DatabaseId).AsEnumerable().FirstOrDefault();

                // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
                var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == LocoEvents_CollectionId).ToArray().FirstOrDefault();

                //var sql = "SELECT * FROM " + TREvents_CollectionId;



                string sql = GetSearchQuerey(TrainId, LocoId, DepLoc, ArrLoc, StartDate, EndDate);

                //var sql = "SELECT * FROM " + TREvents_CollectionId + " Where " + TREvents_CollectionId + "." + "initializationTime " + ">= " + " '2017-06-19 22:22:55' " + " AND " + TREvents_CollectionId + "." + "initializationTime <= " + " '2017-06-20 23:24:07' ";

                // SELECT* FROM c WHERE c.initializationTime >= "2017-06-19 22:22:55" AND c.initializationTime <= "2017-06-20 23:24:07"

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
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public string GetSearchQuerey(string TrainId, int LocoId, string ArrLoc, string DepLoc, string StartDate, string EndDate)
        {
           
            string sql = "";
            sql = "SELECT * FROM " + LocoEvents_CollectionId + " WHERE ";

            try
            {
                if (TrainId != "0")
                {
                    sql = sql + LocoEvents_CollectionId + "." + "trainId " + "=" + "'" + TrainId + "'" + " AND ";
                }

                if (LocoId != 0)
                {
                    //sql = sql + TREvents_CollectionId + "." + "locomotiveId " + "=" + LocoId + " AND "; // for int
                    sql = sql +  LocoEvents_CollectionId + "." + "locomotiveId" + "=" + "'" + LocoId + "'" + " AND ";
                    //sql = sql + "CONTAINS "  + "(" + LocoEvents_CollectionId + "." + "locomotiveId," + "'" + LocoId + "'" + ")" + " AND ";
                    //SELECT* FROM c WHERE CONTAINS(c.locomotiveIds, '7335')

                }

                if (DepLoc != "0")
                {
                    sql = sql + LocoEvents_CollectionId + "." + "departureLocation " + "=" + "'" + DepLoc + "'" + " AND ";
                }

                if (ArrLoc != "0")
                {
                    sql = sql + LocoEvents_CollectionId + "." + "arrivalLocation " + "=" + "'" + ArrLoc + "'" + " AND ";
                }

              
                if (StartDate != "0" && EndDate != "0")
                {
                    if (StartDate.Contains('@'))
                        StartDate = StartDate.Replace('@', ':');

                    if (EndDate.Contains('@'))
                        EndDate = EndDate.Replace('@', ':');

                    sql = sql + LocoEvents_CollectionId + "." + "initializationTime " + ">= " + "'" + StartDate + "'" + " AND " + LocoEvents_CollectionId + "." + "initializationTime <= " + "'" + EndDate + "'" + " AND ";

                }
                if (StartDate != "0" && EndDate == "0")
                {
                    if (StartDate.Contains('@'))
                        StartDate=StartDate.Replace('@', ':');
                       //sql = sql + LocoEvents_CollectionId + "." + "initializationTime " + "= " + "'" + StartDate + "'" + " AND ";
                    sql = sql + "CONTAINS " + "(" + LocoEvents_CollectionId + "." + "initializationTime" + ", "+ "'" + StartDate + "'" + ")" + " AND ";
                }

                sql = sql.Substring(0, sql.Length - 5);

                return sql;
            }   
            catch (Exception ex)
            {
                throw (ex);
            }
        }

    }
}
