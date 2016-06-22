using System;
using System.Web;
using System.Web.Hosting;

namespace Netric.Intercept.Web
{
    public class HttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            
            context.BeginRequest += ContextBeginRequest;
            context.EndRequest += ContextEndRequest;
        }

        public void Dispose()
        {
        }

        private static void ContextBeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var id = Guid.NewGuid();
            application.Context.Items["Netric.Request.Id"] = id;
            Log(application, RequestEventSource.Log.OnBegin,id);
        }

        private static void ContextEndRequest(object sender, EventArgs e)
        {            
            var application = (HttpApplication)sender;

            if (application.Context.Error == null)//we do not log requests with unhandled errors (ex. 404) yet.
            {
                Guid id;
                if (TryGetGuid(application, out id))
                {
                    Log(application, RequestEventSource.Log.OnEnd, id);
                }       
            }                     
        }

        private static bool TryGetGuid(HttpApplication application, out Guid id)
        {
            object idObj = application.Context.Items["Netric.Request.Id"];
            if (idObj == null)
            {
                id = new Guid();
                return false;
            }
            if (!(idObj is Guid))
            {
                id = new Guid();
                return false;
            }
            id = (Guid)idObj;
            return true;
        } 

        private static void Log(HttpApplication application,Action<string,string,string> log,Guid id)
        {            
            var context = application.Context;
            var absolutePath = context.Request.Url.AbsolutePath;
            //todo refactor, make more robust
            if (absolutePath.EndsWith(".js")
                || absolutePath.EndsWith(".css")
                || absolutePath.EndsWith(".png")
                || absolutePath.EndsWith(".jpg")
                || absolutePath.EndsWith(".jpeg")
                || absolutePath.EndsWith(".gif")
                || absolutePath.EndsWith(".ico")
                || absolutePath.EndsWith("netric.axd")
                )
            {
                return;
            }
            if (application.Context != null)
            {
                log(absolutePath, HostingEnvironment.SiteName, id.ToString());
            }
            else
            {
                //todo log
            }
        }
    }
}
