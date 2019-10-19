using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iauq.Information.Models.Home
{
    public class TagCloudsModel : List<TagCloudModel>
    {
        public TagCloudsModel Add(string tag, int count)
        {
            Add(new TagCloudModel()
            {
                Tag = tag,
                Count = count
            });

            return this;
        }
    }
}