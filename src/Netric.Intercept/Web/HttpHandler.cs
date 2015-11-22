using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace Netric.Intercept.Web
{
    public class HttpHandler : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        private static string JsScript;
        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;

            var idStr = context.Request.QueryString["Id"];
            Guid id;
            if (!Guid.TryParse(idStr, out id))
            {
                HandleInvalidRequest(response, "Id");
                return;
            }
            var mode = context.Request.QueryString["mode"];
            if (mode == "script")
            {               
                response.Write(GetJavascriptText(idStr));
                response.ContentType = "application/javascript";
            }
            else
            {
                var networkStr = context.Request.QueryString["Network"];
                var serverStr = context.Request.QueryString["Server"];
                var browserStr = context.Request.QueryString["Browser"];                
                var url = context.Request.QueryString["Url"];

                long network;
                if (!long.TryParse(networkStr, out network))
                {
                    HandleInvalidRequest(response, "network");
                    return;
                }
                long server;
                if (!long.TryParse(serverStr, out server))
                {
                    HandleInvalidRequest(response, "server");
                    return;
                }
                long browser;
                if (!long.TryParse(browserStr, out browser))
                {
                    HandleInvalidRequest(response, "browser");
                    return;
                }

                NavigationTimingEventSource.Log.Statistics(GetPath(url), network, server, browser, DateTime.UtcNow.Ticks, HostingEnvironment.SiteName, idStr);
                response.Write("OK");
            }
        }

        private void HandleInvalidRequest(HttpResponse response,string paramName)
        {
            response.Write(string.Format("Invalid {0} param", paramName));
            response.StatusCode = 512;
        }

        private string GetJavascriptText(string guid)
        {
            if (JsScript == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                const string resourceName = "Netric.Intercept.netric.js";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    JsScript = reader.ReadToEnd();
                }    
            }
            return JsScript.Replace("GUID", guid);
        }

        private string GetPath(string url)
        {            
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                url = uri.AbsolutePath;
            }

            return url;
        }        
    }

}