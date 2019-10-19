using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface ICarouselService
    {
        IQueryable<Carousel> GetAllCarousels();
        Carousel GetCarouselById(int id);
        void SaveCarousel(Carousel carousel);
        void DeleteCarousel(Carousel carousel);
    }
}