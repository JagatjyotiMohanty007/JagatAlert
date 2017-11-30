using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WabtecOneAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
           
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.EnableCors();

            //config.Routes.MapHttpRoute(
            //    name: "ApiWithAction",
            //    routeTemplate: "api/{controller}/{action}/{TrainId}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

 
            // Controllers with Actions
            // To handle routes like `/api/VTRouting/route`
            config.Routes.MapHttpRoute(
                name: "GetTrainRunEventsFilterApi",
                routeTemplate: "api/{controller}/{action}/{StartDate}/{EndDate}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
              name: "GetTrainRunEventsFilterApi2",
              routeTemplate: "api/{controller}/{action}/{TrainId}/{LocoId}/{ArrLoc}/{DepLoc}/{StartDate}/{EndDate}",
              defaults: new { id = RouteParameter.Optional }
          );

            config.Routes.MapHttpRoute(
          name: "GetTrainRunEventsFilterApi3",
          routeTemplate: "api/{controller}/{action}/{TrainId}/{LocoId}/{ArrLoc}/{DepLoc}",
          defaults: new { id = RouteParameter.Optional }
      );


            config.Routes.MapHttpRoute(
           name: "SendTextMsgApi",
           routeTemplate: "api/{controller}/{action}/{msg}",
           defaults: new { id = RouteParameter.Optional }
       );

            config.Routes.MapHttpRoute(
name: "SendMailApi",
routeTemplate: "api/{controller}/{action}/{msg}",
defaults: new { id = RouteParameter.Optional }
);
            config.Routes.MapHttpRoute(
name: "GeTimeseriesApiSource",
routeTemplate: "api/controller/action/{frm}/{to}",
defaults: new { id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
           name: "DayinlifelocodetailsAPI",
           routeTemplate: "api/controller/action/{ID}/{subdiv}",
           defaults: new { id = RouteParameter.Optional }
       );
            config.Routes.MapHttpRoute(
       name: "StrinlineAPI",
       routeTemplate: "api/controller/action/{from}/{to}",
       defaults: new { id = RouteParameter.Optional }
   );
            config.Routes.MapHttpRoute(
           name: "GetEnforcementDashboardAPI",
           routeTemplate: "api/controller/action/",
           defaults: new { id = RouteParameter.Optional }
       );
            config.Routes.MapHttpRoute(
 name: "stringlinedata",
 routeTemplate: "api/DayInLife/GetStringlinechartdata/{username}/{password}",
 defaults: new { controller = "DayInLife", action = "GetStringlinechartdata" }
);

        }
    }
}
