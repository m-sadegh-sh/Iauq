using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateCommentProvider : LogProviderBase<Comment>
    {
        public CreateCommentProvider(Comment instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" posted a new comment: (owner-id: \"{1}\",title: \"{2}\", body: \"{3}\"",
                    Identity, Instance.OwnerId, Instance.Title, Instance.Body);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateCommentProvider : LogProviderBase<Comment>
    {
        public UpdateCommentProvider(Comment instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a comment: (id: \"{1}\", title: \"{2}\", body: \"{3}\")",
                    Identity, Instance.Id, Instance.Title, Instance.Body);
            log.Level = LogLevel.Update;
        }
    }

    public class CommentChangeApprovalProvider : LogProviderBase<Comment>
    {
        public CommentChangeApprovalProvider(Comment instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" changed approval status of a comment: (id: \"{1}\", is-approved: \"{2}\")",
                    Identity, Instance.Id, Instance.IsApproved);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteCommentProvider : LogProviderBase<int>
    {
        public DeleteCommentProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a comment: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}