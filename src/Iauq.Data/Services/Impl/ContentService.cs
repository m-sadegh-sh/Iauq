using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class ContentService : IContentService
    {
        private readonly IDbSet<Content> _contents;
        private readonly IUnitOfWork _unitOfWork;

        public ContentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _contents = _unitOfWork.Set<Content>();
        }

        #region IContentService Members

        public IQueryable<Content> GetAllContents()
        {
            return _contents;
        }

        public Content GetContentById(int contentId)
        {
            return _contents.Find(contentId);
        }

        public Content GetContentBySlug(string slug)
        {
            return _contents.FirstOrDefault(c => c.Metadata.SeoSlug == slug);
        }

        public void SaveContent(Content content)
        {
            _contents.Add(content);
        }

        public void DeleteContent(Content content)
        {
            _contents.Remove(content);
        }

        public IQueryable<Content> GetAllContentsByTypes(ContentType[] types)
        {
            List<short> t = types.Cast<short>().ToList();

            return _contents.Where(c => t.Contains(c.TypeInt));
        }

        public IDictionary<string, int> GetTagClouds()
        {
            IEnumerable<string> tags =
                _contents.Where(c => c.Tags != null).Select(c => c.Tags).ToList().SelectMany(c => c.Split(',',';', '،'));

            IEnumerable<IGrouping<string, string>> x = tags.GroupBy(c => c);

            return x.ToDictionary(c => c.Key, c => c.Count());
        }

        #endregion
    }
}