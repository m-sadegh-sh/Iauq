using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateContentProvider : LogProviderBase<Content>
    {
        public CreateContentProvider(Content instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new content: (type: \"{1}\", title: \"{2}\", abstract: \"{3}\", " +
                    "body: \"{4}\", tags: \"{5}\", metadata-title: \"{6}\", " +
                    "metadata-slug: \"{7}\", metadata-keywords: \"{8}\", metadata-description: \"{9}\")",
                    Identity, Instance.Type, Instance.Title, Instance.Abstract, Instance.Body, Instance.Tags,
                    Instance.Metadata.SeoTitle,
                    Instance.Metadata.SeoSlug, Instance.Metadata.SeoKeywords, Instance.Metadata.SeoDescription);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateContentProvider : LogProviderBase<Content>
    {
        public UpdateContentProvider(Content instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a content: (id: \"{1}\", type: \"{2}\", title: \"{3}\", abstract: \"{4}\", " +
                    "body: \"{5}\", tags: \"{6}\", metadata-title: \"{7}\", " +
                    "metadata-slug: \"{8}\", metadata-keywords: \"{9}\", metadata-description: \"{10}\")",
                    Identity, Instance.Id, Instance.Type, Instance.Title, Instance.Abstract, Instance.Body,
                    Instance.Tags,
                    Instance.Metadata.SeoTitle,
                    Instance.Metadata.SeoSlug, Instance.Metadata.SeoKeywords, Instance.Metadata.SeoDescription);
            log.Level = LogLevel.Update;
        }
    }

    public class ContentChangePublishProvider : LogProviderBase<Content>
    {
        public ContentChangePublishProvider(Content instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" changed publish status of a content: (id: \"{1}\", is-published: \"{2}\")",
                    Identity, Instance.Id, Instance.IsPublished);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteContentProvider : LogProviderBase<int>
    {
        public DeleteContentProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a content: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}