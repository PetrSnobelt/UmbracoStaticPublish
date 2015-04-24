using Umbraco.Core;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using System.Linq;
using Umbraco.Core.Services;
using Umbraco.Core.Publishing;  //assembly: Umbraco.Core.dll

namespace Umbraco.Extensions.EventHandlers
{
    public class RegisterEvents : ApplicationEventHandler
    {
        string StoreFolder { get; private set; }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
           
            //Document.BeforePublish += Document_BeforePublish;
            //Document.AfterPublish += Document_AfterPublish;

            //Umbraco.Core.Models.Content
            //ContentService.Saving += ContentService_Saving; ;

            //PublishingStrategy.Publishing += PublishingStrategy_Publishing;

            base.ApplicationStarted(umbracoApplication, applicationContext);

            StoreFolder = System.Web.Hosting.HostingEnvironment.MapPath("/published");

            string root = System.Web.Hosting.HostingEnvironment.MapPath("~");

            //var a = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            
            Umbraco.Core.Services.ContentService.Published += (sender, args) =>
            {
                //var request = sender as Umbraco.Web.Routing.PublishedContentRequest;
                var urlProvider = Umbraco.Web.UmbracoContext.Current.UrlProvider;
                foreach (var content in args.PublishedEntities)
                {
                    //var helper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
                    //var publishedContent = helper.TypedContent(content.Id);
                    //var strUrl = publishedContent.Url;

                    string url = urlProvider.GetUrl(content.Id);
                    var pagePath = System.Web.Hosting.HostingEnvironment.MapPath(url);
                    var relativePath = pagePath.Substring(root.Length);
                    var newPath = System.IO.Path.Combine(StoreFolder, relativePath, "index.html");

                    string absurl = urlProvider.GetUrl(content.Id, true);

                    try
                    {
                        using (var client = new System.Net.WebClient())
                        {
                            client.Encoding = System.Text.Encoding.UTF8;
                            string s = client.DownloadString(absurl);
                            var fi = new System.IO.FileInfo(newPath);
                            if (!fi.Directory.Exists) fi.Directory.Create();
                            System.IO.File.WriteAllText(newPath, s, System.Text.Encoding.UTF8);
                        }
                        Log.Add(LogTypes.Notify, content.Id, "stored to " + newPath);
                    }
                    catch (System.Exception)
                    {
                        
                        //throw;
                    }
                    
                }
            };
        }
    }
}