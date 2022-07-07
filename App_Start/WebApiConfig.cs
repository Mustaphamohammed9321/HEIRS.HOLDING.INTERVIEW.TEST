using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HEIRS.HOLDING.INTERVIEW.TEST
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services



            // Web API configuration and services
            var settings = config.Formatters.JsonFormatter.SerializerSettings;
            // settings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //no need for camel case
            settings.Formatting = Formatting.Indented;


            //enable cors 
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
          
            // Web API routes
           
         

            config.Routes.MapHttpRoute(
                name: "Heirs Holding",  //Swagger UI
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new Swashbuckle.Application.RedirectHandler(Swashbuckle.Application.SwaggerDocsConfig.DefaultRootUrlResolver,
                                                "swagger/ui/index")
            );




            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
