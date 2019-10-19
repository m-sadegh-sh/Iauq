using System.ComponentModel.DataAnnotations;
using System.IO;
using Iauq.Information.App_GlobalResources;

namespace Iauq.Information.Areas.Administration.Helpers
{
    public class FileNameValidator : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ValidationResources.NameHasInvalidFileNameChars, name);
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            int index = ((string) value).IndexOfAny(Path.GetInvalidFileNameChars());

            return index < 0;
        }
    }
}