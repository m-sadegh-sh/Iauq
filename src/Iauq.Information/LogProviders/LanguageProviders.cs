using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateLanguageProvider : LogProviderBase<Language>
    {
        public CreateLanguageProvider(Language instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new language: (name: \"{1}\", iso-code: \"{2}\")",
                    Identity, Instance.Name, Instance.IsoCode);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateLanguageProvider : LogProviderBase<Language>
    {
        public UpdateLanguageProvider(Language instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a language: (id: \"{1}\", name: \"{2}\", iso-code: \"{3}\")",
                    Identity, Instance.Id, Instance.Name, Instance.IsoCode);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteLanguageProvider : LogProviderBase<int>
    {
        public DeleteLanguageProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a language: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}