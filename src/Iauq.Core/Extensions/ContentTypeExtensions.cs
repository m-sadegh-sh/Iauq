using Iauq.Core.Domain;

namespace Iauq.Core.Extensions
{
    public static class ContentTypeExtensions
    {
        public static string ToLocalizedString(this ContentType type)
        {
            switch (type)
            {
                case ContentType.News:
                    return "خبر";
                case ContentType.Events:
                    return "رخداد";
                case ContentType.Calendars:
                    return "تقویم";
                case ContentType.Pages:
                    return "صفحه";
                case ContentType.Links:
                    return "لینک";
                case ContentType.Menu:
                    return "منو";
                default:
                    return "نامعلوم";
            }
        }
    }
}