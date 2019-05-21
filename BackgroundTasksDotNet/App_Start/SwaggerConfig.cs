using System.Web.Http;
using WebActivatorEx;
using BackgroundTasksDotNet;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace BackgroundTasksDotNet
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                                  .EnableSwagger(c =>
                                  {
                                      c.SingleApiVersion("v1", ".Net 4.8 Background Tasks");
                                      c.IncludeXmlComments(string.Format(@"{0}\bin\BackgroundTasksDotNet.XML",
                                                           System.AppDomain.CurrentDomain.BaseDirectory));
                                  })
                                  .EnableSwaggerUi();
                    }
    }
}
