using System;
using FarsiLibrary.Utils;

namespace Iauq.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static PersianDate ToPersianDate(this DateTime dateTime)
        {
            return PersianDateConverter.ToPersianDate(dateTime);
        }
    }
}