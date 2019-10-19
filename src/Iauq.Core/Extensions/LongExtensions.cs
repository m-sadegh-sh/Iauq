using Iauq.Core.Utilities;

namespace Iauq.Core.Extensions
{
    public static class LongExtensions
    {
        public static string ToFileSize(this long value)
        {
            return string.Format(new FileSizeFormatProvider(), "{0:fs}", value);
        }
    }
}