using System.Web;
using Iauq.Core.Utilities;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Data.Services.Impl;
using Iauq.Web.Utilities;
using StructureMap;

namespace Iauq.Information.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Current
        {
            get { return ObjectFactory.Container; }
        }

        public static void Initialize()
        {
            ObjectFactory.Initialize(ie =>
                                         {
                                             ie.Scan(@as =>
                                                         {
                                                             @as.TheCallingAssembly();
                                                             @as.WithDefaultConventions();
                                                         });

                                             ie.For<IUnitOfWork>().Use(() => new IauqDbContext());
                                             ie.For<ILanguageService>().HttpContextScoped().Use<LanguageService>();
                                             ie.For<ICategoryService>().HttpContextScoped().Use<CategoryService>();
                                             ie.For<IRoleService>().HttpContextScoped().Use<RoleService>();
                                             ie.For<IUserService>().HttpContextScoped().Use<UserService>();
                                             ie.For<IContentService>().HttpContextScoped().Use<ContentService>();
                                             ie.For<ICommentService>().HttpContextScoped().Use<CommentService>();
                                             ie.For<IContentService>().HttpContextScoped().Use<ContentService>();
                                             ie.For<ICarouselService>().HttpContextScoped().Use<CarouselService>();
                                             ie.For<IPollService>().HttpContextScoped().Use<PollService>();
                                             ie.For<ILogService>().HttpContextScoped().Use<LogService>();
                                             ie.For<IFileService>().HttpContextScoped().Use<FileService>();
                                             ie.For<ITemplateService>().HttpContextScoped().Use<TemplateService>();
                                             ie.For<HttpContext>().Use(x => HttpContext.Current);
                                             ie.For<IWebHelper>().HttpContextScoped().Use<WebHelper>();
                                         });
        }
    }
}