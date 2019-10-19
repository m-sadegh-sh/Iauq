using Iauq.Core.Domain;
using Iauq.Data.Logging;
using Iauq.Information.Areas.Administration.Models.FileManager;

namespace Iauq.Information.LogProviders
{
    public class CreateDirectoryProvider : LogProviderBase<CreateDirectoryModel>
    {
        public CreateDirectoryProvider(CreateDirectoryModel instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new directory: (taget-url: \"{1}\", name: \"{2}\")",
                    Identity, Instance.TargetUrl, Instance.Name);
            log.Level = LogLevel.Create;
        }
    }

    public class UploadFileProvider : LogProviderBase<UploadFileModel>
    {
        public UploadFileProvider(UploadFileModel instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" uploaded a new file: (taget-url: \"{1}\", file-name: \"{2}\", content-type: \"{3}\", content-length: \"{4}\")",
                    Identity, Instance.TargetUrl, Instance.FileName, Instance.ContentType, Instance.ContentLength);
            log.Level = LogLevel.Create;
        }
    }

    public class RenameDirectoryOrFileProvider : LogProviderBase<RenameModel>
    {
        public RenameDirectoryOrFileProvider(RenameModel instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" renamed a directory/file: (taget-url: \"{1}\", old-name: \"{2}\", new-name: \"{3}\")",
                    Identity, Instance.TargetUrl, Instance.OldName, Instance.NewName);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteDirectoryOrFileProvider : LogProviderBase<string>
    {
        public DeleteDirectoryOrFileProvider(string instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a directory/file: (path: \"{1}\")", Identity,
                                        Instance);
            log.Level = LogLevel.Delete;
        }
    }
}