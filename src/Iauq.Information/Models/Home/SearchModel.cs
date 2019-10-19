using System.Collections.Generic;
using System.ComponentModel;
using Iauq.Core.Domain;
using MvcContrib.Pagination;

namespace Iauq.Information.Models.Home
{
    public class SearchModel : ModelBase
    {
        public bool IsSearched { get; set; }

        [DisplayName("عبارت مورد نظر را وارد کنید و کلید Enter را فشار دهید...")]
        public string Q { get; set; }

        [DisplayName("کدام تگ؟")]
        public string Tag { get; set; }

        public IPagination<Content> Results { get; set; }
    }
}