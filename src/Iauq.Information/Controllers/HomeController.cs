using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using Iauq.Core.Domain;
using Iauq.Core.Extensions;
using Iauq.Core.Utilities;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;
using Iauq.Information.Models.Home;
using Iauq.Web;
using MvcContrib.Pagination;
using StructureMap;
using File = Iauq.Core.Domain.File;

namespace Iauq.Information.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ICarouselService _carouselService;
        private readonly ICommentService _commentService;
        private readonly IContentService _contentService;
        private readonly IFileService _fileService;
        private readonly IPollService _pollService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;

        public HomeController(IUnitOfWork unitOfWork, IContentService contentService,
                              ICommentService commentService,
                              ICarouselService carouselService, IUserService userService, IPollService pollService,
                              IFileService fileService,
                              IWebHelper webHelper)
        {
            _unitOfWork = unitOfWork;
            _contentService = contentService;
            _commentService = commentService;
            _carouselService = carouselService;
            _userService = userService;
            _pollService = pollService;
            _fileService = fileService;
            _webHelper = webHelper;
        }

        public ActionResult Splash()
        {
            return View();
        }

        public ActionResult Default()
        {
            TempData["IsHome"] = true;
            return ViewOrPartialView();
        }

        public RedirectResult RedirectToOld(string oldUrl)
        {
            return RedirectPermanent("http://old.iauq.ac.ir/" + oldUrl);
        }

        public ActionResult Captcha()
        {
            var rand = new Random((int) DateTime.Now.Ticks);
            //generate new question
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            string captcha = string.Format("{0} + {1} = ?", a, b);

            //store answer
            Session["Captcha"] = a + b;

            //image stream
            FileContentResult img;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise
                int i, r, x, y;
                var pen = new Pen(Color.Yellow);
                for (i = 1; i < 10; i++)
                {
                    pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                    r = rand.Next(0, (130/3));
                    x = rand.Next(0, 130);
                    y = rand.Next(0, 30);

                    gfx.DrawEllipse(pen, x - r, y - r, r, r);
                }

                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                bmp.Save(mem, ImageFormat.Jpeg);
                img = File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
        }

        public ActionResult Sitemap()
        {
            IQueryable<Content> contents =
                _contentService.GetAllContentsByTypes(new[]
                                                          {
                                                              ContentType.Menu
                                                          })
                    .Where(c => c.IsPublished && c.ParentId == null).OrderBy(c => c.DisplayOrder);

            return ViewOrPartialView(contents.ToList());
        }

        public ActionResult SitemapXml()
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            IQueryable<Content> contents =
                _contentService.GetAllContentsByTypes(new[]
                                                          {
                                                              ContentType.Pages, ContentType.Menu
                                                          })
                    .Where(c => c.IsPublished).OrderByDescending(c => c.TypeInt == (short) ContentType.Menu).ThenBy(
                        c => c.DisplayOrder);

            var site = new Uri(Request.Url.AbsoluteUri);

            foreach (Content c in contents)
            {
                root.Add(
                    new XElement("url",
                                 new XElement("loc", new Uri(site, Url.TopicUrl(c)).AbsoluteUri,
                                              new XElement("changefreq", "daily"))));
            }

            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
                root.Save(writer);

            return Content(Encoding.UTF8.GetString(memoryStream.ToArray()), "text/xml", Encoding.UTF8);
        }

        public ActionResult Search(SearchModel searchModel, int page = 1)
        {
            return ViewOrPartialView(searchModel);

            if (page < 1)
                return RedirectToActionPermanent("Search", new {page = 1, searchModel.Q, searchModel.Tag});

            searchModel.IsSearched = !string.IsNullOrEmpty(searchModel.Q) || !string.IsNullOrEmpty(searchModel.Tag);

            if (searchModel.IsSearched)
            {
                searchModel.Q = searchModel.Q.SafeInput();
                searchModel.Tag = searchModel.Tag.SafeInput();

                if (!string.IsNullOrEmpty(searchModel.Q))
                {
                    searchModel.Q = searchModel.Q.Trim();
                    if (searchModel.Q.Length > 25)
                        searchModel.Q = searchModel.Q.Substring(0, 25);
                }

                if (!string.IsNullOrEmpty(searchModel.Tag))
                {
                    searchModel.Tag = searchModel.Tag.Trim();
                    if (searchModel.Tag.Length > 25)
                        searchModel.Tag = searchModel.Tag.Substring(0, 25);
                }

                IQueryable<Content> contents =
                    _contentService.GetAllContentsByTypes(new[]
                                                              {
                                                                  ContentType.News, ContentType.Events,
                                                                  ContentType.Calendars, ContentType.Pages,
                                                              }).Where(
                                                                  c => c.IsPublished);

                if (!string.IsNullOrEmpty(searchModel.Q))
                {
                    contents = contents.Where(c => c.Title.Contains(searchModel.Q) ||
                                                   c.Abstract.Contains(searchModel.Q) ||
                                                   c.Body.Contains(searchModel.Q) ||
                                                   c.Metadata.SeoTitle.Contains(searchModel.Q) ||
                                                   c.Metadata.SeoSlug.Contains(searchModel.Q) ||
                                                   c.Metadata.SeoKeywords.Contains(searchModel.Q) ||
                                                   c.Metadata.SeoDescription.Contains(searchModel.Q));
                }

                if (!string.IsNullOrEmpty(searchModel.Tag))
                    contents = contents.Where(c => c.Tags.Contains(searchModel.Tag) || searchModel.Tag.Contains(c.Tags));

                contents = contents.OrderByDescending(c => c.PageViews).ThenByDescending(c => c.Rate);

                searchModel.Results = new LazyPagination<Content>(contents, page, Constants.RecordPerPage);

                if (!searchModel.Results.Any() && page != 1)
                    return RedirectToActionPermanent("Search", new {page = 1, searchModel.Q, searchModel.Tag});
            }

            return ViewOrPartialView(searchModel);
        }

        public ActionResult Error()
        {
            Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return View();
        }

        public ActionResult NotFound(string catchAll)
        {
            ControllerContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;

            return View(model: catchAll);
        }

        [ChildActionOnly]
        public ActionResult Carousels()
        {
            var carouselsModel = new CarouselsModel();

            IQueryable<Carousel> carousels =
                _carouselService.GetAllCarousels().OrderBy(c => c.DisplayOrder).Take(4);

            foreach (Carousel carousel in carousels)
            {
                carouselsModel.Add(UrlResolver.Carousel(carousel.Id.ToString()), carousel.Title, carousel.Description,
                                   carousel.LinkUrl);
            }

            return ViewOrPartialView(carouselsModel);
        }

        public ActionResult Newest(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("Newest", new {page = 1});

            IQueryable<Content> contents =
                _contentService.GetAllContentsByTypes(new[]
                                                          {
                                                              ContentType.Events, ContentType.Calendars
                                                          })
                    .Where(c => c.IsPublished);
            contents = contents.OrderByDescending(c => c.PublishDateTicks);

            IPagination<Content> results;

            if (ControllerContext.IsChildAction)
                results = new LazyPagination<Content>(contents, page, Constants.RecordPerPartial);
            else
            {
                results = new LazyPagination<Content>(contents, page, Constants.RecordPerPage);
            }

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("Newest", new {page = 1});

            return ViewOrPartialView(results);
        }

        public ActionResult Highlights(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("Highlights", new {page = 1});

            IQueryable<Content> contents =
                _contentService.GetAllContentsByTypes(new[]
                                                          {
                                                              ContentType.Events, ContentType.Calendars
                                                          })
                    .Where(c => c.IsPublished);
            contents = contents.OrderByDescending(c => c.Comments.Count).ThenByDescending(c => c.PublishDateTicks);

            IPagination<Content> results;

            if (ControllerContext.IsChildAction)
                results = new LazyPagination<Content>(contents, page, Constants.RecordPerPartial);
            else
            {
                results = new LazyPagination<Content>(contents, page, Constants.RecordPerPage);
            }

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("Highlights", new {page = 1});

            return ViewOrPartialView(results);
        }

        public ActionResult InternalLinks()
        {
            IQueryable<Content> contents =
                _contentService.GetAllContentsByTypes(new[]
                                                          {
                                                              ContentType.Links
                                                          })
                    .Where(c => c.IsPublished && c.CategoryId.HasValue).OrderBy(c => c.DisplayOrder);

            return ViewOrPartialView(contents.ToList());
        }

        public ActionResult ExternalLinks()
        {
            IQueryable<Content> contents =
                _contentService.GetAllContentsByTypes(new[]
                                                          {
                                                              ContentType.Links
                                                          })
                    .Where(c => c.IsPublished && !c.CategoryId.HasValue).OrderBy(c => c.DisplayOrder);

            return ViewOrPartialView(contents.ToList());
        }

        public ActionResult TagClouds()
        {
            IDictionary<string, int> contents = _contentService.GetTagClouds();

            var tagsModels = new TagCloudsModel();

            foreach (var tags in contents.ToList())
                tagsModels.Add(tags.Key, tags.Value);

            return ViewOrPartialView(tagsModels);
        }

        public ActionResult Weather()
        {
            var weather = new Weather("IRXX0008");
            weather.ConvertToMetric();
            return ViewOrPartialView(weather);
        }

        [HttpGet]
        public ActionResult Login(string done = null)
        {
            return View(new LoginModel {Done = done});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", ValidationResources.InvalidState);
                return View(loginModel);
            }

            if (Session["Captcha"] == null || Session["Captcha"].ToString() != loginModel.Captcha)
            {
                ModelState.AddModelError("Captcha", ValidationResources.InvalidCaptcha);
                //dispay error and generate a new captcha
                return View(loginModel);
            }

            bool isValid = _userService.ValidateUser(loginModel.UserName, loginModel.Password, loginModel.SecurityToken);

            if (isValid)
            {
                FormsAuthentication.SetAuthCookie(loginModel.UserName, false);
                Logger.SaveLog(new UserLoggedInProvider(loginModel.UserName));

                if (Url.IsLocalUrl(loginModel.Done))
                    return Redirect(loginModel.Done);

                return RedirectToAction("Default");
            }

            ModelState.AddModelError("", ValidationResources.IncorrectCredentials);
            Logger.SaveLog(new UserWrongCredentialsProvider(loginModel));
            return View(loginModel);
        }

        public ActionResult Logout()
        {
            Logger.SaveLog(new UserLoggedOutProvider(User.Identity.Name));

            FormsAuthentication.SignOut();
            return RedirectToAction("Default");
        }

        public ActionResult PositiveRate(int id, string returnto)
        {
            return UpdateComment(id, returnto, false);
        }

        public ActionResult NagativeRate(int id, string returnto)
        {
            return UpdateComment(id, returnto, true);
        }

        [CustomAuthorize]
        private ActionResult UpdateComment(int id, string returnto, bool isNagetive)
        {
            Cache cache = ControllerContext.HttpContext.Cache;

            Comment dbComment = _commentService.GetCommentById(id);

            if (dbComment == null)
                return EntityNotFoundView();

            string key = ControllerContext.HttpContext.User.Identity.Name + "-" + id;

            var cachedValue = cache[key] as UserCommentRateCache;

            if (cachedValue != null)
            {
                if (cachedValue.IsNagetive)
                {
                    if (isNagetive)
                    {
                        if (Url.IsLocalUrl(returnto))
                            return Redirect(returnto);

                        return Redirect("~/");
                    }
                    dbComment.NagetiveRates--;
                    dbComment.PositiveRates++;
                }
                else
                {
                    if (isNagetive)
                    {
                        dbComment.PositiveRates--;
                        dbComment.NagetiveRates++;
                    }
                    else
                    {
                        if (Url.IsLocalUrl(returnto))
                            return Redirect(returnto);

                        return Redirect("~/");
                    }
                }

                cachedValue.IsNagetive = isNagetive;
            }
            else
            {
                cachedValue = new UserCommentRateCache
                                  {
                                      UserName = ControllerContext.HttpContext.User.Identity.Name,
                                      CommentId = id,
                                      IsNagetive = isNagetive
                                  };

                if (isNagetive)
                {
                    dbComment.NagetiveRates++;
                }
                else
                {
                    dbComment.PositiveRates++;
                }

                cache.Add(key, cachedValue, null, DateTime.Now.AddMonths(1), TimeSpan.Zero, CacheItemPriority.High,
                          null);
            }

            _unitOfWork.SaveChanges();

            if (Url.IsLocalUrl(returnto))
                return Redirect(returnto);

            return Redirect("~/");
        }

        public PartialViewResult Poll()
        {
            Poll currentPoll = _pollService.GetCurrentPoll();

            User user = _webHelper.GetCurrentUser(HttpContext);

            string ipAddress = ObjectFactory.GetInstance<IWebHelper>().GetIpAddress();

            if (currentPoll == null)
                return PartialView("_Poll");

            if (_pollService.AlreadyVoted(user != null ? user.UserName : ipAddress, currentPoll.Id))
                ViewBag.IsAlreadyVoted = true;

            return PartialView("_Poll", currentPoll);
        }

        public ActionResult Vote(int[] choiceItemIds, bool showResults)
        {
            Poll currentPoll = _pollService.GetCurrentPoll();

            if (currentPoll == null)
                return ViewOrPartialView("Poll");

            if (showResults)
            {
                ViewBag.ShowResults = true;
                return ViewOrPartialView("Poll", currentPoll);
            }

            User user = _webHelper.GetCurrentUser(HttpContext);

            string ipAddress = Request.UserHostAddress;

            if (_pollService.AlreadyVoted(user != null ? user.UserName : ipAddress, currentPoll.Id))
            {
                ViewBag.IsAlreadyVoted = true;
                return ViewOrPartialView("_Poll", currentPoll);
            }

            if (choiceItemIds != null && choiceItemIds.Length > 0)
            {
                foreach (Choice choice in currentPoll.Choices)
                {
                    bool isAnswered = false;

                    foreach (ChoiceItem choiceItem in choice.Items)
                    {
                        if (!choice.IsMultiSelection && isAnswered)
                            break;

                        if (choiceItemIds.Contains(choiceItem.Id))
                        {
                            choiceItem.Answers.Add(new Answer
                                                       {
                                                           Answerer = user,
                                                           IpAddress = user == null ? ipAddress : null,
                                                           SelectedItem = choiceItem
                                                       });

                            isAnswered = true;
                        }
                    }
                }

                _unitOfWork.SaveChanges();
                ViewBag.IsAlreadyVoted = true;
            }

            return ViewOrPartialView("Poll", currentPoll);
        }

        public ActionResult Download(Guid guid, string fn = null)
        {
            File file = _fileService.GetFileByGuid(guid);

            if (file == null || file.ContentType == null)
                return NotFoundView();

            if (!string.IsNullOrEmpty(fn) && string.Compare(file.Name, fn) != 0)
                return NotFoundView();

            File parent = file;
            while (parent != null)
            {
                if (!parent.IsPublished)
                    return AccessDeniedView();

                switch (parent.AccessMode)
                {
                    case AccessMode.OnlyAuthenticated:
                        if (!User.Identity.IsAuthenticated)
                            return AccessDeniedView();
                        break;
                    case AccessMode.None:
                        if (!User.Identity.IsAuthenticated)
                            return AccessDeniedView();

                        User user = _webHelper.GetCurrentUser(HttpContext);
                        if (file.UploaderId != user.Id)
                            return AccessDeniedView();

                        break;
                }

                parent = parent.Parent;
            }

            string filePath = Server.MapPath(Path.Combine(Constants.UploadsUrl, file.Guid.ToString()));

            if (!System.IO.File.Exists(filePath))
                return NotFoundView();

            file.AccessCount++;
            bool isSaved = true;
            try
            {
                _unitOfWork.SaveChanges();
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new FileDownloadProvider(file));
            else
                TempData["Error"] = ValidationResources.DownloadFailure;

            using (FileStream stream = System.IO.File.OpenRead(filePath))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int) stream.Length);

                return File(buffer, file.ContentType, file.Name);
            }
        }

        #region Nested type: UserCommentRateCache

        private class UserCommentRateCache
        {
            public string UserName { get; set; }
            public int CommentId { get; set; }
            public bool IsNagetive { get; set; }
        }

        #endregion
    }
}