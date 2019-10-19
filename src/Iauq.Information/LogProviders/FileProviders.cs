using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateFileProvider : LogProviderBase<File>
    {
        public CreateFileProvider(File instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            if (Instance.IsDirectory)
                log.Message =
                    string.Format(
                        "user \"{0}\" created a new directory: (name: \"{1}\", access-mode: \"{2}\", parent: \"{3}\")",
                        Identity, Instance.Name, Instance.AccessMode,
                        Instance.ParentId.HasValue ? Instance.ParentId.ToString() : "ندارد");
            else
                log.Message =
                    string.Format(
                        "user \"{0}\" uploaded a new file: (name: \"{1}\", content-type: \"{2}\", size: \"{3}\", access-mode: \"{4}\", parent: \"{5}\")",
                        Identity, Instance.Name, Instance.ContentType, Instance.Size, Instance.AccessMode,
                        Instance.ParentId.HasValue ? Instance.ParentId.ToString() : "ندارد");
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateFileProvider : LogProviderBase<File>
    {
        public UpdateFileProvider(File instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            if (Instance.IsDirectory)
                log.Message =
                    string.Format(
                        "user \"{0}\" updated a directory: (id: \"{1}\", name: \"{2}\", access-mode: \"{3}\", parent: \"{4}\")",
                        Identity, Instance.Id, Instance.Name, Instance.AccessMode,
                        Instance.ParentId.HasValue ? Instance.ParentId.ToString() : "ندارد");
            else
                log.Message =
                    string.Format(
                        "user \"{0}\" updated a file: (id: \"{1}\", name: \"{2}\", content-type: \"{3}\", size: \"{4}\", access-mode: \"{5}\", parent: \"{6}\")",
                        Identity, Instance.Id, Instance.Name, Instance.ContentType, Instance.Size, Instance.AccessMode,
                        Instance.ParentId.HasValue ? Instance.ParentId.ToString() : "ندارد");
            log.Level = LogLevel.Update;
        }
    }

    public class FileChangePublishProvider : LogProviderBase<File>
    {
        public FileChangePublishProvider(File instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" changed publish status of a directory/file: (id: \"{1}\", is-published: \"{2}\")",
                    Identity, Instance.Id, Instance.IsPublished);
            log.Level = LogLevel.Update;
        }
    }

    public class FileDownloadProvider : LogProviderBase<File>
    {
        public FileDownloadProvider(File instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" downloaded a file: (id: \"{1}\", download-count: \"{2}\")",
                    Identity, Instance.Id, Instance.AccessCount);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteFileProvider : LogProviderBase<int>
    {
        public DeleteFileProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a directory/file: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}