using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateCategoryProvider : LogProviderBase<Category>
    {
        public CreateCategoryProvider(Category instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new category: (title: \"{1}\")",
                    Identity, Instance.Title);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateCategoryProvider : LogProviderBase<Category>
    {
        public UpdateCategoryProvider(Category instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a category: (id: \"{1}\", title: \"{2}\")",
                    Identity, Instance.Id, Instance.Title);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteCategoryProvider : LogProviderBase<int>
    {
        public DeleteCategoryProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a category: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}