using System;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.DependencyResolution;
using Iauq.Information.LogProviders;
using Iauq.Web.Utilities;
using StructureMap;

namespace Iauq.Information
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("cdn/{*catchAll}");
            routes.IgnoreRoute("filemanager/{*catchAll}");
            routes.RouteExistingFiles = false;

            routes.MapRoute("Splash", "", new {Controller = "Home", Action = "Splash"});

            routes.MapRoute("Home", "home", new {Controller = "Home", Action = "Default"});

            routes.MapRoute("Old", "{oldUrl}", new {Controller = "Home", Action = "RedirectToOld"},
                            new {oldUrl = "conference|fareghotahsil|setadshahed"});

            routes.MapRoute("Captcha", "captcha", new {Controller = "Home", Action = "Captcha"});

            routes.MapRoute("Sitemap", "sitemap", new {Controller = "Home", Action = "Sitemap"});

            routes.MapRoute("SitemapXml", "sitemap.xml", new {Controller = "Home", Action = "SitemapXml"});

            routes.MapRoute("Vote", "vote/", new {Controller = "Home", Action = "Vote", showResults = false});
            routes.MapRoute("PollResults", "poll-results/",
                            new {Controller = "Home", Action = "Vote", showResults = true});

            routes.MapRoute("Search", "search/{page}", new {Controller = "Home", Action = "Search"},
                            new {page = @"\d+"});

            routes.MapRoute("Newest", "newest/{page}", new {Controller = "Home", Action = "Newest"},
                            new {page = @"\d+"});
            routes.MapRoute("Highlights", "highlights/{page}", new {Controller = "Home", Action = "Highlights"},
                            new {page = @"\d+"});

            routes.MapRoute("Log", "{Action}", new {Controller = "Home"},
                            new {Action = "Login|Logout"});
            routes.MapRoute("Error", "error", new {Controller = "Home", Action = "Error"});
            routes.MapRoute("NotFound", "not-found", new {Controller = "Home", Action = "NotFound"});

            routes.MapRoute("Page", "{id}/{slug}/",
                            new {Controller = "Pages", Action = "Details"},
                            new {id = @"\d+", slug = ".+"});
            routes.MapRoute("Page2", "{id}/",
                            new {Controller = "Pages", Action = "Details"},
                            new {id = @"\d+"});

            routes.MapRoute("News", "news/archive/{page}", new {Controller = "News", Action = "Archive"},
                            new {page = @"\d+"});
            routes.MapRoute("NewsDetails", "news/{id}/{slug}/",
                            new {Controller = "News", Action = "Details"},
                            new {id = @"\d+", slug = ".+"});
            routes.MapRoute("NewsDetails2", "news/{id}/",
                            new {Controller = "News", Action = "Details"},
                            new {id = @"\d+"});

            routes.MapRoute("Events", "events/archive/{page}", new {Controller = "Events", Action = "Archive"},
                            new {page = @"\d+"});
            routes.MapRoute("EventDetails", "events/{id}/{slug}/",
                            new {Controller = "Events", Action = "Details"},
                            new {id = @"\d+", slug = ".+"});
            routes.MapRoute("EventDetails2", "events/{id}/",
                            new {Controller = "Events", Action = "Details"},
                            new {id = @"\d+"});

            routes.MapRoute("PositiveRate", "common/{id}/pos/",
                            new {Controller = "Home", Action = "PositiveRate"},
                            new {id = @"\d+"});

            routes.MapRoute("NagativeRate", "common/{id}/nag/",
                            new {Controller = "Home", Action = "NagativeRate"},
                            new {id = @"\d+"});

            routes.MapRoute("Calendars", "calendars/archive/{page}",
                            new {Controller = "Calendars", Action = "Archive"},
                            new {page = @"\d+"});
            routes.MapRoute("CalendarDetails", "calendars/{id}/{slug}/",
                            new {Controller = "Calendars", Action = "Details"},
                            new {id = @"\d+", slug = ".+"});
            routes.MapRoute("CalendarDetails2", "calendars/{id}/",
                            new {Controller = "Calendars", Action = "Details"},
                            new {id = @"\d+"});

            routes.MapRoute("Polls", "polls/archive/{page}",
                            new {Controller = "Polling", Action = "Archive"},
                            new {page = @"\d+"});
            routes.MapRoute("PollDetails", "polls/{id}/",
                            new {Controller = "Polling", Action = "Details"},
                            new {id = @"\d+"});

            routes.MapRoute("Download", "download/{Guid}/{*fn}",
                            new {Action = "Download", Controller = "Home"},
                            new {Guid = new GuidConstraint()});

            routes.MapRoute("_Layout_", "", new {Controller = "Layout"});
            routes.MapRoute("_Home_", "", new {Controller = "Home"});
            routes.MapRoute("_News_", "", new {Controller = "News"});
            routes.MapRoute("_Events_", "", new {Controller = "Events"});
            routes.MapRoute("_Pages_", "", new {Controller = "Pages"});
            routes.MapRoute("_Calendars_", "", new {Controller = "Calendars"});

            routes.MapRoute("_Catcher_", "{*catchAll}", new {Controller = "Home", Action = "NotFound"});
        }

        protected void Application_Start()
        {
            Database.SetInitializer<IauqDbContext>(null);

            DefaultModelBinder.ResourceClassKey = "ValidationResources";

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            InitStructureMap();
        }

        private static void InitStructureMap()
        {
            IoC.Initialize();

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(IoC.Current));

            ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //Request.ToRaw();

            Response.TrySkipIisCustomErrors = true;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects();

            RemoveResponseUnwantedHeaders();
        }

        private void RemoveResponseUnwantedHeaders()
        {
            try
            {
                Response.Headers.Remove("Server");
                Response.Headers.Remove("X-AspNet-Version");
                Response.Headers.Remove("X-AspNetMvc-Version");

                foreach (HttpCookie cookie in Response.Cookies)
                {
                    if (string.CompareOrdinal(cookie.Name, ".iad") != 0 &&
                        string.CompareOrdinal(cookie.Name, "ASP.NET_SessionId") != 0 &&
                        string.CompareOrdinal(cookie.Name, "__RequestVerificationToken_Lw__") != 0)
                    {
                        cookie.Expires = DateTime.Now.AddYears(-100);
                        Response.Cookies.Add(cookie);
                    }
                }
            }
            catch
            {
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception error = Server.GetLastError();
            int code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            if (code == 404)
                return;

            var logger = ObjectFactory.GetInstance<ILogService>();

            logger.SaveLog(new ExceptionProvider(error));

            Response.Clear();
            Server.ClearError();

            Response.Redirect("~/error");
        }
    }
}