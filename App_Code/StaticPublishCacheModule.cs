using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class StaticPublishCacheModule : IHttpModule
{
    /// <summary>
    /// You will need to configure this module in the Web.config file of your
    /// web and register it with IIS before being able to use it. For more information
    /// see the following link: http://go.microsoft.com/?linkid=8101007
    /// </summary>
    public void Init(HttpApplication context)
    {
        context.BeginRequest += context_BeginRequest;
    }

    void context_BeginRequest(object sender, EventArgs e)
    {
        HttpApplication app = (HttpApplication)sender;
        HttpContext context = (HttpContext)app.Context;
        HttpRequest Request = context.Request;

        if (!Request.Path.EndsWith(@"/")) return;
        if (Request.Path.StartsWith("/umbraco", StringComparison.InvariantCultureIgnoreCase)) return;
        if (Request.IsAuthenticated) return;
        if (Request.HttpMethod != "GET") return;
        if (Request.QueryString.ToString() != string.Empty) return;
        
        //if (Request.IsLocal) return;

        string urlpath = @"~/published" + Request.Path + "index.html";
        string filepath = context.Server.MapPath(urlpath);
        if (System.IO.File.Exists(filepath))
        {
            System.Diagnostics.Debug.Write("redirected to static file " + filepath);
            context.RewritePath(urlpath);
        }
    }

    public void Dispose() { }
}