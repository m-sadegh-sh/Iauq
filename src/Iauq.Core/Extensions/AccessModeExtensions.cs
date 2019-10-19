using Iauq.Core.Domain;

namespace Iauq.Core.Extensions
{
    public static class AccessModeExtensions
    {
        public static string ToLocalizedString(this AccessMode mode)
        {
            switch (mode)
            {
                case AccessMode.Any:
                    return "همه";
                case AccessMode.OnlyAuthenticated:
                    return "فقط کاربران";
                case AccessMode.None:
                    return "هیچکس";
                default:
                    return "نامعلوم";
            }
        }
    }
}