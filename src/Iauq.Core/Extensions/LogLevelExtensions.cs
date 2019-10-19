using Iauq.Core.Domain;

namespace Iauq.Core.Extensions
{
    public static class LogLevelExtensions
    {
        public static string ToLocalizedString(this LogLevel level)
        {
            switch (level)
            {
                case LogLevel.WrongCredentials:
                    return "اطلاعات اشتباه";
                case LogLevel.Login:
                    return "ورود";
                case LogLevel.ChangePassword:
                    return "تغییر پسورد";
                case LogLevel.Logout:
                    return "خروج";
                case LogLevel.Create:
                    return "ثبت";
                case LogLevel.Update:
                    return "بروزرسانی";
                case LogLevel.Delete:
                    return "حذف";
                case LogLevel.Exception:
                    return "استثنا";
                default:
                    return "نامعلوم";
            }
        }
    }
}