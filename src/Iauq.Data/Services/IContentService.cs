using System.Collections.Generic;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface IContentService
    {
        IQueryable<Content> GetAllContents();
        IQueryable<Content> GetAllContentsByTypes(ContentType[] types);
        Content GetContentById(int contentId);
        Content GetContentBySlug(string slug);
        void SaveContent(Content content);
        void DeleteContent(Content content);
        IDictionary<string,int> GetTagClouds();
    }
}