# UmbracoStaticPublish
Publish static documents from umbraco and optionally use it as cache

Puth both files into your umbraco App_Code directory
then if you publish document it create corresponding index.html file in "published" directory 

If you want to use this generated static file as cache of your umbraco site 
add following name into your web.config in modules section
<add name="StaticPublishCacheModule" type="StaticPublishCacheModule" />
