using System.Data.Entity;
using System.Linq;
using Iauq.Core;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class TemplateService : ITemplateService
    {
        private readonly IDbSet<Template> _templates;
        private readonly IUnitOfWork _unitOfWork;

        public TemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _templates = _unitOfWork.Set<Template>();
        }

        #region ITemplateService Members

        public IQueryable<Template> GetAllTemplates()
        {
            return _templates;
        }

        public Template GetTemplateById(int templateId)
        {
            return _templates.Find(templateId);
        }


        public void SaveTemplate(Template template)
        {
            _templates.Add(template);
        }

        public void DeleteTemplate(Template template)
        {
            _templates.Remove(template);
        }

        #endregion
    }
}