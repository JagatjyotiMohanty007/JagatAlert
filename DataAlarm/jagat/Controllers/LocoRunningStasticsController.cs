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
using System.Web.Script.Serialization;

namespace WabtecOneAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    //[Route("api/[controller]")]
    public class LocoRunningStasticsController : ApiController
    {
        string LocoEventsEndpointURI_CosmosDB = ConfigurationManager.AppSettings["LocoEventsEndpointURI_CosmosDB"];
        string LocoEventsPrimaryKey_CosmosDB = ConfigurationManager.AppSettings["LocoEventsPrimaryKey_CosmosDB"];
        string LocoEvents_DatabaseId = ConfigurationManager.AppSettings["LocoEvents_DatabaseId"];
        string LocoEvents_CollectionId = ConfigurationManager.AppSettings["LocoEvents_CollectionId"];

        [HttpGet]
        [RequireHttps]      
        public HttpResponseMessage GetLocoRunningStastics()
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

            List<Loco> model_Loco = new List<Loco>();
            foreach (var TRRlog in query)
            {
                string s = Convert.ToString(TRRlog);
                model_Loco.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Loco>(s));
            }

            //var highScores = from student in students
            //            where student.ExamScores[exam] > score
            //            select new {Name = student.FirstName, Score = student.ExamScores[exam]};

            var query1 = model_Loco.GroupBy(u => u.initializationTime).Select(grp => grp.ToList()).ToList();

            List<TRRdashclass> model_TRR1 = new List<TRRdashclass>();

            TRRdashclass obj = new TRRdashclass();

            foreach (var TRRlog1 in query1)
            {
                int cnt = TRRlog1.Count();
                obj.trainIdCnt = cnt;

                foreach (var TRRlog2 in TRRlog1)
                {
                    string Date = TRRlog2.initializationTime;
                    obj.initializationTime = TRRlog2.initializationTime;
                    break;
                }

                var json = new JavaScriptSerializer().Serialize(obj);
                string s = json.ToString();
                model_TRR1.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<TRRdashclass>(s));
                // model_TRR1.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<TRR1>(s));
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_TRR1);

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(jsonString);
            return response;

            //var highScores1 = from s in model_TRR1 select new { Date = s.initializationTime};
            //var TrainRunStatistics_list1 = 

            //   var x  = from n in TrainRunStatistics_list  select count(Id) n;
            //                  where n.DisplayType=="Special Book" 
            //                  select n;
            //int num = specialBook.Count();1
            //shareimprove this answer
            //edited

            //var specialBook = from n in TrainRunStatistics_list  select n;
            //int num = specialBook.Count();

            //           SELECT COUNT(Id), Country 
            // FROM Customer
            //GROUP BY Country
        }


        //[HttpGet]
        ////[Route("api/GetTRStastics")]   
        //public HttpResponseMessage GetTRStastics()
        //{
        //    // Make sure to call client.Dispose() once you've finished all DocumentDB interactions  
        //    // Create a new instance of the DocumentClient  
        //    var client = new DocumentClient(new Uri(TREventsEndpointURI_CosmosDB), TREventsPrimaryKey_CosmosDB);

        //    // Try to retrieve the database (Microsoft.Azure.Documents.Database) whose Id is equal to databaseId            
        //    var database = client.CreateDatabaseQuery().Where(db => db.Id == TREvents_DatabaseId).AsEnumerable().FirstOrDefault();

        //    // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
        //    var collection = client.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == TREvents_CollectionId).ToArray().FirstOrDefault();

        //    var sql = "SELECT * FROM " + TREvents_CollectionId;

        //    var query = client.CreateDocumentQuery(collection.SelfLink, sql).AsQueryable();

        //    List<TRR> model_TRR = new List<TRR>();
        //    foreach (var TRRlog in query)
        //    {
        //        string s = Convert.ToString(TRRlog);
        //        model_TRR.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<TRR>(s));
        //    }

        //    //var highScores = from student in students
        //    //            where student.ExamScores[exam] > score
        //    //            select new {Name = student.FirstName, Score = student.ExamScores[exam]};

        //    var query1 = model_TRR.GroupBy(u => u.initializationTime).Select(grp => grp.ToList()).ToList();

        //    List<TRRdashclass> model_TRR1 = new List<TRRdashclass>();

        //    TRRdashclass obj = new TRRdashclass();

        //    foreach (var TRRlog1 in query1)
        //    {
        //        int cnt = TRRlog1.Count();
        //        obj.trainIdCnt = cnt;

        //        foreach (var TRRlog2 in TRRlog1)
        //        {
        //            string Date = TRRlog2.initializationTime;
        //            obj.initializationTime = TRRlog2.initializationTime;
        //            break;
        //        }

        //        var json = new JavaScriptSerializer().Serialize(obj);
        //        string s = json.ToString();
        //        model_TRR1.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<TRRdashclass>(s));
        //        // model_TRR1.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<TRR1>(s));
        //    }
        //    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model_TRR1);

        //    HttpResponseMessage response = new HttpResponseMessage();
        //    response.Content = new StringContent(jsonString);
        //    return response;

           
        //}

        public class TRRdashclass //old structure
       {
           public int trainIdCnt { get; set; }
           public string initializationTime { get; set; }
        }




    }

    internal class RequireHttpsAttribute : Attribute
    {
    }
}
