using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateTemplateProvider : LogProviderBase<Template>
    {
        public CreateTemplateProvider(Template instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new template: (title: \"{1}\", description: \"{2}\", body: \"{3}\")",
                    Identity, Instance.Title, Instance.Description, Instance.Body);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateTemplateProvider : LogProviderBase<Template>
    {
        public UpdateTemplateProvider(Template instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a template: (id: \"{1}\", title: \"{2}\", description: \"{3}\", body: \"{4}\")",
                    Identity, Instance.Id, Instance.Title, Instance.Description, Instance.Body);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteTemplateProvider : LogProviderBase<int>
    {
        public DeleteTemplateProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a template: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}