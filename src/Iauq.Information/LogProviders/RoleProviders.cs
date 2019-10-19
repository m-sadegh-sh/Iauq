using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreateRoleProvider : LogProviderBase<Role>
    {
        public CreateRoleProvider(Role instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new role: (name: \"{1}\")",
                    Identity, Instance.Name);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateRoleProvider : LogProviderBase<Role>
    {
        public UpdateRoleProvider(Role instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a role: (id: \"{1}\", name: \"{2}\")",
                    Identity, Instance.Id, Instance.Name);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteRoleProvider : LogProviderBase<int>
    {
        public DeleteRoleProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a role: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}