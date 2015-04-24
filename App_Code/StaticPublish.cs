using Umbraco.Core;
using umbraco.BusinessLogic; //For Log
//using umbraco.cms.businesslogic;
//using umbraco.cms.businesslogic.web;
using System.Linq;
using Umbraco.Core.Services;
using Umbraco.Core.Publishing;  //assembly: Umbraco.Core.dll

namespace Umbraco.Extensions.EventHandlers
{
    public class RegisterEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            string StoreFolder = System.Web.Hosting.HostingEnvironment.MapPath("/published");
            string root = System.Web.Hosting.HostingEnvironment.MapPath("~");

            Umbraco.Core.Services.ContentService.Published += (sender, args) =>
            {
                //must be inside Published block
                var urlProvider = Umbraco.Web.UmbracoContext.Current.UrlProvider;
                foreach (var content in args.PublishedEntities)
                {
                    string url = urlProvider.GetUrl(content.Id);
                    if (url == "#") continue;
                    var pagePath = System.Web.Hosting.HostingEnvironment.MapPath(url);
                    var relativePath = pagePath.Substring(root.Length);
                    var newPath = System.IO.Path.Combine(StoreFolder, relativePath, "index.html");

                    string absurl = urlProvider.GetUrl(content.Id, true);

                    //.NET 4.5.2
                    //HostingEnvironment.QueueBackgroundWorkItem(ct => SendMailAsync(user.Email));

                    try
                    {
                        using (var client = new System.Net.WebClient())
                        {
                            client.Encoding = System.Text.Encoding.UTF8;
                            var fi = new System.IO.FileInfo(newPath);
                            if (fi.Exists) fi.Delete();
                            if (!fi.Directory.Exists) fi.Directory.Create();
                            //client.DownloadStringCompleted += (sdr, dsce) =>
                            //{
                            //    string s = dsce.Result;
                            //    System.IO.File.WriteAllText(newPath, s, client.Encoding);
                            //    Log.Add(LogTypes.Notify, content.Id, "stored to " + newPath);
                            //};
                            //client.DownloadStringAsync(new System.Uri(absurl));
                            
                            //sync version
                            string s = client.DownloadString(absurl);
                            s = s.Replace("</body>", string.Format("<!-- static cache from {0} --></body>", System.DateTime.Now));
                            System.IO.File.WriteAllText(newPath, s, client.Encoding);
                            Log.Add(LogTypes.Notify, content.Id, "stored to " + newPath);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("error while publishing to file " + ex.Message);
                        //throw;
                    }
                }
            };
        }
    }
}