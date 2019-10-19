using System.Collections.Generic;

namespace Iauq.Information.Models.Home
{
    public class CarouselsModel : List<CarouselModel>
    {
        public CarouselsModel Add(string imageUrl)
        {
            return Add(imageUrl, null);
        }

        public CarouselsModel Add(string imageUrl, string title)
        {
            return Add(imageUrl, title, null);
        }

        public CarouselsModel Add(string imageUrl, string title, string description)
        {
            return Add(imageUrl, title, null, description);
        }

        public CarouselsModel Add(string imageUrl, string title,
                                  string description, string linkUrl)
        {
            Add(new CarouselModel
                    {
                        ImageUrl = imageUrl,
                        Title = title,
                        Description = description,
                        LinkUrl = linkUrl
                    });

            return this;
        }
    }
}