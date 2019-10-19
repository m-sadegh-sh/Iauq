using System.Collections.Generic;
using FarsiLibrary.Utils;
using Iauq.Core.Domain;

namespace Iauq.Information.Models.News
{
    public class NewsAbstractsModel : List<NewsAbstractModel>
    {
        public NewsAbstractsModel Add(long id, PersianDate publishDate, string title, string slug, string @abstract, Category category)
        {
            Add(new NewsAbstractModel
                    {
                        Id = id,
                        PublishDate = publishDate,
                        Title = title,
                        Slug = slug,
                        Abstract = @abstract,
                        Category = category
                    });

            return this;
        }
    }
}