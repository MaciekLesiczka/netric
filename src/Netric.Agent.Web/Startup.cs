using System.Web.Http;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Netric.Agent.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            // Web Api
            app.UseWebApi(config);

            // File Server
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                //DefaultFilesOptions = { DefaultFileNames = { "index.html" } },
                //FileSystem = new PhysicalFileSystem("Assets"),
                //StaticFileOptions = { ContentTypeProvider = new CustomContentTypeProvider() }
            };

            app.UseFileServer(options);

        }
    }
}