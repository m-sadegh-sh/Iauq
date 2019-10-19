using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateCarouselProvider : LogProviderBase<Carousel>
    {
        public CreateCarouselProvider(Carousel instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new carousel: (title: \"{1}\", description: \"{2}\", link-url: \"{3}\")",
                    Identity, Instance.Title, Instance.Description, Instance.LinkUrl);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateCarouselProvider : LogProviderBase<Carousel>
    {
        public UpdateCarouselProvider(Carousel instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a carousel: (id: \"{1}\", title: \"{2}\", description: \"{3}\", link-url: \"{4}\")",
                    Identity, Instance.Id, Instance.Title, Instance.Description, Instance.LinkUrl);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteCarouselProvider : LogProviderBase<int>
    {
        public DeleteCarouselProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a carousel: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}