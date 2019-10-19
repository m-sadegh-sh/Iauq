using System.Text;

namespace Iauq.Core.Extensions
{
    public static class StringExtensions
    {
        public static string SafeInput(this string inputValue)
        {
            return (inputValue ?? "").Replace("'", "''");
        }

        public static string Scape(this string value)
        {
            value = value.Replace("@", " [at] ");
            value = value.Replace(".", " [dot] ");

            return value;
        }

        public static string EnsureLength(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            value = value.Trim();

            if (value.Length <= length)
                return value;

            value = value.Substring(0, length);

            return value + "...";
        }

        public static string SpliteUppercase(this string value, string splitter = "-")
        {
            var builder = new StringBuilder();

            foreach (char ch in value)
            {
                if (char.IsUpper(ch))
                    builder.Append("-");

                builder.Append(ch);
            }

            return builder.ToString().ToLower().Trim('-');
        }
    }
}