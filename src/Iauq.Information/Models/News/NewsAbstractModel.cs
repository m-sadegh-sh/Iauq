using System;
using FarsiLibrary.Utils;
using Iauq.Core.Domain;

namespace Iauq.Information.Models.News
{
    public class NewsAbstractModel : ModelBase
    {
        public long Id { get; set; }
        public PersianDate PublishDate { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Abstract { get; set; }
        public Category Category { get; set; }
    }
}