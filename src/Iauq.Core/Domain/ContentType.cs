using System;
using System.ComponentModel;

namespace Iauq.Core.Domain
{
    public enum ContentType:short
    {
        [Description("خبر")]
        News,
        [Description("رویداد")]
        Events,
        [Description("تقویم")]
        Calendars,
        [Description("صفحات")]
        Pages,
        [Description("لینک")]
        Links,
        [Description("منو")]
        Menu
    }
}