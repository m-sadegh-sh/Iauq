using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class CarouselService : ICarouselService
    {
        private readonly IDbSet<Carousel> _carousels;
        private readonly IUnitOfWork _unitOfWork;

        public CarouselService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _carousels = _unitOfWork.Set<Carousel>();
        }

        #region ICarouselService Members

        public IQueryable<Carousel> GetAllCarousels()
        {
            return _carousels;
        }

        public Carousel GetCarouselById(int id)
        {
            return _carousels.Find(id);
        }

        public void SaveCarousel(Carousel carousel)
        {
            _carousels.Add(carousel);
        }

        public void DeleteCarousel(Carousel carousel)
        {
            _carousels.Remove(carousel);
        }

        #endregion
    }
}