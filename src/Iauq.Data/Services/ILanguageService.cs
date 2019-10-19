using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface ILanguageService
    {
        IQueryable<Language> GetAllLanguages();
        Language GetLanguageById(int languageId);
        Language GetLanguageByIsoCode(string isoCode);
        Language GetFallbackLanguage();
        void SaveLanguage(Language language);
        void DeleteLanguage(Language language);
    }
}