using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface ITemplateService
    {
        IQueryable<Template> GetAllTemplates();
        Template GetTemplateById(int templateId);
        void SaveTemplate(Template template);
        void DeleteTemplate(Template template);
    }
}