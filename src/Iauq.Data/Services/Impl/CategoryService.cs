using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly IDbSet<Category> _categories;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _categories = _unitOfWork.Set<Category>();
        }

        #region ICategoryService Members

        public IQueryable<Category> GetAllCategories()
        {
            return _categories;
        }

        public Category GetCategoryById(int id)
        {
            return _categories.Find(id);
        }

        public void SaveCategory(Category category)
        {
            _categories.Add(category);
        }

        public void DeleteCategory(Category category)
        {
            _categories.Remove(category);
        }

        #endregion
    }
}