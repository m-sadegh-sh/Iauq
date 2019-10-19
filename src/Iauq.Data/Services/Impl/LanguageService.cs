using System.Data.Entity;
using System.Linq;
using Iauq.Core;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class LanguageService : ILanguageService
    {
        private readonly IDbSet<Language> _languages;
        private readonly IUnitOfWork _unitOfWork;

        public LanguageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _languages = _unitOfWork.Set<Language>();
        }

        #region ILanguageService Members

        public IQueryable<Language> GetAllLanguages()
        {
            return _languages;
        }

        public Language GetLanguageById(int languageId)
        {
            return _languages.Find(languageId);
        }

        public Language GetLanguageByIsoCode(string isoCode)
        {
            return _languages.FirstOrDefault(l => l.IsoCode == isoCode);
        }

        public Language GetFallbackLanguage()
        {
            Language fallbackLanguage = _languages.FirstOrDefault();

            if (fallbackLanguage == null)
                throw new EntityNotFoundException("Fallback language couldn't be found.");

            return fallbackLanguage;
        }

        public void SaveLanguage(Language language)
        {
            _languages.Add(language);
        }

        public void DeleteLanguage(Language language)
        {
            _languages.Remove(language);
        }

        #endregion
    }
}