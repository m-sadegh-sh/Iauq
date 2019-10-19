using Iauq.Core.Domain;
using Iauq.Data.Logging;
using Iauq.Information.Areas.Administration.Models.Administration;
using Iauq.Information.Models.Home;

namespace Iauq.Information.LogProviders
{
    public class UserWrongCredentialsProvider : LogProviderBase<LoginModel>
    {
        public UserWrongCredentialsProvider(LoginModel instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" tried to login with wrong credentials: (password: \"{1}\", security-token: \"{2}\").",
                    Instance.UserName, Instance.Password, Instance.SecurityToken);
            log.Level = LogLevel.WrongCredentials;
        }
    }

    public class UserLoggedInProvider : LogProviderBase<string>
    {
        public UserLoggedInProvider(string instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" logged-in.", Instance);
            log.Level = LogLevel.Login;
        }
    }

    public class UserChangePasswordProvider : LogProviderBase<ChangePasswordModel>
    {
        public UserChangePasswordProvider(ChangePasswordModel instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format("user \"{0}\" changed his/her password: (old-password: \"{1}\", new-password: \"{2}\").",
                              Identity, Instance.OldPassword, Instance.NewPassword);
            log.Level = LogLevel.ChangePassword;
        }
    }

    public class UserLoggedOutProvider : LogProviderBase<string>
    {
        public UserLoggedOutProvider(string instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" logged-out.", Instance);
            log.Level = LogLevel.Logout;
        }
    }

    public class CreateUserProvider : LogProviderBase<User>
    {
        public CreateUserProvider(User instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new user: (user-name: \"{1}\", password: \"{2}\", security-token: \"{3}\", email: \"{4}\")",
                    Identity, Instance.UserName, Instance.Password, Instance.SecurityToken, Instance.Email);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateUserProvider : LogProviderBase<User>
    {
        public UpdateUserProvider(User instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a user: (id: \"{1}\", user-name: \"{2}\", password: \"{3}\", security-token: \"{4}\", email: \"{5}\")",
                    Identity, Instance.Id, Instance.UserName, Instance.Password, Instance.SecurityToken, Instance.Email);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteUserProvider : LogProviderBase<string>
    {
        public DeleteUserProvider(string instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a user: (user-name: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}