using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace WabtecOneAPI.Controllers
{
    public class timeList
    {
        public String tme { get; set; }
        public int count { get; set; }
    }
    public class srctimeList
    {
        public String tme { get; set; }
        public String src { get; set; }
        public int count { get; set; }
    }
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TimeseriesController : ApiController
    {
        static List<timeList> agdata = new List<timeList>();
        static List<srctimeList> srcdata = new List<srctimeList>();
        private static string ApplicationClientId = "5894dfb1-4728-40e9-90b3-d48cc48dc43d";

        // SET the application key of the application registered in your Azure Active Directory
        private static string ApplicationClientSecret = "BTONsdOiNxV5Z3GYqzQaylEocweZZLdpQl3dpJeOxRo=";
        private static DateTime fromAvailabilityTimestamp;
        private static DateTime toAvailabilityTimestamp;
        private static DateTime fromAvailabilityTimestamp1;
        private static DateTime toAvailabilityTimestamp1;
        private static string accessToken;
        private static string environmentFqdn;
        [HttpGet]
        [Route("api/Timeseries/GetTimeseriesdata")]
        public HttpResponseMessage GetTimeseriesdata()
        {
            Task.Run(async () => await SampleAsync()).Wait();
            if (accessToken != null)
            {
                try
                {
                    //fromAvailabilityTimestamp = new DateTime(2017, 7, 6).ToUniversalTime();
                    //toAvailabilityTimestamp = new DateTime(2017, 7, 7).ToUniversalTime();
                    agdata.Clear();
                    Task.Run(async () => await getgeneralts(fromAvailabilityTimestamp, toAvailabilityTimestamp, accessToken, environmentFqdn)).Wait();

                }
                catch (Exception e)
                {
                    return null;
                }
            }
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(JsonConvert.SerializeObject(agdata));

            return response;

        }
        [HttpGet]
        [Route("api/Timeseries/GetTimeseriesdatabySorce/{from}/{to}")]
        public HttpResponseMessage GetTimeseriesdatabySorce(string from, string to)


        {

            Task.Run(async () => await SampleAsync()).Wait();
            if (accessToken != null)
            {
                try
                {
                    //fromAvailabilityTimestamp1 = new DateTime(2017, 7, 6).ToUniversalTime();
                    //toAvailabilityTimestamp1 = new DateTime(2017, 7, 7).ToUniversalTime();
                    fromAvailabilityTimestamp1 = Convert.ToDateTime(from).ToUniversalTime();
                    toAvailabilityTimestamp1 = Convert.ToDateTime(to).ToUniversalTime();
                    srcdata.Clear();
                    //Task.Run(async () => await getgeneralts(fromAvailabilityTimestamp, toAvailabilityTimestamp, accessToken, environmentFqdn)).Wait();
                    Task.Run(async () => await getsourcets(fromAvailabilityTimestamp1, toAvailabilityTimestamp1, accessToken, environmentFqdn)).Wait();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(JsonConvert.SerializeObject(srcdata));
            return response;

        }
        public static async Task SampleAsync()
        {
            // 1. Acquire an access token.
            accessToken = await AcquireAccessTokenAsync();

            // 2. Obtain list of environments and get environment FQDN for the environment of interest.

            {
                Uri uri = new UriBuilder("https", "api.timeseries.azure.com")
                {
                    Path = "environments",
                    Query = "api-version=2016-12-12"
                }.Uri;
                HttpWebRequest request = WebRequest.CreateHttp(uri);
                request.Method = "GET";
                request.Headers.Add("x-ms-client-application-name", "TimeSeriesInsightsQuerySample");
                request.Headers.Add("Authorization", "Bearer " + accessToken);

                using (WebResponse webResponse = await request.GetResponseAsync())
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    string responseJson = await sr.ReadToEndAsync();

                    JObject result = JsonConvert.DeserializeObject<JObject>(responseJson);
                    JArray environmentsList = (JArray)result["environments"];
                    if (environmentsList.Count == 0)
                    {
                        // List of user environments is empty, fallback to sample environment.
                        environmentFqdn = "de801751-1d71-4a6a-90a1-192d1f38dc44.env.timeseries.azure.com";
                    }
                    else
                    {
                        // Assume the first environment is the environment of interest.
                        JObject firstEnvironment = (JObject)environmentsList[0];
                        environmentFqdn = firstEnvironment["environmentFqdn"].Value<string>();
                    }
                }
            }
            //Console.WriteLine("Using environment FQDN '{0}'", environmentFqdn);

            // 3. Obtain availability data for the environment and get availability range.
            //DateTime fromAvailabilityTimestamp;
            //DateTime toAvailabilityTimestamp;
            {
                Uri uri = new UriBuilder("https", environmentFqdn)
                {
                    Path = "availability",
                    Query = "api-version=2016-12-12"
                }.Uri;
                HttpWebRequest request = WebRequest.CreateHttp(uri);
                request.Method = "GET";
                request.Headers.Add("x-ms-client-application-name", "TimeSeriesInsightsQuerySample");
                request.Headers.Add("Authorization", "Bearer " + accessToken);

                using (WebResponse webResponse = await request.GetResponseAsync())
                using (var sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    string responseJson = await sr.ReadToEndAsync();

                    JObject result = JsonConvert.DeserializeObject<JObject>(responseJson);
                    JObject range = (JObject)result["range"];
                    fromAvailabilityTimestamp = range["from"].Value<DateTime>();
                    toAvailabilityTimestamp = range["to"].Value<DateTime>();
                }
            }
            Console.WriteLine(
                "Obtained availability range [{0}, {1}]",
                fromAvailabilityTimestamp,
                toAvailabilityTimestamp);

            // 4. Get aggregates for the environment:
            //    group by Event Source Name and calculate number of events in each group.
            {
                // Assume data for the whole availablility range is requested.

            }
        }
        public static async Task<List<timeList>> getgeneralts(DateTime fromAvailabilityTimestamp, DateTime toAvailabilityTimestamp, string accessToken, string environmentFqdn)
        {
            DateTime from = fromAvailabilityTimestamp;
            DateTime to = toAvailabilityTimestamp;
            try
            {
                JObject inputPayload = new JObject(
                    // Send HTTP headers as a part of the message since .NET WebSocket does not support
                    // sending custom headers on HTTP GET upgrade request to WebSocket protocol request.
                    new JProperty("headers", new JObject(
                        new JProperty("x-ms-client-application-name", "TimeSeriesInsightsQuerySample"),
                        new JProperty("Authorization", "Bearer " + accessToken))),
                    new JProperty("content", new JObject(
                        new JProperty("aggregates", new JArray(new JObject(
                            new JProperty("dimension", new JObject(
                                new JProperty("uniqueValues", new JObject(
                                    new JProperty("input", new JObject(
                                        new JProperty("builtInProperty", "$ts"))),
                                    new JProperty("take", 1000))))),
                            //new JProperty("take", 1000))))),
                            new JProperty("measures", new JArray(new JObject(
                                new JProperty("count", new JObject()))))))),
                        new JProperty("searchSpan", new JObject(
                            new JProperty("from", from),
                            new JProperty("to", to))))));

                //JObject inputPayload = new JObject(
                //   // Send HTTP headers as a part of the message since .NET WebSocket does not support
                //   // sending custom headers on HTTP GET upgrade request to WebSocket protocol request.
                //   new JProperty("headers", new JObject(
                //       new JProperty("x-ms-client-application-name", "TimeSeriesInsightsQuerySample"),
                //       new JProperty("Authorization", "Bearer " + accessToken))),
                //   new JProperty("content", new JObject(
                //          new JProperty("top", new JObject(
                //              new JProperty("sort", new JArray(new JObject(
                //                      new JProperty("input", new JObject(
                //                          new JProperty("builtInProperty", "$ts"))),
                //                      new JProperty("order", "DESC")))),

                //          new JProperty("count", 1000))),
                //           new JProperty("searchSpan", new JObject(
                //           new JProperty("from", from),
                //           new JProperty("to", to)))
                //       )));

                var webSocket = new ClientWebSocket();

                // Establish web socket connection.
                Uri uri = new UriBuilder("wss", environmentFqdn)
                {
                    Path = "aggregates",
                    Query = "api-version=2016-12-12"
                }.Uri;
                await webSocket.ConnectAsync(uri, CancellationToken.None);

                // Send input payload.
                byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(inputPayload.ToString());
                await webSocket.SendAsync(
                    new ArraySegment<byte>(inputPayloadBytes),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None);

                // Read response messages from web socket.
                JObject responseContent = null;
                using (webSocket)
                {
                    while (true)
                    {
                        string message;
                        using (var ms = new MemoryStream())
                        {
                            // Write from socket to memory stream.
                            const int bufferSize = 16 * 1024;
                            var temporaryBuffer = new byte[bufferSize];
                            while (true)
                            {
                                WebSocketReceiveResult response = await webSocket.ReceiveAsync(
                                    new ArraySegment<byte>(temporaryBuffer),
                                    CancellationToken.None);

                                ms.Write(temporaryBuffer, 0, response.Count);
                                if (response.EndOfMessage)
                                {
                                    break;
                                }
                            }

                            // Reset position to the beginning to allow reads.
                            ms.Position = 0;

                            using (var sr = new StreamReader(ms))
                            {
                                message = sr.ReadToEnd();
                            }
                        }

                        JObject messageObj = JsonConvert.DeserializeObject<JObject>(message);

                        // Stop reading if error is emitted.
                        if (messageObj["error"] != null)
                        {
                            break;
                        }

                        // Number of items corresponds to number of aggregates in input payload
                        JArray currentContents = (JArray)messageObj["content"];
                        //JArray Currentevents = (JArray)currentContents["events"];
                        ////JArray jsonarray = Json.Parse(Currentevents)
                        //foreach (JObject item in Currentevents)
                        //{

                        //    //JObject Currentschema = (JObject)item.GetValue("schema");

                        //    Console.WriteLine(item);

                        //    //Console.WriteLine(e.schema);  
                        //}
                        // In this sample list of aggregates in input payload contains
                        // only 1 item since request contains 1 aggregate.
                        responseContent = (JObject)currentContents[0];

                        // Stop reading if 100% of completeness is reached.
                        if (messageObj["percentCompleted"] != null &&
                            Math.Abs((double)messageObj["percentCompleted"] - 100d) < 0.01)
                        {
                            break;
                        }
                    }

                    // Close web socket connection.
                    if (webSocket.State == WebSocketState.Open)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "CompletedByClient",
                            CancellationToken.None);
                    }
                }

                Console.WriteLine("Dimension Value\t\tCount");
                JArray dimensionValues = (JArray)responseContent["dimension"];
                JArray measures = (JArray)responseContent["measures"];
                for (int i = 0; i < dimensionValues.Count; i++)
                {
                    string currentDimensionValue = (string)dimensionValues[i];
                    JArray currentMeasureValues = (JArray)measures[i];
                    double currentCount = (double)currentMeasureValues[0];
                    timeList t = new timeList();
                    t.tme = currentDimensionValue;
                    t.count = Convert.ToInt32(currentCount);
                    agdata.Add(t);

                }
                return agdata;
            }
            catch (Exception e)
            {
                return agdata;
            }
        }
        public static async Task<List<srctimeList>> getsourcets(DateTime fromAvailabilityTimestamp, DateTime toAvailabilityTimestamp, string accessToken, string environmentFqdn)
        {
            try
            {
                DateTime from = fromAvailabilityTimestamp;
                DateTime to = toAvailabilityTimestamp;

                JObject inputPayload = new JObject(
                    // Send HTTP headers as a part of the message since .NET WebSocket does not support
                    // sending custom headers on HTTP GET upgrade request to WebSocket protocol request.
                    new JProperty("headers", new JObject(
                        new JProperty("x-ms-client-application-name", "TimeSeriesInsightsQuerySample"),
                        new JProperty("Authorization", "Bearer " + accessToken))),
                        new JProperty("content", new JObject(
                           new JProperty("aggregates", new JArray(new JObject(
                                new JProperty("dimension", new JObject(
                                       new JProperty("dateHistogram", new JObject(
                                           new JProperty("input", new JObject(
                                               new JProperty("builtInProperty", "$ts"))),
                                           new JProperty("breaks", new JObject(new JProperty("size", "1h")))
                                           ))

                                       )),
                               //new JProperty("dimension", new JObject(
                               //    new JProperty("uniqueValues", new JObject(
                               //        new JProperty("input", new JObject(
                               //            new JProperty("property", "source"),
                               //            new JProperty("type","String"))),
                               //            new JProperty("take", 100)                                       
                               //        ))
                               //    )),
                               new JProperty("aggregate", new JObject(
                                   new JProperty("dimension", new JObject(
                                   new JProperty("uniqueValues", new JObject(
                                       new JProperty("input", new JObject(
                                           new JProperty("property", "source"),
                                           new JProperty("type", "String"))),
                                           new JProperty("take", 100)
                                       ))
                                   )),
                                   //new JProperty("dimension", new JObject(
                                   //    new JProperty("dateHistogram", new JObject(
                                   //        new JProperty("input", new JObject(
                                   //            new JProperty("builtInProperty", "$ts"))),
                                   //        new JProperty("breaks", new JObject(new JProperty("size", "1h")))
                                   //        ))

                                   //    )),
                                   new JProperty("measures", new JArray(new JObject(
                                     new JProperty("count", new JObject()))))

                                   ))
                           //,new JProperty("measures", new JArray(new JObject(
                           //    new JProperty("count", new JObject()))))
                           ))),
                        //new JProperty("measures", new JArray(new JObject(
                        //        new JProperty("count", new JObject())))),
                        //End aggregates
                        new JProperty("searchSpan", new JObject(
                            new JProperty("from", from),
                            new JProperty("to", to)

                            ))
                    ))
                    );



                //JObject inputPayload = new JObject(
                //   // Send HTTP headers as a part of the message since .NET WebSocket does not support
                //   // sending custom headers on HTTP GET upgrade request to WebSocket protocol request.
                //   new JProperty("headers", new JObject(
                //       new JProperty("x-ms-client-application-name", "TimeSeriesInsightsQuerySample"),
                //       new JProperty("Authorization", "Bearer " + accessToken))),
                //   new JProperty("content", new JObject(
                //          new JProperty("top", new JObject(
                //              new JProperty("sort", new JArray(new JObject(
                //                      new JProperty("input", new JObject(
                //                          new JProperty("builtInProperty", "$ts"))),
                //                      new JProperty("order", "DESC")))),

                //          new JProperty("count", 1000))),
                //           new JProperty("searchSpan", new JObject(
                //           new JProperty("from", from),
                //           new JProperty("to", to)))
                //       )));

                var webSocket = new ClientWebSocket();

                // Establish web socket connection.
                Uri uri = new UriBuilder("wss", environmentFqdn)
                {
                    Path = "aggregates",
                    Query = "api-version=2016-12-12"
                }.Uri;
                await webSocket.ConnectAsync(uri, CancellationToken.None);

                // Send input payload.
                byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(inputPayload.ToString());
                await webSocket.SendAsync(
                    new ArraySegment<byte>(inputPayloadBytes),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None);

                // Read response messages from web socket.
                JObject responseContent = null;
                using (webSocket)
                {
                    while (true)
                    {
                        string message;
                        using (var ms = new MemoryStream())
                        {
                            // Write from socket to memory stream.
                            const int bufferSize = 16 * 1024;
                            var temporaryBuffer = new byte[bufferSize];
                            while (true)
                            {
                                WebSocketReceiveResult response = await webSocket.ReceiveAsync(
                                    new ArraySegment<byte>(temporaryBuffer),
                                    CancellationToken.None);

                                ms.Write(temporaryBuffer, 0, response.Count);
                                if (response.EndOfMessage)
                                {
                                    break;
                                }
                            }

                            // Reset position to the beginning to allow reads.
                            ms.Position = 0;

                            using (var sr = new StreamReader(ms))
                            {
                                message = sr.ReadToEnd();
                            }
                        }

                        JObject messageObj = JsonConvert.DeserializeObject<JObject>(message);

                        // Stop reading if error is emitted.
                        if (messageObj["error"] != null)
                        {
                            break;
                        }

                        // Number of items corresponds to number of aggregates in input payload
                        JArray currentContents = (JArray)messageObj["content"];
                        //JArray Currentevents = (JArray)currentContents["events"];
                        ////JArray jsonarray = Json.Parse(Currentevents)
                        //foreach (JObject item in Currentevents)
                        //{

                        //    //JObject Currentschema = (JObject)item.GetValue("schema");

                        //    Console.WriteLine(item);

                        //    //Console.WriteLine(e.schema);  
                        //}
                        // In this sample list of aggregates in input payload contains
                        // only 1 item since request contains 1 aggregate.
                        responseContent = (JObject)currentContents[0];

                        // Stop reading if 100% of completeness is reached.
                        if (messageObj["percentCompleted"] != null &&
                            Math.Abs((double)messageObj["percentCompleted"] - 100d) < 0.01)
                        {
                            break;
                        }
                    }

                    // Close web socket connection.
                    if (webSocket.State == WebSocketState.Open)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "CompletedByClient",
                            CancellationToken.None);
                    }
                }

                Console.WriteLine("Dimension Value\t\tCount");
                JArray timeValues = (JArray)responseContent["dimension"];
                JObject measures = (JObject)responseContent["aggregate"];




                for (int i = 0; i < timeValues.Count; i++)
                {
                    string currenttimeValue = (string)timeValues[i];
                    JArray srcValues = (JArray)measures["dimension"];
                    JArray cnts = (JArray)measures["measures"][i];

                    for (int j = 0; j < srcValues.Count; j++)
                    {
                        string currentsrcValue = (string)srcValues[j];
                        JToken currentMeasureValues = (JToken)cnts[j];
                        if (currentMeasureValues.ToString() != "")
                        {
                            double currentCount = (double)currentMeasureValues[0];

                            srctimeList st = new srctimeList();
                            st.tme = currenttimeValue;
                            st.src = currentsrcValue;
                            st.count = Convert.ToInt32(currentCount);
                            srcdata.Add(st);
                        }

                    }



                }

                return srcdata;
            }
            catch (Exception e)
            {
                return null;

            }

        }
        private static async Task<string> AcquireAccessTokenAsync()
        {
            if (ApplicationClientId == "#DUMMY#" || ApplicationClientSecret == "#DUMMY#")
            {
                throw new Exception(
                    $"Use the link {"https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization"} to update the values of 'ApplicationClientId' and 'ApplicationClientSecret'.");
            }

            var authenticationContext = new AuthenticationContext(
                "https://login.windows.net/90f1aac4-c661-46a9-83fd-e021705adcc9",
                TokenCache.DefaultShared);

            AuthenticationResult token = await authenticationContext.AcquireTokenAsync(
                resource: "https://api.timeseries.azure.com/",
                clientCredential: new ClientCredential(
                    clientId: ApplicationClientId,
                    clientSecret: ApplicationClientSecret));

            // Show interactive logon dialog to acquire token on behalf of the user.
            // Suitable for native apps, and not on server-side of a web application.
            //AuthenticationResult token = await authenticationContext.AcquireTokenAsync(
            //    resource: "https://api.timeseries.azure.com/",
            //    // Set well-known client ID for Azure PowerShell
            //    clientId: "1950a258-227b-4e31-a9cf-717495945fc2",
            //    // Set redirect URI for Azure PowerShell
            //    redirectUri: new Uri("urn:ietf:wg:oauth:2.0:oob"),
            //    parameters: new PlatformParameters(PromptBehavior.Auto));

            return token.AccessToken;
        }

    }

}

//{
//    public class timeList
//    {
//        public String tme { get; set; }
//        public int count { get; set; }
//    }

//    public class srctimeList
//    {
//        public String tme { get; set; }
//        public String src { get; set; }
//        public int count { get; set; }
//    }

//    [EnableCors(origins: "*", headers: "*", methods: "*")]
//    public class TimeseriesController : ApiController
//    {
//        static List<timeList> agdata = new List<timeList>();

//        private static string ApplicationClientId = "5894dfb1-4728-40e9-90b3-d48cc48dc43d";
//        //1e5c6a2a-a4bf-47ae-9264-f952cdf81814
//        //37a4e528-7046-4e7c-ba87-53b407d8e486

//        // SET the application key of the application registered in your Azure Active Directory
//        private static string ApplicationClientSecret = "BTONsdOiNxV5Z3GYqzQaylEocweZZLdpQl3dpJeOxRo=";
//        [HttpGet]
//        [Route("api/Timeseries/GetTimeseriesdata")]
//        //[ActionName("GetTrainRunEvents")]
//        //[Route("api/{TrainRunEvents}/{GetTrainRunEvents}")]
//        public HttpResponseMessage GetTimeseriesdata()
//        {
//            Task.Run(async () => await SampleAsync()).Wait();
//            HttpResponseMessage response = new HttpResponseMessage();
//            response.Content = new StringContent(JsonConvert.SerializeObject(agdata));
//            return response;

//        }
//        public static async Task<List<timeList>> SampleAsync()
//        {
//            // 1. Acquire an access token.
//            string accessToken = await AcquireAccessTokenAsync();

//            // 2. Obtain list of environments and get environment FQDN for the environment of interest.
//            string environmentFqdn;
//            {
//                Uri uri = new UriBuilder("https", "api.timeseries.azure.com")
//                {
//                    Path = "environments",
//                    Query = "api-version=2016-12-12"
//                }.Uri;
//                HttpWebRequest request = WebRequest.CreateHttp(uri);
//                request.Method = "GET";
//                request.Headers.Add("x-ms-client-application-name", "TimeSeriesInsightsQuerySample");
//                request.Headers.Add("Authorization", "Bearer " + accessToken);

//                using (WebResponse webResponse = await request.GetResponseAsync())
//                using (var sr = new StreamReader(webResponse.GetResponseStream()))
//                {
//                    string responseJson = await sr.ReadToEndAsync();

//                    JObject result = JsonConvert.DeserializeObject<JObject>(responseJson);
//                    JArray environmentsList = (JArray)result["environments"];
//                    if (environmentsList.Count == 0)
//                    {
//                        // List of user environments is empty, fallback to sample environment.
//                        environmentFqdn = "de801751-1d71-4a6a-90a1-192d1f38dc44.env.timeseries.azure.com";
//                    }
//                    else
//                    {
//                        // Assume the first environment is the environment of interest.
//                        JObject firstEnvironment = (JObject)environmentsList[0];
//                        environmentFqdn = firstEnvironment["environmentFqdn"].Value<string>();
//                    }
//                }
//            }
//            //Console.WriteLine("Using environment FQDN '{0}'", environmentFqdn);

//            // 3. Obtain availability data for the environment and get availability range.
//            DateTime fromAvailabilityTimestamp;
//            DateTime toAvailabilityTimestamp;
//            {
//                Uri uri = new UriBuilder("https", environmentFqdn)
//                {
//                    Path = "availability",
//                    Query = "api-version=2016-12-12"
//                }.Uri;
//                HttpWebRequest request = WebRequest.CreateHttp(uri);
//                request.Method = "GET";
//                request.Headers.Add("x-ms-client-application-name", "TimeSeriesInsightsQuerySample");
//                request.Headers.Add("Authorization", "Bearer " + accessToken);

//                using (WebResponse webResponse = await request.GetResponseAsync())
//                using (var sr = new StreamReader(webResponse.GetResponseStream()))
//                {
//                    string responseJson = await sr.ReadToEndAsync();

//                    JObject result = JsonConvert.DeserializeObject<JObject>(responseJson);
//                    JObject range = (JObject)result["range"];
//                    fromAvailabilityTimestamp = range["from"].Value<DateTime>();
//                    toAvailabilityTimestamp = range["to"].Value<DateTime>();
//                }
//            }
//            //Console.WriteLine(
//            //    "Obtained availability range [{0}, {1}]",
//            //    fromAvailabilityTimestamp,
//            //    toAvailabilityTimestamp);

//            // 4. Get aggregates for the environment:
//            //    group by Event Source Name and calculate number of events in each group.
//            {
//                // Assume data for the whole availablility range is requested.
//                DateTime from = fromAvailabilityTimestamp;
//                DateTime to = toAvailabilityTimestamp;

//                JObject inputPayload = new JObject(
//                    // Send HTTP headers as a part of the message since .NET WebSocket does not support
//                    // sending custom headers on HTTP GET upgrade request to WebSocket protocol request.
//                    new JProperty("headers", new JObject(
//                        new JProperty("x-ms-client-application-name", "TimeSeriesInsightsQuerySample"),
//                        new JProperty("Authorization", "Bearer " + accessToken))),
//                    new JProperty("content", new JObject(
//                        new JProperty("aggregates", new JArray(new JObject(
//                            new JProperty("dimension", new JObject(
//                                new JProperty("uniqueValues", new JObject(
//                                    new JProperty("input", new JObject(
//                                        new JProperty("builtInProperty", "$ts"))),
//                                    new JProperty("take", 1000))))),
//                            //new JProperty("take", 1000))))),
//                            new JProperty("measures", new JArray(new JObject(
//                                new JProperty("count", new JObject()))))))),
//                        new JProperty("searchSpan", new JObject(
//                            new JProperty("from", from),
//                            new JProperty("to", to))))));

//                //JObject inputPayload = new JObject(
//                //   // Send HTTP headers as a part of the message since .NET WebSocket does not support
//                //   // sending custom headers on HTTP GET upgrade request to WebSocket protocol request.
//                //   new JProperty("headers", new JObject(
//                //       new JProperty("x-ms-client-application-name", "TimeSeriesInsightsQuerySample"),
//                //       new JProperty("Authorization", "Bearer " + accessToken))),
//                //   new JProperty("content", new JObject(
//                //          new JProperty("top", new JObject(
//                //              new JProperty("sort", new JArray(new JObject(
//                //                      new JProperty("input", new JObject(
//                //                          new JProperty("builtInProperty", "$ts"))),
//                //                      new JProperty("order", "DESC")))),

//                //          new JProperty("count", 1000))),
//                //           new JProperty("searchSpan", new JObject(
//                //           new JProperty("from", from),
//                //           new JProperty("to", to)))
//                //       )));

//                var webSocket = new ClientWebSocket();

//                // Establish web socket connection.
//                Uri uri = new UriBuilder("wss", environmentFqdn)
//                {
//                    Path = "aggregates",
//                    Query = "api-version=2016-12-12"
//                }.Uri;
//                await webSocket.ConnectAsync(uri, CancellationToken.None);

//                // Send input payload.
//                byte[] inputPayloadBytes = Encoding.UTF8.GetBytes(inputPayload.ToString());
//                await webSocket.SendAsync(
//                    new ArraySegment<byte>(inputPayloadBytes),
//                    WebSocketMessageType.Text,
//                    endOfMessage: true,
//                    cancellationToken: CancellationToken.None);

//                // Read response messages from web socket.
//                JObject responseContent = null;
//                using (webSocket)
//                {
//                    while (true)
//                    {
//                        string message;
//                        using (var ms = new MemoryStream())
//                        {
//                            // Write from socket to memory stream.
//                            const int bufferSize = 16 * 1024;
//                            var temporaryBuffer = new byte[bufferSize];
//                            while (true)
//                            {
//                                WebSocketReceiveResult response = await webSocket.ReceiveAsync(
//                                    new ArraySegment<byte>(temporaryBuffer),
//                                    CancellationToken.None);

//                                ms.Write(temporaryBuffer, 0, response.Count);
//                                if (response.EndOfMessage)
//                                {
//                                    break;
//                                }
//                            }

//                            // Reset position to the beginning to allow reads.
//                            ms.Position = 0;

//                            using (var sr = new StreamReader(ms))
//                            {
//                                message = sr.ReadToEnd();
//                            }
//                        }

//                        JObject messageObj = JsonConvert.DeserializeObject<JObject>(message);

//                        // Stop reading if error is emitted.
//                        if (messageObj["error"] != null)
//                        {
//                            break;
//                        }

//                        // Number of items corresponds to number of aggregates in input payload
//                        JArray currentContents = (JArray)messageObj["content"];
//                        //JArray Currentevents = (JArray)currentContents["events"];
//                        ////JArray jsonarray = Json.Parse(Currentevents)
//                        //foreach (JObject item in Currentevents)
//                        //{

//                        //    //JObject Currentschema = (JObject)item.GetValue("schema");

//                        //    Console.WriteLine(item);

//                        //    //Console.WriteLine(e.schema);  
//                        //}
//                        // In this sample list of aggregates in input payload contains
//                        // only 1 item since request contains 1 aggregate.
//                        responseContent = (JObject)currentContents[0];

//                        // Stop reading if 100% of completeness is reached.
//                        if (messageObj["percentCompleted"] != null &&
//                            Math.Abs((double)messageObj["percentCompleted"] - 100d) < 0.01)
//                        {
//                            break;
//                        }
//                    }

//                    // Close web socket connection.
//                    if (webSocket.State == WebSocketState.Open)
//                    {
//                        await webSocket.CloseAsync(
//                            WebSocketCloseStatus.NormalClosure,
//                            "CompletedByClient",
//                            CancellationToken.None);
//                    }
//                }

//                Console.WriteLine("Dimension Value\t\tCount");
//                JArray dimensionValues = (JArray)responseContent["dimension"];
//                JArray measures = (JArray)responseContent["measures"];
//                for (int i = 0; i < dimensionValues.Count; i++)
//                {
//                    string currentDimensionValue = (string)dimensionValues[i];
//                    JArray currentMeasureValues = (JArray)measures[i];
//                    double currentCount = (double)currentMeasureValues[0];
//                    timeList t = new timeList();
//                    t.tme = currentDimensionValue;
//                    t.count = Convert.ToInt32(currentCount);
//                    agdata.Add(t);

//                }
//                return agdata;
//            }
//        }

//        private static async Task<string> AcquireAccessTokenAsync()
//        {
//            if (ApplicationClientId == "#DUMMY#" || ApplicationClientSecret == "#DUMMY#")
//            {
//                throw new Exception(
//                    $"Use the link {"https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization"} to update the values of 'ApplicationClientId' and 'ApplicationClientSecret'.");
//            }

//            var authenticationContext = new AuthenticationContext(
//                "https://login.windows.net/90f1aac4-c661-46a9-83fd-e021705adcc9",
//                TokenCache.DefaultShared);

//            AuthenticationResult token = await authenticationContext.AcquireTokenAsync(
//                resource: "https://api.timeseries.azure.com/",
//                clientCredential: new ClientCredential(
//                    clientId: ApplicationClientId,
//                    clientSecret: ApplicationClientSecret));

//            // Show interactive logon dialog to acquire token on behalf of the user.
//            // Suitable for native apps, and not on server-side of a web application.
//            //AuthenticationResult token = await authenticationContext.AcquireTokenAsync(
//            //    resource: "https://api.timeseries.azure.com/",
//            //    // Set well-known client ID for Azure PowerShell
//            //    clientId: "1950a258-227b-4e31-a9cf-717495945fc2",
//            //    // Set redirect URI for Azure PowerShell
//            //    redirectUri: new Uri("urn:ietf:wg:oauth:2.0:oob"),
//            //    parameters: new PlatformParameters(PromptBehavior.Auto));

//            return token.AccessToken;
//        }

//    }
//}
